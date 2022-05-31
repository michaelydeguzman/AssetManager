import { TYPE } from '../../messageTextConstants';

export default function retrieveData(
	state = {
		auditLoading: false,
		auditSuccess: false,
		auditRejected: false,
		auditData: {}
	},
	action
) {
	switch (action.type) {
		case TYPE.GET_AUDITS:
			return {
				...state,
				auditLoading: false,
				auditSuccess: false,
				auditRejected: false
			};
		case TYPE.GET_AUDITS_FULFILLED:
			return {
				...state,
				auditLoading: false,
				auditSuccess: true,
				auditRejected: false,
				auditData: action.payload.auditTrail
			};
		case TYPE.GET_AUDITS_REJECTED:
			return {
				...state,
				auditLoading: false,
				auditSuccess: false,
				auditRejected: true,
				auditData: {}
			};
		default:
			return {
				...state
			};
	}
}
