import { message } from 'antd';
import { useTranslation } from 'react-i18next';
import React from 'react';

export const TYPE = {
	_FULFILLED: '_FULFILLED',
	_REJECTED: '_REJECTED',
	_LOADING: 'LOADING',

	GET_FOLDER_DATA: 'GET_FOLDER_DATA',
	GET_FOLDER_DATA_FULFILLED: 'GET_FOLDER_DATA_FULFILLED',
	GET_FOLDER_DATA_REJECTED: 'GET_FOLDER_DATA_REJECTED',
	GET_FOLDER_DETAIL_FULFILLED: 'GET_FOLDER_DETAIL_FULFILLED',
	GET_FOLDER_DETAIL_REJECTED: 'GET_FOLDER_DETAIL_REJECTED',
	ADD_FOLDER: 'ADD_FOLDER',
	ADD_FOLDER_FULFILLED: 'ADD_FOLDER_FULFILLED',
	ADD_FOLDER_REJECTED: 'ADD_FOLDER_REJECTED',
	UPDATE_FOLDER: 'UPDATE_FOLDER',
	UPDATE_FOLDER_FULFILLED: 'UPDATE_FOLDER_FULFILLED',
	UPDATE_FOLDER_REJECTED: 'UPDATE_FOLDER_REJECTED',
	DELETE_FOLDER: 'DELETE_FOLDER',
	DELETE_FOLDER_FULFILLED: 'DELETE_FOLDER_FULFILLED',
	DELETE_FOLDER_REJECTED: 'DELETE_FOLDER_REJECTED',
	MOVE_FOLDER: 'MOVE_FOLDER',
	MOVE_FOLDER_FULFILLED: 'MOVE_FOLDER_FULFILLED',
	MOVE_FOLDER_REJECTED: 'MOVE_FOLDER_REJECTED',
	COPY_FOLDER: 'COPY_FOLDER',
	COPY_FOLDER_FULFILLED: 'COPY_FOLDER_FULFILLED',
	COPY_FOLDER_REJECTED: 'COPY_FOLDER_REJECTED',
	BULK_MOVE_FOLDER: 'BULK_MOVE_FOLDER',
	BULK_MOVE_FOLDER_FULFILLED: 'BULK_MOVE_FOLDER_FULFILLED',
	BULK_MOVE_FOLDER_REJECTED: 'BULK_MOVE_FOLDER_REJECTED',
	ORDER_FOLDER: 'ORDER_FOLDER',
	ORDER_FOLDER_FULFILLED: 'ORDER_FOLDER_FULFILLED',
	ORDER_FOLDER_REJECTED: 'ORDER_FOLDER_REJECTED',

	GET_FILE_DATA: 'GET_FILE_DATA',
	GET_FILE_DATA_FULFILLED: 'GET_FILE_DATA_FULFILLED',
	GET_FILE_DATA_REJECTED: 'GET_FILE_DATA_REJECTED',
	GET_FILE_DATA_BY_ID: 'GET_FILE_DATA_BY_ID',
	GET_FILE_DATA_BY_ID_FULFILLED: 'GET_FILE_DATA_BY_ID_FULFILLED',
	GET_FILE_DATA_BY_ID_REJECTED: 'GET_FILE_DATA_BY_ID_REJECTED',

	GET_PIN_ASSETS: 'GET_PIN_ASSETS',
	GET_PIN_ASSETS_FULFILLED: 'GET_PIN_ASSETS_FULFILLED',
	GET_PIN_ASSETS_REJECTED: 'GET_PIN_ASSETS_REJECTED',
	GET_PIN_ASSETS_DETAIL_FULFILLED: 'GET_PIN_ASSETS_DETAIL_FULFILLED',
	GET_PIN_ASSETS_DETAIL_REJECTED: 'GET_PIN_ASSETS_DETAIL_REJECTED',
	ADD_PIN_ASSETS_FULFILLED: 'ADD_PIN_ASSETS_FULFILLED',
	ADD_PIN_ASSETS_REJECTED: 'ADD_PIN_ASSETS_REJECTED',
	REMOVE_PIN_ASSETS_FULFILLED: 'REMOVE_PIN_ASSETS_FULFILLED',
	REMOVE_PIN_ASSETS_REJECTED: 'REMOVE_PIN_ASSETS_REJECTED',
	REPLACE_PIN_ASSETS_FULFILLED: 'REPLACE_PIN_ASSETS_FULFILLED',
	REPLACE_PIN_ASSETS_REJECTED: 'REPLACE_PIN_ASSETS_REJECTED',
	ORDER_PIN_ASSETS_FULFILLED: 'ORDER_PIN_ASSETS_FULFILLED',
	ORDER_PIN_ASSETS_REJECTED: 'ORDER_PIN_ASSETS_REJECTED',
	CHECK_DUPLICATE_ASSET: 'CHECK_DUPLICATE_ASSET',
	ADD_ASSETTHUMBNAIL_FULFILLED: 'ADD_ASSETTHUMBNAILA_FULFILLED',
	ADD_ASSETTHUMBNAIL_REJECTED: 'ADD_ASSETTHUMBNAILA_REJECTED',
	REMOVE_ASSETTHUMBNAIL_FULFILLED: 'REMOVE_ASSETTHUMBNAILA_FULFILLED',
	REMOVE_ASSETTHUMBNAIL_REJECTED: 'REMOVE_ASSETTHUMBNAILA_REJECTED',
	ADD_ASSETPACKAGE_FULFILLED: 'ADD_ASSETPACKAGE_FULFILLED',
	ADD_ASSETPACKAGE_REJECTED: 'ADD_ASSETPACKAGE_REJECTED',

	GET_PIN_FOLDERS: 'GET_PIN_FOLDERS',
	GET_PIN_FOLDERS_FULFILLED: 'GET_PIN_FOLDERS_FULFILLED',
	GET_PIN_FOLDERS_REJECTED: 'GET_PIN_FOLDERS_REJECTED',
	GET_PIN_FOLDERS_DETAIL_FULFILLED: 'GET_PIN_FOLDERS_DETAIL_FULFILLED',
	GET_PIN_FOLDERS_DETAIL_REJECTED: 'GET_PIN_FOLDERS_DETAIL_REJECTED',
	ADD_PIN_FOLDERS_FULFILLED: 'ADD_PIN_FOLDERS_FULFILLED',
	ADD_PIN_FOLDERS_REJECTED: 'ADD_PIN_FOLDERS_REJECTED',
	REMOVE_PIN_FOLDERS_FULFILLED: 'REMOVE_PIN_FOLDERS_FULFILLED',
	REMOVE_PIN_FOLDERS_REJECTED: 'REMOVE_PIN_FOLDERS_REJECTED',
	REPLACE_PIN_FOLDERS_FULFILLED: 'REPLACE_PIN_FOLDERS_FULFILLED',
	REPLACE_PIN_FOLDERS_REJECTED: 'REPLACE_PIN_FOLDERS_REJECTED',
	ORDER_PIN_FOLDERS_FULFILLED: 'ORDER_PIN_FOLDERS_FULFILLED',
	ORDER_PIN_FOLDERS_REJECTED: 'ORDER_PIN_FOLDERS_REJECTED',

	GET_FILE_VERSION_DATA: 'GET_FILE_VERSION_DATA',
	GET_FILE_VERSION_DATA_FULFILLED: 'GET_FILE_VERSION_DATA_FULFILLED',
	GET_FILE_VERSION_DATA_REJECTED: 'GET_FILE_VERSION_DATA_REJECTED',

	REVERT_VERSION_DATA: 'REVERT_VERSION_DATA',
	REVERT_VERSION_DATA_FULFILLED: 'REVERT_VERSION_DATA_FULFILLED',
	REVERT_VERSION_DATA_REJECTED: 'REVERT_VERSION_DATA_REJECTED',

	GET_ACCOUNTS: 'GET_ACCOUNTS',
	GET_ACCOUNTS_FULFILLED: 'GET_ACCOUNTS_FULFILLED',
	GET_ACCOUNTS_REJECTED: 'GET_ACCOUNTS_REJECTED',

	GET_COUNTRIES: 'GET_COUNTRIES',
	GET_COUNTRIES_FULFILLED: 'GET_COUNTRIES_FULFILLED',
	GET_COUNTRIES_REJECTED: 'GET_COUNTRIES_REJECTED',

	GET_REGIONS: 'GET_REGIONS',
	GET_REGIONS_FULFILLED: 'GET_REGIONS_FULFILLED',
	GET_REGIONS_REJECTED: 'GET_REGIONS_REJECTED',

	UPLOAD_ASSET: 'UPLOAD_ASSET',
	UPLOAD_ASSET_FULFILLED: 'UPLOAD_ASSET_FULFILLED',
	UPLOAD_ASSET_REJECTED: 'UPLOAD_ASSET_REJECTED',

	UPLOAD_ASSET_VERSION: 'UPLOAD_ASSET_VERSION',
	UPLOAD_ASSET_VERSION_FULFILLED: 'UPLOAD_ASSET_VERSION_FULFILLED',
	UPLOAD_ASSET_VERSION_REJECTED: 'UPLOAD_ASSET_VERSION_REJECTED',

	UPDATE_ASSET: 'UPDATE_ASSET',
	UPDATE_ASSET_FULFILLED: 'UPDATE_ASSET_FULFILLED',
	UPDATE_ASSET_REJECTED: 'UPDATE_ASSET_REJECTED',

	MOVE_ASSETS: 'MOVE_ASSETS',
	MOVE_ASSETS_FULFILLED: 'MOVE_ASSETS_FULFILLED',
	MOVE_ASSETS_REJECTED: 'MOVE_ASSETS_REJECTED',

	ARCHIVE_ASSETS: 'ARCHIVE_ASSETS',
	ARCHIVE_ASSETS_FULFILLED: 'ARCHIVE_ASSETS_FULFILLED',
	ARCHIVE_ASSETS_REJECTED: 'ARCHIVE_ASSETS_REJECTED',

	GET_WOPI_PARAMS: 'GET_WOPI_PARAMS',
	GET_WOPI_PARAMS_FULFILLED: 'GET_WOPI_PARAMS_FULFILLED',
	GET_WOPI_PARAMS_REJECTED: 'GET_WOPI_PARAMS_REJECTED',

	GET_USERS: 'GET_USERS',
	GET_USERS_FULFILLED: 'GET_USERS_FULFILLED',
	GET_USERS_REJECTED: 'GET_USERS_REJECTED',

	GET_TEAMS: 'GET_TEAMS',
	GET_TEAMS_FULFILLED: 'GET_TEAMS_FULFILLED',
	GET_TEAMS_REJECTED: 'GET_TEAMS_REJECTED',

	SAVE_TEAM: 'SAVE_TEAM',
	SAVE_TEAM_FULFILLED: 'SAVE_TEAM_FULFILLED',
	SAVE_TEAM_REJECTED: 'SAVE_TEAM_REJECTED',

	DELETE_TEAMS: 'DELETE_TEAMS',
	DELETE_TEAMS_FULFILLED: 'DELETE_TEAMS_FULFILLED',
	DELETE_TEAMS_REJECTED: 'DELETE_TEAMS_REJECTED',

	GET_APPROVERS: 'GET_APPROVERS',
	GET_APPROVERS_FULFILLED: 'GET_APPROVERS_FULFILLED',
	GET_APPROVERS_REJECTED: 'GET_APPROVERS_REJECTED',

	ADD_USERS: 'ADD_USERS',
	ADD_USERS_FULFILLED: 'ADD_USERS_FULFILLED',
	ADD_USERS_REJECTED: 'ADD_USERS_REJECTED',

	UPDATE_USERS: 'UPDATE_USERS',
	UPDATE_USERS_FULFILLED: 'UPDATE_USERS_FULFILLED',
	UPDATE_USERS_REJECTED: 'UPDATE_USERS_REJECTED',

	UPDATE_USERS_ADMIN: 'UPDATE_USERS_ADMIN',
	UPDATE_USERS_ADMIN_FULFILLED: 'UPDATE_USERS_ADMIN_FULFILLED',
	UPDATE_USERS_ADMIN_REJECTED: 'UPDATE_USERS_ADMIN_REJECTED',

	UPDATE_IMAGE_USERS: 'UPDATE_IMAGE_USERS',
	UPDATE_IMAGE_USERS_FULFILLED: 'UPDATE_IMAGE_USERS_FULFILLED',
	UPDATE_IMAGE_USERS_REJECTED: 'UPDATE_IMAGE_USERS_REJECTED',

	SHARE_ASSET: 'SHARE_ASSET',
	SHARE_ASSET_FULFILLED: 'SHARE_ASSET_FULFILLED',
	SHARE_ASSET_REJECTED: 'SHARE_ASSET_REJECTED',
	SHARE_BULK_ASSETS: 'SHARE_BULK_ASSETS',
	SHARE_BULK_ASSETS_FULFILLED: 'SHARE_BULK_ASSETS_FULFILLED',
	SHARE_BULK_ASSETS_REJECTED: 'SHARE_BULK_ASSETS_REJECTED',
	SHARE_FOLDER_TO_USER_FULFILLED: 'SHARE_FOLDER_TO_USER_FULFILLED',
	SHARE_FOLDER_TO_USER_REJECTED: 'SHARE_FOLDER_TO_USER_REJECTED',

	DOWNLOAD_ASSET: 'DOWNLOAD_ASSET',
	DOWNLOAD_ASSET_FULFILLED: 'DOWNLOAD_ASSET_FULFILLED',
	DOWNLOAD_ASSET_REJECTED: 'DOWNLOAD_ASSET_REJECTED',

	ADD_TO_DYNAMICS: 'ADD_TO_DYNAMICS',
	ADD_TO_DYNAMICS_FULFILLED: 'ADD_TO_DYNAMICS_FULFILLED',
	ADD_TO_DYNAMICS_REJECTED: 'ADD_TO_DYNAMICS_REJECTED',

	ADD_APPROVALS: 'ADD_APPROVALS',
	ADD_APPROVALS_FULFILLED: 'ADD_APPROVALS_FULFILLED',
	ADD_APPROVALS_REJECTED: 'ADD_APPROVALS_REJECTED',

	GET_ASSETAPPROVALS: 'GET_ASSETAPPROVALS',
	GET_ASSETAPPROVALS_FULFILLED: 'GET_ASSETAPPROVALS_FULFILLED',
	GET_ASSETAPPROVALS_REJECTED: 'GET_ASSETAPPROVALS_REJECTED',

	ADD_APPROVALLEVELS: 'ADD_APPROVALLEVELS',
	ADD_APPROVALLEVELS_FULFILLED: 'ADD_APPROVALLEVELS_FULFILLED',
	ADD_APPROVALLEVELS_REJECTED: 'ADD_APPROVALLEVELS_REJECTED',

	UPDATE_APPROVALLEVELS: 'UPDATE_APPROVALLEVELS',
	UPDATE_APPROVALLEVELS_FULFILLED: 'UPDATE_APPROVALLEVELS_FULFILLED',
	UPDATE_APPROVALLEVELS_REJECTED: 'UPDATE_APPROVALLEVELS_REJECTED',

	GET_APPROVALLEVELS: 'GET_APPROVALLEVELS',
	GET_APPROVALLEVELS_FULFILLED: 'GET_APPROVALLEVELS_FULFILLED',
	GET_APPROVALLEVELS_REJECTED: 'GET_APPROVALLEVELS_REJECTED',

	GET_USERROLES: 'GET_USERROLES',
	GET_USERROLES_FULFILLED: 'GET_USERROLES_FULFILLED',
	GET_USERROLES_REJECTED: 'GET_USERROLES_REJECTED',

	GET_FEATURE_FLAGS_FULFILLED: 'GET_FEATURE_FLAGS_FULFILLED',
	GET_FEATURE_FLAGS_REJECTED: 'GET_FEATURE_FLAGS_REJECTED',

	GET_PARTNERS: 'GET_PARTNERS',
	GET_PARTNERS_FULFILLED: 'GET_PARTNERS_FULFILLED',
	GET_PARTNERS_REJECTED: 'GET_PARTNERS_REJECTED',

	ADD_PARTNERS: 'ADD_PARTNERS',
	ADD_PARTNERS_FULFILLED: 'ADD_PARTNERS_FULFILLED',
	ADD_PARTNERS_REJECTED: 'ADD_PARTNERS_REJECTED',

	UPDATE_PARTNERS: 'UPDATE_PARTNERS',
	UPDATE_PARTNERS_FULFILLED: 'UPDATE_PARTNERS_FULFILLED',
	UPDATE_PARTNERS_REJECTED: 'UPDATE_PARTNERS_REJECTED',

	INVITE_NEW_USER: 'INVITE_NEW_USER',
	INVITE_NEW_USER_FULFILLED: 'INVITE_NEW_USER_FULFILLED',
	INVITE_NEW_USER_REJECTED: 'INVITE_NEW_USER_REJECTED',

	CHANGE_PASSWORD: 'CHANGE_PASSWORD',
	CHANGE_PASSWORD_FULFILLED: 'CHANGE_PASSWORD_FULFILLED',
	CHANGE_PASSWORD_REJECTED: 'CHANGE_PASSWORD_REJECTED',

	GET_USER_PARTNERS: 'GET_USER_PARTNERS',
	GET_USER_PARTNERS_FULFILLED: 'GET_USER_PARTNERS_FULFILLED',
	GET_USER_PARTNERS_REJECTED: 'GET_USER_PARTNERS_REJECTED',

	GET_USER_FOLDERS: 'GET_USER_FOLDERS',
	GET_USER_FOLDERS_FULFILLED: 'GET_USER_FOLDERS_FULFILLED',
	GET_USER_FOLDERS_REJECTED: 'GET_USER_FOLDERS_REJECTED',

	GET_APPROVALS: 'GET_APPROVALS',
	GET_APPROVALS_FULFILLED: 'GET_APPROVALS_FULFILLED',
	GET_APPROVALS_REJECTED: 'GET_APPROVALS_REJECTED',

	UPDATE_APPROVALS: 'UPDATE_APPROVALS',
	UPDATE_APPROVALS_FULFILLED: 'UPDATE_APPROVALS_FULFILLED',
	UPDATE_APPROVALS_REJECTED: 'UPDATE_APPROVALS_REJECTED',

	GET_AUDITS: 'GET_AUDITS',
	GET_AUDITS_FULFILLED: 'GET_AUDITS_FULFILLED',
	GET_AUDITS_REJECTED: 'GET_AUDITS_FULFILLED',

	GET_ARCHIVE: 'GET_ARCHIVE',
	GET_ARCHIVE_FULFILLED: 'GET_ARCHIVE_FULFILLED',
	GET_ARCHIVE_REJECTED: 'GET_ARCHIVE_REJECTED',

	UPDATE_ASSET_STATUS: 'UPDATE_ASSET_STATUS',
	UPDATE_ASSET_STATUS_FULFILLED: 'UPDATE_ASSET_STATUS_FULFILLED',
	UPDATE_ASSET_STATUS_REJECTED: 'UPDATE_ASSET_STATUS_REJECTED',

	POST_LOGIN_DATA: 'POST_LOGIN_DATA',
	POST_LOGIN_DATA_FULFILLED: 'POST_LOGIN_DATA_FULFILLED',
	POST_LOGIN_DATA_REJECTED: 'POST_LOGIN_DATA_REJECTED',

	POST_REQ_PW_DATA: 'POST_REQ_PW_DATA',
	POST_REQ_PW_DATA_FULFILLED: 'POST_REQ_PW_DATA_FULFILLED',
	POST_REQ_PW_DATA_REJECTED: 'POST_REQ_PW_DATA_REJECTED',

	POST_REQ_RES_DATA: 'POST_REQ_RES_DATA',
	POST_REQ_RES_DATA_FULFILLED: 'POST_REQ_RES_DATA_FULFILLED',
	POST_REQ_RES_DATA_REJECTED: 'POST_REQ_RES_DATA_REJECTED',

	POST_CREATE_PASSWORD_DATA: 'POST_CREATE_PASSWORD_DATA',
	POST_CREATE_PASSWORD_DATA_FULFILLED: 'POST_CREATE_PASSWORD_DATA_FULFILLED',
	POST_CREATE_PASSWORD_DATA_REJECTED: 'POST_CREATE_PASSWORD_DATA_REJECTED',

	POST_RESET_PASSWORD_DATA: 'POST_RESET_PASSWORD_DATA',
	POST_RESET_PASSWORD_DATA_FULFILLED: 'POST_RESET_PASSWORD_DATA_FULFILLED',
	POST_RESET_PASSWORD_DATA_REJECTED: 'POST_RESET_PASSWORD_DATA_REJECTED',

	GET_ALL_TAGS: 'GET_ALL_TAGS',
	GET_ALL_TAGS_FULFILLED: 'GET_ALL_TAGS_FULFILLED',
	GET_ALL_TAGS_REJECTED: 'GET_ALL_TAGS_REJECTED',

	POWERBI_URL: 'POWERBI_URL',
	POWERBI_URL_FULFILLED: 'POWERBI_URL_FULFILLED',
	POWERBI_URL_REJECTED: 'POWERBI_URL_REJECTED',

	ASSET_CONTAINER: 'ASSET_CONTAINER',
	ASSET_CONTAINER_FULFILLED: 'ASSET_CONTAINER_FULFILLED',
	ASSET_CONTAINER_REJECTED: 'ASSET_CONTAINER_REJECTED',

	ASSET_PREVIEW_CONTAINER: 'ASSET_PREVIEW_CONTAINER',
	ASSET_PREVIEW_CONTAINER_FULFILLED: 'ASSET_PREVIEW_CONTAINER_FULFILLED',
	ASSET_PREVIEW_CONTAINER_REJECTED: 'ASSET_PREVIEW_CONTAINER_REJECTED',

	GET_APPROVAL_TEMPLATES: 'GET_APPROVAL_TEMPLATES',
	GET_APPROVAL_TEMPLATES_FULFILLED: 'GET_APPROVAL_TEMPLATES_FULFILLED',
	GET_APPROVAL_TEMPLATES_REJECTED: 'GET_APPROVAL_TEMPLATES_REJECTED',

	UPDATE_APPROVAL_TEMPLATE: 'UPDATE_APPROVAL_TEMPLATE',
	UPDATE_APPROVAL_TEMPLATE_FULFILLED: 'UPDATE_APPROVAL_TEMPLATE_FULFILLED',
	UPDATE_APPROVAL_TEMPLATE_REJECTED: 'UPDATE_APPROVAL_TEMPLATE_REJECTED',

	CREATE_APPROVAL_TEMPLATE: 'CREATE_APPROVAL_TEMPLATE',
	CREATE_APPROVAL_TEMPLATE_FULFILLED: 'CREATE_APPROVAL_TEMPLATE_FULFILLED',
	CREATE_APPROVAL_TEMPLATE_REJECTED: 'CREATE_APPROVAL_TEMPLATE_REJECTED',

	DELETE_APPROVAL_TEMPLATE: 'DELETE_APPROVAL_TEMPLATE',
	DELETE_APPROVAL_TEMPLATE_FULFILLED: 'DELETE_APPROVAL_TEMPLATE_FULFILLED',
	DELETE_APPROVAL_TEMPLATE_REJECTED: 'DELETE_APPROVAL_TEMPLATE_REJECTED',

	ADD_CART: 'ADD_CART',
	ADD_CART_SUCCESS: 'ADD_CART_SUCESS',
	ADD_CART_FAILED: 'ADD_CART_FAILED',

	GET_CART: 'GET_CART',
	GET_CART_SUCESS: 'GET_CART_SUCESS',
	GET_CART_FAILED: 'ADD_CART_FAILED',

	DELETE_CART: 'DELETE_CART',
	DELETE_CART_SUCESS: 'DELETE_CART_SUCESS',
	DELETE_CART_FAILED: 'DELETE_CART_FAILED',
	GET_USERS_OOO: 'GET_USERS_OOO',
	GET_USERS_OOO_FULFILLED: 'GET_USERS_OOO_FULFILLED',
	GET_USERS_OOO_REJECTED: 'GET_USERS_OOO_REJECTED',

	ADD_USERS_OOO: 'ADD_USERS_OOO',
	ADD_USERS_OOO_FULFILLED: 'ADD_USERS_OOO_FULFILLED',
	ADD_USERS_OOO_REJECTED: 'ADD_USERS_OOO_REJECTED',

	EDIT_USERS_OOO: 'EDIT_USERS_OOO',
	EDIT_USERS_OOO_FULFILLED: 'EDIT_USERS_OOO_FULFILLED',
	EDIT_USERS_OOO_REJECTED: 'EDIT_USERS_OOO_REJECTED',

	GET_ASSETS_FOR_APPROVAL_OOO: 'GET_ASSETS_FOR_APPROVAL_OOO',
	GET_ASSETS_FOR_APPROVAL_OOO_FULFILLED: 'GET_ASSETS_FOR_APPROVAL_OOO_FULFILLED',
	GET_ASSETS_FOR_APPROVAL_OOO_REJECTED: 'GET_ASSETS_FOR_APPROVAL_OOO_REJECTED',

	DELEGATE_ASSETS_FOR_APPROVAL_OOO: 'DELEGATE_ASSETS_FOR_APPROVAL_OOO',
	DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED: 'DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED',
	DELEGATE_ASSETS_FOR_APPROVAL_OOO_REJECTED: 'DELEGATE_ASSETS_FOR_APPROVAL_OOO_REJECTED',

	SAVE_DEFAULT_WATERMARK: 'SAVE_DEFAULT_WATERMARK',
	SAVE_DEFAULT_WATERMARK_FULFILLED: 'SAVE_DEFAULT_WATERMARK_FULFILLED',
	SAVE_DEFAULT_WATERMARK_REJECTED: 'SAVE_DEFAULT_WATERMARK_REJECTED',

	GET_DEFAULT_WATERMARK: 'GET_DEFAULT_WATERMARK',
	GET_DEFAULT_WATERMARK_FULFILLED: 'GET_DEFAULT_WATERMARK_FULFILLED',
	GET_DEFAULT_WATERMARK_REJECTED: 'GET_DEFAULT_WATERMARK_REJECTED',

	ADD_TO_COLLECTION_FULFILLED: 'ADD_TO_COLLECTION_FULFILLED',
	ADD_TO_COLLECTION_REJECTED: 'ADD_TO_COLLECTION_REJECTED',

	EXPORT_TO_FOLDER_FULFILLED: 'EXPORT_TO_FOLDER_FULFILLED',
	EXPORT_TO_FOLDER_REJECTED: 'EXPORT_TO_FOLDER_REJECTED',

	BULK_UPDATE_TAGS_FULFILLED: 'BULK_UPDATE_TAGS_FULFILLED',
	BULK_UPDATE_TAGS_REJECTED: 'BULK_UPDATE_TAGS_REJECTED',

	GET_STYLES: 'GET_STYLES',
	GET_STYLES_FULFILLED: 'GET_STYLES_FULFILLED',
	GET_STYLES_REJECTED: 'GET_STYLES_REJECTED',

	UPDATE_STYLES_FULFILLED: 'UPDATE_STYLES_FULFILLED',
	UPDATE_STYLES_REJECTED: 'UPDATE_STYLES_REJECTED',

	GET_LOGOS_FULFILLED: 'GET_LOGOS_FULFILLED',
	GET_LOGOS_REJECTED: 'GET_LOGOS_REJECTED',

	UPDATE_LOGOS_FULFILLED: 'UPDATE_LOGOS_FULFILLED',
	UPDATE_LOGOS_REJECTED: 'UPDATE_LOGOS_REJECTED',

	SAVE_PROJECT: 'SAVE_PROJECT',
	SAVE_PROJECT_SUCCESS: 'SAVE_PROJECT_SUCCESS',
	SAVE_PROJECT_FAILED: 'SAVE_PROJECT_FAILED',

	SAVE_PROJECT_COMMENT: 'SAVE_PROJECT_COMMENT',
	SAVE_PROJECT_COMMENT_SUCCESS: 'SAVE_PROJECT_COMMENT_SUCCESS',
	SAVE_PROJECT_COMMENT_FAILED: 'SAVE_PROJECT_COMMENT_FAILED',

	DELETE_PROJECT_COMMENT: 'DELETE_PROJECT_COMMENT',
	DELETE_PROJECT_COMMENT_SUCCESS: 'DELETE_PROJECT_COMMENT_SUCCESS',
	DELETE_PROJECT_COMMENT_FAILED: 'DELETE_PROJECT_COMMENT_FAILED',

	IMPORT_ASSET_TO_PROJECT: 'IMPORT_ASSET_TO_PROJECT',
	IMPORT_ASSET_TO_PROJECT_SUCCESS: 'IMPORT_ASSET_TO_PROJECT_SUCCESS',
	IMPORT_ASSET_TO_PROJECT_FAILED: 'IMPORT_ASSET_TO_PROJECT_FAILED',

	GET_IMPORT_ASSET_TO_PROJECT: 'GET_IMPORT_ASSET_TO_PROJECT',
	GET_IMPORT_ASSET_TO_PROJECT_SUCCESS: 'GET_IMPORT_ASSET_TO_PROJECT_SUCCESS',
	GET_IMPORT_ASSET_TO_PROJECT_FAILED: 'GET_IMPORT_ASSET_TO_PROJECT_FAILED',

	REMOVE_ASSET_FROM_PROJECT: 'REMOVE_ASSET_FROM_PROJECT',
	REMOVE_ASSET_FROM_PROJECT_SUCCESS: 'REMOVE_ASSET_FROM_PROJECT_SUCCESS',
	REMOVE_ASSET_FROM_PROJECT_FAILED: 'REMOVE_ASSET_FROM_PROJECT_FAILED',

	DELETE_PROJECT: 'DELETE_PROJECT',
	DELETE_PROJECT_SUCCESS: 'DELETE_PROJECT_SUCCESS',
	DELETE_PROJECT_FAILED: 'DELETE_PROJECT_FAILED',

	ARCHIVE_PROJECT: 'ARCHIVE_PROJECT',
	ARCHIVE_PROJECT_SUCCESS: 'ARCHIVE_PROJECT_SUCCESS',
	ARCHIVE_PROJECT_FAILED: 'ARCHIVE_PROJECT_FAILED',

	UNARCHIVE_PROJECT: 'UNARCHIVE_PROJECT',
	UNARCHIVE_PROJECT_SUCCESS: 'UNARCHIVE_PROJECT_SUCCESS',
	UNARCHIVE_PROJECT_FAILED: 'UNARCHIVE_PROJECT_FAILED',
	CONFIRM_EMAIL_FULFILLED: 'CONFIRM_EMAIL_FULFILLED',
	CONFIRM_EMAIL_REJECTED: 'CONFIRM_EMAIL_REJECTED',

	GET_EMAIL_TEMPLATES_REJECTED: 'GET_EMAIL_TEMPLATES_REJECTED',

	SAVE_EMAIL_TEMPLATE: 'SAVE_EMAIL_TEMPLATE',
	SAVE_EMAIL_TEMPLATE_FULFILLED: 'SAVE_EMAIL_TEMPLATE_FULFILLED',
	SAVE_EMAIL_TEMPLATE_REJECTED: 'SAVE_EMAIL_TEMPLATE_REJECTED'
};

