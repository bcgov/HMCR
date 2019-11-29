import React from 'react';
import PropTypes from 'prop-types';
import { Table } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import Authorize from '../fragments/Authorize';
import MaterialCard from './MaterialCard';
import FontAwesomeButton from './FontAwesomeButton';
import DeleteButton from './DeleteButton';
import PaginationControl from './PaginationControl';

const DataTableControl = ({
  dataList,
  tableColumns,
  editable,
  editPermissionName,
  searchPagination,
  onPageNumberChange,
  onPageSizeChange,
  onEditClicked,
  onDeleteClicked,
  onHeadingSortClicked,
}) => {
  const handleEditClicked = id => {
    if (onEditClicked) onEditClicked(id);
  };

  return (
    <MaterialCard>
      <Table size="sm" responsive>
        <thead className="thead-dark">
          <tr>
            {tableColumns.map(column => (
              <th key={column.heading}>
                {column.heading}
                {!column.nosort && <FontAwesomeButton icon="sort" onClick={() => onHeadingSortClicked(column.key)} />}
              </th>
            ))}
            {editable && (
              <Authorize requires={editPermissionName}>
                <th></th>
              </Authorize>
            )}
          </tr>
        </thead>
        <tbody>
          {dataList.map(item => {
            return (
              <tr key={item.id}>
                {tableColumns.map(column =>
                  column.key === 'isActive' ? (
                    <td key={column.key}>
                      {item[column.key] ? (
                        <FontAwesomeIcon icon="check-circle" className="fa-color-primary" />
                      ) : (
                        <FontAwesomeIcon icon="ban" className="fa-color-danger" />
                      )}
                    </td>
                  ) : (
                    <td key={column.key}>{item[column.key]}</td>
                  )
                )}
                {editable && (
                  <Authorize requires={editPermissionName}>
                    <td style={{ width: '1%', whiteSpace: 'nowrap' }}>
                      <FontAwesomeButton icon="edit" className="mr-1" onClick={() => handleEditClicked(item.id)} />
                      <DeleteButton
                        itemId={item.id}
                        buttonId={`item_${item.id}_delete`}
                        defaultEndDate={item.endDate}
                        onDeleteClicked={onDeleteClicked}
                      ></DeleteButton>
                    </td>
                  </Authorize>
                )}
              </tr>
            );
          })}
        </tbody>
      </Table>
      <PaginationControl
        currentPage={searchPagination.pageNumber}
        pageCount={searchPagination.pageCount}
        onPageChange={onPageNumberChange}
        pageSize={searchPagination.pageSize}
        onPageSizeChange={onPageSizeChange}
      />
    </MaterialCard>
  );
};

DataTableControl.propTypes = {
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
  onEditClicked: PropTypes.func.isRequired,
  onDeleteClicked: PropTypes.func.isRequired,
  onHeadingSortClicked: PropTypes.func.isRequired,
};

DataTableControl.defaultProps = {
  editable: false,
};

export default DataTableControl;
