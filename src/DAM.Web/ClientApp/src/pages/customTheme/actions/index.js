import axios from 'axios';
import { headers, logout } from '@damtoken';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';

export const getThemeStyles = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_STYLES,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Styles`)
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_STYLES_FULFILLED,
						payload: response.data
					});
				}
				return response.data.styles;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.GET_STYLES_REJECTED,
					payload: error
				});
			});
	};
};

export const saveThemeStyle = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Styles/Save`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_STYLES_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_FULFILLED, true);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_STYLES_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_REJECTED);
			});
	};
};

export const addThemeStyle = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Styles/Add`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_STYLES_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_FULFILLED, true);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_STYLES_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_REJECTED);
			});
	};
};

export const copyThemeStyle = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Styles/Copy`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_STYLES_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_FULFILLED, true);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_STYLES_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_REJECTED);
			});
	};
};

export const deleteThemeStyle = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Styles/Delete`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_STYLES_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_FULFILLED, true);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_STYLES_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_STYLES_REJECTED);
			});
	};
};

export const getLogos = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Logos`)
			.then(function (response) {
				console.log('response', response);
				if (response.data) {
					dispatch({
						type: TYPE.GET_LOGOS_FULFILLED,
						payload: response.data
					});
				}
				return response.data.logos;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.GET_LOGOS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.GET_LOGOS_REJECTED);
			});
	};
};

export const saveLogo = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Logos/Save`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_LOGOS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_LOGOS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				logout();
				dispatch({
					type: TYPE.UPDATE_LOGOS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_LOGOS_REJECTED);
			});
	};
};
