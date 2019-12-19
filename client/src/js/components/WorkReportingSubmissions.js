import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col } from 'reactstrap';
import moment from 'moment';
import { DateRangePicker } from 'react-dates';

import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import PageSpinner from './ui/PageSpinner';

import { searchSubmissions, setSingleSubmissionsSeachCriteria } from '../actions';

import * as Constants from '../Constants';

const startDateLimit = moment('2019-01-01');

const tableColumns = [
  { heading: 'Submission #', key: 'id', nosort: true },
  { heading: 'File', key: 'fileName', nosort: true },
  { heading: 'Submitted Date', key: 'date', nosort: true },
  { heading: 'Submitted By', key: 'name', nosort: true },
  { heading: 'Report Type', key: 'streamName', nosort: true },
  { heading: 'Submission Status', key: 'description', nosort: true },
];

const WorkReportingSubmissions = ({
  searchSubmissions,
  setSingleSubmissionsSeachCriteria,
  searchResult,
  searchPagination,
  searchCriteria,
}) => {
  const [startDate, setStartDate] = useState(searchCriteria.dateFrom ? moment(searchCriteria.dateFrom) : null);
  const [endDate, setEndDate] = useState(searchCriteria.dateTo ? moment(searchCriteria.dateTo) : null);
  const [focusedInput, setFocusedInput] = useState(null);
  const [searching, setSearching] = useState(false);

  useEffect(() => {
    const search = async () => {
      setSearching(true);
      await searchSubmissions();
      setSearching(false);
    };

    search();
  }, [searchSubmissions]);

  const startSearch = async () => {
    setSearching(true);
    await searchSubmissions();
    setSearching(false);
  };

  const handleDateChanged = (startDate, endDate) => {
    if (!(startDate && endDate && startDate.isSameOrBefore(endDate))) return;

    setSingleSubmissionsSeachCriteria('dateFrom', startDate.format(Constants.DATE_FORMAT));
    setSingleSubmissionsSeachCriteria('dateTo', endDate.format(Constants.DATE_FORMAT));

    startSearch();
  };

  const handleChangePage = newPage => {
    setSingleSubmissionsSeachCriteria('pageNumber', newPage);
    startSearch();
  };

  const handleChangePageSize = newSize => {
    setSingleSubmissionsSeachCriteria('pageSize', newSize);
    setSingleSubmissionsSeachCriteria('pageNumber', 1);
    startSearch();
  };

  return (
    <React.Fragment>
      <Row className="mb-3">
        <Col>
          <div style={{ display: 'flex', alignItems: 'center' }}>
            <strong className="mr-2">Report Submit Date</strong>
            <DateRangePicker
              startDate={startDate}
              startDateId="searchStartDate"
              endDate={endDate}
              endDateId="searchEndDate"
              onDatesChange={({ startDate, endDate }) => {
                setStartDate(startDate);
                setEndDate(endDate);
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
              isOutsideRange={date => date.isBefore(startDateLimit) || moment().isBefore(date)}
              minimumNights={0}
            />
          </div>
        </Col>
      </Row>
      {searching && <PageSpinner />}
      {!searching && searchResult.length > 0 && (
        <Row>
          <Col>
            <DataTableWithPaginaionControl
              dataList={searchResult.map(item => ({
                ...item,
                name: `${item.firstName} ${item.lastName}`,
                date: moment(item.appCreateTimestamp).format(Constants.DATE_FORMAT),
              }))}
              tableColumns={tableColumns}
              searchPagination={searchPagination}
              onPageNumberChange={handleChangePage}
              onPageSizeChange={handleChangePageSize}
            />
          </Col>
        </Row>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    searchResult: Object.values(state.submissions.list),
    searchPagination: state.submissions.searchPagination,
    searchCriteria: state.submissions.searchCriteria,
  };
};

export default connect(mapStateToProps, { searchSubmissions, setSingleSubmissionsSeachCriteria })(
  WorkReportingSubmissions
);
