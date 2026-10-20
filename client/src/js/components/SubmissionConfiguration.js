import React, { useCallback, useEffect, useState } from 'react';
import {
  Alert,
  Badge,
  Button,
  Input,
  Label,
  Modal,
  ModalBody,
  ModalFooter,
  ModalHeader,
  Spinner,
  Table,
} from 'reactstrap';
import { toast } from 'react-toastify';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';

import * as api from '../Api';

const EMPTY_VALUE = '—';
const MONTH_NAMES = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December',
];

const normalizeState = (state) => String(state || '').toUpperCase();

const formatState = (state) => {
  switch (normalizeState(state)) {
    case 'INACTIVE':
      return 'Inactive';
    case 'REMINDER':
      return 'Reminder active';
    case 'BLACKOUT':
      return 'Blackout active';
    case 'OUTSIDE_WINDOW':
      return 'Outside window';
    default:
      return state || EMPTY_VALUE;
  }
};

const formatWindowType = (windowType) => {
  switch (normalizeState(windowType)) {
    case 'REMINDER':
      return 'Reminder';
    case 'BLACKOUT':
      return 'Blackout';
    default:
      return windowType || EMPTY_VALUE;
  }
};

const stateColor = (state) => {
  switch (normalizeState(state)) {
    case 'BLACKOUT':
      return 'danger';
    case 'REMINDER':
      return 'warning';
    case 'INACTIVE':
      return 'secondary';
    default:
      return 'info';
  }
};

const noticeColor = (severity) => {
  switch (String(severity || '').toUpperCase()) {
    case 'DANGER':
    case 'ERROR':
      return 'danger';
    case 'WARNING':
      return 'warning';
    default:
      return 'info';
  }
};

const formatDate = (date) => (date ? String(date).substring(0, 10) : EMPTY_VALUE);

const formatDateTime = (dateTime) => {
  if (!dateTime) return EMPTY_VALUE;

  const parsedDate = new Date(dateTime);
  return Number.isNaN(parsedDate.getTime()) ? String(dateTime) : parsedDate.toLocaleString('en-CA');
};

const formatDateRange = (start, end) => {
  if (!start && !end) return EMPTY_VALUE;
  if (!end) return `From ${formatDate(start)}`;
  return `${formatDate(start)} to ${formatDate(end)}`;
};

const formatRecurringWindow = (window) => {
  const startMonth = MONTH_NAMES[Number(window.startMonth) - 1] || window.startMonth;
  const endMonth = MONTH_NAMES[Number(window.endMonth) - 1] || window.endMonth;
  return `${startMonth} ${window.startDay} to ${endMonth} ${window.endDay}`;
};

const formatOperator = (operator) => {
  switch (String(operator || '').toUpperCase()) {
    case 'GT':
      return 'More than';
    case 'GTE':
      return 'At least';
    case 'LT':
      return 'Less than';
    case 'LTE':
      return 'At most';
    case 'EQ':
      return 'Exactly';
    default:
      return operator || EMPTY_VALUE;
  }
};

const formatUnit = (unit) => {
  switch (String(unit || '').toLowerCase()) {
    case 'tonne':
    case 'tonnes':
      return 'tonnes';
    case 'm2':
    case 'm²':
    case 'square metres':
      return 'square metres';
    default:
      return unit || '';
  }
};

const formatThreshold = (rule) => {
  const numericThreshold = Number(rule.thresholdValue);
  const threshold = Number.isNaN(numericThreshold)
    ? rule.thresholdValue
    : numericThreshold.toLocaleString('en-CA');
  return `${formatOperator(rule.comparisonOperator)} ${threshold} ${formatUnit(rule.unitOfMeasure)}`.trim();
};

const formatActivities = (activities = []) =>
  activities
    .map((activity) =>
      [activity.activityNumber, activity.activityName].filter(Boolean).join(' – ')
    )
    .join(', ') || EMPTY_VALUE;

const formatAudiences = (audiences = []) =>
  audiences
    .map((audience) => {
      const value = audience.audienceValue || audience.audienceType;
      return String(value).toUpperCase() === 'BUSINESS' ? 'Maintenance Contractors' : value;
    })
    .filter(Boolean)
    .join(', ') || EMPTY_VALUE;

const formatScope = (configuration) =>
  String(configuration.scopeType).toUpperCase() === 'GLOBAL'
    ? 'All service areas'
    : (configuration.serviceAreaNumbers || []).join(', ') || 'Selected service areas';

const Detail = ({ label, children }) => (
  <div className="submission-configuration__detail">
    <dt>{label}</dt>
    <dd>{children ?? EMPTY_VALUE}</dd>
  </div>
);

