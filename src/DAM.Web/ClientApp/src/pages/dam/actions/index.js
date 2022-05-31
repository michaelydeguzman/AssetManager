import axios from 'axios';
import { headers, logout } from '@damtoken';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';
const API_URL = process.env.REACT_APP_API_BASE_URL;

const CancelToken = axios.CancelToken;
let cache = {};

// 请求拦截器中用于判断数据是否存在以及过期 未过期直接返回
// intercept the request and check if the data is expired or not
axios.interceptors.request.use((config) => {
	// 如果需要缓存--考虑到并不是所有接口都需要缓存的情况
	// if the request need to be cached, then check if the data is expired or not
	if (config.cache && !config.refresh) {
		let source = CancelToken.source();
		config.cancelToken = source.token;
		// 去缓存池获取缓存数据
		// get the data from the cache pool
		let data = cache[config.url];
		// 获取当前时间戳
		// get the current timestamp
		let current_time = getCurrentTime();
		// 判断缓存池中是否存在已有数据 存在的话 再判断是否过期
		// if the data is existed in the cache pool, then check if the data is expired or not
		// 未过期 source.cancel会取消当前的请求 并将内容返回到拦截器的err中
		// if the data is not expired, then cancel the current request and return the data to the interceptor
		if (data && current_time - data.expire < config.expire_time) {
			source.cancel(data);
		}
	}
	return config;
});

// 响应拦截器中用于缓存数据 如果数据没有缓存的话
// intercept the response and cache the data if the data is not cached
axios.interceptors.response.use(
	(response) => {
		// 只缓存get请求
		// only cache the get request
		if (response.config.method === 'get' && response.config.cache) {
			// 缓存数据 并将当前时间存入 方便之后判断是否过期
			// cache the data and save the current timestamp to check if the data is expired
			let data = {
				expire: getCurrentTime(),
				data: response
			};
			cache[`${response.config.url}`] = data;
		}
		return response;
	},
	(error) => {
		// 请求拦截器中的source.cancel会将内容发送到error中
		// if the request is canceled, then the content will be sent to the error
		// 通过axios.isCancel(error)来判断是否返回有数据 有的话直接返回给用户
		// if the request is canceled, then the content will be sent to the error
		if (axios.isCancel(error)) return Promise.resolve(error.message.data);
		// 如果没有的话 则是正常的接口错误 直接返回错误信息给用户
		// if the request is not canceled, then the content will be sent to the error
		return Promise.reject(error);
	}
);

// 获取当前时间
// get the current time
function getCurrentTime() {
	return new Date().getTime();
}

export const checkLogin = () => {
	return (dispatch) => {
		// dispatch({
		// 	type: TYPE.GET_ACCOUNTS,
		// 	payload: {}
		// });
		return axios
			.get(`${window.location.origin}/api/app/Users/Check`, {
				headers: headers(),
				cache: false,
				refresh: false,
				expire_time: 3600000
			})
			.then(function (response) {
				return true;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					return false;
				}
				dispatch({
					type: TYPE.GET_ACCOUNTS_REJECTED,
					payload: error
				});
			});
	};
};

export const getAccounts = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_ACCOUNTS,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Accounts`, {
				headers: headers(),
				cache: true,
				refresh: false,
				expire_time: 3600000
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_ACCOUNTS_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_ACCOUNTS_REJECTED,
					payload: error
				});
			});
	};
};

export const getCountries = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_COUNTRIES,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Countries`, {
				headers: headers(),
				cache: true,
				refresh: false,
				expire_time: 3600000
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_COUNTRIES_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_COUNTRIES_REJECTED,
					payload: error
				});
			});
	};
};

export const getRegions = () => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_REGIONS,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/`, { headers: headers(), cache: true, refresh: false, expire_time: 3600000 })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_REGIONS_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_REGIONS_REJECTED,
					payload: error
				});
			});
	};
};

export const getAssetApprovals = (id, verId = 0, refresh = false) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_ASSETAPPROVALS,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Approval/${id}/${verId}`, {
				headers: headers(),
				cache: true,
				refresh: refresh,
				expire_time: 60000
			})
			.then(function (response) {
				if (response.data) {
					return response.data;
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_ASSETAPPROVALS_REJECTED,
					payload: error
				});
			});
	};
};

export const getFolders = (refresh = false) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_FOLDER_DATA,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Folders`, {
				headers: headers(),
				cache: true,
				refresh: refresh,
				expire_time: 360000
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_FOLDER_DATA_FULFILLED,
						payload: response.data
					});
				}
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_FOLDER_DATA_REJECTED,
					payload: error
				});
			});
	};
};

export const getFolderDetail = (id) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_FOLDER_DATA,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Folders/${id}`, {
				headers: headers()
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_FOLDER_DETAIL_FULFILLED,
						payload: response.data
					});
				}
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_FOLDER_DETAIL_REJECTED,
					payload: error
				});
			});
	};
};

