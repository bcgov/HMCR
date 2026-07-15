import React from 'react';

import { createSupportId, reportClientError } from '../Api';

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, supportId: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    const supportId = createSupportId();
    this.setState({ supportId });

    reportClientError(error, {
      supportId,
      componentStack: errorInfo?.componentStack,
    });
  }

  render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return (
        <div className="container mt-5" style={{ maxWidth: '640px' }}>
          <h1>Something went wrong.</h1>
          <p>
            The page encountered an unexpected error and could not be displayed. This is a problem with the
            application, not with anything you submitted.
          </p>
          <p>
            Please reload the page and try again. If the problem continues, contact the administrator and describe what
            you were doing when the error occurred.
          </p>
          {this.state.supportId && (
            <p>
              <strong>Support ID:</strong> <code style={{ wordBreak: 'break-all' }}>{this.state.supportId}</code>
            </p>
          )}
          <button type="button" className="btn btn-primary" onClick={() => window.location.reload()}>
            Reload Page
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
