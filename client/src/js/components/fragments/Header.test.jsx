import React from 'react';
import { Provider } from 'react-redux';
import { createStore } from 'redux';
import { MemoryRouter } from 'react-router-dom';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import Header from './Header';
import * as api from '../../Api';
import addIconsToLibrary from '../../fontAwesome';

vi.mock('../../Api', () => ({
  getVersion: vi.fn(),
}));

vi.mock('../../Keycloak', () => ({
  logout: vi.fn(),
}));

const renderHeader = (permissions) => {
  const store = createStore(() => ({
    user: {
      current: {
        username: 'ADMIN',
        userType: 'INTERNAL',
        permissions,
      },
    },
  }));

  return render(
    <Provider store={store}>
      <MemoryRouter>
        <Header />
      </MemoryRouter>
    </Provider>
  );
};

describe('Header submission configuration link', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.stubGlobal('localStorage', { setItem: vi.fn(), getItem: vi.fn() });
    addIconsToLibrary();
    api.getVersion.mockResolvedValue({ data: { environment: 'Development' } });
  });

  it('shows the profile menu link when the user has SUB_CONFIG_W', async () => {
    const user = userEvent.setup();
    renderHeader(['SUB_CONFIG_W']);

    await user.click(screen.getByRole('link', { name: 'User menu for ADMIN' }));

    expect(screen.getByRole('menuitem', { name: 'Submission Configuration' })).toHaveAttribute(
      'href',
      '/submission-configuration'
    );
  });

  it('hides the profile menu link without SUB_CONFIG_W', async () => {
    const user = userEvent.setup();
    renderHeader([]);

    await user.click(screen.getByRole('link', { name: 'User menu for ADMIN' }));

    expect(screen.queryByRole('menuitem', { name: 'Submission Configuration' })).not.toBeInTheDocument();
  });
});
