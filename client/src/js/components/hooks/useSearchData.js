import { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import _ from 'lodash';
import moment from 'moment';

import { updateQueryParamsFromHistory } from '../../utils';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const useSearchData = defaultSearchOptions => {
  const history = useHistory();
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
  const [searchOptions, setSearchOptions] = useState(null);
  const [refreshTrigger, setRefreshTrigger] = useState(null);

  const updateSearchOptions = options => {
    if (!options.pageNumber) options.pageNumber = 1;
    if (!options.pageSize) options.pageSize = Constants.DEFAULT_PAGE_SIZE;

    setSearchOptions(options);
  };

  const handleChangePage = newPage => {
    const options = { ...searchOptions, pageNumber: newPage };
    setSearchOptions(options);
  };

  const handleChangePageSize = newSize => {
    const options = { ...searchOptions, pageNumber: 1, pageSize: newSize };
    setSearchOptions(options);
  };

  const handleHeadingSortClicked = headingKey => {
    const direction =
      !searchOptions.direction || searchOptions.direction === Constants.SORT_DIRECTION.ASCENDING
        ? Constants.SORT_DIRECTION.DESCENDING
        : Constants.SORT_DIRECTION.ASCENDING;

    const options = { ...searchOptions, pageNumber: 1, orderBy: headingKey, direction };
    setSearchOptions(options);
  };

  const refresh = reset => {
    if (reset === true) {
      updateSearchOptions(defaultSearchOptions);

      if (history) {
        history.push(
          `?${updateQueryParamsFromHistory(
            history,
            _.omit(defaultSearchOptions, ['serviceAreaNumber', 'dataPath']),
            true
          )}`
        );
      }
    } else {
      setRefreshTrigger(Math.random());
    }
  };

  useEffect(() => {
    const updateHistoryLocationSearch = () => {
      if (!history) {
        console.warn('userSearchData: history object is null, skipping updating query params');
        return;
      }

      history.push(
        `?${updateQueryParamsFromHistory(history, _.omit(searchOptions, ['serviceAreaNumber', 'dataPath']))}`
      );
    };

    const loadData = () => {
      if (searchOptions === null) return;

      const dataPath = searchOptions.dataPath;
      const options = { ...searchOptions };

      // convert moment objects to string
      Object.keys(options).forEach(key => {
        if (moment.isMoment(options[key])) {
          if (key === 'dateTo') {
            options[key] = moment(options[key])
              .endOf('day')
              .format(Constants.DATE_UTC_FORMAT);

            return;
          } else if (key === 'dateFrom') {
            options[key] = moment(options[key])
              .startOf('day')
              .format(Constants.DATE_UTC_FORMAT);

            return;
          }

          options[key] = moment(options[key]).format(Constants.DATE_UTC_FORMAT);
        }
      });

      if (!options.pageSize) options.pageSize = Constants.DEFAULT_PAGE_SIZE;

      if (!options.pageNumber) options.pageNumber = 1;

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
    updateHistoryLocationSearch();
  }, [searchOptions, refreshTrigger, history]);

  return {
    data,
    pagination,
    loading,
    searchOptions,
    updateSearchOptions,
    handleChangePage,
    handleChangePageSize,
    handleHeadingSortClicked,
    refresh,
  };
};

export default useSearchData;
