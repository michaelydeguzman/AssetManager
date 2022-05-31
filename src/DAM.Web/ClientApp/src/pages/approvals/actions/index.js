import axios from 'axios';
import { headers, logout } from '@damtoken';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';

const API_URL = process.env.REACT_APP_API_BASE_URL;

export const getApprovals = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_APPROVALS,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Assets/Approvals`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_APPROVALS_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				//logout();
				dispatch({
					type: TYPE.GET_APPROVALS_REJECTED,
					payload: error
				});
			});
	};
};

export const submitApprovals = (data, isApproved) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Approval/Update`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_APPROVALS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_APPROVALS_FULFILLED, isApproved);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_APPROVALS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_APPROVALS_REJECTED);
			});
	};
};
