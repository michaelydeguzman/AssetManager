import { TYPE } from '../../messageTextConstants';

export default function retrieveData(
	state = {
		fileLoading: false,
		fileSuccess: false,
		fileRejected: false,
		fileData: []
	},
	action
) {
	switch (action.type) {
		case TYPE.GET_APPROVALS:
			return {
				...state,
				fileLoading: false,
				fileSuccess: false,
				fileRejected: false
			};
		case TYPE.GET_APPROVALS_FULFILLED:
			return {
				...state,
				fileLoading: false,
				fileSuccess: true,
				fileRejected: false,
				fileData: action.payload.assets
			};
		case TYPE.GET_APPROVALS_REJECTED:
			return {
				...state,
				fileLoading: false,
				fileSuccess: false,
				fileRejected: true
			};
		default:
			return {
				...state
			};
	}
}
