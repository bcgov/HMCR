import { describe, expect, it } from 'vitest';

import { getUploadableSubmissionStreams } from './utils';

const streams = {
  work: { id: 1, stagingTableName: 'HMR_WORK_REPORT', isActive: true },
  rockfall: { id: 2, stagingTableName: 'HMR_ROCKFALL_REPORT', isActive: true },
  inactive: { id: 3, stagingTableName: 'HMR_WILDLIFE_REPORT', isActive: false },
};

describe('getUploadableSubmissionStreams', () => {
  it('uses WORK_REPORT_W only for Work Reports', () => {
    expect(getUploadableSubmissionStreams(streams, ['WORK_REPORT_W']).map((stream) => stream.id)).toEqual([1]);
  });

  it('uses FILE_W only for non-Work Report streams', () => {
    expect(getUploadableSubmissionStreams(streams, ['FILE_W']).map((stream) => stream.id)).toEqual([2]);
  });

  it('returns all active permitted streams when the user has both permissions', () => {
    expect(
      getUploadableSubmissionStreams(streams, ['WORK_REPORT_W', 'FILE_W']).map((stream) => stream.id)
    ).toEqual([1, 2]);
  });
});
