import _ from 'lodash';

import { FETCH_SUBMISSION_STATUSES, FETCH_SUBMISSION_STREAMS } from '../actions/types';

const defaultState = {
  statuses: {},
  streams: {},
};

const submissionReducer = (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_SUBMISSION_STATUSES:
      return { ...state, statuses: { ..._.mapKeys(action.payload, 'statusCode') } };
    case FETCH_SUBMISSION_STREAMS:
      return {
        ...state,
        streams: {
          ..._.mapKeys(
            action.payload.map((stream) => ({
              ...stream,
              fileSizeLimitMb: stream.fileSizeLimit / 1024 / 1024,
            })),
            'stagingTableName'
          ),
        },
      };
    default:
      return state;
  }
};

export default submissionReducer;