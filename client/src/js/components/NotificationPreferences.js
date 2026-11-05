import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { connect } from 'react-redux';
import { Alert, Button, Input, Label, Spinner, Table } from 'reactstrap';
import { toast } from 'react-toastify';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';

import * as api from '../Api';
import * as Constants from '../Constants';

const SUPPORTED_REPORT_ORDER = [
  Constants.REPORT_TYPES.HMR_WORK_REPORT.name,
  Constants.REPORT_TYPES.HMR_ROCKFALL_REPORT.name,
  Constants.REPORT_TYPES.HMR_WILDLIFE_REPORT.name,
];

const EMPTY_VALUE = '—';

const getRowKey = (serviceAreaNumber, submissionStreamId) => `${serviceAreaNumber}:${submissionStreamId}`;

const updatePreferenceRow = (serviceAreas, serviceAreaNumber, submissionStreamId, update) =>
  serviceAreas.map((serviceArea) =>
    serviceArea.serviceAreaNumber !== serviceAreaNumber
      ? serviceArea
      : {
          ...serviceArea,
          reportTypes: serviceArea.reportTypes.map((reportType) =>
            reportType.submissionStreamId !== submissionStreamId ? reportType : update(reportType)
          ),
        }
  );

const sortPreferences = (serviceAreas = []) =>
  [...serviceAreas]
    .sort((left, right) => Number(left.serviceAreaNumber) - Number(right.serviceAreaNumber))
    .map((serviceArea) => ({
      ...serviceArea,
      reportTypes: [...(serviceArea.reportTypes || [])]
        .filter((reportType) => SUPPORTED_REPORT_ORDER.includes(reportType.stagingTableName))
        .sort(
          (left, right) =>
            SUPPORTED_REPORT_ORDER.indexOf(left.stagingTableName) -
            SUPPORTED_REPORT_ORDER.indexOf(right.stagingTableName)
        ),
    }));

const getRoleNames = (roles = []) =>
  roles
    .map((role) => (typeof role === 'string' ? role : role.name))
    .filter(Boolean)
    .join(', ') || EMPTY_VALUE;

const formatServiceArea = (serviceArea) => {
  const number = serviceArea.serviceAreaNumber ?? serviceArea.id;
  const name = serviceArea.serviceAreaName || serviceArea.name || '';

  if (name && String(name).trim().startsWith(`${number} `)) return name;
  return [number, name].filter((value) => value !== undefined && value !== null && value !== '').join(' ');
};

const getServiceAreaNames = (currentUser, preferenceServiceAreas) => {
  const serviceAreas = currentUser.serviceAreas?.length ? currentUser.serviceAreas : preferenceServiceAreas;

  return [...(serviceAreas || [])]
    .sort(
      (left, right) =>
        Number(left.serviceAreaNumber ?? left.id) - Number(right.serviceAreaNumber ?? right.id)
    )
    .map(formatServiceArea)
    .filter(Boolean)
    .join(', ') || EMPTY_VALUE;
};

const ProfileItem = ({ label, children }) => (
  <div className="notification-profile__item">
    <dt>{label}</dt>
    <dd>{children || EMPTY_VALUE}</dd>
  </div>
);

