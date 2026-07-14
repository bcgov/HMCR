import React from 'react';

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    console.log(error);
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    // You can also log the error to an error reporting service
    //logErrorToMyService(error, errorInfo);
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
