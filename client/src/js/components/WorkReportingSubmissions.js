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
  direction: Constants.SORT_DIRECTION.DESCENDING,
  dataPath: Constants.API_PATHS.SUBMISSIONS,
};

const WorkReportingSubmissions = ({ serviceArea, history }, ref) => {
  const searchData = useSearchData(defaultSearchOptions, history);
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

    searchData.updateSearchOptions(options);
    setSearchText(options.searchText);
    setDateFrom(options.dateFrom);
    setDateTo(options.dateTo);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Update the service area search options when serviceArea is changed
  useEffect(() => {
    if (searchData.searchOptions) {
      if (searchData.searchOptions.serviceAreaNumber !== serviceArea) {
        searchData.updateSearchOptions({ ...searchData.searchOptions, serviceAreaNumber: serviceArea, serviceArea });
      }
    }
  }, [serviceArea, searchData]);

  const handleDateChanged = (dateFrom, dateTo) => {
    if (!(dateFrom && dateTo && dateFrom.isSameOrBefore(dateTo))) return;

    searchData.updateSearchOptions({ ...searchData.searchOptions, dateFrom, dateTo, pageNumber: 1 });
  };

  const handleSearchFormSubmit = () =>
    searchData.updateSearchOptions({ ...searchData.searchOptions, searchText, pageNumber: 1 });

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
                style={{
                  position: 'relative',
                  display: 'inline-block',
                  height: 'calc(1.5em + 0.75rem + 2px)',
                  width: '160px',
                }}
                className="ml-2"
              >
                <Input
                  type="text"
                  style={{ position: 'absolute', top: '15px' }}
                  placeholder="Name"
                  value={searchText}
                  onChange={e => setSearchText(e.target.value)}
                  onKeyDown={e => {
                    if (e.key === 'Enter') {
                      handleSearchFormSubmit();
                    }
                  }}
                />
              </div>
              <Button color="primary" type="button" className="ml-2" onClick={handleSearchFormSubmit}>
                Search
              </Button>
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
            {searchData.data.length <= 0 && <div>No records found</div>}
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
