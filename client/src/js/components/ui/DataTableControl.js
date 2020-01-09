import React from 'react';
import PropTypes from 'prop-types';
import { Table, Badge } from 'reactstrap';

import Authorize from '../fragments/Authorize';
import FontAwesomeButton from './FontAwesomeButton';
import DeleteButton from './DeleteButton';

const DataTableControl = ({
  dataList,
  tableColumns,
  editable,
  editPermissionName,
  onEditClicked,
  onDeleteClicked,
  onHeadingSortClicked,
}) => {
  const handleEditClicked = id => {
    if (onEditClicked) onEditClicked(id);
  };

  return (
    <React.Fragment>
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
          {dataList.map((item, index) => {
            return (
              <tr key={index}>
                {tableColumns.map(column =>
                  column.key === 'isActive' ? (
                    <td key={column.key}>
                      {item[column.key] ? (
                        <Badge color="success">Active</Badge>
                      ) : (
                        <Badge color="danger">Inactive</Badge>
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
    </React.Fragment>
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
  onEditClicked: PropTypes.func,
  onDeleteClicked: PropTypes.func,
  onHeadingSortClicked: PropTypes.func,
};

DataTableControl.defaultProps = {
  editable: false,
};

export default DataTableControl;
