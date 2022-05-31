const initialData = {
	loggingIn: false,
	loggedIn: false,
	loginRejected: false,
	loggedData: {
		user: {
			username: '',
			email: ''
		}
	},
	message: '',

	requestingPassword: false,
	requestPasswordSuccess: false,
	requestPasswordFailed: false,
	requestPasswordData: {
		requestPasswordDto: {
			email: ''
		}
	},
	requestMessage: ''
};

export default function login(state = initialData, action) {
	switch (action.type) {
		case 'POST_LOGIN_DATA':
			return {
				...state,
				loggingIn: true,
				loggedIn: false,
				loginRejected: false
			};
		case 'POST_LOGIN_DATA_FULFILLED':
			return {
				...state,
				loggingIn: false,
				loggedIn: true,
				loggedData: action.payload
			};
		case 'POST_LOGIN_DATA_REJECTED':
			return {
				...state,
				loggingIn: false,
				loginRejected: true,
				message: action.payload.message
			};
		case 'POST_REQ_PW_DATA':
			return {
				...state,
				requestingPassword: true,
				requestPasswordSuccess: false,
				requestPasswordFailed: false
			};
		case 'POST_REQ_PW_DATA_FULFILLED':
			return {
				...state,
				requestingPassword: false,
				requestPasswordSuccess: true,
				requestPasswordData: action.payload
			};
		case 'POST_REQ_PW_DATA_REJECTED':
			return {
				...state,
				requestingPassword: false,
				requestPasswordFailed: true,
				requestMessage: action.payload.message
			};
		default:
			return {
				...state
			};
	}
}
