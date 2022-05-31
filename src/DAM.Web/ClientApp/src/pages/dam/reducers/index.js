import { TYPE } from '../../messageTextConstants';

export default function retrieveData(
	state = {
		fileRefresh: false,
		folderRefresh: false,
		folderLoading: false,
		folderSuccess: false,
		folderRejected: false,
		folderData: [],
		fileLoading: false,
		fileSuccess: false,
		fileRejected: false,
		fileData: [],
		assetDetail: [],
		accounts: [],
		accountsLoading: false,
		accountsSuccess: false,
		accountsRejected: false,
		countries: [],
		countriesLoading: false,
		countriesSuccess: false,
		countriesRejected: false,
		regions: [],
		regionsLoading: false,
		regionsSuccess: false,
		regionsRejected: false,
		approvalTemplates: []
	},
	action
) {
	switch (action.type) {
		case TYPE.GET_ACCOUNTS:
			return {
				...state,
				accountsLoading: false,
				accountSuccess: false,
				accountRejected: false
			};
		case TYPE.GET_ACCOUNTS_FULFILLED:
			return {
				...state,
				accountsLoading: false,
				accountSuccess: true,
				accountRejected: false,
				accounts: action.payload.accounts
			};
		case TYPE.GET_ACCOUNTS_REJECTED:
			return {
				...state,
				accountsLoading: false,
				accountSuccess: false,
				accountRejected: true
			};

		case TYPE.GET_COUNTRIES:
			return {
				...state,
				countriesLoading: false,
				countriesSuccess: false,
				countriesRejected: false
			};
		case TYPE.GET_COUNTRIES_FULFILLED:
			var regions = [];
			action.payload.countries.forEach((c) => {
				regions.push(...c.regions);
			});
			return {
				...state,
				countriesLoading: false,
				countriesSuccess: true,
				countriesRejected: false,
				countries: action.payload.countries,
				regions: regions
			};
		case TYPE.GET_COUNTRIES_REJECTED:
			return {
				...state,
				countriesLoading: false,
				countriesSuccess: false,
				countriesRejected: true
			};
		case TYPE.GET_FOLDER_DATA:
			return {
				...state,
				folderLoading: false,
				folderSuccess: false,
				folderRejected: false
			};
		case TYPE.GET_FOLDER_DATA_FULFILLED:
			return {
				...state,
				folderLoading: false,
				folderSuccess: true,
				folderRejected: false,
				folderRefresh: false,
				folderData: action.payload.folders
			};
		case TYPE.GET_FOLDER_DATA_REJECTED:
			return {
				...state,
				folderLoading: false,
				folderSuccess: false,
				folderRejected: true
			};
		case TYPE.GET_FILE_DATA:
			return {
				...state,
				fileLoading: true,
				fileSuccess: false,
				fileRejected: false
			};
		case TYPE.GET_FILE_DATA_FULFILLED:
			return {
				...state,
				fileLoading: false,
				fileRefresh: false,
				fileSuccess: true,
				fileRejected: false,
				fileData: action.payload.assets
			};
		case TYPE.GET_FILE_DATA_REJECTED:
			return {
				...state,
				fileLoading: false,
				fileSuccess: false,
				fileRejected: true
			};
		case TYPE.GET_FILE_DATA_BY_ID:
			return {
				...state,
				fileLoading: true
			};
		case TYPE.GET_FILE_DATA_BY_ID_FULFILLED:
			return {
				...state,
				fileLoading: false,
				fileRefresh: false,
				assetDetail: action.payload.assets
			};
		case TYPE.GET_FILE_DATA_BY_ID_REJECTED:
			return {
				...state,
				fileLoading: false
			};
		case TYPE.GET_PIN_ASSETS_DETAIL_FULFILLED:
			return {
				...state,
				fileLoading: false,
				fileRefresh: false
			};
		case TYPE.ADD_APPROVALS_FULFILLED:
			return {
				...state,
				fileRefresh: true
			};
		case TYPE.ADD_FOLDER_FULFILLED:
			return {
				...state,
				folderRefresh: true
			};
		case TYPE.UPDATE_FOLDER_FULFILLED:
			return {
				...state,
				folderRefresh: true
			};
		case TYPE.MOVE_FOLDER_FULFILLED:
			return {
				...state,
				folderRefresh: true
			};
		case TYPE.DELETE_FOLDER_FULFILLED:
			return {
				...state,
				folderRefresh: true
			};
		case TYPE.GET_APPROVAL_TEMPLATES_FULFILLED:
			return {
				...state,
				approvalTemplates: action.payload.approvalTemplates
			};
		case TYPE.COPY_FOLDER_FULFILLED:
			return {
				...state,
				folderRefresh: true
			};
		default:
			return {
				...state
			};
	}
}

