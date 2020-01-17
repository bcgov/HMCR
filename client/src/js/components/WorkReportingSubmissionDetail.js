import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalHeader } from 'reactstrap';
import moment from 'moment';
import _ from 'lodash';

import PageSpinner from './ui/PageSpinner';

import * as Constants from '../Constants';
import * as Api from '../Api';
import DataTableControl from './ui/DataTableControl';

const tableColumns = [
  { heading: 'Row #', key: 'rowNum', nosort: true },
  { heading: 'Service Area', key: 'serviceArea', nosort: true },
  { heading: 'Record Number', key: 'recordNumber', nosort: true },
  { heading: 'Error', key: 'errors', nosort: true },
];

const parseErrorDetailJson = json => JSON.parse(json).fieldMessages;

const submissionRowErrors = (rowNum, errorDetail) => {
  return (
    <ul>
      {parseErrorDetailJson(errorDetail).map(error => (
        <li key={`${rowNum}_${error.field}`}>
          <strong className="mr-1">{error.field}:</strong>
          {error.messages.map((msg, k) => (
            <span key={`${rowNum}_${error.field}_${k}`}>{msg}</span>
          ))}
        </li>
      ))}
    </ul>
  );
};

const WorkReportingSubmissionDetail = ({ toggle, submission }) => {
  const [submissionResultData, setSubmissionResultData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [modalSize, setModalSize] = useState('modal-sm');

  useEffect(() => {
    Api.getSubmissionResult(submission)
      .then(response => {
        setSubmissionResultData(response.data);

        setModalSize('modal-xl');
      })
      .finally(() => setLoading(false));
  }, [submission]);

  const submissionHeader = () => {
    return (
      <React.Fragment>
        <span>Submission #: {submissionResultData.id}</span>
        <span className="ml-3 mr-3">
          Submission Date: {moment(submissionResultData.appCreateTimestamp).format(Constants.DATE_FORMAT)}
        </span>
        <span>Serivce Area: {submissionResultData.serviceAreaNumber}</span>
      </React.Fragment>
    );
  };

  const submissionObject = () => {
    return (
      <div style={{ display: 'flex', justifyContent: 'space-between' }}>
        <div>
          <ul className="no-bullet">
            <li>
              <strong>File name:</strong> {submissionResultData.fileName}
            </li>
            <li>
              <strong>Report Type:</strong> {submissionResultData.streamName}
            </li>
            <li>
              <strong>Re-submitted Records:</strong> {submissionResultData.resubmissionCount}
            </li>
            <li>
              <strong>Status:</strong> {submissionResultData.description}
            </li>
            {submissionResultData.errorDetail && (
              <li>
                <strong>Status Detail:</strong> <ul>{submissionRowErrors(0, submissionResultData.errorDetail)}</ul>
              </li>
            )}
          </ul>
        </div>
        <div>Buttons go here</div>
      </div>
    );
  };

  const submissionRows = () => {
    if (submissionResultData.submissionRows.length <= 0) return;

    const tableRowData = submissionResultData.submissionRows.map(row => {
      return {
        ..._.pick(row, ['rowNum', 'recordNumber']),
        errors: submissionRowErrors(row.rowNum, row.errorDetail),
        serviceArea: submissionResultData.serviceAreaNumber,
      };
    });

    return <DataTableControl dataList={tableRowData} tableColumns={tableColumns} />;
  };

  return (
    <Modal isOpen={true} className={modalSize} centered={true}>
      {!loading && <ModalHeader toggle={toggle}>{submissionHeader()}</ModalHeader>}
      <ModalBody>
        {loading ? (
          <PageSpinner />
        ) : (
          <React.Fragment>
            {submissionObject()}
            {submissionRows()}
          </React.Fragment>
        )}
      </ModalBody>
    </Modal>
  );
};

export default WorkReportingSubmissionDetail;
