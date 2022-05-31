const initialData = {
	creatingPassword: false,
	createPasswordSuccess: false,
	createPasswordFailed: false,
	createPasswordData: {
		user: {
			email: '',
			password: ''
		}
	},
	message: ''
};

export default function createPassword(state = initialData, action) {
	switch (action.type) {
		case 'POST_CREATE_PASSWORD_DATA':
			return {
				...state,
				creatingPassword: true,
				createPasswordSuccess: false,
				createPasswordFailed: false
			};
		case 'POST_CREATE_PASSWORD_DATA_FULFILLED':
			return {
				...state,
				creatingPassword: false,
				createPasswordSuccess: true,
				createPasswordFailed: action.payload
			};
		case 'POST_CREATE_PASSWORD_DATA_REJECTED':
			return {
				...state,
				creatingPassword: false,
				createPasswordFailed: true,
				message: action.payload.message
			};
		default:
			return {
				...state
			};
	}
}
