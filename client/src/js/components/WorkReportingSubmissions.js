import React, { useState, useEffect } from 'react';
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
import { updateQueryParamsFromHistory, stringifyQueryParams } from '../utils';

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
  serviceAreaNumber: 10,
};

const WorkReportingSubmissions = ({ searchSubmissions, serviceArea, triggerRefresh, history }) => {
  const [refreshTrigger, setRefreshTrigger] = useState(null);

  const { data, pagination, loading, searchOptions, setSearchOptions } = useSearchData({
    defaultSearchOptions,
    refreshTrigger,
  });
  const [searchText, setSearchText] = useState(defaultSearchOptions.searchText);

  const [showResultScreen, setShowResultScreen] = useState({ isOpen: false, submission: null });

  // Date picker
  const [focusedInput, setFocusedInput] = useState(null);
  const [dateFrom, setDateFrom] = useState(defaultSearchOptions.dateFrom);
  const [dateTo, setDateTo] = useState(defaultSearchOptions.dateTo);

  // Run on load and trigger refresh
  useEffect(() => {
    const params = queryString.parse(history.location.search);
    // const options = {
    //   ...defaultSearchOptions,
    //   ..._.omit(params, ['dateFrom', 'dateTo']),
    //   dateFrom: params.dateFrom ? moment(params.dateFrom) : defaultSearchOptions.dateFrom,
    //   dateTo: params.dateFrom ? moment(params.dateTo) : defaultSearchOptions.dateTo,
    //   serviceAreaNumber: serviceArea,
    // };

    if (params.showResult) {
      setShowResultScreen({ isOpen: true, submission: params.showResult });
    }

    // setSearchOptionsOld(options);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [triggerRefresh, serviceArea, searchSubmissions]);

  // Run search when searchOptions object has changed
  useEffect(() => {
    if (searchOptions !== defaultSearchOptions && searchOptions.dateFrom && searchOptions.dateTo) {
      updateHistoryLocationSearch(searchOptions);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchOptions, searchSubmissions]);

  const updateHistoryLocationSearch = options => {
    history.push(
      `?${updateQueryParamsFromHistory(history, _.omit(options, ['dateTo', 'dateFrom', 'serviceAreaNumber']))}`
    );
  };

  const handleDateChanged = (dateFrom, dateTo) => {
    if (!(dateFrom && dateTo && dateFrom.isSameOrBefore(dateTo))) return;

    setSearchOptions({ ...searchOptions, dateFrom, dateTo });

    history.push(
      `?${updateQueryParamsFromHistory(history, {
        dateFrom: dateFrom.format(Constants.DATE_FORMAT),
        dateTo: dateTo.format(Constants.DATE_FORMAT),
      })}`
    );
  };

  const handleChangePage = newPage => {
    // const options = { ...searchOptions, pageNumber: newPage };
    // setSearchOptionsOld(options);
  };

  const handleChangePageSize = newSize => {
    // const options = { ...searchOptions, pageNumber: 1, pageSize: newSize };
    // setSearchOptionsOld(options);
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
                displayFormat={Constants.DATE_FORMAT}
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
                      setSearchOptions({ ...searchOptions, searchText });
                    }
                  }}
                />
              </div>
            </div>
            <div>
              <FontAwesomeButton
                size="sm"
                icon="sync"
                spin={loading}
                disabled={loading}
                onClick={() => setRefreshTrigger(Math.random())}
              />
            </div>
          </div>
        </Col>
      </Row>
      {loading && <PageSpinner />}
      {!loading && (
        <Row>
          <Col>
            {data.length > 0 && (
              <DataTableWithPaginaionControl
                dataList={data.map(item => ({
                  ...item,
                  name: `${item.firstName} ${item.lastName}`,
                  date: moment(item.appCreateTimestamp).format(Constants.DATE_FORMAT),
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
                searchPagination={pagination}
                onPageNumberChange={handleChangePage}
                onPageSizeChange={handleChangePageSize}
              />
            )}
            {data.length <= 0 && <div>No submissions found</div>}
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

export default WorkReportingSubmissions;
