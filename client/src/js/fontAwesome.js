import { library } from '@fortawesome/fontawesome-svg-core';
import { faCalendarAlt, faEdit, faTrashAlt, faCheckCircle, faBan, faSort } from '@fortawesome/free-solid-svg-icons';

const addIconsToLibrary = () => {
  library.add(faCalendarAlt);
  library.add(faEdit);
  library.add(faTrashAlt);
  library.add(faCheckCircle);
  library.add(faBan);
  library.add(faSort);
};

export default addIconsToLibrary;
