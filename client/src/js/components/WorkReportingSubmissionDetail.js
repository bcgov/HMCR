import React, { useEffect, useState } from 'react';
import { Button, Modal, ModalBody, ModalHeader } from 'reactstrap';
import moment from 'moment';
import _ from 'lodash';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FileSaver from 'file-saver';
import Clipboard from 'react-clipboard.js';
import { toast } from 'react-toastify';

import PageSpinner from './ui/PageSpinner';

import * as Constants from '../Constants';
import * as api from '../Api';
import DataTableControl from './ui/DataTableControl';

const tableColumns = [
  { heading: 'Row #', key: 'rowNum', nosort: true },
  { heading: 'Service Area', key: 'serviceArea', nosort: true },
  { heading: 'Record Number', key: 'recordNumber', nosort: true },
  { heading: 'Field', key: 'fieldName', nosort: true },
  { heading: 'Error', key: 'errors', nosort: true },
];

const parseErrorDetailJson = json => JSON.parse(json).fieldMessages;

const submissionRowErrors = (rowNum, errorDetail) => {
  return (
    <ul style={{ paddingInlineStart: '20px' }}>
      {parseErrorDetailJson(errorDetail).map(error => (
        <li key={`${rowNum}_${error.field}`}>
          <strong className="mr-1">{error.field}:</strong>
          <ul>
            {error.messages.map((msg, k) => (
              <li key={`${rowNum}_${error.field}_${k}`}>{`${msg} `}</li>
            ))}
          </ul>
        </li>
      ))}
    </ul>
  );
};

const createClipboardText = data => {
  let clipboardData = '';

  clipboardData += 'submission #\tsubmission date\tservice area\n';
  clipboardData += `${data.id}\t${moment(data.appCreateTimestamp).format(Constants.DATE_FORMAT)}\t${
    data.serviceAreaNumber
  }\n`;

  clipboardData += 'file name\treport type\tstatus\n';
  clipboardData += `${data.fileName}\t${data.streamName}\t${data.description}\n`;

  if (data.errorDetail) {
    clipboardData += '\nstatus detail\n';

    clipboardData += parseErrorDetailJson(data.errorDetail)
      .map(field => field.messages.map(msg => `${field.field}\t${msg}`))
      .join('\n');
  }

  if (data.submissionRows.length > 0) {
    clipboardData += '\n\nrow errors\n';

    clipboardData += 'row\tservice area\trecord number\tfield\tmessage\n';

    data.submissionRows.forEach(row => {
      const errors = parseErrorDetailJson(row.errorDetail);

      clipboardData += errors
        .map(field =>
          field.messages.map(
            msg => `${row.rowNum}\t${data.serviceAreaNumber}\t${row.recordNumber}\t${field.field}\t"${msg}"`
          )
        )
        .join('\n');
      clipboardData += '\n';
    });
  }

  return clipboardData;
};

const WorkReportingSubmissionDetail = ({ toggle, submission }) => {
  const [submissionResultData, setSubmissionResultData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [modalSize, setModalSize] = useState('modal-lg');

  useEffect(() => {
    api
      .getSubmissionResult(submission)
      .then(response => {
        setSubmissionResultData(response.data);

        if (response.data.submissionRows.length > 0) setModalSize('modal-xl');
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
            {submissionResultData.numResubmitRows && submissionResultData.numResubmitRows > 0 && (
              <li>
                <strong>Re-submitted Records:</strong> {submissionResultData.numResubmitRows}
              </li>
            )}
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
        <div style={{ whiteSpace: 'nowrap' }}>
          <Button
            size="sm"
            color="primary"
            className="mr-2"
            onClick={() =>
              api.getSubmissionFile(submissionResultData.id).then(response => {
                const filename = response.headers['content-disposition']
                  .split(';')
                  .find(token => token.trim().startsWith('filename='))
                  .replace('filename=', '')
                  .trim();

                FileSaver.saveAs(new Blob([response.data]), filename);
              })
            }
            title="Download original submission"
          >
            <FontAwesomeIcon icon="download" /> Original
          </Button>
          <Clipboard
            className="btn btn-primary btn-sm"
            data-clipboard-text={createClipboardText(submissionResultData)}
            onSuccess={() => {
              toast.info(<div className="text-center">Error details copied to clipboard.</div>);
            }}
            title="Copy errors to clipboard, with tab delimiter"
          >
            <FontAwesomeIcon icon="copy" /> Copy
          </Clipboard>
        </div>
      </div>
    );
  };

  const submissionRows = () => {
    if (submissionResultData.submissionRows.length <= 0) return;

    const tableRowData = [];

    submissionResultData.submissionRows.forEach(row =>
      parseErrorDetailJson(row.errorDetail).forEach(field =>
        tableRowData.push({
          ..._.pick(row, ['rowNum', 'recordNumber']),
          serviceArea: submissionResultData.serviceAreaNumber,
          fieldName: field.field,
          errors: field.messages.map((msg, k) => <div key={`${row.rowNum}_${field.field}_${k}`}>{msg}</div>),
        })
      )
    );

    return (
      <React.Fragment>
        <strong>Row Errors:</strong>
        <DataTableControl dataList={tableRowData} tableColumns={tableColumns} />
      </React.Fragment>
    );
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
