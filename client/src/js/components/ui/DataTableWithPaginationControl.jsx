import _ from 'lodash';
import PropTypes from 'prop-types';

import DataTableControl from './DataTableControl';
import PaginationControl from './PaginationControl';

const DataTableWithPaginationControl = ({ searchPagination, onPageNumberChange, onPageSizeChange, ...props }) => {
  return (
    <>
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
    </>
  );
};

DataTableWithPaginationControl.propTypes = {
  dataList: PropTypes.arrayOf(PropTypes.object).isRequired,
  tableColumns: PropTypes.arrayOf(
    PropTypes.shape({
      heading: PropTypes.string.isRequired,
      key: PropTypes.string.isRequired,
      nosort: PropTypes.bool,
      format: PropTypes.func,
    }),
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

DataTableWithPaginationControl.defaultProps = {
  editable: false,
};

export default DataTableWithPaginationControl;
