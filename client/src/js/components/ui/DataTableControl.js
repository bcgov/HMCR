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
  editKey,
  deletable,
  onEditClicked,
  onDeleteClicked,
  onHeadingSortClicked,
  showExportButton,
  onExportClicked,
}) => {
  const handleEditClicked = (id) => {
    if (onEditClicked) onEditClicked(id);
  };

  const editControls = (item) => (
    <React.Fragment>
      <FontAwesomeButton
        icon="edit"
        className="me-1"
        onClick={() => handleEditClicked(item[editKey])}
        title="Edit Record"
      />
      {deletable && (
        <DeleteButton
          itemId={item.id}
          buttonId={`item_${item.id}_delete`}
          defaultEndDate={item.endDate}
          onDeleteClicked={onDeleteClicked}
          permanentDelete={item.canDelete}
          title={item.canDelete ? 'Delete Record' : 'Disable Record'}
        ></DeleteButton>
      )}
    </React.Fragment>
  );

  return (
    <React.Fragment>
      <Table size="sm" responsive hover>
        <thead className="thead-dark">
          <tr>
            {tableColumns.map((column) => {
              let style = { whiteSpace: 'nowrap' };

              if (column.maxWidth) style.maxWidth = column.maxWidth;

              return (
                <th key={column.heading} style={style}>
                  {column.heading}
                  {!column.nosort && <FontAwesomeButton className="no-border" icon="sort" onClick={() => onHeadingSortClicked(column.key)} />}
                </th>
              );
            })}
            {editable && !showExportButton && (
              <Authorize requires={editPermissionName}>
                <th></th>
              </Authorize>
            )}
            {showExportButton && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {dataList.map((item, index) => {
            return (
              <tr key={index}>
                {tableColumns.map((column) => {
                  if (column.key === 'isActive')
                    return (
                      <td key={column.key}>
                        {item[column.key] ? (
                          <Badge color="success">Active</Badge>
                        ) : (
                          <Badge color="danger">Inactive</Badge>
                        )}
                      </td>
                    );

                  let style = { position: 'relative' };
                  if (column.maxWidth) {
                    style.maxWidth = column.maxWidth;
                  }

                  return (
                    <td key={column.key} className={column.maxWidth ? 'text-overflow-hiden' : ''} style={style}>
                      {column.format ? column.format(item[column.key]) : item[column.key]}
                    </td>
                  );
                })}
                {editable && !showExportButton && (
                  <Authorize requires={editPermissionName}>
                    <td style={{ width: '1%', whiteSpace: 'nowrap' }}>
                      {editControls(item)}
                    </td>
                  </Authorize>
                )}
                {showExportButton && (
                  <td style={{ width: '1%', whiteSpace: 'nowrap' }}>
                    {editable && <Authorize requires={editPermissionName}>{editControls(item)}</Authorize>}
                    <FontAwesomeButton
                      icon="download"
                      className="me-1"
                      onClick={() => onExportClicked(item.saltReportId)}
                      title="Export Report"
                    />
                  </td>
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
      format: PropTypes.func
    })
  ).isRequired,
  editable: PropTypes.bool.isRequired,
  editPermissionName: PropTypes.string,
  editKey: PropTypes.string,
  deletable: PropTypes.bool,
  onEditClicked: PropTypes.func,
  onDeleteClicked: PropTypes.func,
  onHeadingSortClicked: PropTypes.func,
};

DataTableControl.defaultProps = {
  editable: false,
  editKey: 'id',
  deletable: true,
};

export default DataTableControl;