const MESSAGE_KEY = 'MESSAGE_KEY';
const SuccesTemplate = 'Successfully ';
const ErrorTemplate = 'Error ';

export const globalMessage = (responseMessage, dataValue) => {
	switch (responseMessage) {
		case TYPE.REVERT_VERSION_DATA_FULFILLED:
			return [TYPE.REVERT_VERSION_DATA_FULFILLED];
		case TYPE.REVERT_VERSION_DATA_REJECTED:
			return [TYPE.REVERT_VERSION_DATA_REJECTED];
		case TYPE.UPLOAD_ASSET_FULFILLED:
			return [TYPE.UPLOAD_ASSET_FULFILLED, { dataValue: dataValue }]; //dataValue
		case TYPE.UPLOAD_ASSET_REJECTED:
			return [TYPE.UPLOAD_ASSET_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.UPLOAD_ASSET_VERSION_FULFILLED:
			return [TYPE.UPLOAD_ASSET_VERSION_FULFILLED, { dataValue: dataValue }]; //dataValue
		case TYPE.UPLOAD_ASSET_VERSION_REJECTED:
			return [TYPE.UPLOAD_ASSET_VERSION_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.UPDATE_ASSET_FULFILLED:
			return [`${TYPE.UPDATE_ASSET_FULFILLED}${dataValue == 'rename' ? '.RENAMED' : '.UPDATED'}`];
		case TYPE.UPDATE_ASSET_REJECTED:
			return [`${TYPE.UPDATE_ASSET_REJECTED}${dataValue == 'rename' ? '.RENAMING' : '.UPDATING'}`];
		case TYPE.MOVE_ASSETS_FULFILLED:
			return [TYPE.MOVE_ASSETS_FULFILLED, { dataValue: dataValue }]; //dataValue
		case TYPE.MOVE_ASSETS_REJECTED:
			return [TYPE.MOVE_ASSETS_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.ARCHIVE_ASSETS_FULFILLED:
			return [TYPE.ARCHIVE_ASSETS_FULFILLED];
		case TYPE.ARCHIVE_ASSETS_REJECTED:
			return [TYPE.ARCHIVE_ASSETS_REJECTED];
		case TYPE.ADD_FOLDER_FULFILLED:
			return [TYPE.ADD_FOLDER_FULFILLED];
		case TYPE.ADD_FOLDER_REJECTED:
			return [TYPE.ADD_FOLDER_REJECTED];
		case TYPE.UPDATE_FOLDER_FULFILLED:
			return [TYPE.UPDATE_FOLDER_FULFILLED];
		case TYPE.UPDATE_FOLDER_REJECTED:
			return [TYPE.UPDATE_FOLDER_REJECTED];
		case TYPE.DELETE_FOLDER_FULFILLED:
			return [TYPE.DELETE_FOLDER_FULFILLED];
		case TYPE.DELETE_FOLDER_REJECTED:
			return [TYPE.DELETE_FOLDER_REJECTED];
		case TYPE.MOVE_FOLDER_FULFILLED:
			return [TYPE.MOVE_FOLDER_FULFILLED, { dataValue: dataValue }]; //dataValue
		case TYPE.MOVE_FOLDER_REJECTED:
			return [TYPE.MOVE_FOLDER_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.COPY_FOLDER_FULFILLED:
			return [TYPE.COPY_FOLDER_FULFILLED, { dataValue: dataValue }];
		case TYPE.COPY_FOLDER_REJECTED:
			return [TYPE.COPY_FOLDER_REJECTED, { dataValue: dataValue }];
		case TYPE.ADD_USERS_FULFILLED:
			return [TYPE.ADD_USERS_FULFILLED];
		case TYPE.ADD_USERS_REJECTED:
			return [TYPE.ADD_USERS_REJECTED];
		case TYPE.UPDATE_USERS_FULFILLED:
			return [TYPE.UPDATE_USERS_FULFILLED];
		case TYPE.UPDATE_USERS_REJECTED:
			return [TYPE.UPDATE_USERS_REJECTED];
		case TYPE.UPDATE_USERS_ADMIN_FULFILLED:
			return [TYPE.UPDATE_USERS_ADMIN_FULFILLED];
		case TYPE.UPDATE_USERS_ADMIN_REJECTED:
			return [TYPE.UPDATE_USERS_ADMIN_REJECTED];
		case TYPE.UPDATE_IMAGE_USERS_FULFILLED:
			return [TYPE.UPDATE_IMAGE_USERS_FULFILLED];
		case TYPE.UPDATE_IMAGE_USERS_REJECTED:
			return [TYPE.UPDATE_IMAGE_USERS_REJECTED];
		case TYPE.SHARE_ASSET_FULFILLED:
			return [TYPE.SHARE_ASSET_FULFILLED];
		case TYPE.SHARE_ASSET_REJECTED:
			return [TYPE.SHARE_ASSET_REJECTED];
		case TYPE.DOWNLOAD_ASSET_FULFILLED:
			return [TYPE.DOWNLOAD_ASSET_FULFILLED, { dataValue: dataValue }]; //dataValue
		case TYPE.DOWNLOAD_ASSET_REJECTED:
			return [TYPE.DOWNLOAD_ASSET_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.ADD_APPROVALS_FULFILLED:
			return [TYPE.ADD_APPROVALS_FULFILLED];
		case TYPE.ADD_APPROVALS_REJECTED:
			return [TYPE.ADD_APPROVALS_REJECTED];
		case TYPE.ADD_APPROVALLEVELS_FULFILLED:
			return [TYPE.ADD_APPROVALLEVELS_FULFILLED];
		case TYPE.ADD_APPROVALLEVELS_REJECTED:
			return [TYPE.ADD_APPROVALLEVELS_REJECTED];
		case TYPE.UPDATE_APPROVALLEVELS_FULFILLED:
			return [TYPE.UPDATE_APPROVALLEVELS_FULFILLED];
		case TYPE.UPDATE_APPROVALLEVELS_REJECTED:
			return [TYPE.UPDATE_APPROVALLEVELS_REJECTED];
		case TYPE.ADD_PARTNERS_FULFILLED:
			return [TYPE.ADD_PARTNERS_FULFILLED];
		case TYPE.ADD_PARTNERS_REJECTED:
			return [TYPE.ADD_PARTNERS_REJECTED];
		case TYPE.UPDATE_PARTNERS_FULFILLED:
			return [TYPE.UPDATE_PARTNERS_FULFILLED];
		case TYPE.UPDATE_PARTNERS_REJECTED:
			return [TYPE.UPDATE_PARTNERS_REJECTED];
		case TYPE.INVITE_NEW_USER_FULFILLED:
			return [TYPE.INVITE_NEW_USER_FULFILLED];
		case TYPE.INVITE_NEW_USER_REJECTED:
			return [TYPE.INVITE_NEW_USER_REJECTED, { dataValue: dataValue }];
		case TYPE.UPDATE_APPROVALS_FULFILLED:
			return [`${TYPE.UPDATE_APPROVALS_FULFILLED}${dataValue ? '.APPROVED' : '.REJECTED'}`];
		case TYPE.UPDATE_APPROVALS_REJECTED:
			return [`${TYPE.UPDATE_APPROVALS_REJECTED}${dataValue ? '.APPROVING' : '.REJECTING'}`];
		case TYPE.UPDATE_ASSET_STATUS_FULFILLED:
			return [
				`${TYPE.UPDATE_ASSET_STATUS_FULFILLED}${
					dataValue.includes('R') ? '.RESTORED' : dataValue.includes('D') && '.DELETED'
				}`
			];
		case TYPE.UPDATE_ASSET_STATUS_REJECTED:
			return [
				`${TYPE.UPDATE_ASSET_STATUS_REJECTED}${
					dataValue.includes('R') ? '.RESTORING' : dataValue.includes('D') && '.DELETING'
				}`
			];
		case TYPE.ADD_TO_DYNAMICS_FULFILLED:
			return [TYPE.ADD_TO_DYNAMICS_FULFILLED];
		case TYPE.ADD_TO_DYNAMICS_REJECTED:
			return [TYPE.ADD_TO_DYNAMICS_REJECTED];
		case TYPE.POST_LOGIN_DATA_FULFILLED:
			return [TYPE.POST_LOGIN_DATA_FULFILLED];
		case TYPE.POST_LOGIN_DATA_REJECTED:
			return [TYPE.POST_LOGIN_DATA_REJECTED, { dataValue: dataValue }]; //dataValue
		case TYPE.POST_CREATE_PASSWORD_DATA_FULFILLED:
			return [TYPE.POST_CREATE_PASSWORD_DATA_FULFILLED];
		case TYPE.POST_RESET_PASSWORD_DATA_REJECTED:
			if (dataValue === '') {
				return [TYPE.POST_RESET_PASSWORD_DATA_REJECTED];
			} else {
				console.log(dataValue);
				dataValue += dataValue.includes('token') ? ' Please resend the resetpassword email.' : '';
				return [dataValue];
			}
		case TYPE.CHANGE_PASSWORD_FULFILLED:
			return [TYPE.CHANGE_PASSWORD_FULFILLED];
		case TYPE.CHANGE_PASSWORD_REJECTED:
			return [TYPE.CHANGE_PASSWORD_REJECTED, { dataValue: dataValue }];
		case TYPE.POST_REQ_PW_DATA_FULFILLED:
			return [TYPE.POST_REQ_PW_DATA_FULFILLED];
		case TYPE.POST_REQ_PW_DATA_REJECTED:
			return [TYPE.POST_REQ_PW_DATA_REJECTED];
		case TYPE.POST_REQ_RES_DATA_FULFILLED:
			return [TYPE.POST_REQ_RES_DATA_FULFILLED];
		case TYPE.POST_REQ_RES_DATA_REJECTED:
			return [TYPE.POST_REQ_RES_DATA_REJECTED, { dataValue: dataValue }];
		case TYPE.POST_RESET_PASSWORD_DATA_FULFILLED:
			return [TYPE.POST_RESET_PASSWORD_DATA_FULFILLED];
		case TYPE.SHARE_BULK_ASSETS_FULFILLED:
			return [TYPE.SHARE_BULK_ASSETS_FULFILLED];
		case TYPE.SHARE_BULK_ASSETS_REJECTED:
			return [TYPE.SHARE_BULK_ASSETS_REJECTED];
		case TYPE.SHARE_FOLDER_TO_USER_FULFILLED:
			return [TYPE.SHARE_FOLDER_TO_USER_FULFILLED];
		case TYPE.SHARE_FOLDER_TO_USER_REJECTED:
			return [TYPE.SHARE_FOLDER_TO_USER_REJECTED];

		case TYPE.GET_PIN_ASSETS_FULFILLED:
			return [TYPE.GET_PIN_ASSETS_FULFILLED];
		case TYPE.GET_PIN_ASSETS_REJECTED:
			return [TYPE.GET_PIN_ASSETS_REJECTED];
		case TYPE.ADD_PIN_ASSETS_FULFILLED:
			return [TYPE.ADD_PIN_ASSETS_FULFILLED];
		case TYPE.ADD_PIN_ASSETS_REJECTED:
			return [TYPE.ADD_PIN_ASSETS_REJECTED];
		case TYPE.REMOVE_PIN_ASSETS_FULFILLED:
			return [TYPE.REMOVE_PIN_ASSETS_FULFILLED];
		case TYPE.REMOVE_PIN_ASSETS_REJECTED:
			return [TYPE.REMOVE_PIN_ASSETS_REJECTED];
		case TYPE.GET_PIN_ASSETS_DETAIL_FULFILLED:
			return [TYPE.GET_PIN_ASSETS_DETAIL_FULFILLED];
		case TYPE.GET_PIN_ASSETS_DETAIL_REJECTED:
			return [TYPE.GET_PIN_ASSETS_DETAIL_REJECTED];
		case TYPE.REPLACE_PIN_ASSETS_FULFILLED:
			return [TYPE.REPLACE_PIN_ASSETS_FULFILLED];
		case TYPE.REPLACE_PIN_ASSETS_REJECTED:
			return [TYPE.REPLACE_PIN_ASSETS_REJECTED];
		case TYPE.ORDER_PIN_ASSETS_FULFILLED:
			return [TYPE.ORDER_PIN_ASSETS_FULFILLED];
		case TYPE.ORDER_PIN_ASSETS_REJECTED:
			return [TYPE.ORDER_PIN_ASSETS_REJECTED];

		case TYPE.GET_PIN_FOLDERS_FULFILLED:
			return [TYPE.GET_PIN_FOLDERS_FULFILLED];
		case TYPE.GET_PIN_FOLDERS_REJECTED:
			return [TYPE.GET_PIN_FOLDERS_REJECTED];
		case TYPE.ADD_PIN_FOLDERS_FULFILLED:
			return [TYPE.ADD_PIN_FOLDERS_FULFILLED];
		case TYPE.ADD_PIN_FOLDERS_REJECTED:
			return [TYPE.ADD_PIN_FOLDERS_REJECTED];
		case TYPE.REMOVE_PIN_FOLDERS_FULFILLED:
			return [TYPE.REMOVE_PIN_FOLDERS_FULFILLED];
		case TYPE.REMOVE_PIN_FOLDERS_REJECTED:
			return [TYPE.REMOVE_PIN_FOLDERS_REJECTED];
		case TYPE.GET_PIN_FOLDERS_DETAIL_FULFILLED:
			return [TYPE.GET_PIN_FOLDERS_DETAIL_FULFILLED];
		case TYPE.GET_PIN_FOLDERS_DETAIL_REJECTED:
			return [TYPE.GET_PIN_FOLDERS_DETAIL_REJECTED];
		case TYPE.REPLACE_PIN_FOLDERS_FULFILLED:
			return [TYPE.REPLACE_PIN_FOLDERS_FULFILLED];
		case TYPE.REPLACE_PIN_FOLDERS_REJECTED:
			return [TYPE.REPLACE_PIN_FOLDERS_REJECTED];
		case TYPE.ORDER_PIN_FOLDERS_FULFILLED:
			return [TYPE.ORDER_PIN_FOLDERS_FULFILLED];
		case TYPE.ORDER_PIN_FOLDERS_REJECTED:
			return [TYPE.ORDER_PIN_FOLDERS_REJECTED];

		case TYPE.GET_FILE_DATA_BY_ID_FULFILLED:
			return [TYPE.GET_FILE_DATA_BY_ID_FULFILLED];
		case TYPE.GET_FILE_DATA_BY_ID_REJECTED:
			return [TYPE.GET_FILE_DATA_BY_ID_REJECTED];
		case TYPE.BULK_MOVE_FOLDER_FULFILLED:
			return [TYPE.BULK_MOVE_FOLDER_FULFILLED];
		case TYPE.BULK_MOVE_FOLDER_REJECTED:
			return [TYPE.BULK_MOVE_FOLDER_REJECTED];
		case TYPE.ORDER_FOLDER_FULFILLED:
			return [TYPE.ORDER_FOLDER_FULFILLED];
		case TYPE.ORDER_FOLDER_REJECTED:
			return [TYPE.ORDER_FOLDER_REJECTED];
		case TYPE.UPDATE_APPROVAL_TEMPLATE_FULFILLED:
			return [TYPE.UPDATE_APPROVAL_TEMPLATE_FULFILLED];
		case TYPE.UPDATE_APPROVAL_TEMPLATE_REJECTED:
			return [TYPE.UPDATE_APPROVAL_TEMPLATE_REJECTED];
		case TYPE.CREATE_APPROVAL_TEMPLATE_FULFILLED:
			return [TYPE.CREATE_APPROVAL_TEMPLATE_FULFILLED];
		case TYPE.CREATE_APPROVAL_TEMPLATE_REJECTED:
			return [TYPE.CREATE_APPROVAL_TEMPLATE_REJECTED];
		case TYPE.DELETE_APPROVAL_TEMPLATE_FULFILLED:
			return [TYPE.DELETE_APPROVAL_TEMPLATE_FULFILLED];
		case TYPE.DELETE_APPROVAL_TEMPLATE_REJECTED:
			return [TYPE.DELETE_APPROVAL_TEMPLATE_REJECTED];
		case TYPE.GET_USERS_OOO_FULFILLED:
			return [TYPE.GET_USERS_OOO_FULFILLED];
		case TYPE.GET_USERS_OOO_REJECTED:
			return [TYPE.GET_USERS_OOO_REJECTED];
		case TYPE.ADD_USERS_OOO_FULFILLED:
			return [TYPE.ADD_USERS_OOO_FULFILLED];
		case TYPE.ADD_USERS_OOO_REJECTED:
			return [TYPE.ADD_USERS_OOO_REJECTED, { dataValue: dataValue }];
		case TYPE.EDIT_USERS_OOO_FULFILLED:
			return [TYPE.EDIT_USERS_OOO_FULFILLED];
		case TYPE.EDIT_USERS_OOO_REJECTED:
			return [TYPE.EDIT_USERS_OOO_REJECTED, { dataValue: dataValue }];
		case TYPE.GET_ASSETS_FOR_APPROVAL_OOO_FULFILLED:
			return [TYPE.GET_ASSETS_FOR_APPROVAL_OOO_FULFILLED];
		case TYPE.GET_ASSETS_FOR_APPROVAL_OOO_REJECTED:
			return [TYPE.GET_ASSETS_FOR_APPROVAL_OOO_REJECTED, { dataValue: dataValue }];
		case TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED:
			return [TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_FULFILLED];
		case TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_REJECTED:
			return [TYPE.DELEGATE_ASSETS_FOR_APPROVAL_OOO_REJECTED, { dataValue: dataValue }];
		case TYPE.SAVE_DEFAULT_WATERMARK_FULFILLED:
			return [TYPE.SAVE_DEFAULT_WATERMARK_FULFILLED];
		case TYPE.SAVE_DEFAULT_WATERMARK_REJECTED:
			return [TYPE.SAVE_DEFAULT_WATERMARK_REJECTED];
		case TYPE.ADD_TO_COLLECTION_FULFILLED:
			return [TYPE.ADD_TO_COLLECTION_FULFILLED];
		case TYPE.EXPORT_TO_FOLDER_FULFILLED:
			return [TYPE.EXPORT_TO_FOLDER_FULFILLED];
		case TYPE.EXPORT_TO_FOLDER_REJECTED:
			return [TYPE.EXPORT_TO_FOLDER_REJECTED];
		case TYPE.BULK_UPDATE_TAGS_FULFILLED:
			return [TYPE.BULK_UPDATE_TAGS_FULFILLED];
		case TYPE.BULK_UPDATE_TAGS_REJECTED:
			return [TYPE.BULK_UPDATE_TAGS_REJECTED];
		case TYPE.GET_STYLES_FULFILLED:
			return [TYPE.GET_STYLES_FULFILLED];
		case TYPE.GET_STYLES_REJECTED:
			return [TYPE.GET_STYLES_REJECTED];
		case TYPE.UPDATE_STYLES_FULFILLED:
			return [TYPE.UPDATE_STYLES_FULFILLED];
		case TYPE.UPDATE_STYLES_REJECTED:
			return [TYPE.UPDATE_STYLES_REJECTED];
		case TYPE.GET_LOGOS_FULFILLED:
			return [TYPE.GET_LOGOS_FULFILLED];
		case TYPE.GET_LOGOS_REJECTED:
			return [TYPE.GET_LOGOS_REJECTED];
		case TYPE.UPDATE_LOGOS_FULFILLED:
			return [TYPE.UPDATE_LOGOS_FULFILLED];
		case TYPE.UPDATE_LOGOS_REJECTED:
			return [TYPE.UPDATE_LOGOS_REJECTED];
		case TYPE.SAVE_PROJECT_FAILED:
			return [TYPE.SAVE_PROJECT_FAILED];
		case TYPE.SAVE_PROJECT_SUCCESS:
			return [TYPE.SAVE_PROJECT_SUCCESS];
		case TYPE.SAVE_PROJECT_COMMENT_SUCCESS:
			return [TYPE.SAVE_PROJECT_COMMENT_SUCCESS];
		case TYPE.SAVE_PROJECT_COMMENT_FAILED:
			return [TYPE.SAVE_PROJECT_COMMENT_FAILED];
		case TYPE.DELETE_PROJECT_COMMENT_SUCCESS:
			return [TYPE.DELETE_PROJECT_COMMENT_SUCCESS];
		case TYPE.DELETE_PROJECT_COMMENT_FAILED:
			return [TYPE.DELETE_PROJECT_COMMENT_FAILED];
		case TYPE.IMPORT_ASSET_TO_PROJECT_SUCCESS:
			return [TYPE.IMPORT_ASSET_TO_PROJECT_SUCCESS];
		case TYPE.IMPORT_ASSET_TO_PROJECT_FAILED:
			return [TYPE.IMPORT_ASSET_TO_PROJECT_FAILED];
		case TYPE.GET_IMPORT_ASSET_TO_PROJECT_SUCCESS:
			return [TYPE.GET_IMPORT_ASSET_TO_PROJECT_SUCCESS];
		case TYPE.GET_IMPORT_ASSET_TO_PROJECT_FAILED:
			return [TYPE.GET_IMPORT_ASSET_TO_PROJECT_FAILED];
		case TYPE.REMOVE_ASSET_FROM_PROJECT_SUCCESS:
			return [TYPE.REMOVE_ASSET_FROM_PROJECT_SUCCESS];
		case TYPE.REMOVE_ASSET_FROM_PROJECT_FAILED:
			return [TYPE.REMOVE_ASSET_FROM_PROJECT_FAILED];
		case TYPE.DELETE_PROJECT_SUCCESS:
			return [TYPE.DELETE_PROJECT_SUCCESS];
		case TYPE.DELETE_PROJECT_FAILED:
			return [TYPE.DELETE_PROJECT_FAILED];
		case TYPE.ARCHIVE_PROJECT_SUCCESS:
			return [TYPE.ARCHIVE_PROJECT_SUCCESS];
		case TYPE.ARCHIVE_PROJECT_FAILED:
			return [TYPE.ARCHIVE_PROJECT_FAILED];
		case TYPE.UNARCHIVE_PROJECT_SUCCESS:
			return [TYPE.UNARCHIVE_PROJECT_SUCCESS];
		case TYPE.UNARCHIVE_PROJECT_FAILED:
			return [TYPE.UNARCHIVE_PROJECT_FAILED];
		case TYPE.SAVE_TEAM_FULFILLED:
			return [TYPE.SAVE_TEAM_FULFILLED];
		case TYPE.SAVE_TEAM_REJECTED:
			return [TYPE.SAVE_TEAM_REJECTED];
		case TYPE.DELETE_TEAMS_FULFILLED:
			return [TYPE.DELETE_TEAMS_FULFILLED];
		case TYPE.DELETE_TEAMS_REJECTED:
			return [TYPE.DELETE_TEAMS_REJECTED];

		case TYPE.CONFIRM_EMAIL_FULFILLED:
			return [TYPE.CONFIRM_EMAIL_FULFILLED];
		case TYPE.ADD_ASSETTHUMBNAIL_FULFILLED:
			return [TYPE.ADD_ASSETTHUMBNAIL_FULFILLED];
		case TYPE.ADD_ASSETTHUMBNAIL_REJECTED:
			return [TYPE.ADD_ASSETTHUMBNAIL_REJECTED];
		case TYPE.REMOVE_ASSETTHUMBNAIL_FULFILLED:
			return [TYPE.REMOVE_ASSETTHUMBNAIL_FULFILLED];
		case TYPE.REMOVE_ASSETTHUMBNAIL_REJECTED:
			return [TYPE.REMOVE_ASSETTHUMBNAIL_REJECTED];
		case TYPE.ADD_ASSETPACKAGE_FULFILLED:
			return [TYPE.ADD_ASSETPACKAGE_FULFILLED];
		case TYPE.ADD_ASSETPACKAGE_REJECTED:
			return [TYPE.ADD_ASSETPACKAGE_REJECTED];
		case TYPE.SAVE_EMAIL_TEMPLATE_FULFILLED:
			return [TYPE.SAVE_EMAIL_TEMPLATE_FULFILLED];
		case TYPE.SAVE_EMAIL_TEMPLATE_REJECTED:
			return [TYPE.SAVE_EMAIL_TEMPLATE_REJECTED];
		// case TYPE._FULFILLED:
		// 	return `${SuccesTemplate}`;
		// case TYPE._REJECTED:
		// 	return `${ErrorTemplate}`;
		default:
			return ['POPUP.DEFAULT', { dataValue: dataValue == '' ? responseMessage : dataValue }];
	}
};

const TextSwitcher = (dataValue) => {
	if (dataValue.includes(TYPE._FULFILLED)) {
		return TYPE._FULFILLED;
	} else if (dataValue.includes(TYPE._REJECTED)) {
		return TYPE._REJECTED;
	} else if (dataValue.includes(TYPE._LOADING)) {
		return TYPE._LOADING;
	} else {
		return;
	}
};

function MessageComp(props) {
	const { t } = useTranslation();
	const data = globalMessage(props.responseMessage, props.dataValue);
	return t(`POPUP.${data[0]}`, data[1]);
}

export const FeedbackMessage = (responseMessage, dataValue = '') => {
	switch (TextSwitcher(responseMessage)) {
		case TYPE._FULFILLED: {
			message.success({
				content: <MessageComp responseMessage={responseMessage} dataValue={dataValue} />,
				key: MESSAGE_KEY
			});
			break;
		}
		case TYPE._REJECTED: {
			message.error({
				content: <MessageComp responseMessage={responseMessage} dataValue={dataValue} />,
				key: MESSAGE_KEY
			});
			break;
		}
		case TYPE._LOADING: {
			message.loading({
				content: <MessageComp responseMessage={responseMessage} dataValue={dataValue} />,
				// content: globalMessage(responseMessage, dataValue == '' ? 'Loading...' : dataValue),
				key: MESSAGE_KEY
			});
			break;
		}
		default: {
			message.info({
				content: <MessageComp responseMessage={responseMessage} dataValue={dataValue} />,
				key: MESSAGE_KEY
			});
			break;
		}
	}
};