export const deleteFolder = (folders) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Delete`, { ...folders }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.DELETE_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.DELETE_FOLDER_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.DELETE_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DELETE_FOLDER_REJECTED);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const moveFolder = (data, folderName) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Move`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.MOVE_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.MOVE_FOLDER_FULFILLED, folderName);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.MOVE_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.MOVE_FOLDER_REJECTED, folderName);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const addFolder = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Add`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_FOLDER_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_FOLDER_REJECTED);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const copyFolder = (data, folderName) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Copy`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.COPY_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.COPY_FOLDER_FULFILLED, folderName);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.COPY_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.COPY_FOLDER_REJECTED, folderName);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const updateFolder = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Update`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_FOLDER_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.UPDATE_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_FOLDER_REJECTED);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const getFiles = (refresh = false) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_FILE_DATA,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Assets`, {
				headers: headers(),
				cache: true,
				refresh: refresh,
				expire_time: 10000
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_FILE_DATA_FULFILLED,
						payload: response.data
					});
				}
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_FILE_DATA_REJECTED,
					payload: error
				});
			});
	};
};

export const getFilesVersion = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_FILE_VERSION_DATA,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Assets/GetAssetVersion/${data.assetId}/${data.versionId}`, {
				headers: headers()
			})
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_FILE_VERSION_DATA_FULFILLED,
						payload: response.data
					});
				}
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_FILE_VERSION_DATA_REJECTED,
					payload: error
				});
			});
	};
};

export const revertAssetVersion = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.REVERT_VERSION_DATA,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Assets/Revert/${data.assetId}/${data.versionId}`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.REVERT_VERSION_DATA_FULFILLED,
						payload: response.data
					});
				}
				//FeedbackMessage(TYPE.REVERT_VERSION_DATA_FULFILLED);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.REVERT_VERSION_DATA_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.REVERT_VERSION_DATA_REJECTED);
			});
	};
};

export const uploadAsset = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/Add`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPLOAD_ASSET_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPLOAD_ASSET_FULFILLED, data.fileName);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPLOAD_ASSET_REJECTED, data.fileName);
			});
	};
};

export const uploadAssetVersion = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/AddVersion`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPLOAD_ASSET_VERSION_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPLOAD_ASSET_VERSION_FULFILLED, data.fileName);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPLOAD_ASSET_VERSION_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPLOAD_ASSET_VERSION_REJECTED, data.fileName);
			});
	};
};

export const updateAsset = (data, type) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/Update`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_ASSET_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_ASSET_FULFILLED, type);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_ASSET_REJECTED, type);
			});
	};
};

export const moveAssets = (data, folderName) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/Move`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.MOVE_ASSETS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.MOVE_ASSETS_FULFILLED, folderName);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.MOVE_ASSETS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.MOVE_ASSETS_REJECTED, folderName);
			});
	};
};

export const archiveAssets = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/Status`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ARCHIVE_ASSETS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ARCHIVE_ASSETS_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ARCHIVE_ASSETS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ARCHIVE_ASSETS_REJECTED);
			});
	};
};

export const getWopiParams = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Assets/view/${id}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);

				return response;
				//dispatch({
				//    type: TYPE.GET_WOPI_PARAMS_FULFILLED,
				//    payload: response.data
				//})
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_WOPI_PARAMS_REJECTED,
					payload: error
				});
			});
	};
};

export const getUsers = (id, refresh = false) => {
	return (dispatch) => {
		let url = id ? `${window.location.origin}/api/App/Users/${id}` : `${window.location.origin}/api/App/Users`;
		return axios
			.get(url, { headers: headers(), cache: true, refresh: refresh, expire_time: 10000 })
			.then(function (response) {
				console.log('response', response);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_USERS_REJECTED,
					payload: error
				});
			});
	};
};

export const getTeams = (id) => {
	return (dispatch) => {
		let url = id ? `${window.location.origin}/api/Teams/${id}` : `${window.location.origin}/api/Teams`;
		return axios
			.get(url, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_TEAMS_REJECTED,
					payload: error
				});
			});
	};
};

export const getAllTags = () => {
	return (dispatch) => {
		let url = `${window.location.origin}/api/Tags`;
		return axios
			.get(url, { headers: headers(), cache: true, refresh: false, expire_time: 10000 })
			.then(function (response) {
				console.log('response', response);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_ALL_TAGS_REJECTED,
					payload: error
				});
			});
	};
};

export const getUsersByCompany = (id) => {
	return (dispatch) => {
		let url = `${window.location.origin}/api/App/Users/Company/${id}`;
		return axios
			.get(url, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_USERS_REJECTED,
					payload: error
				});
			});
	};
};

export const getApprovers = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/admin/api/Users/GetApprovers`, { headers: headers() })
			.then(function (response) {
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_APPROVERS_REJECTED,
					payload: error
				});
			});
	};
};

