const initialData = {
	resetting: false,
	resetPasswordSuccess: false,
	resetPasswordFailed: false,
	resetPasswordData: {
		user: {
			email: '',
			password: '',
			token: ''
		}
	},
	message: ''
};

export default function resetPassword(state = initialData, action) {
	switch (action.type) {
		case 'POST_RESET_PASSWORD_DATA':
			return {
				...state,
				resetting: true,
				resetPasswordSuccess: false,
				resetPasswordFailed: false
			};
		case 'POST_RESET_PASSWORD_DATA_FULFILLED':
			return {
				...state,
				resetting: false,
				resetPasswordSuccess: true,
				resetPasswordFailed: action.payload
			};
		case 'POST_RESET_PASSWORD_DATA_REJECTED':
			return {
				...state,
				resetting: false,
				resetPasswordFailed: true,
				message: action.payload.message
			};
		default:
			return {
				...state
			};
	}
}
