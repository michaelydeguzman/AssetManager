import {
	EMAIL_TEMPLATE_CLASSIFICATION,
	EMAIL_TEMPLATE_CATEGORIES,
	getFilterType,
	PROJECT_STATUS
} from '../pages/constants';

export const getAllowedRoutes = (routes, isAdministrator) => {
	var permittedRoutes = [];

	if (isAdministrator) {
		permittedRoutes = routes;
	} else {
		permittedRoutes = routes.filter(route => !route.adminOnly);
    }

	return permittedRoutes;
}

// recursive data
export const NestedChild = (array, root) => {
	let result = [];
	for (let arrayKey in array) {
		if (array[arrayKey].parentFolderId === root) {
			const children = NestedChild(array, array[arrayKey].key);
			if (children.length) {
				children.sort((a, b) => {
					return a.orderNumber - b.orderNumber;
				});
				array[arrayKey].children = children;
			}
			result.push(array[arrayKey]);
		}
	}
	return result;
};

export const NestedChildUser = (array, root, arrayE) => {
	let result = [];
	for (let arrayKey in array) {
		if (array[arrayKey].parentFolderId === root) {
			const children = NestedChildUser(array, array[arrayKey].key, arrayE);
			if (children.length) {
				var unique = Array.from(new Set(children));
				array[arrayKey].children = unique;
			}
			var b = arrayE.find((item) => {
				return item === array[arrayKey].id;
			});
			if (b !== undefined) {
				result.push(array[arrayKey]);
			}
			if (array[arrayKey].children && array[arrayKey].children.length > 0) {
				////array[arrayKey].disabled = true;
				if (b === undefined) array[arrayKey].selectable = false;
				result.push(array[arrayKey]);
			}
		}
	}
	return result;
};

export const Compare = (a, b) => {
	// Use toUpperCase() to ignore character casing
	const descA = a.description.toUpperCase();
	const descB = b.description.toUpperCase();

	let comparison = 0;
	if (descA > descB) {
		comparison = 1;
	} else if (descA < descB) {
		comparison = -1;
	}
	return comparison;
};

export const NestedFoldersChild = (array, root) => {
	let result = [];
	for (let arrayKey in array) {
		if (array[arrayKey].parentFolderId === root) {
			const children = NestedFoldersChild(array, array[arrayKey].id);
			if (children.length) {
				const child = {
					key: children.key,
					id: children.id,
					title: children.title,
					parentFolderId: children.parentFolderId,
					comments: children.comments,
					value: children.id
				};
				array[arrayKey].children = child;
			}
			const child = {
				key: array[arrayKey].key,
				id: array[arrayKey].id,
				title: array[arrayKey].title,
				parentFolderId: array[arrayKey].parentFolderId,
				comments: array[arrayKey].comments,
				value: array[arrayKey].id
			};
			result.push(child);
		}
	}
	return result;
};

export const Uint8ToBase64 = (u8Arr) => {
	var CHUNK_SIZE = 0x8000; //arbitrary number
	var index = 0;
	var { length } = u8Arr;
	var result = '';
	var slice;
	while (index < length) {
		slice = u8Arr.subarray(index, Math.min(index + CHUNK_SIZE, length));
		result += String.fromCharCode.apply(null, slice);
		index += CHUNK_SIZE;
	}
	return btoa(result);
};

export const getSelectedCountriesRegions = (countries, selectedCountries) => {
	let regionOptions = [];
	var selectedCountryOptions = countries.filter((c) => {
		if (selectedCountries.filter((x) => x.id == c.id).length > 0) {
			return true;
		} else {
			return false;
		}
	});

	selectedCountryOptions.map((c) => {
		c.regions.map((r) => {
			regionOptions.push({
				id: r.id,
				country: c.name,
				countryId: c.id,
				description: r.description
			});
		});
	});

	return regionOptions;
};

