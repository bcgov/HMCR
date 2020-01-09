import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
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
  serviceArea,
  triggerRefresh,
}) => {
  const [startDate, setStartDate] = useState(searchCriteria.dateFrom ? moment(searchCriteria.dateFrom) : null);
  const [endDate, setEndDate] = useState(searchCriteria.dateTo ? moment(searchCriteria.dateTo) : null);
  const [focusedInput, setFocusedInput] = useState(null);
  const [searching, setSearching] = useState(false);

  useEffect(() => {
    const search = async () => {
      setSearching(true);
      if (searchCriteria.serviceAreaNumber !== serviceArea)
        setSingleSubmissionsSeachCriteria('serviceAreaNumber', serviceArea);
      await searchSubmissions();
      setSearching(false);
    };

    search();
  }, [searchSubmissions, serviceArea, setSingleSubmissionsSeachCriteria, triggerRefresh, searchCriteria]);

  const handleDateChanged = (startDate, endDate) => {
    if (!(startDate && endDate && startDate.isSameOrBefore(endDate))) return;

    setSingleSubmissionsSeachCriteria('dateFrom', startDate.format(Constants.DATE_FORMAT));
    setSingleSubmissionsSeachCriteria('dateTo', endDate.format(Constants.DATE_FORMAT));
  };

  const handleChangePage = newPage => {
    setSingleSubmissionsSeachCriteria('pageNumber', newPage);
  };

  const handleChangePageSize = newSize => {
    setSingleSubmissionsSeachCriteria('pageSize', newSize);
    setSingleSubmissionsSeachCriteria('pageNumber', 1);
  };

  return (
    <React.Fragment>
      <Row className="mb-3">
        <Col>
          <div style={{ display: 'flex', alignItems: 'flex-end' }}>
            <span className="mr-2">Report Submit Date</span>
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
              isOutsideRange={date =>
                date.isBefore(startDateLimit) ||
                moment()
                  .endOf('day')
                  .isBefore(date)
              }
              minimumNights={0}
            />
          </div>
        </Col>
      </Row>
      {searching && <PageSpinner />}
      {!searching && (
        <Row>
          <Col>
            {searchResult.length > 0 && (
              <DataTableWithPaginaionControl
                dataList={searchResult.map(item => ({
                  ...item,
                  name: `${item.firstName} ${item.lastName}`,
                  date: moment(item.appCreateTimestamp).format(Constants.DATE_FORMAT),
                  id: <Link to="#">{item.id}</Link>,
                }))}
                tableColumns={tableColumns}
                searchPagination={searchPagination}
                onPageNumberChange={handleChangePage}
                onPageSizeChange={handleChangePageSize}
              />
            )}
            {searchResult.length <= 0 && <div>No submissions found</div>}
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
