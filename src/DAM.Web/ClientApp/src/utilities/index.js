const localStore = localStorage; // linter should declared this as undefined.

export const getUserDataFromLocalStorage = (key) => {
    let localStorageData;
    if (localStore.getItem('USER_AUTH_DATA') !== null) {
        localStorageData = JSON.parse(localStore.getItem('USER_AUTH_DATA'))[key];
    }
    return localStorageData;
};

export const isLocalStorageEmpty = () => {
    let strResult;
    if (localStore.getItem('SESSION_TOKEN') !== null) {
        strResult = true;
    } else {
        strResult = false;
    }
    return strResult;
};

export const clearLocalStorage = () => {
    localStore.clear();
};

export const getToken = () => {
    return window.localStorage.getItem('SESSION_TOKEN');
};

export const headers = () => {
    return {
        Authorization: `Bearer ${getToken()}`,
        Accept: 'application/json',
        'Content-Type': 'application/json'
    };
};

export const numberFormat = (amount, decimalPlace = 2) => {
    if (isNaN(amount)) {
        return amount;
    }

    const pieces = parseFloat(amount).toFixed(decimalPlace).split('');
    let ii = pieces.length - 3;
    while ((ii -= 3) > 0) {
        pieces.splice(ii, 0, ',');
    }

    return pieces.join('');
};

export const getBase64 = (img, callback) => {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result));
    reader.readAsDataURL(img);
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