export const getSortResult = (value, assets, isAsc) => {
	const data = assets;
	switch (value) {
		case 'Date Created':
			data.sort((a, b) =>
				a.createdDate.localeCompare(b.createdDate, undefined, {
					numeric: true,
					sensitivity: 'base'
				})
			);
			break;
		case 'Display Name':
			data.sort((a, b) =>
				a.name.localeCompare(b.name, undefined, {
					numeric: true,
					sensitivity: 'base'
				})
			);
			break;
		case 'Date Approved':
			data.sort((a, b) => {
				if (a.statusName === 'Approved' && b.statueName === 'Approved') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Approved' && b.statueName !== 'Approved') {
					return -1;
				} else if (a.statusName !== 'Approved' && b.statueName === 'Approved') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		case 'Date Rejected':
			data.sort((a, b) => {
				if (a.statusName === 'Rejected' && b.statueName === 'Rejected') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Rejected' && b.statueName !== 'Rejected') {
					return -1;
				} else if (a.statusName !== 'Rejected' && b.statueName === 'Rejected') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		case 'Date Submitted':
			data.sort((a, b) => {
				if (a.statusName === 'Submitted For Review' && b.statueName === 'Submitted For Review') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Submitted For Review' && b.statueName !== 'Submitted For Review') {
					return -1;
				} else if (a.statusName !== 'Submitted For Review' && b.statueName === 'Submitted For Review') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		default:
			break;
	}
	if (isAsc) {
		data.reverse();
	}
	return data;
};
export const getFilterResult = (checkedFilters, source) => {
	if (Object.keys(checkedFilters).length === 0) return source;
	const result = source.filter((row) => {
		let flag = true;
		if (
			checkedFilters.Type &&
			checkedFilters.Type.length > 0 &&
			!checkedFilters.Type.includes(getFilterType(row.fileType.split('/').shift()))
		) {
			flag = false;
		}
		if (
			checkedFilters.Status &&
			checkedFilters.Status.length > 0 &&
			!checkedFilters.Status.includes(row.statusName.split('/').shift())
		) {
			flag = false;
		}
		if (checkedFilters.Tag && checkedFilters.Tag.length > 0) {
			let result = row.tags.find((tag) => checkedFilters.Tag.includes(getFilterType(tag.isCognitive)));
			if (!result || result.length === 0) {
				flag = false;
			}
		}
		if (checkedFilters.Account && checkedFilters.Account.length > 0) {
			let result = row.accounts.find((account) => checkedFilters.Account.find((a) => a === account.id));
			if (!result || result.length === 0) {
				flag = false;
			}
		}
		if (checkedFilters.Country && checkedFilters.Country.length > 0) {
			let result = row.countries.find((country) => checkedFilters.Country.find((c) => c === country.id));
			if (!result || result.length === 0) {
				flag = false;
			}
		}
		return flag;
	});
	return result;
};

export const getSearchedResult = (keys, assets) => {
	if (keys.length === 0) {
		return assets;
	}
	let ids = [];
	assets &&
		assets.map((data) => {
			keys.map((key) => {
				let flag = true;
				data.tags.map((tag) => {
					if (tag.name.toLowerCase().includes(key.toLowerCase())) {
						ids.push(data.id);
						flag = false;
					}
				});
				if (flag && data.name && data.name.toLowerCase().includes(key.toLowerCase())) {
					ids.push(data.id);
					flag = false;
				}
				if (flag && data.extension && data.extension.toLowerCase().includes(key.toLowerCase())) {
					ids.push(data.id);
					flag = false;
				}
				if (flag && data.fileName && data.fileName.toLowerCase().includes(key.toLowerCase())) {
					ids.push(data.id);
					flag = false;
				}
				if (flag && data.comments && data.comments.toLowerCase().includes(key.toLowerCase())) {
					ids.push(data.id);
					flag = false;
				}
			});
		});

	return assets.filter((row) => ids.includes(row.id));
};

export const getGridLayout = (gridSetting) => {
	switch (gridSetting) {
		case 1:
			return {
				gutter: 16,
				xs: 1,
				sm: 1,
				md: 2,
				lg: 2,
				xl: 2,
				xxl: 3
			};
		case 2:
			return {
				gutter: 16,
				xs: 2,
				sm: 3,
				md: 4,
				lg: 4,
				xl: 6,
				xxl: 8
			};
		case 3:
			return {
				xs: 1,
				sm: 1,
				md: 1,
				lg: 1,
				xl: 1,
				xxl: 1
			};
		case 4:
			return {
				gutter: 16,
				xs: 1,
				sm: 1,
				md: 3,
				lg: 4,
				xl: 4,
				xxl: 6
			};
		default:
			return {};
	}
};

export const getPinSortResult = (value, assets) => {
	const data = assets;
	switch (value) {
		case 'Default':
			data.sort((a, b) => {
				return a.orderNumber - b.orderNumber;
			});
			break;
		case 'Display Name':
			data.sort((a, b) =>
				a.name.localeCompare(b.name, undefined, {
					numeric: true,
					sensitivity: 'base'
				})
			);
			break;
		case 'Date Approved':
			data.sort((a, b) => {
				if (a.statusName === 'Approved' && b.statueName === 'Approved') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Approved' && b.statueName !== 'Approved') {
					return -1;
				} else if (a.statusName !== 'Approved' && b.statueName === 'Approved') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		case 'Date Rejected':
			data.sort((a, b) => {
				if (a.statusName === 'Rejected' && b.statueName === 'Rejected') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Rejected' && b.statueName !== 'Rejected') {
					return -1;
				} else if (a.statusName !== 'Rejected' && b.statueName === 'Rejected') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		case 'Date Submitted':
			data.sort((a, b) => {
				if (a.statusName === 'Submitted For Review' && b.statueName === 'Submitted For Review') {
					return a.statusUpdatedDate.localeCompare(b.statusUpdatedDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				} else if (a.statusName === 'Submitted For Review' && b.statueName !== 'Submitted For Review') {
					return -1;
				} else if (a.statusName !== 'Submitted For Review' && b.statueName === 'Submitted For Review') {
					return 1;
				} else {
					return a.createdDate.localeCompare(b.createdDate, undefined, {
						numeric: true,
						sensitivity: 'base'
					});
				}
			});
			break;
		case 'Date Created':
			data.sort((a, b) =>
				a.createdDate.localeCompare(b.createdDate, undefined, {
					numeric: true,
					sensitivity: 'base'
				})
			);
			break;
		default:
			break;
	}
	return data;
};

export const CheckCompanyAccess = (array, folerId, user) => {
	let result = false;
	for (let arrayKey in array) {
		if (array[arrayKey].id === folerId) {
			if (array[arrayKey].company.length > 0 && user.companyId === array[arrayKey].company[0].id) {
				result = true;
			} else {
				result = CheckCompanyAccess(array, array[arrayKey].parentFolderId, user);
			}
		}
	}
	return result;
};

// function onSearch(e) {
// 	searchDispatch({
// 		type: 'KEY',
// 		payload: e.currentTarget.value
// 	});

// 	efiles &&
// 		searchDispatch({
// 			type: 'DATA',
// 			payload: efiles.filter((row) => {
// 				let flag = false;
// 				row.tags.map((tag) => {
// 					if (tag.name.toLowerCase().includes(e.currentTarget.value.toLowerCase())) {
// 						flag = true;
// 					}
// 				});
// 				if (flag) {
// 					return true;
// 				}
// 				if (row.name && row.name.toLowerCase().includes(e.currentTarget.value.toLowerCase())) {
// 					return true;
// 				}
// 				if (row.extension && row.extension.toLowerCase().includes(e.currentTarget.value.toLowerCase())) {
// 					return true;
// 				}
// 				if (row.fileName && row.fileName.toLowerCase().includes(e.currentTarget.value.toLowerCase())) {
// 					return true;
// 				}
// 				if (row.comments && row.comments.toLowerCase().includes(e.currentTarget.value.toLowerCase())) {
// 					return true;
// 				}
// 				return false;
// 			})
// 		});
// }

export const getUserFromUsers = (userId, users) => {
	var user = null;
	if (users) {
		var filterUser = users.filter((u) => u.id === userId);

		if (filterUser.length > 0) {
			user = filterUser[0];
		}
	}

	return user;
};

export const getProjectStatusTagName = (id) => {
	return PROJECT_STATUS[id];
};

export const getProjectStatusTagColor = (id) => {
	switch (id) {
		case 0:
			return '#b4b4b4';
			break;
		case 1:
			return '#DC143C';
			break;
		case 2:
			return '#FFA500';
			break;
		case 3:
			return '#008000';
			break;
		case 4:
			return '#565656';
			break;
	}
};

export const getAvatarAlt = (username) => {
	var uarr = username.split(/(\s+)/);

	return uarr[0].substr(0, 1) + uarr[uarr.length - 1].substr(0, 1);
};

export const getEmailTemplateClassification = (id) => {
	return EMAIL_TEMPLATE_CLASSIFICATION[id - 1];
};

export const getEmailTemplateCategory = (id) => {
	return EMAIL_TEMPLATE_CATEGORIES[id - 1];
};
