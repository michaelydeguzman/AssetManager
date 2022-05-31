const DAM_Token = 'DAM_Token';
const DAM_User = 'DAM_User';
const DAM_UserRole = 'DAM_UserRole';
const DAM_Settings = 'DAM_Settings';

//getAdminMode(SiderBar content for Home page or Admin page)
export const getAdminMode = () => {
	return window.location.pathname.includes('admin');
};

export const headers = () => {
	return {
		Authorization: `Bearer ${getToken()}`
	};
};

//DAM_Token
export const setToken = (accessToken) => {
	window.localStorage.setItem(DAM_Token, accessToken);
	return accessToken;
};

export const getToken = () => {
	return window.localStorage.getItem(DAM_Token);
};

//DAM_User
export const setUser = (user) => {
	window.localStorage.setItem(DAM_User, JSON.stringify(user));
	return user;
};

export const getUser = () => {
	return getData(DAM_User);
};

//DAM_UserRole
export const setUserRole = (user) => {
	window.localStorage.setItem(DAM_UserRole, JSON.stringify(user));
	return user;
};

export const getUserRole = () => {
	return getData(DAM_UserRole);
};

export const initialAppMode = () => {
	const appMode = JSON.parse(window.localStorage.getItem(DAM_Settings));
	if (appMode && appMode.CardGridPreference && appMode.History) {
		setAppMode(appMode.CardGridPreference, appMode.History);
	} else {
		setAppMode();
	}
};

export const getAppMode = () => {
	return getData(DAM_Settings);
};

export const setAppMode = (gridPreference = 1, searchHistory = []) => {
	window.localStorage.setItem(
		DAM_Settings,
		JSON.stringify({ CardGridPreference: gridPreference, History: searchHistory })
	);
};

export const setGridPreference = (gridPreference) => {
	const appMode = JSON.parse(window.localStorage.getItem(DAM_Settings));
	setAppMode(gridPreference, appMode.History);
};

export const setSearchHistory = (searchHistory) => {
	const appMode = JSON.parse(window.localStorage.getItem(DAM_Settings));
	setAppMode(appMode.CardGridPreference, searchHistory);
};

const getData = (type) => {
	const data = JSON.parse(window.localStorage.getItem(type));
	if (data) {
		return data;
	} else {
		window.localStorage.clear();
		logout();
	}
};

const deleteData = (type) => {
	window.localStorage.removeItem(type);
};

//LogOut
export const logout = () => {
	deleteData(DAM_Token);
	deleteData(DAM_User);
	deleteData(DAM_UserRole);
	if (
		!window.location.href.includes('login') &&
		!window.location.href.includes('createpassword') &&
		!window.location.href.includes('resetpassword')
	) {
		window.location.reload();
	}
};
