import { createBrowserHistory } from 'history';

const base = import.meta.env.VITE_BASE_URL?.replace(/\/+$/, '') || '/';
export default createBrowserHistory({ basename: base });
