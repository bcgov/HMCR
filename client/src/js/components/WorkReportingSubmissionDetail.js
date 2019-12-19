import React from 'react';
import { Link } from 'react-router-dom';

import * as Constants from '../Constants';

const WorkReportingSubmissionDetail = () => {
  return (
    <div>
      WorkReportingSubmissionDetail{' '}
      <div>
        <Link to={Constants.PATHS.WORK_REPORTING}>Back</Link>
      </div>
    </div>
  );
};

export default WorkReportingSubmissionDetail;
