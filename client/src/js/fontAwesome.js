import { library } from '@fortawesome/fontawesome-svg-core';
import { faCalendarAlt, faEdit, faTrashAlt } from '@fortawesome/free-solid-svg-icons';

const addIconsToLibrary = () => {
  library.add(faCalendarAlt);
  library.add(faEdit);
  library.add(faTrashAlt);
};

export default addIconsToLibrary;
