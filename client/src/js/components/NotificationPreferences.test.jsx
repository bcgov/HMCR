import React from 'react';
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import { act, render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import NotificationPreferences from './NotificationPreferences';
import * as api from '../Api';
import { toast } from 'react-toastify';

vi.mock('../Api', () => ({
  getCurrentUserNotificationPreferences: vi.fn(),
  putCurrentUserNotificationPreference: vi.fn(),
  putAllCurrentUserNotificationPreferences: vi.fn(),
}));

vi.mock('react-toastify', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}));

const currentUser = {
  firstName: 'Taylor',
  lastName: 'Jordan',
  username: 'TJORDAN',
  email: 'taylor.jordan@example.gov.bc.ca',
  userType: 'INTERNAL',
  roles: [{ id: 4, name: 'MoTI Staff', description: 'Ministry staff' }],
  serviceAreas: [
    { id: 2, serviceAreaName: 'Central Island', name: '2 Central Island' },
    { id: 1, serviceAreaName: 'South Island', name: '1 South Island' },
  ],
  isActive: true,
  accountStatus: 'ACTIVE',
};

const reportTypes = [
  {
    submissionStreamId: 13,
    reportTypeName: 'Wildlife Reporting',
    stagingTableName: 'HMR_WILDLIFE_REPORT',
    successfulUploadsEnabled: true,
    uploadErrorsEnabled: true,
  },
  {
    submissionStreamId: 11,
    reportTypeName: 'MC Work Reporting',
    stagingTableName: 'HMR_WORK_REPORT',
    successfulUploadsEnabled: true,
    uploadErrorsEnabled: true,
  },
  {
    submissionStreamId: 12,
    reportTypeName: 'Rockfall Reporting',
    stagingTableName: 'HMR_ROCKFALL_REPORT',
    successfulUploadsEnabled: true,
    uploadErrorsEnabled: true,
  },
];

const notificationResponse = {
  data: {
    serviceAreas: [
      {
        serviceAreaNumber: 2,
        serviceAreaName: 'Central Island',
        reportTypes: reportTypes.map((reportType) => ({ ...reportType })),
      },
      {
        serviceAreaNumber: 1,
        serviceAreaName: 'South Island',
        reportTypes: reportTypes.map((reportType) => ({ ...reportType })),
      },
    ],
  },
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

const renderPage = () => {
  const store = createStore(() => ({ user: { current: currentUser } }));
  return render(
    <Provider store={store}>
      <NotificationPreferences />
    </Provider>
  );
};

const findSuccessfulWorkCheckbox = () =>
  screen.findByRole('checkbox', {
    name: 'Successful upload emails for MC Work Reporting, service area 1',
  });

describe('NotificationPreferences', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    api.getCurrentUserNotificationPreferences.mockResolvedValue(notificationResponse);
  });

  it('groups all supported report rows beneath service areas in numeric order', async () => {
    renderPage();

    const serviceAreaHeadings = await screen.findAllByRole('heading', { level: 3 });
    expect(serviceAreaHeadings.map((heading) => heading.textContent)).toEqual([
      'Service Area 1 South Island',
      'Service Area 2 Central Island',
    ]);

    serviceAreaHeadings.forEach((heading) => {
      const serviceAreaSection = heading.closest('section');
      expect(within(serviceAreaSection).getAllByRole('rowheader').map((cell) => cell.textContent)).toEqual([
        'MC Work Reporting',
        'Rockfall Reporting',
        'Wildlife Reporting',
      ]);
      expect(within(serviceAreaSection).getAllByRole('checkbox')).toHaveLength(6);
    });

    expect(screen.getByText('Taylor Jordan')).toBeInTheDocument();
    expect(screen.getByText('MoTI Staff')).toBeInTheDocument();
  });

  it('optimistically saves an individual preference and keeps the row disabled until success', async () => {
    const user = userEvent.setup();
    const request = createDeferred();
    api.putCurrentUserNotificationPreference.mockReturnValueOnce(request.promise);
    renderPage();

    const successfulCheckbox = await findSuccessfulWorkCheckbox();
    const errorCheckbox = screen.getByRole('checkbox', {
      name: 'Upload error emails for MC Work Reporting, service area 1',
    });

    await user.click(successfulCheckbox);

    expect(successfulCheckbox).not.toBeChecked();
    expect(successfulCheckbox).toBeDisabled();
    expect(errorCheckbox).toBeDisabled();
    expect(api.putCurrentUserNotificationPreference).toHaveBeenCalledWith(1, 11, {
      successfulUploadsEnabled: false,
      uploadErrorsEnabled: true,
    });

    await act(async () => {
      request.resolve({ status: 204 });
      await request.promise;
    });

    await waitFor(() => expect(successfulCheckbox).toBeEnabled());
    expect(successfulCheckbox).not.toBeChecked();
    expect(toast.success).toHaveBeenCalledTimes(1);
    expect(toast.error).not.toHaveBeenCalled();
  });

  it('rolls an optimistic individual change back and shows an error when saving fails', async () => {
    const user = userEvent.setup();
    const request = createDeferred();
    api.putCurrentUserNotificationPreference.mockReturnValueOnce(request.promise);
    renderPage();

    const successfulCheckbox = await findSuccessfulWorkCheckbox();
    await user.click(successfulCheckbox);

    expect(successfulCheckbox).not.toBeChecked();
    expect(successfulCheckbox).toBeDisabled();

    await act(async () => {
      request.reject(new Error('Save failed'));
      try {
        await request.promise;
      } catch {
        // The component handles and reports the rejected request.
      }
    });

    await waitFor(() => expect(successfulCheckbox).toBeEnabled());
    expect(successfulCheckbox).toBeChecked();
    expect(toast.error).toHaveBeenCalledTimes(1);
    expect(toast.success).not.toHaveBeenCalled();
  });

  it('optimistically disables and enables every preference with the bulk controls', async () => {
    const user = userEvent.setup();
    const disableRequest = createDeferred();
    const enableRequest = createDeferred();
    api.putAllCurrentUserNotificationPreferences
      .mockReturnValueOnce(disableRequest.promise)
      .mockReturnValueOnce(enableRequest.promise);
    renderPage();

    await findSuccessfulWorkCheckbox();
    const checkboxes = screen.getAllByRole('checkbox');

    await user.click(screen.getByRole('button', { name: 'Disable all' }));

    checkboxes.forEach((checkbox) => {
      expect(checkbox).not.toBeChecked();
      expect(checkbox).toBeDisabled();
    });
    expect(api.putAllCurrentUserNotificationPreferences).toHaveBeenLastCalledWith(false);

    await act(async () => {
      disableRequest.resolve({ status: 204 });
      await disableRequest.promise;
    });
    await waitFor(() => checkboxes.forEach((checkbox) => expect(checkbox).toBeEnabled()));

    await user.click(screen.getByRole('button', { name: 'Enable all' }));

    checkboxes.forEach((checkbox) => {
      expect(checkbox).toBeChecked();
      expect(checkbox).toBeDisabled();
    });
    expect(api.putAllCurrentUserNotificationPreferences).toHaveBeenLastCalledWith(true);

    await act(async () => {
      enableRequest.resolve({ status: 204 });
      await enableRequest.promise;
    });
    await waitFor(() => checkboxes.forEach((checkbox) => expect(checkbox).toBeEnabled()));
    expect(toast.success).toHaveBeenCalledTimes(2);
  });
});
