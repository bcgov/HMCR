import { library } from '@fortawesome/fontawesome-svg-core';
import {
  faCalendarAlt,
  faEdit,
  faTrashAlt,
  faCheckCircle,
  faBan,
  faSort,
  faTimesCircle,
  faExclamationCircle,
  faUser,
  faSync,
  faDownload,
  faCopy,
  faQuestionCircle,
  faEye,
  faEyeSlash,
} from '@fortawesome/free-solid-svg-icons';
import { faCheckCircle as farCheckCircle } from '@fortawesome/free-regular-svg-icons';

const addIconsToLibrary = () => {
  library.add(faCalendarAlt);
  library.add(faEdit);
  library.add(faTrashAlt);
  library.add(faCheckCircle);
  library.add(faTimesCircle);
  library.add(faExclamationCircle);
  library.add(faBan);
  library.add(faSort);
  library.add(faUser);
  library.add(faSync);
  library.add(farCheckCircle);
  library.add(faDownload);
  library.add(faCopy);
  library.add(faQuestionCircle);
  library.add(faEye);
  library.add(faEyeSlash);
};

export default addIconsToLibrary;
