import axios from 'axios';
import { history } from '../../../utilities/history';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';

const API_URL = process.env.REACT_APP_API_BASE_URL;

export const submitCreatePassword = (data) => {
	return (dispatch) => {
		console.log(dispatch);
		dispatch({
			type: TYPE.POST_CREATE_PASSWORD_DATA,
			payload: {}
		});
		return axios
			.post(`${window.location.origin}/admin/api/Users/CreatePassword`, { ...data })
			.then(function (response) {
				dispatch({
					type: TYPE.POST_CREATE_PASSWORD_DATA_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.POST_CREATE_PASSWORD_DATA_FULFILLED);
				history.push('/login');
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.POST_CREATE_PASSWORD_DATA_REJECTED,
					payload: error.response.data
				});
				FeedbackMessage(TYPE.POST_CREATE_PASSWORD_DATA_REJECTED, error.response.data.message);
			});
	};
};