export const addUser = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Users/Add`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ADD_USERS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_USERS_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_USERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_USERS_REJECTED);
			});
	};
};
export const shareAsset = (assetKey, emailAddress) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Assets/Share/${assetKey}/${emailAddress}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.SHARE_ASSET_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.SHARE_ASSET_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.SHARE_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.SHARE_ASSET_REJECTED);
			});
	};
};

export const updateUserProfile = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/admin/api/Users/Update`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.UPDATE_USERS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_USERS_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_USERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_USERS_REJECTED);
				console.log(error);
			});
	};
};

export const updateUserByAdmin = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/admin/api/Users/AdminUpdate`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.UPDATE_USERS_ADMIN_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_USERS_ADMIN_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_USERS_ADMIN_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_USERS_ADMIN_REJECTED);
				console.log(error);
			});
	};
};

export const uploadUserImage = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/App/Users/UploadProfilePicture`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_IMAGE_USERS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_IMAGE_USERS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_IMAGE_USERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_IMAGE_USERS_REJECTED);
			});
	};
};

export const downloadAsset = (assetKey, userId, fileName, fileExt, showWatermark, extension) => {
	const FileDownload = require('js-file-download');
	var url =
		extension && extension.length > 0
			? `${window.location.origin}/api/Assets/Download/${assetKey}/${userId}/${showWatermark}/${extension}`
			: `${window.location.origin}/api/Assets/Download/${assetKey}/${userId}/${showWatermark}`;

	return (dispatch) => {
		return axios
			.get(url, {
				headers: headers(),
				responseType: 'blob'
			})
			.then(function (response) {
				console.log(response);
				FileDownload(
					response.data,
					extension && extension.length > 0 ? fileName.replace(fileExt, extension) : fileName
				);
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DOWNLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_REJECTED);
			});
	};
};

export const downloadPDFAsImage = (assetKey, userId, fileName, fileExt, showWatermark, extension) => {
	const FileDownload = require('js-file-download');
	var url = `${window.location.origin}/api/Assets/DownloadPDFAsImage/${assetKey}/${userId}/${showWatermark}/${extension}`;

	return (dispatch) => {
		return axios
			.get(url, {
				headers: headers(),
				responseType: 'blob'
			})
			.then(function (response) {
				console.log(response);
				FileDownload(response.data, `${fileName}.zip`);
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DOWNLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_REJECTED);
			});
	};
};

export const downloadOfficeAsPDF = (assetKey, userId, fileName) => {
	const FileDownload = require('js-file-download');
	var url = `${window.location.origin}/api/Assets/DownloadOfficeAsPdf/${assetKey}/${userId}`;

	return (dispatch) => {
		return axios
			.get(url, {
				headers: headers(),
				responseType: 'blob'
			})
			.then(function (response) {
				console.log(response);
				FileDownload(response.data, `${fileName}.pdf`);
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DOWNLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_REJECTED);
			});
	};
};

export const bulkDownloadAsset = (assetKey, userId, showWatermark) => {
	const FileDownload = require('js-file-download');

	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Assets/DownloadBulk/${assetKey}/${userId}/${showWatermark}`, {
				headers: headers(),
				responseType: 'blob'
			})
			.then(function (response) {
				console.log(response);
				FileDownload(response.data, 'MyZipfile.zip');
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DOWNLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_REJECTED);
			});
	};
};

export const downloadAssetPackage = (assetKey, fileName, fileExt) => {
	const FileDownload = require('js-file-download');

	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Assets/Download/Package/${assetKey}`, {
				headers: headers(),
				responseType: 'blob'
			})
			.then(function (response) {
				console.log(response);
				FileDownload(response.data, fileName + '.' + fileExt);
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DOWNLOAD_ASSET_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DOWNLOAD_ASSET_REJECTED);
			});
	};
};

export const uploadToDynamics = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/UploadToDynamics`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ADD_TO_DYNAMICS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_TO_DYNAMICS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_TO_DYNAMICS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_TO_DYNAMICS_REJECTED);
			});
	};
};

export const createApprovals = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Approval/Save`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ADD_APPROVALS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_APPROVALS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_APPROVALS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_APPROVALS_REJECTED);
			});
	};
};

export const getApprovalLevels = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/ApprovalLevel`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_APPROVALLEVELS_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_APPROVALLEVELS_REJECTED,
					payload: error
				});
			});
	};
};

export const createApprovalLevels = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/ApprovalLevel/CreateApprovalLevel`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ADD_APPROVALLEVELS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_APPROVALLEVELS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_APPROVALLEVELS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_APPROVALLEVELS_REJECTED);
			});
	};
};

export const updateApprovalLevels = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/ApprovalLevel/UpdateApprovalLevel`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.UPDATE_APPROVALLEVELS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_APPROVALLEVELS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_APPROVALLEVELS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_APPROVALLEVELS_REJECTED);
			});
	};
};

export const getUserRoles = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/UserRoles`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_USERROLES_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_USERROLES_REJECTED,
					payload: error
				});
			});
	};
};

