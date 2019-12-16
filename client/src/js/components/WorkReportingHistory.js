import React from 'react';
import { Link } from 'react-router-dom';

import * as Constants from '../Constants';

const WorkReportingHistory = () => {
  return (
    <div>
      WorkReportingHistory{' '}
      <div>
        <Link to={Constants.PATHS.WORK_REPORTING}>Back</Link>
      </div>
    </div>
  );
};

export default WorkReportingHistory;