const NotificationPreferences = ({ currentUser }) => {
  const [serviceAreas, setServiceAreas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState(false);
  const [savingRows, setSavingRows] = useState({});
  const [bulkSaving, setBulkSaving] = useState(false);

  const loadPreferences = useCallback(() => {
    setLoading(true);
    setLoadError(false);

    return api
      .getCurrentUserNotificationPreferences()
      .then((response) => setServiceAreas(sortPreferences(response.data.serviceAreas || [])))
      .catch(() => setLoadError(true))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    loadPreferences();
  }, [loadPreferences]);

  const profileServiceAreas = useMemo(
    () => getServiceAreaNames(currentUser, serviceAreas),
    [currentUser, serviceAreas]
  );
  const hasPreferences = serviceAreas.some((serviceArea) => serviceArea.reportTypes.length > 0);
  const hasRowSaving = Object.keys(savingRows).length > 0;

  const showSavedToast = (message) => toast.success(<div className="text-center">{message}</div>);
  const showSaveErrorToast = () =>
    toast.error(
      <div className="text-center">Unable to save notification preferences. Your previous selection was restored.</div>
    );

  const handlePreferenceChange = async (serviceAreaNumber, reportType, field, enabled) => {
    const rowKey = getRowKey(serviceAreaNumber, reportType.submissionStreamId);
    if (bulkSaving || savingRows[rowKey]) return;

    const previousReportType = { ...reportType };
    const updatedReportType = { ...reportType, [field]: enabled };

    setSavingRows((current) => ({ ...current, [rowKey]: true }));
    setServiceAreas((current) =>
      updatePreferenceRow(current, serviceAreaNumber, reportType.submissionStreamId, () => updatedReportType)
    );

    try {
      await api.putCurrentUserNotificationPreference(serviceAreaNumber, reportType.submissionStreamId, {
        successfulUploadsEnabled: updatedReportType.successfulUploadsEnabled,
        uploadErrorsEnabled: updatedReportType.uploadErrorsEnabled,
      });
      showSavedToast('Notification preference saved.');
    } catch {
      setServiceAreas((current) =>
        updatePreferenceRow(current, serviceAreaNumber, reportType.submissionStreamId, () => previousReportType)
      );
      showSaveErrorToast();
    } finally {
      setSavingRows((current) => {
        const next = { ...current };
        delete next[rowKey];
        return next;
      });
    }
  };

  const handleBulkChange = async (enabled) => {
    if (bulkSaving || hasRowSaving || !hasPreferences) return;

    const previousServiceAreas = serviceAreas;
    setBulkSaving(true);
    setServiceAreas((current) =>
      current.map((serviceArea) => ({
        ...serviceArea,
        reportTypes: serviceArea.reportTypes.map((reportType) => ({
          ...reportType,
          successfulUploadsEnabled: enabled,
          uploadErrorsEnabled: enabled,
        })),
      }))
    );

    try {
      await api.putAllCurrentUserNotificationPreferences(enabled);
      showSavedToast(enabled ? 'All notification preferences enabled.' : 'All notification preferences disabled.');
    } catch {
      setServiceAreas(previousServiceAreas);
      showSaveErrorToast();
    } finally {
      setBulkSaving(false);
    }
  };

  const accountStatus =
    currentUser.accountStatus || (currentUser.isActive === false ? Constants.ACTIVE_STATUS.INACTIVE : Constants.ACTIVE_STATUS.ACTIVE);
  const displayName = [currentUser.firstName, currentUser.lastName].filter(Boolean).join(' ') || EMPTY_VALUE;
  const displayUserType = currentUser.userType === Constants.USER_TYPE.INTERNAL ? 'Internal (IDIR)' : currentUser.userType;

  return (
    <React.Fragment>
      <MaterialCard>
        <UIHeader>Notification Preferences</UIHeader>
        <section aria-labelledby="profile-heading">
          <h2 id="profile-heading">Profile</h2>
          <dl className="notification-profile">
            <ProfileItem label="Name">{displayName}</ProfileItem>
            <ProfileItem label="IDIR username">{currentUser.username}</ProfileItem>
            <ProfileItem label="Email address">{currentUser.email}</ProfileItem>
            <ProfileItem label="User type">{displayUserType}</ProfileItem>
            <ProfileItem label="Active roles">{getRoleNames(currentUser.roles)}</ProfileItem>
            <ProfileItem label="Assigned service areas">{profileServiceAreas}</ProfileItem>
            <ProfileItem label="Account status">{accountStatus}</ProfileItem>
          </dl>
        </section>
      </MaterialCard>

      <MaterialCard>
        <section aria-labelledby="email-preferences-heading">
          <div className="notification-preferences__heading">
            <div>
              <h2 id="email-preferences-heading">Email notifications</h2>
              <p className="mb-0">
                Choose which upload result emails you receive for each assigned service area and report type.
                Changes are saved automatically.
              </p>
            </div>
            <div
              className="notification-preferences__bulk-actions"
              role="group"
              aria-label="Set all email notifications"
            >
              <Button
                color="primary"
                size="sm"
                disabled={loading || loadError || bulkSaving || hasRowSaving || !hasPreferences}
                onClick={() => handleBulkChange(true)}
              >
                Enable all
              </Button>
              <Button
                color="secondary"
                size="sm"
                disabled={loading || loadError || bulkSaving || hasRowSaving || !hasPreferences}
                onClick={() => handleBulkChange(false)}
              >
                Disable all
              </Button>
            </div>
          </div>

          {loading && (
            <div className="text-center my-5" role="status">
              <Spinner color="primary" />
              <span className="visually-hidden">Loading notification preferences</span>
            </div>
          )}

          {!loading && loadError && (
            <Alert color="danger" role="alert">
              <p>Notification preferences could not be loaded.</p>
              <Button color="danger" outline size="sm" onClick={loadPreferences}>
                Try again
              </Button>
            </Alert>
          )}

          {!loading && !loadError && serviceAreas.length === 0 && (
            <Alert color="info">You do not have any assigned service areas.</Alert>
          )}

          {!loading && !loadError && serviceAreas.length > 0 && (
            <div aria-live="polite" aria-busy={bulkSaving}>
              {serviceAreas.map((serviceArea) => (
                <section
                  className="notification-service-area"
                  key={serviceArea.serviceAreaNumber}
                  aria-labelledby={`service-area-${serviceArea.serviceAreaNumber}`}
                >
                  <h3
                    className="notification-service-area__heading"
                    id={`service-area-${serviceArea.serviceAreaNumber}`}
                  >
                    Service Area {formatServiceArea(serviceArea)}
                  </h3>
                  <Table responsive className="notification-preferences__table">
                    <caption className="visually-hidden">
                      Email notification preferences for service area {formatServiceArea(serviceArea)}
                    </caption>
                    <thead>
                      <tr>
                        <th scope="col">Report type</th>
                        <th scope="col">Successful uploads</th>
                        <th scope="col">Upload errors</th>
                      </tr>
                    </thead>
                    <tbody>
                      {serviceArea.reportTypes.map((reportType) => {
                        const rowKey = getRowKey(
                          serviceArea.serviceAreaNumber,
                          reportType.submissionStreamId
                        );
                        const rowSaving = Boolean(savingRows[rowKey]);
                        const controlsDisabled = bulkSaving || rowSaving;
                        const successfulId = `notification-${serviceArea.serviceAreaNumber}-${reportType.submissionStreamId}-success`;
                        const errorsId = `notification-${serviceArea.serviceAreaNumber}-${reportType.submissionStreamId}-error`;

                        return (
                          <tr key={reportType.submissionStreamId} aria-busy={rowSaving}>
                            <th scope="row">
                              {reportType.reportTypeName}
                              {rowSaving && (
                                <span className="notification-preferences__saving" role="status">
                                  <Spinner size="sm" aria-hidden="true" />
                                  <span className="visually-hidden">Saving {reportType.reportTypeName}</span>
                                </span>
                              )}
                            </th>
                            <td>
                              <div className="notification-preferences__checkbox">
                                <Input
                                  id={successfulId}
                                  type="checkbox"
                                  checked={Boolean(reportType.successfulUploadsEnabled)}
                                  disabled={controlsDisabled}
                                  onChange={(event) =>
                                    handlePreferenceChange(
                                      serviceArea.serviceAreaNumber,
                                      reportType,
                                      'successfulUploadsEnabled',
                                      event.target.checked
                                    )
                                  }
                                />
                                <Label for={successfulId} className="visually-hidden">
                                  Successful upload emails for {reportType.reportTypeName}, service area{' '}
                                  {serviceArea.serviceAreaNumber}
                                </Label>
                              </div>
                            </td>
                            <td>
                              <div className="notification-preferences__checkbox">
                                <Input
                                  id={errorsId}
                                  type="checkbox"
                                  checked={Boolean(reportType.uploadErrorsEnabled)}
                                  disabled={controlsDisabled}
                                  onChange={(event) =>
                                    handlePreferenceChange(
                                      serviceArea.serviceAreaNumber,
                                      reportType,
                                      'uploadErrorsEnabled',
                                      event.target.checked
                                    )
                                  }
                                />
                                <Label for={errorsId} className="visually-hidden">
                                  Upload error emails for {reportType.reportTypeName}, service area{' '}
                                  {serviceArea.serviceAreaNumber}
                                </Label>
                              </div>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </Table>
                </section>
              ))}
            </div>
          )}
        </section>
      </MaterialCard>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => ({
  currentUser: state.user.current,
});

export default connect(mapStateToProps)(NotificationPreferences);
