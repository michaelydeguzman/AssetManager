import axios from 'axios';
import { headers, logout } from '@damtoken';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';

const API_URL = process.env.REACT_APP_API_BASE_URL;

export const getArchive = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_ARCHIVE,
			payload: {}
		});
		return axios
			.get(`${API_URL}Assets/Archive`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_ARCHIVE_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status == 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_ARCHIVE_REJECTED,
					payload: error
				});
			});
	};
};

export const updateAssetStatus = (data, status) => {
	return (dispatch) => {
		return axios
			.put(`${API_URL}Assets/Status`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.UPDATE_ASSET_STATUS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_ASSET_STATUS_FULFILLED, status);
			})
			.catch(function (error) {
				if (error.response && error.response.status == 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_ASSET_STATUS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_ASSET_STATUS_REJECTED, status);
			});
	};
};