// export function uploadAsset(
// 	state = {
// 		//assetData: []
// 		assetFile: '',
// 		assetData: {
// 			title: '',
// 			fileName: '',
// 			description: '',
// 			extension: '',
// 			fileBytes: '',
// 			fileType: ''
// 		}
// 	},
// 	action
// ) {
// 	switch (action.type) {
// 		case TYPE.UPLOAD_ASSET:
// 			return {
// 				...state
// 			};
// 		case TYPE.UPLOAD_ASSET_FULFILLED:
// 			return {
// 				...state,
// 				assetData: action.payload
// 			};
// 		case TYPE.UPLOAD_ASSET_REJECTED:
// 			return {
// 				...state,
// 				assetData: action.payload
// 			};
// 		default:
// 			return {
// 				...state
// 			};
// 	}
// }

export function uploadAssetVersion(
	state = {
		//assetData: []
		assetFile: '',
		assetData: {
			title: '',
			fileName: '',
			description: '',
			extension: '',
			fileBytes: '',
			fileType: '',
			id: '',
			status: ''
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.UPLOAD_ASSET_VERSION:
			return {
				...state
			};
		case TYPE.UPLOAD_ASSET_VERSION_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.UPLOAD_ASSET_VERSION_REJECTED:
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

export function updateAsset(
	state = {
		assetData: {
			id: null,
			title: '',
			description: '',
			accounts: null,
			countries: null,
			thumbnail: null
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.UPDATE_ASSET:
			return {
				...state
			};
		case TYPE.UPDATE_ASSET_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.UPDATE_ASSET_REJECTED:
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

export function moveAssets(
	state = {
		moveData: {
			assets: []
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.MOVE_ASSETS:
			return {
				...state
			};
		case TYPE.MOVE_ASSETS_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.MOVE_ASSETS_REJECTED:
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

export function archiveAssets(
	state = {
		archiveData: {
			assets: []
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.ARCHIVE_ASSETS:
			return {
				...state
			};
		case TYPE.ARCHIVE_ASSETS_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.ARCHIVE_ASSETS_REJECTED:
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

export function addFolder(
	state = {
		folderData: {
			id: 0,
			folderName: '',
			parentFolderId: 0,
			accounts: [],
			countries: [],
			regions: [],
			comments: ''
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.ADD_FOLDER:
			return {
				...state
			};
		case TYPE.ADD_FOLDER_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.ADD_FOLDER_REJECTED:
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

export function deleteFolder(
	state = {
		folderData: {
			id: 0,
			folderName: '',
			parentFolderId: 0,
			accounts: [],
			countries: [],
			regions: [],
			comments: ''
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.DELETE_FOLDER:
			return {
				...state
			};
		case TYPE.DELETE_FOLDER_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.DELETE_FOLDER_REJECTED:
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
export function updateFolder(
	state = {
		folderData: {
			id: 0,
			folderName: '',
			parentFolderId: 0,
			accounts: [],
			countries: [],
			regions: [],
			comments: ''
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.UPDATE_FOLDER:
			return {
				...state
			};
		case TYPE.UPDATE_FOLDER_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.UPDATE_FOLDER_REJECTED:
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

export function addUserImage(
	state = {
		userData: {
			id: 0,
			displayName: '',
			emailAddress: '',
			password: '',
			userRoleId: 0,
			userRole: {},
			active: false,
			imageUrl: '',
			fileBytes: [],
			ImageFileExtension: ''
		}
	},
	action
) {
	switch (action.type) {
		case TYPE.UPDATE_IMAGE_USERS:
			return {
				...state
			};
		case TYPE.UPDATE_IMAGE_USERS_FULFILLED:
			return {
				...state,
				assetData: action.payload
			};
		case TYPE.UPDATE_IMAGE_USERS_REJECTED:
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
