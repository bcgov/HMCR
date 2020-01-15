import React from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';

import PaginationControl from './PaginationControl';
import DataTableControl from './DataTableControl';

const DataTableWithPaginaionControl = ({ searchPagination, onPageNumberChange, onPageSizeChange, ...props }) => {
  return (
    <React.Fragment>
      <DataTableControl {..._.omit(props, ['searchPagination', 'onPageNumberChange', 'onPageSizeChange'])} />
      <PaginationControl
        currentPage={searchPagination.pageNumber}
        pageCount={searchPagination.pageCount}
        onPageChange={onPageNumberChange}
        pageSize={searchPagination.pageSize}
        onPageSizeChange={onPageSizeChange}
        totalCount={searchPagination.totalCount}
        itemCount={props.dataList.length}
      />
    </React.Fragment>
  );
};

DataTableWithPaginaionControl.propTypes = {
  dataList: PropTypes.arrayOf(PropTypes.object).isRequired,
  tableColumns: PropTypes.arrayOf(
    PropTypes.shape({
      heading: PropTypes.string.isRequired,
      key: PropTypes.string.isRequired,
      nosort: PropTypes.bool,
    })
  ).isRequired,
  editable: PropTypes.bool.isRequired,
  editPermissionName: PropTypes.string,
  searchPagination: PropTypes.shape({
    pageNumber: PropTypes.number.isRequired,
    pageSize: PropTypes.number.isRequired,
    pageCount: PropTypes.number.isRequired,
    totalCount: PropTypes.number,
    hasPreviousPage: PropTypes.bool,
    hasNextPage: PropTypes.bool,
  }).isRequired,
  onPageNumberChange: PropTypes.func.isRequired,
  onPageSizeChange: PropTypes.func.isRequired,
  onEditClicked: PropTypes.func,
  onDeleteClicked: PropTypes.func,
  onHeadingSortClicked: PropTypes.func,
};

DataTableWithPaginaionControl.defaultProps = {
  editable: false,
};

export default DataTableWithPaginaionControl;
