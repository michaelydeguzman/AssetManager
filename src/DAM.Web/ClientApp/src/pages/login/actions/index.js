import axios from 'axios';
import { setToken, setUser, initialAppMode } from '@damtoken';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';
import { history } from '../../../utilities/history';

export const submitLogin = (data) => {
	return (dispatch) => {
		console.log(dispatch);
		dispatch({
			type: TYPE.POST_LOGIN_DATA,
			payload: {}
		});
		return axios
			.post(`${window.location.origin}/api/App/Login`, { ...data })
			.then(function (response) {
				if (response.data) {
					setToken(response.data.token);
					setUser(response.data.user);
					initialAppMode();

					dispatch({
						type: TYPE.POST_LOGIN_DATA_FULFILLED,
						payload: response.data
					});
					FeedbackMessage(TYPE.POST_LOGIN_DATA_FULFILLED);
				} else {
					dispatch({
						type: TYPE.POST_LOGIN_DATA_REJECTED,
						payload: response.data
					});
					FeedbackMessage(TYPE.POST_LOGIN_DATA_REJECTED, 'Error!');
					history.push('/');
				}
			})
			.catch(function (error) {
				if (error && error.response) {
					dispatch({
						type: TYPE.POST_LOGIN_DATA_REJECTED,
						payload: error.response.data
					});
					FeedbackMessage(TYPE.POST_LOGIN_DATA_REJECTED, error.response.data.message);
					history.push('/');
				} else {
					dispatch({
						type: TYPE.POST_LOGIN_DATA_REJECTED,
						payload: error
					});
					FeedbackMessage(TYPE.POST_LOGIN_DATA_REJECTED, error);
					history.push('/');
				}
			});
	};
};

export const sendRequestPasswordRequest = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.POST_REQ_PW_DATA,
			payload: {}
		});
		return axios
			.post(`${window.location.origin}/api/App/Users/ForgotPassword`, { ...data })
			.then(function (response) {
				dispatch({
					type: TYPE.POST_REQ_PW_DATA_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.POST_REQ_PW_DATA_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.POST_REQ_PW_DATA_REJECTED,
					payload: error.response.data
				});
				FeedbackMessage(TYPE.POST_REQ_PW_DATA_REJECTED);
			});
	};
};

export const sendRequestResendConfirm = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.POST_REQ_RES_DATA,
			payload: {}
		});
		return axios
			.post(`${window.location.origin}/api/App/Users/ResendVerification`, { ...data })
			.then(function (response) {
				dispatch({
					type: TYPE.POST_REQ_RES_DATA_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.POST_REQ_RES_DATA_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.POST_REQ_RES_DATA_REJECTED,
					payload: error.response.data
				});
				FeedbackMessage(TYPE.POST_REQ_RES_DATA_REJECTED, error.response.data ? (error.response.data.message ? error.response.data.message : error.response.data) : '');
			});
	};
};