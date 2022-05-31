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
		case TYPE.GET_ARCHIVE:
			return {
				...state,
				fileLoading: false,
				fileSuccess: false,
				fileRejected: false
			};
		case TYPE.GET_ARCHIVE_FULFILLED:
			return {
				...state,
				fileLoading: false,
				fileSuccess: true,
				fileRejected: false,
				fileData: action.payload.assets
			};
		case TYPE.GET_ARCHIVE_REJECTED:
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

export function updateAssetStatus(
	state = {
		updateData: {
			assets: []
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.UPDATE_ASSET_STATUS:
			return {
				...state
			};
		case TYPE.UPDATE_ASSET_STATUS_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.UPDATE_ASSET_STATUS_REJECTED:
			return {
				...state,
				assetData: action.payload
			};
		default:
			return {
				...state
			};
	}
}