export const authenticateVideoForIndexing = () => {
	return (dispatch) => {
		return axios
			.get(
				`https://api.videoindexer.ai/auth/trial/Accounts/06f672dd-a001-45fb-919c-29771f87f85f/AccessToken?allowEdit=True`,
				{
					headers: {
						'Ocp-Apim-Subscription-Key': '058b08005c2c44c39a062a4cb1d621b2'
					}
				}
			)
			.then(function (response) {
				dispatch({
					type: 'TYPE.AUTHENTICATE_FOR_INDEXING_FULFILLED',
					payload: response.data
				});

				return response;
			})
			.catch(function (error) {
				dispatch({
					type: 'TYPE.AUTHENTICATE_FOR_INDEXING_REJECTED',
					payload: error
				});
			});
	};
};

export const uploadVideoForIndexing = (url, data, config) => {
	return (dispatch) => {
		return axios
			.post(`https://api.videoindexer.ai/${url}`, data, config)
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: 'TYPE.UPLOAD_VIDEO_INDEXING_FULFILLED',
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: 'TYPE.UPLOAD_VIDEO_INDEXING_REJECTED',
					payload: error
				});
			});
	};
};

export const getIndexingState = (url, config) => {
	return (dispatch) => {
		return axios
			.get(`https://api.videoindexer.ai/${url}`, config)
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: 'TYPE.VIDEO_INDEXING_STATE_FULFILLED',
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: 'TYPE.VIDEO_INDEXING_STATE_REJECTED',
					payload: error
				});
			});
	};
};

export const getVideoIndexList = (url, config) => {
	return (dispatch) => {
		return axios
			.get(`https://api.videoindexer.ai/${url}`, config)
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: 'TYPE.VIDEO_INDEXING_STATE_FULFILLED',
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: 'TYPE.VIDEO_INDEXING_STATE_REJECTED',
					payload: error
				});
			});
	};
};

export const addCompany = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Company/AddCompany`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_PARTNERS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_PARTNERS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_PARTNERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ADD_PARTNERS_REJECTED);
			});
	};
};

export const updateCompany = (data) => {
	return (dispatch) => {
		FeedbackMessage('LOADING');
		return axios
			.put(`${window.location.origin}/api/Company/UpdateCompany`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.UPDATE_PARTNERS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.UPDATE_PARTNERS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.UPDATE_PARTNERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_PARTNERS_REJECTED);
			});
	};
};

export const getPartner = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Company/${id}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_PARTNERS_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.GET_PARTNERS_REJECTED,
					payload: error
				});
			});
	};
};

export const getFeatureFlag = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/FeatureFlags`, {
				headers: headers(),
				cache: true,
				refresh: false,
				expire_time: 10000
			})
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_FEATURE_FLAGS_FULFILLED,
					payload: response.data
				});
				return response.data;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.GET_FEATURE_FLAGS_REJECTED,
					payload: error
				});
			});
	};
};

export const getCompanies = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Company`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_PARTNERS_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.GET_PARTNERS_REJECTED,
					payload: error
				});
			});
	};
};

export const inviteNewUser = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/admin/api/Users/inviteNewUser`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.INVITE_NEW_USER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.INVITE_NEW_USER_FULFILLED);

				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.INVITE_NEW_USER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.INVITE_NEW_USER_REJECTED, error.response.data.message);
			});
	};
};

export const getUserPartner = (userId) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/admin/api/Users/UserPartner/${userId}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_USER_PARTNERS_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.GET_USER_PARTNERS_REJECTED,
					payload: error
				});
			});
	};
};

export const getUserFolder = (userId) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/admin/api/Users/UserFolder/${userId}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_USER_FOLDERS_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_USER_FOLDERS_REJECTED,
					payload: error
				});
			});
	};
};

export const changePassword = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/App/Users/ChangePassword`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.CHANGE_PASSWORD_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.CHANGE_PASSWORD_FULFILLED);

				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.CHANGE_PASSWORD_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.CHANGE_PASSWORD_REJECTED, error.response.data.message);
			});
	};
};

export const getAssetById = (id) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_FILE_DATA_BY_ID,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Assets/${id}`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_FILE_DATA_BY_ID_FULFILLED,
					payload: response.data
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_FILE_DATA_BY_ID_REJECTED,
					payload: error
				});
			});
	};
};

export const shareBulkAssets = (assetIds, folderIds) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Assets/ShareBulk/${assetIds}/${folderIds}`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.SHARE_BULK_ASSETS,
					payload: response.data
				});
				FeedbackMessage(TYPE.SHARE_BULK_ASSETS_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.SHARE_BULK_ASSETS_REJECTED,
					payload: error
				});
			});
	};
};

export const shareFoldertoUser = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/admin/api/Users/ShareFoldertoUser`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.SHARE_FOLDER_TO_USER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.SHARE_FOLDER_TO_USER_FULFILLED);
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.SHARE_FOLDER_TO_USER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.SHARE_FOLDER_TO_USER_REJECTED, error.response.data.message);
			});
	};
};

