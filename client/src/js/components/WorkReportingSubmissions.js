import React, { useState, useEffect, forwardRef, useImperativeHandle } from 'react';
import { Button, Row, Col, Input } from 'reactstrap';
import moment from 'moment';
import { DateRangePicker } from 'react-dates';
import queryString from 'query-string';
import _ from 'lodash';

import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import PageSpinner from './ui/PageSpinner';
import FontAwesomeButton from './ui/FontAwesomeButton';
import WorkReportingSubmissionDetail from './WorkReportingSubmissionDetail';
import useSearchData from './hooks/useSearchData';

import * as Constants from '../Constants';
import { stringifyQueryParams } from '../utils';

const startDateLimit = moment('2019-01-01');

const tableColumns = [
  { heading: 'Submission #', key: 'id', nosort: true },
  { heading: 'File', key: 'fileName', nosort: true },
  { heading: 'Submitted Date', key: 'date', nosort: true },
  { heading: 'Submitted By', key: 'name', nosort: true },
  { heading: 'Report Type', key: 'streamName', nosort: true },
  { heading: 'Submission Status', key: 'description', nosort: true },
];

const defaultSearchOptions = {
  dateFrom: moment().subtract(1, 'months'),
  dateTo: moment(),
  searchText: '',
  pageSize: Constants.DEFAULT_PAGE_SIZE,
  pageNumber: 1,
  dataPath: Constants.API_PATHS.SUBMISSIONS,
};

const WorkReportingSubmissions = ({ serviceArea, history }, ref) => {
  const searchData = useSearchData(null, history);
  const [searchText, setSearchText] = useState(defaultSearchOptions.searchText);

  const [showResultScreen, setShowResultScreen] = useState({ isOpen: false, submission: null });

  // Date picker
  const [focusedInput, setFocusedInput] = useState(null);
  const [dateFrom, setDateFrom] = useState(defaultSearchOptions.dateFrom);
  const [dateTo, setDateTo] = useState(defaultSearchOptions.dateTo);

  useImperativeHandle(ref, () => ({
    refresh() {
      searchData.refresh();
    },
  }));

  // Run on load, parse URL query params
  useEffect(() => {
    const params = queryString.parse(history.location.search);

    const options = {
      ...defaultSearchOptions,
      ..._.omit(params, ['dateFrom', 'dateTo']),
      dateFrom: params.dateFrom ? moment(params.dateFrom) : defaultSearchOptions.dateFrom,
      dateTo: params.dateFrom ? moment(params.dateTo) : defaultSearchOptions.dateTo,
      serviceAreaNumber: serviceArea,
    };

    if (params.showResult) {
      setShowResultScreen({ isOpen: true, submission: params.showResult });
    }

    searchData.setSearchOptions(options);
    setSearchText(options.searchText);
    setDateFrom(options.dateFrom);
    setDateTo(options.dateTo);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleDateChanged = (dateFrom, dateTo) => {
    if (!(dateFrom && dateTo && dateFrom.isSameOrBefore(dateTo))) return;

    searchData.setSearchOptions({ ...searchData.searchOptions, dateFrom, dateTo });
  };

  return (
    <React.Fragment>
      <Row className="mb-3">
        <Col>
          <div style={{ display: 'flex', alignItems: 'flex-end', justifyContent: 'space-between' }}>
            <div>
              <span className="mr-2">Report Submit Date</span>
              <DateRangePicker
                startDate={dateFrom}
                startDateId="searchStartDate"
                endDate={dateTo}
                endDateId="searchEndDate"
                onDatesChange={({ startDate, endDate }) => {
                  setDateFrom(startDate);
                  setDateTo(endDate);
                  handleDateChanged(startDate, endDate);
                }}
                focusedInput={focusedInput}
                onFocusChange={focusedInput => setFocusedInput(focusedInput)}
                showDefaultInputIcon
                hideKeyboardShortcutsPanel
                inputIconPosition="after"
                small
                displayFormat={Constants.DATE_DISPLAY_FORMAT}
                startDatePlaceholderText="Date From"
                endDatePlaceholderText="Date To"
                isOutsideRange={date =>
                  date.isBefore(startDateLimit) ||
                  moment()
                    .endOf('day')
                    .isBefore(date)
                }
                minimumNights={0}
              />
              <div
                style={{ position: 'relative', display: 'inline-block', height: 'calc(1.5em + 0.75rem + 2px)' }}
                className="ml-2"
              >
                <Input
                  type="text"
                  style={{ width: '160px', position: 'absolute', top: '15px' }}
                  placeholder="Name"
                  value={searchText}
                  onChange={e => setSearchText(e.target.value)}
                  onKeyDown={e => {
                    if (e.key === 'Enter') {
                      searchData.setSearchOptions({ ...searchData.searchOptions, searchText });
                    }
                  }}
                />
              </div>
            </div>
            <div>
              <FontAwesomeButton
                size="sm"
                icon="sync"
                spin={searchData.loading}
                disabled={searchData.loading}
                onClick={() => searchData.refresh()}
              />
            </div>
          </div>
        </Col>
      </Row>
      {searchData.loading && <PageSpinner />}
      {!searchData.loading && (
        <Row>
          <Col>
            {searchData.data.length > 0 && (
              <DataTableWithPaginaionControl
                dataList={searchData.data.map(item => ({
                  ...item,
                  name: `${item.firstName} ${item.lastName}`,
                  date: moment(item.appCreateTimestamp).format(Constants.DATE_DISPLAY_FORMAT),
                  id: (
                    <Button
                      color="link"
                      size="sm"
                      onClick={() => setShowResultScreen({ isOpen: true, submission: item.id })}
                    >
                      {item.id}
                    </Button>
                  ),
                }))}
                tableColumns={tableColumns}
                searchPagination={searchData.pagination}
                onPageNumberChange={searchData.handleChangePage}
                onPageSizeChange={searchData.handleChangePageSize}
              />
            )}
            {searchData.data.length <= 0 && <div>No submissions found</div>}
          </Col>
        </Row>
      )}
      {showResultScreen.isOpen && showResultScreen.submission && (
        <WorkReportingSubmissionDetail
          submission={showResultScreen.submission}
          toggle={() => {
            setShowResultScreen({ isOpen: false });
            const params = queryString.parse(history.location.search);
            if (params.showResult) history.push(`?${stringifyQueryParams(_.omit(params, ['showResult']))}`);
          }}
        />
      )}
    </React.Fragment>
  );
};

export default forwardRef(WorkReportingSubmissions);
