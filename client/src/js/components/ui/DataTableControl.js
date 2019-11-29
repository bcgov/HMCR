import React from 'react';
import { Table } from 'reactstrap';

import Authorize from '../fragments/Authorize';
import MaterialCard from './MaterialCard';
import FontAwesomeButton from './FontAwesomeButton';
import DeleteButton from './DeleteButton';
import PaginationControl from './PaginationControl';

const DataTableControl = ({
  searchPagination,
  handleChangePage,
  handleChangePageSize,
  dataList,
  onEditClicked,
  onDeleteClicked,
  tableHeadings,
  dataColumnKeys,
  writePermissionName,
}) => {
  const handleEditClicked = id => {
    if (onEditClicked) onEditClicked(id);
  };

  return (
    <MaterialCard>
      <Table size="sm" responsive>
        <thead className="thead-dark">
          <tr>
            {tableHeadings.map(heading => (
              <th key={heading}>{heading}</th>
            ))}
            <Authorize requires={writePermissionName}>
              <th></th>
            </Authorize>
          </tr>
        </thead>
        <tbody>
          {dataList.map(item => {
            return (
              <tr key={item.id}>
                {dataColumnKeys.map(key => (
                  <td key={key}>{item[key]}</td>
                ))}
                <Authorize requires={writePermissionName}>
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
              </tr>
            );
          })}
        </tbody>
      </Table>
      <PaginationControl
        currentPage={searchPagination.pageNumber}
        pageCount={searchPagination.pageCount}
        onPageChange={handleChangePage}
        pageSize={searchPagination.pageSize}
        onPageSizeChange={handleChangePageSize}
      />
    </MaterialCard>
  );
};

export default DataTableControl;