export const getPinAssets = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Pin/Assets`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_PIN_ASSETS_FULFILLED,
					payload: response.data.assets
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_PIN_ASSETS_REJECTED,
					payload: error
				});
			});
	};
};

export const addPinAssets = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Assets`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_PIN_ASSETS_FULFILLED,
					payload: response.data.assets
				});
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_PIN_ASSETS_REJECTED,
					payload: error
				});
			});
	};
};

export const removePinAssets = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Assets/Remove`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REMOVE_PIN_ASSETS_FULFILLED,
					payload: response.data.assets
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.REMOVE_PIN_ASSETS_REJECTED,
					payload: error
				});
			});
	};
};

export const getPinAssetsDetail = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Pin/Assets/Detail`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_PIN_ASSETS_DETAIL_FULFILLED,
					payload: response.data.assets
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_PIN_ASSETS_DETAIL_REJECTED,
					payload: error
				});
			});
	};
};

export const replacePinAssets = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Assets/Replace`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REPLACE_PIN_ASSETS_FULFILLED,
					payload: response.data.assets
				});
				FeedbackMessage(TYPE.REPLACE_PIN_ASSETS_FULFILLED);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.REPLACE_PIN_ASSETS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.REPLACE_PIN_ASSETS_REJECTED);
			});
	};
};

export const orderPinAssets = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Assets/Order`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ORDER_PIN_ASSETS_FULFILLED,
					payload: response.data.assets
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ORDER_PIN_ASSETS_REJECTED,
					payload: error
				});
			});
	};
};

//... Folder...//
export const getPinFolders = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Pin/Folders`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_PIN_FOLDERS_FULFILLED,
					payload: response.data.folders
				});
				return response.data.folders;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_PIN_FOLDERS_REJECTED,
					payload: error
				});
			});
	};
};

export const addPinFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Folders`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_PIN_FOLDERS_FULFILLED,
					payload: response.data.folders
				});
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_PIN_FOLDERS_REJECTED,
					payload: error
				});
			});
	};
};

export const removePinFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Folders/Remove`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REMOVE_PIN_FOLDERS_FULFILLED,
					payload: response.data.folders
				});
				return response.data.folders;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.REMOVE_PIN_FOLDERS_REJECTED,
					payload: error
				});
			});
	};
};

export const getPinFoldersDetail = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Pin/Folders/Detail`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_PIN_FOLDERS_DETAIL_FULFILLED,
					payload: response.data.folders
				});
				return response.data.folders;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_PIN_FOLDERS_DETAIL_REJECTED,
					payload: error
				});
			});
	};
};

export const replacePinFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Folders/Replace`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REPLACE_PIN_FOLDERS_FULFILLED,
					payload: response.data.folders
				});
				FeedbackMessage(TYPE.REPLACE_PIN_FOLDERS_FULFILLED);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.REPLACE_PIN_FOLDERS_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.REPLACE_PIN_FOLDERS_REJECTED);
			});
	};
};

export const orderPinFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Pin/Folders/Order`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ORDER_PIN_FOLDERS_FULFILLED,
					payload: response.data.folders
				});
				return response.data.folders;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ORDER_PIN_FOLDERS_REJECTED,
					payload: error
				});
			});
	};
};
//...Folder..//

export const GetPowerBiURL = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/App/PowerBIUrl`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.POWERBI_URL_FULFILLED,
					payload: response.data
				});
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.POWERBI_URL_REJECTED,
					payload: error
				});
			});
	};
};

export const GetAssetContainer = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/App/AssetContainer`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ASSET_CONTAINER_FULFILLED,
					payload: response.data
				});
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ASSET_CONTAINER_REJECTED,
					payload: error
				});
			});
	};
};

export const GetAssetPreviewContainer = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/App/AssetPreviewContainer`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ASSET_PREVIEW_CONTAINER_FULFILLED,
					payload: response.data
				});
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ASSET_PREVIEW_CONTAINER_REJECTED,
					payload: error
				});
			});
	};
};

export const bulkMoveFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/BulkMove`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.BULK_MOVE_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.BULK_MOVE_FOLDER_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.BULK_MOVE_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.BULK_MOVE_FOLDER_REJECTED);
				// if (error.response && error.response.status) {
				// 	logout();
				// }
			});
	};
};