const RulesTable = ({ rules, caption }) => (
  <Table responsive bordered size="sm" className="submission-configuration__rules mb-0">
    <caption className="visually-hidden">{caption}</caption>
    <thead>
      <tr>
        <th scope="col">Rule</th>
        <th scope="col">Activities</th>
        <th scope="col">Threshold</th>
      </tr>
    </thead>
    <tbody>
      {(rules || []).map((rule) => (
        <tr key={rule.submissionConfigRuleId || `${rule.displayLabel}-${rule.thresholdValue}`}>
          <th scope="row">{rule.displayLabel}</th>
          <td>{formatActivities(rule.activities)}</td>
          <td>{formatThreshold(rule)}</td>
        </tr>
      ))}
    </tbody>
  </Table>
);

const SubmissionConfiguration = () => {
  const [configurations, setConfigurations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState(false);
  const [savingIds, setSavingIds] = useState({});
  const [pendingChange, setPendingChange] = useState(null);

  const loadConfigurations = useCallback(() => {
    setLoading(true);
    setLoadError(false);

    return api
      .getSubmissionConfigurations()
      .then((response) => setConfigurations(response.data || []))
      .catch(() => setLoadError(true))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    loadConfigurations();
  }, [loadConfigurations]);

  const pendingSaving = Boolean(
    pendingChange && savingIds[pendingChange.configuration.submissionConfigurationId]
  );

  const replaceConfiguration = (updatedConfiguration) => {
    setConfigurations((current) =>
      current.map((configuration) =>
        configuration.submissionConfigurationId === updatedConfiguration.submissionConfigurationId
          ? updatedConfiguration
          : configuration
      )
    );
  };

  const confirmActivationChange = async () => {
    if (!pendingChange) return;

    const { configuration, isActive } = pendingChange;
    const id = configuration.submissionConfigurationId;
    const optimisticConfiguration = {
      ...configuration,
      isActive,
      currentState: isActive ? configuration.currentState : 'INACTIVE',
    };

    setSavingIds((current) => ({ ...current, [id]: true }));
    replaceConfiguration(optimisticConfiguration);

    try {
      const response = await api.putSubmissionConfigurationActivation(
        id,
        isActive,
        configuration.concurrencyControlNumber
      );
      replaceConfiguration(response.data);
      toast.success(
        <div className="text-center">
          {configuration.name} was {isActive ? 'activated' : 'deactivated'}.
        </div>
      );
    } catch (error) {
      replaceConfiguration(configuration);

      if (error.response?.status === 409) {
        toast.error(
          <div className="text-center">
            This configuration was changed by another user. The latest version has been loaded.
          </div>
        );
        await loadConfigurations();
      } else {
        toast.error(
          <div className="text-center">
            The configuration could not be updated. Its previous setting was restored.
          </div>
        );
      }
    } finally {
      setSavingIds((current) => {
        const next = { ...current };
        delete next[id];
        return next;
      });
      setPendingChange(null);
    }
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <UIHeader>Submission Configuration</UIHeader>
        <p className="mb-0">
          Review submission restrictions and activate or deactivate each configuration. Changes take effect
          immediately.
        </p>
      </MaterialCard>

      {loading && (
        <MaterialCard>
          <div className="text-center my-5" role="status">
            <Spinner color="primary" />
            <span className="visually-hidden">Loading submission configurations</span>
          </div>
        </MaterialCard>
      )}

      {!loading && loadError && (
        <MaterialCard>
          <Alert color="danger" role="alert">
            <p>Submission configurations could not be loaded.</p>
            <Button color="danger" outline size="sm" onClick={loadConfigurations}>
              Try again
            </Button>
          </Alert>
        </MaterialCard>
      )}

      {!loading && !loadError && configurations.length === 0 && (
        <MaterialCard>
          <Alert color="info" className="mb-0">
            No submission configurations are available.
          </Alert>
        </MaterialCard>
      )}

      {!loading &&
        !loadError &&
        configurations.map((configuration) => {
          const id = configuration.submissionConfigurationId;
          const isSaving = Boolean(savingIds[id]);
          const blackoutActive =
            configuration.isActive && normalizeState(configuration.currentState) === 'BLACKOUT';

          return (
            <MaterialCard key={id} aria-busy={isSaving}>
              <section aria-labelledby={`submission-configuration-${id}`}>
                <div className="submission-configuration__heading">
                  <div>
                    <h2 id={`submission-configuration-${id}`}>{configuration.name}</h2>
                    <Badge color={stateColor(configuration.currentState)}>
                      {formatState(configuration.currentState)}
                    </Badge>
                  </div>
                  <div className="form-check form-switch submission-configuration__toggle">
                    <Input
                      id={`submission-configuration-active-${id}`}
                      type="switch"
                      checked={Boolean(configuration.isActive)}
                      disabled={isSaving}
                      onChange={() =>
                        setPendingChange({ configuration, isActive: !configuration.isActive })
                      }
                    />
                    <Label for={`submission-configuration-active-${id}`}>
                      {configuration.isActive ? 'Active' : 'Inactive'}
                      {isSaving && <Spinner size="sm" className="ms-2" aria-label="Saving" />}
                    </Label>
                  </div>
                </div>

                <dl className="submission-configuration__details">
                  <Detail label="Configuration key">{configuration.configurationKey}</Detail>
                  <Detail label="Submission type">{configuration.submissionStreamName}</Detail>
                  <Detail label="Restricted audience">{formatAudiences(configuration.audiences)}</Detail>
                  <Detail label="Scope">{formatScope(configuration)}</Detail>
                  <Detail label="Effective dates">
                    {formatDateRange(configuration.effectiveStartDate, configuration.effectiveEndDate)}
                  </Detail>
                  <Detail label="Time zone">{configuration.timeZone}</Detail>
                  <Detail label="Current window">
                    {formatDateRange(
                      configuration.currentWindowStartDate,
                      configuration.currentWindowEndDate
                    )}
                  </Detail>
                  <Detail label="Next blackout">
                    {formatDateRange(
                      configuration.nextBlackoutStartDate,
                      configuration.nextBlackoutEndDate
                    )}
                  </Detail>
                </dl>

                <h3>Annual schedule</h3>
                <Table responsive bordered size="sm" className="submission-configuration__windows">
                  <caption className="visually-hidden">Annual windows for {configuration.name}</caption>
                  <thead>
                    <tr>
                      <th scope="col">Window</th>
                      <th scope="col">Recurring dates</th>
                    </tr>
                  </thead>
                  <tbody>
                    {(configuration.windows || []).map((window) => (
                      <tr key={window.submissionConfigWindowId || window.windowType}>
                        <th scope="row">{formatWindowType(window.windowType)}</th>
                        <td>{formatRecurringWindow(window)}</td>
                      </tr>
                    ))}
                  </tbody>
                </Table>

                <h3>Restricted activities</h3>
                <RulesTable
                  rules={configuration.rules}
                  caption={`Restricted activities for ${configuration.name}`}
                />

                <h3 className="mt-3">Banner preview</h3>
                {configuration.generatedNotice ? (
                  <Alert color={noticeColor(configuration.noticeSeverity)}>
                    {configuration.generatedNotice}
                  </Alert>
                ) : (
                  <Alert color="light">No banner is currently displayed.</Alert>
                )}

                <h3>Currently blocked</h3>
                {blackoutActive ? (
                  <RulesTable
                    rules={configuration.rules}
                    caption={`Rules currently blocked by ${configuration.name}`}
                  />
                ) : (
                  <Alert color="info">No submissions are currently blocked by this configuration.</Alert>
                )}

                <p className="text-muted mb-0 mt-3">
                  Last updated {formatDateTime(configuration.lastUpdatedTimestamp)} by{' '}
                  {configuration.lastUpdatedBy || EMPTY_VALUE}
                </p>
              </section>
            </MaterialCard>
          );
        })}

      <Modal
        isOpen={Boolean(pendingChange)}
        toggle={() => !pendingSaving && setPendingChange(null)}
        backdrop="static"
      >
        <ModalHeader toggle={pendingSaving ? undefined : () => setPendingChange(null)}>
          {pendingChange?.isActive ? 'Activate configuration' : 'Deactivate configuration'}
        </ModalHeader>
        <ModalBody>
          {pendingChange?.isActive ? (
            <p className="mb-0">
              Activate <strong>{pendingChange.configuration.name}</strong>? If the current date is within its
              blackout period, submission blocking and its banner will start immediately.
            </p>
          ) : (
            <p className="mb-0">
              Deactivate <strong>{pendingChange?.configuration.name}</strong>? Its submission validation and all
              associated banners will stop immediately.
            </p>
          )}
        </ModalBody>
        <ModalFooter>
          <Button color="secondary" size="sm" disabled={pendingSaving} onClick={() => setPendingChange(null)}>
            Cancel
          </Button>
          <Button
            color={pendingChange?.isActive ? 'primary' : 'danger'}
            size="sm"
            disabled={pendingSaving}
            onClick={confirmActivationChange}
          >
            {pendingSaving ? <Spinner size="sm" aria-label="Saving configuration" /> : 'Confirm'}
          </Button>
        </ModalFooter>
      </Modal>
    </React.Fragment>
  );
};

export default SubmissionConfiguration;
