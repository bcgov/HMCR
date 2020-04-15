import React from 'react';
import PropTypes from 'prop-types';
import { Input, Pagination, PaginationItem, PaginationLink } from 'reactstrap';

import * as Constants from '../../Constants';

const PaginationControl = ({
  currentPage,
  pageCount,
  onPageChange,
  pageSize,
  pageSizeOptions,
  onPageSizeChange,
  totalCount,
  itemCount,
}) => {
  const pageItems = [];

  for (let i = 1; i <= pageCount; i++) {
    if (i < currentPage - 5 && pageCount - i > 10) continue;
    if (i > currentPage + 5 && pageItems.length > 10) continue;

    pageItems.push(
      <PaginationItem key={i} active={i === currentPage}>
        <PaginationLink onClick={() => onPageChange(i)}>{i}</PaginationLink>
      </PaginationItem>
    );
  }

  const handlePageSizeChange = (e) => onPageSizeChange(e.target.value);

  return (
    <div style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'space-between' }}>
      <span>{`${(currentPage - 1) * pageSize + 1} - ${
        (currentPage - 1) * pageSize + itemCount
      } of ${totalCount}`}</span>
      {!(pageCount <= 1 && pageSize === pageSizeOptions[0]) && (
        <Pagination size="sm" aria-label="Pagination">
          <Input
            bsSize="sm"
            type="select"
            name="select"
            className="mr-2 ml-2"
            style={{ width: '100px' }}
            onChange={handlePageSizeChange}
            value={pageSize}
          >
            {pageSizeOptions.map((count) => (
              <option key={count} value={count}>{`Show ${count}`}</option>
            ))}
          </Input>
          <PaginationItem disabled={currentPage <= 1}>
            <PaginationLink first onClick={() => onPageChange(1)} />
          </PaginationItem>
          <PaginationItem disabled={currentPage <= 1}>
            <PaginationLink previous onClick={() => onPageChange(currentPage - 1)} />
          </PaginationItem>
          {pageItems}
          <PaginationItem disabled={currentPage >= pageCount}>
            <PaginationLink next onClick={() => onPageChange(currentPage + 1)} />
          </PaginationItem>
          <PaginationItem disabled={currentPage >= pageCount}>
            <PaginationLink last onClick={() => onPageChange(pageCount)} />
          </PaginationItem>
        </Pagination>
      )}
    </div>
  );
};

PaginationControl.propTypes = {
  currentPage: PropTypes.number.isRequired,
  pageCount: PropTypes.number.isRequired,
  onPageChange: PropTypes.func.isRequired,
  pageSize: PropTypes.number.isRequired,
  onPageSizeChange: PropTypes.func.isRequired,
  pageSizeOptions: PropTypes.arrayOf(PropTypes.number),
};

PaginationControl.defaultProps = {
  currentPage: 1,
  pageSize: Constants.DEFAULT_PAGE_SIZE,
  pageSizeOptions: Constants.DEFAULT_PAGE_SIZE_OPTIONS,
};

export default PaginationControl;