export const orderFolders = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Folders/Order`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ORDER_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ORDER_FOLDER_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ORDER_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.ORDER_FOLDER_REJECTED);
				// if (error.response && error.response.status) {
				// 	logout();
				// }
			});
	};
};

export const getApprovalTemplates = (id) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.GET_APPROVAL_TEMPLATES,
			payload: {}
		});
		return axios
			.get(`${window.location.origin}/api/Approval/Templates/${id}`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_APPROVAL_TEMPLATES_FULFILLED,
						payload: response.data
					});
				}
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_APPROVAL_TEMPLATES_REJECTED,
					payload: error
				});
			});
	};
};

export const updateApprovalTemplate = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.UPDATE_APPROVAL_TEMPLATE,
			payload: {}
		});
		return axios
			.put(`${window.location.origin}/api/Approval/UpdateTemplate`, data, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.UPDATE_APPROVAL_TEMPLATE_FULFILLED,
						payload: response.data
					});
				}
				FeedbackMessage(TYPE.UPDATE_APPROVAL_TEMPLATE_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.UPDATE_APPROVAL_TEMPLATE_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.UPDATE_APPROVAL_TEMPLATE_REJECTED);
			});
	};
};

export const createApprovalTemplate = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.CREATE_APPROVAL_TEMPLATE,
			payload: {}
		});
		return axios
			.put(`${window.location.origin}/api/Approval/CreateTemplate`, data, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.CREATE_APPROVAL_TEMPLATE_FULFILLED,
						payload: response.data
					});
				}
				FeedbackMessage(TYPE.CREATE_APPROVAL_TEMPLATE_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.CREATE_APPROVAL_TEMPLATE_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.CREATE_APPROVAL_TEMPLATE_REJECTED);
			});
	};
};

export const deleteApprovalTemplate = (data) => {
	return (dispatch) => {
		dispatch({
			type: TYPE.DELETE_APPROVAL_TEMPLATE,
			payload: {}
		});
		return axios
			.put(`${window.location.origin}/api/Approval/DeleteTemplate`, data, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.DELETE_APPROVAL_TEMPLATE_FULFILLED,
						payload: response.data
					});
				}
				FeedbackMessage(TYPE.DELETE_APPROVAL_TEMPLATE_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.DELETE_APPROVAL_TEMPLATE_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.DELETE_APPROVAL_TEMPLATE_REJECTED);
			});
	};
};

export const addCollection = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Cart`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_CART,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_TO_COLLECTION_FULFILLED);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_TO_COLLECTION_REJECTED,
					payload: error
				});

				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const deleteCollection = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Cart/delete`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.DELETE_CART,
					payload: response.data
				});
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.DELETE_CART_FAILED,
					payload: error
				});
				FeedbackMessage(TYPE.DELETE_CART_FAILED);
				if (error.response && error.response.status) {
					logout();
				}
			});
	};
};

export const getCarts = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Cart`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					return response.data;
				}
			})
			.catch(function (error) {
				console.log('getCarts_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getCart = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Cart/${id}`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					return response.data;
				}
			})
			.catch(function (error) {
				console.log('getCarts_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getCurrentCart = (data) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Cart/Current`, { headers: headers() })
			.then(function (response) {
				if (response.data) {
					dispatch({
						type: TYPE.GET_CART,
						payload: response.data
					});
					return response.data;
				}
			})
			.catch(function (error) {
				console.log('getCurrentCart_ERROR', error);
				if (error.response && error.response.status === 401) {
					logout();
				}
				if (error.response && error.response.status === 404) {
					console.log('cart not found');
					return null;
				}
				dispatch({
					type: TYPE.GET_CART_FAILED,
					payload: error
				});
			});
	};
};

export const getUserOutOfOffice = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/admin/api/Users/OOO/${id}`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_USERS_OOO_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_USERS_OOO_REJECTED,
					payload: error
				});
				FeedbackMessage(TYPE.GET_USERS_OOO_REJECTED);
			});
	};
};

export const addUserOutOfOffice = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/admin/api/Users/OOO/Add`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.ADD_USERS_OOO_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_USERS_OOO_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.ADD_USERS_OOO_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.ADD_USERS_OOO_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const editUserOutOfOffice = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/admin/api/Users/OOO/Edit`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.EDIT_USERS_OOO_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.EDIT_USERS_OOO_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.EDIT_USERS_OOO_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.EDIT_USERS_OOO_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const getAssetsForApprovalOnOOO = (data) => {
	return (dispatch) => {
		return (
			axios
				//.get(`${window.location.origin}/api/Approvals/ApprovalsOnOOO/userId=${userId}&startDate=${startDate}&endDate=${endDate}`, { headers: headers() })
				.post(`${window.location.origin}/api/Approval/ApprovalsOnOOO`, data, { headers: headers() })
				.then(function (response) {
					console.log('response', response);
					dispatch({
						type: TYPE.GET_ASSETS_FOR_APPROVAL_OOO_FULFILLED,
						payload: response.data
					});
					return response;
				})
				.catch(function (error) {
					if (error.response && error.response.status === 401) {
						logout();
					}
					dispatch({
						type: TYPE.GET_ASSETS_FOR_APPROVAL_OOO_REJECTED,
						payload: error
					});
					return error;
				})
		);
	};
};

export const delegateApprovals = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Approval/Delegate`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_ASSETS_FOR_APPROVAL_OOO_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.GET_ASSETS_FOR_APPROVAL_OOO_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const checkDuplicateAsset = (data) => {
	return (dispatch) => {
		return axios
			.put(`${window.location.origin}/api/Assets/duplicateAsset`, { ...data }, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.CHECK_DUPLICATE_ASSET,
					payload: response.data
				});
				return response.data;
			})
			.catch(function (error) {});
	};
};

