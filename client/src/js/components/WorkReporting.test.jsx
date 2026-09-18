import React, { forwardRef, useImperativeHandle } from 'react';
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import { MemoryRouter } from 'react-router-dom';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import WorkReporting from './WorkReporting';
import * as api from '../Api';

vi.mock('../Api', () => ({
  getSubmissionConfigurationNotices: vi.fn(),
}));

vi.mock('./WorkReportingUpload', () => ({
  default: ({ serviceArea }) => <div>Upload form for {serviceArea}</div>,
}));

vi.mock('./WorkReportingSubmissions', () => ({
  default: forwardRef(({ serviceArea }, ref) => {
    useImperativeHandle(ref, () => ({ refresh: vi.fn() }));
    return <div>Submissions for {serviceArea}</div>;
  }),
}));

const submissionStreams = {
  HMR_WORK_REPORT: {
    id: 11,
    name: 'MC Work Reporting',
    stagingTableName: 'HMR_WORK_REPORT',
    isActive: true,
  },
  HMR_ROCKFALL_REPORT: {
    id: 12,
    name: 'Rockfall Reporting',
    stagingTableName: 'HMR_ROCKFALL_REPORT',
    isActive: true,
  },
};

const renderPage = (permissions = ['FILE_R', 'WORK_REPORT_W']) => {
  const store = createStore(() => ({
    user: {
      current: {
        permissions,
        serviceAreas: [{ id: 1, name: '1 South Island' }],
      },
    },
    submissions: { streams: submissionStreams },
  }));

  return render(
    <Provider store={store}>
      <MemoryRouter initialEntries={['/workreporting']}>
        <WorkReporting />
      </MemoryRouter>
    </Provider>
  );
};

describe('WorkReporting submission notices', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    api.getSubmissionConfigurationNotices.mockResolvedValue({
      data: [
        {
          submissionConfigurationId: 41,
          phase: 'REMINDER',
          severity: 'WARNING',
          message: 'Submit qualifying reports by January 31.',
        },
      ],
    });
  });

  it('loads global notices before a service area is selected', async () => {
    renderPage();

    expect(await screen.findByText('Submit qualifying reports by January 31.')).toBeInTheDocument();
    expect(api.getSubmissionConfigurationNotices).toHaveBeenCalledWith(11, null);
    expect(screen.queryByText(/Upload form/)).not.toBeInTheDocument();
  });

  it('reloads notices for the selected service area and shows the upload form', async () => {
    const user = userEvent.setup();
    renderPage();
    await screen.findByText('Submit qualifying reports by January 31.');

    await user.click(screen.getByRole('button', { name: /Select Service Area/ }));
    await user.click(screen.getByText('1 South Island'));

    await waitFor(() => expect(api.getSubmissionConfigurationNotices).toHaveBeenLastCalledWith(11, 1));
    expect(screen.getByText('Upload form for 1')).toBeInTheDocument();
  });

  it('does not show an upload form when no active report type is permitted', async () => {
    const user = userEvent.setup();
    renderPage(['FILE_R']);
    await screen.findByText('Submit qualifying reports by January 31.');

    await user.click(screen.getByRole('button', { name: /Select Service Area/ }));
    await user.click(screen.getByText('1 South Island'));

    expect(screen.queryByText(/Upload form/)).not.toBeInTheDocument();
    expect(screen.getByText('Submissions for 1')).toBeInTheDocument();
  });
});
