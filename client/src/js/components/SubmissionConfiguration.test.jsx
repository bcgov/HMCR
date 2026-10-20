import React from 'react';
import { act, render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import SubmissionConfiguration from './SubmissionConfiguration';
import * as api from '../Api';
import { toast } from 'react-toastify';

vi.mock('../Api', () => ({
  getSubmissionConfigurations: vi.fn(),
  putSubmissionConfigurationActivation: vi.fn(),
}));

vi.mock('react-toastify', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}));

const configuration = {
  submissionConfigurationId: 41,
  configurationKey: 'CAPITALIZED_HARD_SURFACING',
  name: 'Capitalized Hard Surfacing Works',
  submissionStreamId: 11,
  submissionStreamName: 'MC Work Reporting',
  isActive: true,
  effectiveStartDate: '2027-01-01',
  effectiveEndDate: null,
  timeZone: 'America/Vancouver',
  scopeType: 'GLOBAL',
  currentState: 'BLACKOUT',
  generatedNotice: 'Submission blackout in effect.',
  noticeSeverity: 'DANGER',
  currentWindowStartDate: '2027-02-01',
  currentWindowEndDate: '2027-03-31',
  nextBlackoutStartDate: '2028-02-01',
  nextBlackoutEndDate: '2028-03-31',
  concurrencyControlNumber: 3,
  lastUpdatedBy: 'HMR_DEV',
  lastUpdatedTimestamp: '2026-09-16T12:00:00Z',
  windows: [
    {
      submissionConfigWindowId: 1,
      windowType: 'REMINDER',
      startMonth: 1,
      startDay: 1,
      endMonth: 1,
      endDay: 31,
    },
    {
      submissionConfigWindowId: 2,
      windowType: 'BLACKOUT',
      startMonth: 2,
      startDay: 1,
      endMonth: 3,
      endDay: 31,
    },
  ],
  rules: [
    {
      submissionConfigRuleId: 1,
      ruleType: 'ACTIVITY_ACCOMPLISHMENT',
      displayLabel: 'Asphalt paving activities',
      comparisonOperator: 'GT',
      thresholdValue: 450,
      unitOfMeasure: 'tonne',
      activities: [
        { activityCodeId: 1, activityNumber: '101300', activityName: 'Overlay Patch' },
        { activityCodeId: 2, activityNumber: '933300', activityName: 'Reclaimed Asphalt Pavement' },
      ],
    },
    {
      submissionConfigRuleId: 2,
      ruleType: 'ACTIVITY_ACCOMPLISHMENT',
      displayLabel: 'Graded aggregate seal activities',
      comparisonOperator: 'GTE',
      thresholdValue: 7000,
      unitOfMeasure: 'm2',
      activities: [
        { activityCodeId: 3, activityNumber: '102300', activityName: 'Graded Aggregate Seal' },
      ],
    },
  ],
  audiences: [{ audienceType: 'USER_TYPE', audienceValue: 'BUSINESS' }],
  serviceAreaNumbers: [],
};

const createDeferred = () => {
  let resolve;
  let reject;
  const promise = new Promise((promiseResolve, promiseReject) => {
    resolve = promiseResolve;
    reject = promiseReject;
  });

  return { promise, resolve, reject };
};

describe('SubmissionConfiguration', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    api.getSubmissionConfigurations.mockResolvedValue({ data: [configuration] });
  });

  it('shows the server-calculated state, schedule, banner, and currently blocked rules', async () => {
    render(<SubmissionConfiguration />);

    expect(await screen.findByRole('heading', { name: configuration.name })).toBeInTheDocument();
    expect(screen.getByText('Blackout active')).toBeInTheDocument();
    expect(screen.getByText('All service areas')).toBeInTheDocument();
    expect(screen.getByText('Maintenance Contractors')).toBeInTheDocument();
    expect(screen.getByText('January 1 to January 31')).toBeInTheDocument();
    expect(screen.getByText('February 1 to March 31')).toBeInTheDocument();
    expect(screen.getByText('Submission blackout in effect.')).toBeInTheDocument();
    expect(screen.getAllByText('More than 450 tonnes')).toHaveLength(2);
    expect(screen.getAllByText('At least 7,000 square metres')).toHaveLength(2);
  });

  it('confirms deactivation and replaces the configuration with the API response', async () => {
    const user = userEvent.setup();
    const request = createDeferred();
    api.putSubmissionConfigurationActivation.mockReturnValueOnce(request.promise);
    render(<SubmissionConfiguration />);

    const activeSwitch = await screen.findByRole('checkbox', { name: 'Active' });
    await user.click(activeSwitch);

    expect(screen.getByRole('dialog')).toHaveTextContent(
      'Its submission validation and all associated banners will stop immediately.'
    );

    await user.click(screen.getByRole('button', { name: 'Confirm' }));
    expect(api.putSubmissionConfigurationActivation).toHaveBeenCalledWith(41, false, 3);
    expect(activeSwitch).not.toBeChecked();

    await act(async () => {
      request.resolve({
        data: { ...configuration, isActive: false, currentState: 'INACTIVE', concurrencyControlNumber: 4 },
      });
      await request.promise;
    });

    await waitFor(() => expect(screen.queryByRole('dialog')).not.toBeInTheDocument());
    expect(screen.getByRole('checkbox', { name: 'Inactive' })).not.toBeChecked();
    expect(screen.getByText('No submissions are currently blocked by this configuration.')).toBeInTheDocument();
    expect(toast.success).toHaveBeenCalledTimes(1);
  });

  it('restores the prior activation value when an update fails', async () => {
    const user = userEvent.setup();
    api.putSubmissionConfigurationActivation.mockRejectedValueOnce(new Error('Save failed'));
    render(<SubmissionConfiguration />);

    const activeSwitch = await screen.findByRole('checkbox', { name: 'Active' });
    await user.click(activeSwitch);
    await user.click(screen.getByRole('button', { name: 'Confirm' }));

    await waitFor(() => expect(screen.queryByRole('dialog')).not.toBeInTheDocument());
    expect(screen.getByRole('checkbox', { name: 'Active' })).toBeChecked();
    expect(toast.error).toHaveBeenCalledTimes(1);
  });
});