export const getDefaultWatermark = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Watermark/Default`, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.GET_DEFAULT_WATERMARK_FULFILLED,
					payload: response.data
				});
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_DEFAULT_WATERMARK_REJECTED,
					payload: error
				});
				return error;
			});
	};
};

export const saveDefaultWatermark = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Watermark/Default/Save`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.SAVE_DEFAULT_WATERMARK_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.SAVE_DEFAULT_WATERMARK_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.SAVE_DEFAULT_WATERMARK_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.SAVE_DEFAULT_WATERMARK_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const exportToFolder = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/copyToFolder`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.EXPORT_TO_FOLDER_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.EXPORT_TO_FOLDER_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.EXPORT_TO_FOLDER_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.EXPORT_TO_FOLDER_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const bulkUpdateAssetTags = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/bulkUpdateTags`, data, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				dispatch({
					type: TYPE.BULK_UPDATE_TAGS_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.BULK_UPDATE_TAGS_FULFILLED);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.BULK_UPDATE_TAGS_REJECTED,
					payload: error
				});
				FeedbackMessage(
					TYPE.BULK_UPDATE_TAGS_REJECTED,
					error.response && error.response.data && error.response.data.message ? error.response.data.message : ''
				);

				return error;
			});
	};
};

export const getMySentAssetsDetail = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Approval/MySentApproval`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.GET_PIN_ASSETS_DETAIL_FULFILLED,
					payload: response.data.assets
				});
				return response.data.assets;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_PIN_ASSETS_DETAIL_REJECTED,
					payload: error
				});
			});
	};
};

export const uploadChunk = async (
	chunk,
	setBeginingOfTheChunk,
	setEndOfTheChunk,
	endOfTheChunk,
	chunkSize,
	counter,
	chunkCount,
	fileGuid,
	setProgress,
	fileOriginalName,
	fileType,
	folderId,
	projectId,
	setUploadedFiles
) => {
	return (dispatch) => {
		try {
			return axios
				.post(`${window.location.origin}/api/Assets/UploadChunks`, chunk, {
					params: {
						id: counter,
						fileName: fileGuid
					},
					headers: {
						'Content-Type': 'application/json',
						Authorization: 'Bearer ' + window.localStorage.getItem('DAM_Token')
					}
				})
				.then(async function (response) {
					dispatch({
						type: TYPE.UPLOAD_ASSET_FULFILLED,
						payload: response.data
					});
					if (response.data.entity === 'success') {
						setBeginingOfTheChunk(endOfTheChunk);
						setEndOfTheChunk(endOfTheChunk + chunkSize);
						console.log(chunkCount);
						if (counter == chunkCount) {
							var result = await uploadCompleted(
								fileGuid,
								setProgress,
								fileOriginalName,
								fileType,
								folderId,
								projectId,
								setUploadedFiles
							);
							console.log('Process is complete, counter', counter);
							console.log(result);
							return result;
						} else {
							var percentage = (counter / chunkCount) * 100;
							setProgress(percentage);
						}
					} else {
						console.log('Error Occurred:', response.data.message);
					}
				})
				.catch(function (error) {
					if (error.response && error.response.status === 401) {
						logout();
					}
					dispatch({
						type: TYPE.UPLOAD_ASSET_REJECTED,
						payload: error
					});
				});
		} catch (error) {
			debugger;
			console.log('error', error);
		}
	};
};

export const uploadCompleted = async (
	fileGuid,
	setProgress,
	fileOriginalName,
	fileType,
	folderId,
	projectId,
	setUploadedFiles
) => {
	var formData = new FormData();
	formData.append('fileName', fileGuid);
	console.log(formData);
	console.log(fileGuid);
	axios
		.post(
			`${window.location.origin}/api/Assets/UploadComplete`,
			{},
			{
				params: {
					fileName: fileGuid,
					fileOriginalName: fileOriginalName,
					fileType: fileType,
					folderId: folderId ? folderId : 0,
					projectId: projectId
				},
				data: formData,
				headers: {
					'Content-Type': 'application/json',
					Authorization: 'Bearer ' + window.localStorage.getItem('DAM_Token')
				}
			}
		)
		.then(function (response) {
			// if (response.data.entity === 'success') {
			// 	setProgress(100);
			// }
			setProgress(100);
			console.log(response);
			console.log(response.data);
			setUploadedFiles((files) => [...files, response.data.asset]);
			return response.data;
		})
		.catch(function (error) {
			if (error.response && error.response.status === 401) {
				FeedbackMessage(TYPE.UPLOAD_ASSET_REJECTED);
			}
		});
};

export const getProjects = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getProjects_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getArchivedProjects = () => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Archived`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getArchivedProjects_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getProjectOwners = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Owners/${id}`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getProjectOwners_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getProjectFollowers = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Followers/${id}`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getProjectFollowers_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getProjectComments = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Comments/${id}`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getProjectComments_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const saveProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Save`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.SAVE_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.SAVE_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.SAVE_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const saveProjectComment = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Comments/Save`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.SAVE_PROJECT_COMMENT,
					payload: response.data
				});
				FeedbackMessage(TYPE.SAVE_PROJECT_COMMENT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.SAVE_PROJECT_COMMENT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const deleteProjectComment = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Comments/Delete`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.DELETE_PROJECT_COMMENT,
					payload: response.data
				});
				FeedbackMessage(TYPE.DELETE_PROJECT_COMMENT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.DELETE_PROJECT_COMMENT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const importAssetsToProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Assets/Import`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.IMPORT_ASSET_TO_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.IMPORT_ASSET_TO_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.IMPORT_ASSET_TO_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const getImportedAssetsToProject = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Assets/Import/${id}`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getImportedAssetsToProject_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const getProjectUploads = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/Projects/Assets/Uploads/${id}`, { headers: headers() })
			.then(function (response) {
				if (response) {
					return response;
				}
			})
			.catch(function (error) {
				console.log('getProjectUploads_ERROR', error.response);
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: 'fail',
					payload: error
				});
			});
	};
};

export const removeAssetsFromProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Assets/Remove`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REMOVE_ASSET_FROM_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.REMOVE_ASSET_FROM_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.REMOVE_ASSET_FROM_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const deleteProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Delete`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.DELETE_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.DELETE_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.DELETE_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const archiveProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Archive`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ARCHIVE_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.ARCHIVE_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ARCHIVE_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const unarchiveProject = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Projects/Unarchive`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.UNARCHIVE_PROJECT,
					payload: response.data
				});
				FeedbackMessage(TYPE.UNARCHIVE_PROJECT_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.UNARCHIVE_PROJECT_FAILED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const confirmEmail = (id) => {
	return (dispatch) => {
		return axios
			.get(`${window.location.origin}/api/SuperAdmin/ConfirmEmail/${id}`, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.CONFIRM_EMAIL_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.CONFIRM_EMAIL_FULFILLED);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.CONFIRM_EMAIL_REJECTED,
					payload: error
				});
			});
	};
};

export const resetPassword = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/SuperAdmin/ResetPassword`, data, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.POST_RESET_PASSWORD_DATA_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.POST_RESET_PASSWORD_DATA_FULFILLED);
				return response.data;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.POST_RESET_PASSWORD_DATA_REJECTED,
					payload: error
				});
			});
	};
};

