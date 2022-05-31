import axios from 'axios';
import { headers, logout } from '@damtoken';
import { TYPE } from '../../messageTextConstants';

const API_URL = process.env.REACT_APP_API_BASE_URL;

export const getAuditTrails = (
	currentPageNumber,
	pageSize,
	sortOrder,
	sortColumnName,
	fileName = '',
	folderName = ''
) => {
	let defaultPageSize = false;
	if (pageSize === 0) {
		defaultPageSize = true;
	} else {
		defaultPageSize = false;
	}

	const config = {
		method: 'GET',
		url: `${API_URL}AuditTrail?currentPageNumber=${currentPageNumber}&sortOrder=${sortOrder}&pageSize=${
			defaultPageSize ? 10 : pageSize
		}&sortColumnName=${sortColumnName}&fileName=${fileName}&folderName=${folderName}`,
		headers: headers()
	};

	return (dispatch) => {
		dispatch({
			type: TYPE.GET_AUDITS,
			payload: {}
		});
		return axios(config)
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_AUDITS_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status == 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_AUDITS_REJECTED,
					payload: error
				});
			});
	};
};
