import { useState, useEffect } from 'react';
import _ from 'lodash';
import moment from 'moment';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const useSearchData = ({ defaultSearchOptions, refreshTrigger, history }) => {
  const [data, setData] = useState([]);
  const [pagination, setPagination] = useState({
    currentPage: null,
    pageSize: null,
    pageCount: null,
    hasPreviousPage: null,
    hasNextPage: null,
    totalCount: null,
  });
  const [loading, setLoading] = useState(false);
  const [searchOptions, setSearchOptions] = useState(defaultSearchOptions);

  useEffect(() => {
    const loadData = () => {
      const dataPath = searchOptions.dataPath;
      const options = { ...searchOptions };

      // convert moment objects to string
      Object.keys(options).forEach(key => {
        if (moment.isMoment(options[key])) options[key] = options[key].format(Constants.DATE_FORMAT);
      });

      setLoading(true);
      api.instance
        .get(dataPath, { params: { ..._.omit(options, ['dataPath']) } })
        .then(response => {
          setData(response.data.sourceList);

          const { hasPreviousPage, hasNextPage } = response.data;
          const pageNumber = parseInt(response.data.pageNumber);
          const pageSize = parseInt(response.data.pageSize);
          const totalCount = parseInt(response.data.totalCount);
          const pageCount = parseInt(response.data.pageCount);

          setPagination({ hasPreviousPage, hasNextPage, pageNumber, pageSize, totalCount, pageCount });
        })
        .finally(() => setLoading(false));
    };

    loadData();
  }, [searchOptions, refreshTrigger]);

  return { data, pagination, loading, searchOptions, setSearchOptions };
};

export default useSearchData;