export const saveTeam = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Teams/Save`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.SAVE_TEAM,
					payload: response.data
				});
				FeedbackMessage(TYPE.SAVE_TEAM_FULFILLED);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.SAVE_TEAM_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const deleteTeam = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Teams/Delete`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.DELETE_TEAMS,
					payload: response.data
				});
				FeedbackMessage(TYPE.DELETE_TEAMS_SUCCESS);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.DELETE_TEAMS_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const addThumbnail = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/addThumbnail`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_ASSETTHUMBNAIL_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_ASSETTHUMBNAIL_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_ASSETTHUMBNAIL_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const removeThumbnail = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/removeThumbnail`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.REMOVE_ASSETTHUMBNAIL_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.REMOVE_ASSETTHUMBNAIL_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.REMOVE_ASSETTHUMBNAIL_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const addPackage = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/addPackage`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.ADD_ASSETPACKAGE_FULFILLED,
					payload: response.data
				});
				FeedbackMessage(TYPE.ADD_ASSETPACKAGE_FULFILLED);
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_ASSETPACKAGE_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const convertPDFPreview = (data) => {
	const FileDownload = require('js-file-download');
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/Assets/convertPDFPreview`, { ...data }, { headers: headers() })
			.then(function (response) {
				//FileDownload(response.data, `img.jpeg`);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.ADD_ASSETTHUMBNAIL_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};

export const getEmailTemplates = () => {
	return (dispatch) => {
		let url = `${window.location.origin}/api/EmailTemplates`;
		return axios
			.get(url, { headers: headers() })
			.then(function (response) {
				console.log('response', response);
				return response;
			})
			.catch(function (error) {
				if (error.response && error.response.status === 401) {
					logout();
				}
				dispatch({
					type: TYPE.GET_EMAIL_TEMPLATES_REJECTED,
					payload: error
				});
			});
	};
};

export const saveEmailTemplate = (data) => {
	return (dispatch) => {
		return axios
			.post(`${window.location.origin}/api/EmailTemplates/Save`, { ...data }, { headers: headers() })
			.then(function (response) {
				dispatch({
					type: TYPE.SAVE_EMAIL_TEMPLATE,
					payload: response.data
				});
				FeedbackMessage(TYPE.SAVE_EMAIL_TEMPLATE_FULFILLED);
				return response;
			})
			.catch(function (error) {
				dispatch({
					type: TYPE.SAVE_EMAIL_TEMPLATE_REJECTED,
					payload: error
				});

				if (error.response && error.response.status === 401) {
					logout();
				}
			});
	};
};
