import React, { useState, useEffect, useCallback, useRef, memo, useContext } from 'react';
import { connect } from 'react-redux';
import moment from 'moment';
import CRC32 from 'crc-32';
import { v4 as uuidv4 } from 'uuid';

import ToolsMenu from './toolsMenu';
import Folders from './folders';
import CollectionList from './collectionlist';
import AssetsHeader from './assetsHeader';
import NoFiles from './nofiles';
import AssetThumbnail from './assetThumbnail';
import AddFolderModal from './modals/addModal';
import EditModal from './modals/editModal';
import MoveAssetModal from './modals/moveModal';
import MoveFolderModal from './modals/moveFolderModal';
import ArchiveModal from './modals/archiveModal';
import DropFolderModal from './modals/dropFolderModal';
import WopiModal from './modals/wopiModal';
import RenameModal from './modals/renameModal';
import EditMetaDetails from './modals/editMetaDetails';
import ShareModal from './modals/shareModal';
import ShareFolderModal from './modals/shareFolderModal';
import ApprovalModal from './modals/approvalModal';
import CartModal from './modals/cartModal';
import CopyFolderModal from './modals/copyFolderModal';
import BulkTagUploadModal from './modals/bulktaguploadModal';
import UploadThumbnailModal from './modals/uploadThumbnailModal';
import UploadPackageModal from './modals/uploadPackageModal';

import { getAppMode, getUser, getUserRole } from '@damtoken';
import { Layout, Row, Col, Card, Select, List, Form, Tooltip, Upload, Spin, Modal, Tabs, message } from 'antd';
import { useTranslation } from 'react-i18next';

import { IoMdImages } from 'react-icons/io';
import { ExclamationCircleOutlined, FolderViewOutlined, ShoppingCartOutlined } from '@ant-design/icons';

import useModal from '../../shared/useModal';
import useTree from '../../../hooks/useTree';

import { DefaultFilterState, USER_ROLES } from '../../constants';
import {
	NestedChild,
	Compare,
	getSelectedCountriesRegions,
	Uint8ToBase64,
	getSortResult,
	getFilterResult,
	getSearchedResult
} from '@damhelper';

import {
	getFolders,
	getFiles,
	getAccounts,
	getCountries,
	uploadAsset,
	uploadAssetVersion,
	updateAsset,
	moveAssets,
	archiveAssets,
	addFolder,
	updateFolder,
	deleteFolder,
	moveFolder,
	getWopiParams,
	getUsers,
	addUser,
	shareAsset,
	downloadAsset,
	bulkDownloadAsset,
	uploadToDynamics,
	uploadVideoForIndexing,
	authenticateVideoForIndexing,
	getIndexingState,
	getFeatureFlag,
	getUserPartner,
	getUserFolder,
	getFilesVersion,
	revertAssetVersion,
	getCompanies,
	getAssetById,
	getPinAssets,
	checkDuplicateAsset,
	getDefaultWatermark,
	getCarts,
	addCollection,
	downloadPDFAsImage,
	downloadOfficeAsPDF,
	copyFolder,
	uploadChunk,
	uploadCompleted,
	GetAssetContainer,
	GetAssetPreviewContainer,
	removeThumbnail
} from '../actions';
import { LowFrequencyContext } from '@damcontext';

function Dam(props) {
	const { t } = useTranslation();
	const { confirm } = Modal;

	const {
		filterState,
		setFilterState,
		isDynamicsAddFromDam,
		allCountries,
		allAccounts,
		checkDuplicateFlag,
		setAssetContainer,
		setAssetPreviewContainer
	} = useContext(LowFrequencyContext);

	const { TabPane } = Tabs;
	const [selectedViewTab, setSelectedViewTab] = useState('1');
	const [form] = Form.useForm();
	const [treeState, treeDispatch] = useTree();
	const [modalState, modalDispatch] = useModal();
	const [touchMoveFolder, setTouchMoveFolder] = useState(null);
	const [isUploading, setIsUploading] = useState(false);
	const [isUpdating, setIsUpdating] = useState(false);
	const [mFolders, setMFolders] = useState([]);
	const [touchFolder, setTouchFolder] = useState(null);
	const [touchFile, setTouchFile] = useState(null);
	const [checkFile, setCheckFile] = useState([]);
	const [isDropzoneShown, setDropzoneShown] = useState(false);
	const [isEditing, setIsEditing] = useState(false);
	const [regionOptions, setRegionOptions] = useState([]);
	const [findFileState, setFindFileState] = useState({});
	const [modalType, setModalType] = useState('');
	const imageElem = useRef();
	const [shareModalState, setShareModalState] = useState(false);
	const [shareFolderModalState, setShareFolderModalState] = useState(false);

	// placeholder for folder state
	const [selectedFolderTitle, setSelectedFolderTitle] = useState('');
	const [selectedFolderAccounts, setSelectedFolderAccounts] = useState([]);
	const [selectedFolderCountries, setSelectedFolderCountries] = useState([]);
	const [selectedFolderRegions, setSelectedFolderRegions] = useState([]);
	const [selectedFolderComments, setSelectedFolderComments] = useState('');
	const [selectedFolderParentId, setSelectedFolderParentId] = useState(0);
	const [selectedFolderIds, setSelectedFolderIds] = useState([]);

	// placeholder for file state
	const [selectedFileName, setSelectedFileName] = useState('');
	const [selectedFileAccounts, setSelectedFileAccounts] = useState([]);
	const [selectedFileCountries, setSelectedFileCountries] = useState([]);
	const [selectedFileRegions, setSelectedFileRegions] = useState([]);
	const [selectedFileComments, setSelectedFileComments] = useState('');
	const [selectedFileFolders, setSelectedFileFolders] = useState([]);
	const [selectedFileTags, setSelectedFileTags] = useState([]);
	const [selectedFileExpiryDate, setSelectedFileExpiryDate] = useState(null);
	const [selectedFileKey, setSelectedFileKey] = useState('');
	const [selectedAssetKey, setSelectedAssetKey] = useState('');
	const [selectedAssetID, setSelectedAssetID] = useState('');
	const [selectedFileIsReadOnly, setSelectedFileIsReadOnly] = useState(false);
	const [selectedFileCurrentVersion, setSelectedFileCurrentVersion] = useState('');
	const [selectedShareFolders, setSelectedShareFolders] = useState([]);
	const [selectedShareAssets, setSelectedShareAssets] = useState([]);
	const [pinAssets, setPinAssets] = useState([]);

	// logged in user id
	const [currentUserId, setCurrentUserId] = useState('');

	// get role access
	const userRole = getUserRole();

	const [uploadUsingReplace, setUploadUsingReplace] = useState(null);

	const [checkedAssets, setCheckedAssets] = useState([]);
	const [checkedAssetsItem, setCheckedAssetsItem] = useState([]);
	const [selectedAssetsId, setSelectedAssetsId] = useState(null);
	const assetsSlider = useRef();
	const [assetsSliderControls, setAssetSliderControls] = useState([false, false]);
	const [assetsSliderPrev, assetsSliderNext] = assetsSliderControls;

	const [videoPreview, setVideoPreview] = useState(null);
	const [videoType, setVideoType] = useState(null);
	const [fileInfo, setFileInfo] = useState(null);

	// For image viewer
	const [imagePreview, setImagePreview] = useState(null);
	const [imagePreviewType, setImagePreviewType] = useState(null);
	// For PDF Viewer
	const [numPages, setNumPages] = useState(null);
	// For Tags
	// For Doc Viewer
	const [docViewerDoc, setDocViewerDoc] = useState([]);
	//Grid preference
	const [gridState, setGridState] = useState(getAppMode().CardGridPreference);

	const [assetOpened, setAssetOpened] = useState([]);

	const [selectAll, setSelectAll] = useState(false);
	const [isApprovalFromModal, setIsApprovalFromModal] = useState(false);
	const [keySearchUsed, dataSearched] =
		filterState.Search && filterState.Search.length > 1 ? filterState.Search : DefaultFilterState.Search;
	// Asset Status enum
	const assetStatus = Object.freeze({
		Draft: 0,
		Archived: 1,
		Deleted: 2,
		Approved: 3,
		Rejected: 4
	});

	// Approval Flag and current user
	const [currentUser, setCurrentUser] = useState(null);
	const [approvalFlag, setApprovalFlag] = useState(false);
	const [videoIndexerFlag, setVideoIndexerFlag] = useState(false);
	const [userFolder, setUserFolder] = useState(null);
	const [isAssetUrl, setIsAssetUrl] = useState(false);
	const [croppedImage, setCroppedImage] = useState(null);
	const [uploadingFileCount, setUploadingFileCount] = useState(0);

	// Watermark flagging
	const [hasWatermark, setHasWatermark] = useState(false);

	//upload big file
	const chunkSize = 1048576 * 5; //its 3MB, increase the number measure in mb
	const [showProgress, setShowProgress] = useState(false);
	const [counter, setCounter] = useState(1);
	const [fileToBeUpload, setFileToBeUpload] = useState({});
	const [beginingOfTheChunk, setBeginingOfTheChunk] = useState(0);
	const [endOfTheChunk, setEndOfTheChunk] = useState(chunkSize);
	const [progress, setProgress] = useState(0);
	const [fileGuid, setFileGuid] = useState('');
	const [fileSize, setFileSize] = useState(0);
	const [chunkCount, setChunkCount] = useState(0);

	const customTreeTitle = (folderName, assetCount) => {
		return (
			<Tooltip color="#000" placement="right" title={folderName + ' (' + assetCount + ')'}>
				<span>{folderName}</span>
			</Tooltip>
		);
	};

	// Selected Collections
	const [selectedCollection, setSelectedCollection] = useState(null);
	const [userCollections, setUserCollections] = useState([]);

	// Upload Bulk Tagging
	const [uploadedFiles, setUploadedFiles] = useState([]);
	const [isBulkTagOpen, setIsBulkUploadTagOpen] = useState(false);

	const efolders =
		props.folderData &&
		props.folderData.map((folder) => {
			return {
				key: folder.id,
				id: folder.id,
				title: customTreeTitle(folder.folderName, folder.assetCount),
				parentFolderId: folder.parentFolderId,
				accounts: folder.accounts,
				countries: folder.countries,
				regions: folder.regions,
				comments: folder.comments,
				folderName: folder.folderName,
				company: folder.company,
				orderNumber: folder.orderNumber
			};
		});

	// edit files data
	let efiles =
		props.fileData &&
		props.fileData.map((file) => {
			return {
				key: JSON.stringify(file.folderId),
				id: file.id,
				name: file.name,
				fileName: file.fileName,
				extension: file.extension,
				folderId: file.folderId,
				type: 'file',
				accounts: file.accounts,
				countries: file.countries,
				regions: file.regions,
				thumbnail: file.thumbnail,
				downloadUrl: file.downloadUrl,
				comments: file.comments,
				originalUrl: file.originalUrl,
				fileType: file.fileType,
				tags: file.tags,
				fileSizeText: file.fileSizeText,
				createdById: file.createdById,
				createdByName: file.createdByName,
				createdDate: file.createdDate,
				expiryDate: file.expiryDate,
				downloadCount: file.downloadCount,
				copyUrl: file.copyUrl,
				status: file.status,
				statusName: file.statusName,
				assetVersions: file.assetVersions,
				statusUpdatedDate: file.statusUpdatedDate,
				assetKey: file.key,
				shareFolderIds: file.shareFolderIds,
				projectId: file.projectId,
				preview: file.thumbnailPreview
			};
		});

	// combine edited folders and files data
	const combinedData = [...efolders];
	const findFile = efiles && efiles.find((file) => file.id === selectedAssetsId);

	//upload big file
	useEffect(() => {
		console.log('change');
		console.log(fileToBeUpload);
		console.log(fileSize);
		if (fileSize > 0) {
			fileUpload();
		}
		if (progress === 100) {
			setIsUploading(false);
			onLoad(props);
			setProgress(0);
			setFileSize(0);
			setCounter(1);
			setBeginingOfTheChunk(0);
			setEndOfTheChunk(chunkSize);
			setFileToBeUpload({});
		}
	}, [fileToBeUpload, progress]);

	const getFileContext = (e) => {
		console.log(e);
		const _file = e.file;
		setFileSize(_file.size);
		console.log(chunkSize);
		const _totalCount = _file.size % chunkSize == 0 ? _file.size / chunkSize : Math.floor(_file.size / chunkSize) + 1; // Total count of chunks will have been upload to finish the file
		console.log(_totalCount);
		setChunkCount(_totalCount);
		const _fileID = uuidv4() + '.' + _file.name.split('.').pop();
		console.log(_fileID);
		setFileGuid(_fileID);
		setFileToBeUpload(_file);
	};

	const fileUpload = async () => {
		console.log('fileUpload');
		console.log(counter);
		setCounter(counter + 1);
		setShowProgress(true);
		if (counter <= chunkCount) {
			var chunk = fileToBeUpload.slice(beginingOfTheChunk, endOfTheChunk);
			console.log(chunk);
			console.log(endOfTheChunk);
			console.log(chunkSize);
			console.log(counter);
			console.log(chunkCount);
			console.log(fileGuid);
			await props.uploadChunk(
				chunk,
				setBeginingOfTheChunk,
				setEndOfTheChunk,
				endOfTheChunk,
				chunkSize,
				counter,
				chunkCount,
				fileGuid,
				setProgress,
				fileToBeUpload.name,
				fileToBeUpload.type ? fileToBeUpload.type : '',
				touchFolder,
				setUploadedFiles
			);
		}
	};

	useEffect(() => {
		props.getAssetContainer().then((result) => {
			setAssetContainer(result);
		});

		props.getAssetPreviewContainer().then((result) => {
			setAssetPreviewContainer(result);
		});

		async function getFeatureFlagsAndUser() {
			let featureObj = await props.getFeatureFlag();
			let userObj = null;
			let folderObj = null;
			let companyObj = null;
			let user = await getUser();
			let rootfolder = 0;
			if (user) {
				userObj = await props.loadUsers(user.id);
			}
			if (userObj) {
				folderObj = await props.getUserFolder(user.id);
				companyObj = await props.getCompanies();
				const currentUser = userObj.data.users;
				const myCompanyId = currentUser.companyId;
				const myCompany = companyObj ? companyObj.data.companies.find((c) => c.id === myCompanyId) : null;
				if (currentUser.userRole.Name !== USER_ROLES.DAM_ADMIN && myCompanyId) {
					rootfolder = myCompany.rootFolderId;
				}
				currentUser.rootFolderId = rootfolder;
				currentUser.company = myCompany;
				setTouchFolder(rootfolder);
				setCurrentUserId(user.id);
				setCurrentUser(userObj && userObj.data.users);
				setUserFolder(folderObj && folderObj.data.userFolders);
			}

			featureObj.featureFlags.map((flag) => {
				if (flag.featureFlagName === 'Approvals') {
					setApprovalFlag(flag.isActivated);
				}
				if (flag.featureFlagName === 'VideoIndexer') {
					setVideoIndexerFlag(flag.isActivated);
				}
			});
		}
		async function loadFoldersAndFiles() {
			props.loadFolders();
			props.loadFiles();
		}
		async function loadPinAssets() {
			var pinAssets = await props.getPinAssets();
			setPinAssets(pinAssets);
		}
		async function checkIfHasWatermark() {
			var result = await props.getDefaultWatermark();
			if (result && result.data.watermark) {
				var wm = result.data.watermark;
				if (wm.id) {
					setHasWatermark(true);
				}
			}
		}
		getFeatureFlagsAndUser();
		loadFoldersAndFiles();
		loadPinAssets();
		checkIfHasWatermark();
		loadCollections();
	}, []);

	async function loadCollections() {
		var result = await props.getCarts();

		if (result) {
			setUserCollections(result);

			if (selectedCollection && result.filter((collection) => collection.id == selectedCollection.id)[0]) {
				setSelectedCollection(result.filter((collection) => collection.id == selectedCollection.id)[0]);
			}
		}
	}

	useEffect(() => {
		if (modalState.type === 'rename' && findFile.name) {
			form.setFieldsValue({
				name: selectedFileName
			});
		}
	}, [modalState.type, findFile, form]);

	useEffect(() => {
		if (props.match.params.id && props.fileData && efiles.length > 0) {
			setIsAssetUrl(true);

			let filteredFile = efiles.filter((x) => x.id === parseInt(props.match.params.id));
			if (filteredFile.length > 0) {
				var file = filteredFile[0];
				setSelectedAssetsId(file.id);
				modal().editDetails(file);
			}
		}
		if (modalState.type === 'edit-details') {
			let filteredFile = efiles.filter((x) => x.id === parseInt(selectedAssetsId));
			if (filteredFile.length > 0) {
				var file = filteredFile[0];
				setIsUploading(false);
				modal().editDetails(file);
			}
		}
	}, [props.fileData]);

	useEffect(() => {
		if (modalState.type === 'edit' && touchFolder) {
		}
		if (modalState.type === 'add' && touchFolder) {
			form.setFieldsValue({
				folder_name: selectedFolderTitle,
				accounts: selectedFolderAccounts.map((row) => row.id),
				country: selectedFolderCountries.map((row) => row.id),
				region: selectedFolderRegions.map((row) => row.id),
				comments: selectedFolderComments
			});
		}
	}, [modalState.type, form, touchFolder, efolders]);

	useEffect(() => {
		if (modalState.isVisible === false) {
			setCheckedAssets([]);
			setTouchFile(null);
		}
	}, [modalState]);

	useEffect(() => {
		if (props.folderData) {
			if (props.folderData.length > 0) {
				if (!touchFolder) {
					selectTreeKey([props.folderData[0].id]);
				}
			}
		}
	}, [props.folderData]);

	useEffect(() => {
		if (props.fileRefresh) {
			props.loadFiles(true);
		}
	}, [props.fileRefresh]);

	useEffect(() => {
		if (uploadedFiles && uploadedFiles.length > 0 && !isBulkTagOpen) {
			// Launch bulk tagging modal
			if (uploadedFiles.length === uploadingFileCount) {
				toggleBulkTagModal();
				setTouchFolder(touchFolder);
				onLoad(false, true);
				setIsUploading(false);
				setUploadingFileCount(0);
			}
		}
	}, [uploadedFiles]);

	function onCheckedAssets(item) {
		let tempIds = checkedAssets;
		let tempItems = checkedAssetsItem;
		const isChecked = tempIds.indexOf(item.id);

		if (isChecked < 0) {
			tempIds.push(item.id);
			tempItems.push(item);
			setCheckedAssets(tempIds);
			setCheckedAssetsItem(tempItems);

			if (tempIds.length === dataSource().length) {
				setSelectAll(true);
			} else {
				setSelectAll(false);
			}

			return;
		}

		tempIds = tempIds.filter((fId) => fId !== item.id);
		tempItems = tempItems.filter((fId) => fId.id !== item.id);

		setSelectedFileKey(item.key);
		setCheckedAssets(tempIds);
		setCheckedAssetsItem(tempItems);
		//setCopyUrl(item.copyUrl);

		if (tempIds.length === dataSource().length) {
			setSelectAll(true);
		} else {
			setSelectAll(false);
		}
	}

	function onCurrentAssetsId(e, id) {
		e.preventDefault();
		setCroppedImage(null);

		var newId = 0;
		setSelectedAssetsId((prevId) => {
			if (prevId === id) {
				setCheckedAssets((prevId) => prevId.reverse());
			}
			newId = prevId === id ? checkedAssets.find((fId) => fId !== id) : id;
			return prevId === id ? checkedAssets.find((fId) => fId !== id) : id;
		});

		if (newId === 0) {
			newId = id;
		}
		const selectedFile = efiles && efiles.find((row) => row.id === newId);

		if (selectedFile.fileType && selectedFile.fileType.includes('video')) {
			setVideoPreview(selectedFile.originalUrl);
			setVideoType(selectedFile.fileType);
			setModalType('video');
		} else if (selectedFile.fileType && selectedFile.fileType.includes('audio')) {
			setImagePreview(selectedFile.originalUrl);
			setModalType('audio');
		} else {
			setImagePreview(selectedFile.originalUrl);
			setImagePreviewType(selectedFile.extension);
			setModalType('image');
		}

		setSelectedFileName(selectedFile.name);
		setSelectedFileComments(selectedFile.comments);
		setSelectedFileTags(selectedFile.tags);
		setSelectedFileExpiryDate(selectedFile.expiryDate ? moment(selectedFile.expiryDate) : null);
		setSelectedFileKey(selectedFile.key);

		//get current version
		var currentVersion = selectedFile.assetVersions.filter((x) => x.activeVersion === 1)[0];
		setSelectedFileCurrentVersion(currentVersion.id);

		if (
			(selectedFile.createdById == currentUserId || userRole.canEdit) &&
			(!approvalFlag || selectedFile.statusName !== 'Submitted For Review')
		) {
			setSelectedFileIsReadOnly(false);
		} else {
			setSelectedFileIsReadOnly(true);
		}

		setFindFileState(selectedFile);

		let docViewerDocList = [...docViewerDoc];
		docViewerDocList = [];
		docViewerDocList.push({
			uri: selectedFile.originalUrl
		});
		setDocViewerDoc(docViewerDocList);

		var fileFolders = selectedFileFolders;
		fileFolders = efolders.filter((f) => f.id == selectedFile.folderId);
		setSelectedFileFolders(fileFolders);

		const regionOptions = getSelectedCountriesRegions(allCountries, selectedFile.countries);
		setRegionOptions(regionOptions);
		//setCopyUrl(selectedFile.copyUrl);
	}

	async function onSubmit(values) {
		if (modalState.type === 'edit') {
		} else if (modalState.type === 'add') {
			handleAddFolder().then(() => {
				onLoad(true);
				modalDispatch({});
				setIsEditing(false);
				form.resetFields();
			});
		} else if (modalState.type === 'drop-folder') {
			handleDeleteFolder().then(() => {
				onLoad(true);
				setTouchFolder(null);
				setIsEditing(false);
				modalDispatch({});
			});
		} else {
			if (modalState.type === 'move') {
				const folderName = touchMoveFolder !== null && efolders.find((row) => row.id === touchMoveFolder).folderName;

				handleMoveAssets(folderName).then(() => {
					modalDispatch({});
					onLoad(false, true);
					setTouchFolder(touchFolder);
					setTouchFile(null);
					setTouchMoveFolder(null);
					setCheckFile([]);
					setCheckedAssets([]);
					setIsEditing(false);
				});
			} else if (modalState.type === 'move-folder') {
				const folderName = touchMoveFolder !== null && efolders.find((row) => row.id === touchMoveFolder).folderName;

				handleMoveFolder(folderName).then(() => {
					onLoad(true);
					setTouchFolder(null);
					setIsEditing(false);
					modalDispatch({});
				});
			} else if (modalState.type === 'copy-folder') {
				const folderName = touchMoveFolder !== null && efolders.find((row) => row.id === touchMoveFolder).folderName;

				handleCopyFolder(folderName).then(() => {
					onLoad(true);
					setTouchFolder(null);
					setIsUpdating(false);
					modalDispatch({});
				});
			} else if (modalState.type === 'archive') {
				handleArchiveAssets().then(() => {
					modalDispatch({});
					onLoad(false, true);
					setTouchFolder(touchFolder);
					setTouchFile(null);
					setTouchMoveFolder(null);
					setCheckFile([]);
					setAssetOpened([]);
					setCheckedAssets([]);
					setIsEditing(false);
				});
			} else if (modalState.type === 'rename') {
				handleUpdateAssets('rename').then(() => {
					setIsEditing(false);
					onLoad(false, true);
				});
			}
			// else if (modalState.type === 'edit-meta-data' || modalState.type === 'edit-details') {
			// 	if (selectedAssetVersionKey != selectedFileCurrentVersion) {
			// 		handleRevertAssetVersion(selectedAssetVersionKey);
			// 	}
			// 	handleUpdateAssets('update').then(() => {
			// 		setIsEditing(false);
			// 		setSelectedAssetVersionKey('');
			// 		onLoad(props);
			// 	});
			// }
		}
	}

	async function onLoad(folderRefresh = false, fileRefresh = false) {
		props.loadFolders(folderRefresh);
		props.loadFiles(fileRefresh);
		// props.loadAccounts();
		// props.loadCountries();
	}

	// tree actions
	function expandThreeKeys(expandedKeys) {
		treeDispatch({ type: 'EXPANDED_KEYS', payload: expandedKeys });
		treeDispatch({ type: 'AUTO_EXPAND_PARENT', payload: false });
	}

	function checkKey(checkedKeys) {
		treeDispatch({ type: 'CHECKED_KEYS', payload: checkedKeys });
	}

	function selectTreeKey(selectedKeys, info) {
		let folderId = 0;
		if (info) {
			folderId = info.node.id;
		} else {
			if (selectedKeys.length > 0) {
				folderId = selectedKeys[0];
			}
		}

		treeDispatch({ type: 'SELECTED_KEYS', payload: selectedKeys });
		setTouchFolder(folderId);
		setTouchFile(null);
		setCheckFile([]);

		const selectedFolder = efolders && efolders.find((row) => row.id === folderId);

		if (selectedFolder) {
			setSelectedFolderIds(folderId);
			setSelectedFolderTitle(selectedFolder.folderName);
			setSelectedFolderAccounts(selectedFolder.accounts);
			setSelectedFolderCountries(selectedFolder.countries);
			setSelectedFolderRegions(selectedFolder.regions);
			setSelectedFolderComments(selectedFolder.comments);
			setSelectedFolderParentId(selectedFolder.parentFolderId ? selectedFolder.parentFolderId : 0);
		}
		setCheckedAssetsItem([]);
		setSelectedShareAssets([]);
		setCheckedAssets([]);
		setSelectAll(false);
	}

	function selectOptionsType(type = []) {
		return type.map((row) => {
			return (
				<Select.Option value={row.id} key={row.id}>
					{row.name ? row.name : row.description ? row.description : row.title}
				</Select.Option>
			);
		});
	}

	function selectGroupOptionsType(type = []) {
		var flags = [],
			countries = [],
			l = type.length,
			i;
		for (i = 0; i < l; i++) {
			if (flags[type[i].country]) continue;
			flags[type[i].country] = true;
			countries.push(type[i].country);
		}

		return countries.map((country) => {
			return (
				<Select.OptGroup key={country.key} label={country}>
					{selectRegion(type, country)}
				</Select.OptGroup>
			);
		});
	}

	function selectRegion(type = [], country) {
		return type.map((region) => {
			if (country === region.country) {
				return (
					<Select.Option value={region.id} key={region.id}>
						{region.name ? region.name : region.description ? region.description : region.title}
					</Select.Option>
				);
			}
		});
	}

	function selectOptionsVersionType(type = []) {
		return type.map((row) => {
			return (
				<Select.Option value={row.id} key={row.id}>
					{row.fileName ? row.fileName : row.fileName ? row.fileName : row.fileName}
				</Select.Option>
			);
		});
	}

	function toggleBulkTagModal() {
		setIsBulkUploadTagOpen(!isBulkTagOpen);
	}

	function handleBulkUploadModalClose() {
		toggleBulkTagModal();
		setUploadedFiles([]);
	}

	function modal() {
		return {
			header() {
				switch (modalState.type) {
					case 'add':
						return t('ModalDetail.Add Folder'); //'Add Folder'
					case 'edit':
						return t('ModalDetail.Edit Folder'); //'Edit Folder'
					case 'archive':
						return t('ModalDetail.Archive Message'); //'Archive Message'
					case 'move':
					case 'move-folder':
						return t('ModalDetail.Move to'); //'Move to...'
					case 'copy-folder':
						return t('ModalDetail.Copy to'); //'Copy to...'
					case 'approval':
						return t('ModalDetail.Send For Approval', { current: 1, total: 1 }); //'Send For Approval: 1 of 1 Asset'
					case 'rename':
						return t('ModalDetail.Rename file'); //'Rename file(s)'
					case 'edit-meta-data':
					case 'edit-details':
						return t('ModalDetail.Asset Preview'); //'Asset Preview'
					case 'drop-folder':
						return t('Delete Folder.Title');
					default:
						return '';
				}
			},
			submitText() {
				switch (modalState.type) {
					case 'add':
					case 'rename':
						return t('Button.Save'); //'Save'
					case 'edit':
						return t('Button.Update'); //'Update'
					case 'archive':
						return t('Button.Archive'); //'Archive'
					case 'drop-folder':
						return t('Button.Delete'); //'Delete Folder'
					case 'approval':
						return t('Button.Submit'); //'Submit'
					case 'move':
					case 'move-folder':
						return t('Button.Move'); //'Move'
					case 'copy-folder':
						return t('Button.Copy');
					case 'edit-meta-data':
					case 'edit-details':
						return t('Button.Update'); //'Update'
					default:
						return '';
				}
			},
			submitDisabled() {
				var isDisabled =
					modalState.type === 'move' ||
					modalState.type === 'move-folder' ||
					modalState.type === 'copy-folder' ||
					modalState.type === 'video' ||
					modalState.type === 'wopi'
						? touchMoveFolder == null
							? true
							: false
						: false;

				return isDisabled;
			},
			isReadOnly() {
				var isReadOnly = false;

				if (modalState.type === 'rename' || modalState.type === 'edit-metadata' || modalState.type === 'edit-details') {
					isReadOnly = selectedFileIsReadOnly;
				}
				return isReadOnly;
			},
			closeModal() {
				modalDispatch({});
				form.resetFields(); // in editeModal and editeMetaDetailModal there is no relaible form so there is a console error about useForm, TODO fix this
				setFindFileState({});
				setIsEditing(false);
				if (modalState.type === 'edit' || modalState.type === 'add') {
					// reset folder details state
					const selectedFolder = efolders && efolders.find((row) => row.id === touchFolder);

					if (selectedFolder) {
						setSelectedFolderTitle(selectedFolder.folderName);
						setSelectedFolderAccounts(selectedFolder.accounts);
						setSelectedFolderCountries(selectedFolder.countries);
						setSelectedFolderRegions(selectedFolder.regions);
						setSelectedFolderComments(selectedFolder.comments);
					}
				}
				setAssetOpened([]);
				setCheckedAssets([]);
				setCheckedAssetsItem([]);
				setSelectedShareAssets([]);
				setSelectAll(false);
				setCroppedImage(null);
				setNumPages(null);

				if (isAssetUrl) {
					window.location.href = '/';
				}
			},
			edit() {
				modalDispatch({
					type: 'TYPE',
					payload: 'edit-only'
				});
			},
			addFolder() {
				setSelectedFolderTitle('');
				setSelectedFolderAccounts([]);
				setSelectedFolderCountries([]);
				setSelectedFolderRegions([]);
				setSelectedFolderComments('');
				modalDispatch({
					type: 'BOTH',
					payload: 'add'
				});
			},
			approval(checkedAsset, isFromModal = false) {
				setFindFileState(checkedAsset);
				setIsApprovalFromModal(isFromModal);

				modalDispatch({
					type: 'BOTH',
					payload: 'approval'
				});
			},
			move() {
				modalDispatch({
					type: 'BOTH',
					payload: 'move'
				});
			},
			archive() {
				modalDispatch({
					type: 'BOTH',
					payload: 'archive'
				});
			},
			editDetails(item) {
				let selectedFile;
				if (checkedAssets.length > 0) {
					setSelectedAssetsId(checkedAssets[0]);
					selectedFile = efiles && efiles.find((row) => row.id === checkedAssets[0]);
				} else {
					if (item) {
						selectedFile = item;

						let assetOpened = [];
						assetOpened.push(item.id);
						setAssetOpened(assetOpened);
					}
				}

				if (selectedFile) {
					if (selectedFile.fileType && selectedFile.fileType.includes('video')) {
						setVideoPreview(selectedFile.originalUrl);
						setVideoType(selectedFile.fileType);
						setModalType('video');
					} else if (selectedFile.fileType && selectedFile.fileType.includes('audio')) {
						setImagePreview(selectedFile.originalUrl);
						setModalType('audio');
					} else {
						setImagePreview(selectedFile.originalUrl);
						setImagePreviewType(selectedFile.extension);
						setModalType('image');
					}

					setSelectedFileName(selectedFile.name);
					setSelectedFileComments(selectedFile.comments);
					setSelectedFileTags(selectedFile.tags);
					setSelectedFileExpiryDate(selectedFile.expiryDate ? moment(selectedFile.expiryDate) : null);
					setSelectedFileKey(selectedFile.key);

					var currentVersion = selectedFile.assetVersions.filter((x) => x.activeVersion === 1)[0];
					setSelectedFileCurrentVersion(currentVersion.id);
					if (
						(selectedFile.createdById == currentUserId || userRole.canEdit) &&
						(!approvalFlag || selectedFile.statusName !== 'Submitted For Review')
					) {
						setSelectedFileIsReadOnly(false);
					} else {
						setSelectedFileIsReadOnly(true);
					}

					setFindFileState(selectedFile);

					let docViewerDocList = [...docViewerDoc];
					docViewerDocList = [];
					docViewerDocList.push({
						uri: selectedFile.originalUrl
					});
					setDocViewerDoc(docViewerDocList);

					var fileFolders = selectedFileFolders;
					fileFolders = efolders.filter((f) => f.id == selectedFile.folderId);
					setSelectedFileFolders(fileFolders);
					modalDispatch({
						type: 'BOTH',
						payload: 'edit-details'
					});
				}
			},
			moveFolder() {
				const folders =
					props.folderData &&
					props.folderData.filter((folder) => {
						return folder.id !== touchFolder;
					});
				const mfolders =
					folders &&
					folders.map((folder) => {
						return {
							key: folder.id,
							id: folder.id,
							title: folder.folderName,
							parentFolderId: folder.parentFolderId,
							accounts: folder.accounts,
							countries: folder.countries,
							regions: folder.regions,
							comments: folder.comments,
							folderName: folder.folderName
						};
					});
				setMFolders(mfolders);
				modalDispatch({
					type: 'BOTH',
					payload: 'move-folder'
				});
			},
			copyFolder() {
				const folders =
					props.folderData &&
					props.folderData.filter((folder) => {
						return folder.id !== touchFolder;
					});
				const mfolders =
					folders &&
					folders.map((folder) => {
						return {
							key: folder.id,
							id: folder.id,
							title: folder.folderName,
							parentFolderId: folder.parentFolderId,
							accounts: folder.accounts,
							countries: folder.countries,
							regions: folder.regions,
							comments: folder.comments,
							folderName: folder.folderName
						};
					});
				setMFolders(mfolders);
				modalDispatch({
					type: 'BOTH',
					payload: 'copy-folder'
				});
			},
			dropFolder() {
				modalDispatch({
					type: 'BOTH',
					payload: 'drop-folder'
				});
			},
			rename() {
				setSelectedFileName(efiles.find((f) => f.id === checkedAssets[0]).name);
				setSelectedAssetsId(checkedAssets[0]);

				if (
					(efiles.find((f) => f.id === checkedAssets[0]).createdById == currentUserId || userRole.canEdit) &&
					(!approvalFlag || efiles.find((f) => f.id === checkedAssets[0]).statusName !== 'Submitted For Review')
				) {
					setSelectedFileIsReadOnly(false);
				} else {
					setSelectedFileIsReadOnly(true);
				}

				modalDispatch({
					type: 'BOTH',
					payload: 'rename'
				});
			},
			editMetadata() {
				if (touchFolder) {
					const selectedFolder = efolders && efolders.find((row) => row.id === touchFolder);
					const regionOptions = getSelectedCountriesRegions(allCountries, selectedFolder.countries);
					setRegionOptions(regionOptions);
				}
				modalDispatch({
					type: 'BOTH',
					payload: 'edit'
				});
			},
			addNewThumbnail(assetId) {
				if (assetId > 0) {
					const selectedFile = efiles && efiles.find((row) => row.id === assetId);
					setFindFileState(selectedFile);
				}
				modalDispatch({
					type: 'BOTH',
					payload: 'newThumbnail'
				});
			},
			removeThumbnail(assetId) {
				props.removeThumbnail({ AssetId: assetId }).then((res) => {
					props.loadFiles(true);
				});
			},
			addNewPackage(assetId) {
				if (assetId > 0) {
					const selectedFile = efiles && efiles.find((row) => row.id === assetId);
					setFindFileState(selectedFile);
				}
				modalDispatch({
					type: 'BOTH',
					payload: 'newPackage'
				});
			},
			editSelectedAsset(e, fileId) {
				setSelectedAssetsId(fileId);
				setTouchFile(fileId);

				const selectedFile = efiles && efiles.find((row) => row.id === fileId);

				setSelectedFileName(selectedFile.name);
				setSelectedFileComments(selectedFile.comments);
				setSelectedFileTags(selectedFile.tags);
				setSelectedFileExpiryDate(selectedFile.expiryDate ? moment(selectedFile.expiryDate) : null);
				setSelectedFileKey(selectedFile.key);

				var currentVersion = selectedFile.assetVersions.filter((x) => x.activeVersion === 1)[0];
				setSelectedFileCurrentVersion(currentVersion.id);

				if (
					(selectedFile.createdById == currentUserId || userRole.canEdit) &&
					(!approvalFlag || selectedFile.statusName !== 'Submitted For Review')
				) {
					setSelectedFileIsReadOnly(false);
				} else {
					setSelectedFileIsReadOnly(true);
				}

				var fileFolders = selectedFileFolders;
				fileFolders = efolders.filter((f) => f.id == selectedFile.folderId);
				setSelectedFileFolders(fileFolders);
				const regionOptions = getSelectedCountriesRegions(allCountries, selectedFile.countries);
				setRegionOptions(regionOptions);

				modalDispatch({
					type: 'BOTH',
					payload: 'edit-meta-data'
				});
			},
			selectEditMetaFolder() {
				//
			},
			async download(files, showWatermark = false, extension = '') {
				if (files.length === 1) {
					let fileName = files[0].name;
					if (files[0].extension && files[0].name.indexOf(files[0].extension) < 0) {
						fileName = fileName + '.' + files[0].extension;
					}

					if (files[0].extension === 'pdf' && (extension === 'png' || extension === 'jpeg')) {
						await props.downloadPDFAsImage(
							files[0].id,
							currentUserId,
							fileName,
							files[0].extension,
							showWatermark,
							extension
						);
					} else if (
						(files[0].extension.includes('doc') ||
							files[0].extension.includes('xls') ||
							files[0].extension.includes('ppt')) &&
						extension === 'pdf'
					) {
						await props.downloadOfficeAsPDF(files[0].id, currentUserId, fileName);
					} else {
						await props.downloadAsset(
							files[0].id,
							currentUserId,
							fileName,
							files[0].extension,
							showWatermark,
							extension
						);
					}
				}
				if (!files.length) {
					let fileName = files.name;
					if (files.extension && files.name.indexOf(files.extension) < 0) {
						fileName = fileName + '.' + files.extension;
					}

					if (extension) {
						if (files.extension === 'pdf' && (extension === 'png' || extension === 'jpeg')) {
							await props.downloadPDFAsImage(
								files.id,
								currentUserId,
								fileName,
								files.extension,
								showWatermark,
								extension
							);
						} else if (
							(files.extension.includes('doc') || files.extension.includes('xls') || files.extension.includes('ppt')) &&
							extension === 'pdf'
						) {
							await props.downloadOfficeAsPDF(files.id, currentUserId, fileName);
						} else {
							await props.downloadAsset(files.id, currentUserId, fileName, files.extension, showWatermark, extension);
						}
					} else {
						await props.downloadAsset(files.id, currentUserId, fileName, files.extension, showWatermark);
					}
				}
				if (files.length > 1) {
					var ids = [];
					files.map((f) => {
						ids.push(f.id);
					});
					await props.bulkDownloadAsset(ids.toString(), currentUserId, showWatermark);
				}
			},
			selectAccount(e) {
				var selectedAccountOptions = allAccounts.filter((c) => {
					if (e.filter((x) => x == c.id).length > 0) {
						return true;
					} else {
						return false;
					}
				});

				if (modalState.type === 'edit' || modalState.type === 'add') {
					setSelectedFolderAccounts(selectedAccountOptions);
				} else {
					setSelectedFileAccounts(selectedAccountOptions);
				}
			},
			setTitle(e) {
				if (modalState.type === 'edit' || modalState.type === 'add') {
					setSelectedFolderTitle(e.target.value);
				} else {
					setSelectedFileName(e.target.value);
				}
			},
			setComments(e) {
				if (modalState.type === 'edit' || modalState.type === 'add') {
					setSelectedFolderComments(e.target.value);
				} else {
					setSelectedFileComments(e.target.value);
				}
			},
			setExpiryDate(e) {
				if (e) {
					setSelectedFileExpiryDate(e.format());
				} else {
					setSelectedFileExpiryDate(null);
				}
			},
			selectCountry(e) {
				// Change region options depending on selected countries
				let regionOptions = [];
				var selectedCountryOptions = allCountries.filter((c) => {
					if (e.filter((x) => x == c.id).length > 0) {
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
				setRegionOptions(regionOptions);

				if (modalState.type === 'edit' || modalState.type === 'add') {
					var updateFolderRegions = selectedFolderRegions.filter((t) => {
						if (regionOptions.filter((x) => x.id == t.id).length > 0) {
							return true;
						}
					});
					setSelectedFolderCountries(selectedCountryOptions);
					setSelectedFolderRegions(updateFolderRegions);
				} else {
					var updateFileRegions = selectedFileRegions.filter((t) => {
						if (regionOptions.filter((x) => x.id == t.id).length > 0) {
							return true;
						}
					});
					setSelectedFileCountries(selectedCountryOptions);
					setSelectedFileRegions(updateFileRegions);
				}
			},
			selectRegion(e) {
				var selectedRegionOptions = regionOptions.filter((c) => {
					if (e.filter((x) => x == c.id).length > 0) {
						return true;
					} else {
						return false;
					}
				});

				if (modalState.type === 'edit' || modalState.type === 'add') {
					setSelectedFolderRegions(selectedRegionOptions);
				} else {
					setSelectedFileRegions(selectedRegionOptions);
				}
			},
			renameFile(e) {
				var newTitle = e.target.value;
				setSelectedFileName(newTitle);
			},
			addFromDam() {
				if (checkedAssets.length > 0) {
					handleUploadToDynamics().then((res) => {
						if (res) {
							setCheckedAssets([]);
							setIsUploading(false);
						} else {
							setCheckedAssets([]);
							setIsUploading(false);
						}
					});
				}
			},
			promoteAsset(isMarketing, assetId) {
				handlePromoteAsset(isMarketing, assetId).then((res) => {
					setCheckedAssets([]);
					setIsUploading(false);
				});
			},
			addToCart() {
				handleAddToCollection();
			}
		};
	}

	async function handleAddToCollection() {
		if (checkedAssets.length > 0) {
			var checkResult = checkIfAssetInCart(checkedAssets);

			if (checkResult.newCheckedAssets.length > 0) {
				if (checkResult.duplicateIds.length > 0) {
					if (checkedAssets.length !== checkResult.duplicateIds.length) {
						message.info(
							`${t('Messages.Skipped')} ${checkResult.duplicateIds.length} ${t('Messages.Asset count in collection')}`
						);
					} else {
						if (checkedAssets.length === 1) {
							message.info(t('Messages.Asset Already In Collection'));
							return;
						} else {
							message.info(t('Messages.Assets Already In Collection'));
							return;
						}
					}
				}

				const data = {
					id: selectedCollection.id,
					assetIds: checkResult.newCheckedAssets,
					name: selectedCollection.name
				};

				await props.addCollection(data);

				loadCollections();
			}
		}
	}

	function checkIfAssetInCart(assetIds) {
		var idsToAdd = [];
		var duplicateIds = [];

		assetIds.forEach((item) => {
			selectedCollection.assetIds.forEach((id) => {
				if (id == item) {
					if (duplicateIds.filter((i) => i === item).length === 0) {
						duplicateIds.push(item);
					}
				}
			});
			if (idsToAdd.filter((i) => i === item).length === 0) {
				idsToAdd.push(item);
			}
		});

		/// add to idsToAdd if not selected

		selectedCollection.assetIds.forEach((id) => {
			if (idsToAdd.filter((i) => i === id).length === 0) {
				idsToAdd.push(id);
			}
		});

		return {
			newCheckedAssets: idsToAdd,
			duplicateIds: duplicateIds
		};
	}

	function renderLoadingText(uploading, loading) {
		if (uploading) {
			if (showProgress) {
				return t('Messages.Uploading') + ' - ' + progress.toFixed(0) + '%';
			}
			return t('Messages.Uploading');
		}
		if (loading) return t('Messages.Loading');
	}

	function dataSource() {
		//assets by folder
		const folderAssets =
			touchFolder === null || touchFolder === 1
				? touchFolder === 1
					? efiles.filter((file) => {
							if (
								file.folderId === touchFolder ||
								(file.shareFolderIds && file.shareFolderIds.split(',').find((f) => Number(f) === touchFolder))
							) {
								return true;
							}
					  })
					: []
				: efiles.filter((file) => {
						if (
							file.folderId === touchFolder ||
							(file.shareFolderIds && file.shareFolderIds.split(',').find((f) => Number(f) === touchFolder))
						) {
							if (
								currentUser &&
								currentUser.userRole.name === USER_ROLES.SUBSCRIBER &&
								approvalFlag &&
								file.status !== 4
							) {
								return false;
							} else if (
								currentUser &&
								currentUser.userRole.name === USER_ROLES.USER &&
								approvalFlag &&
								file.status !== 4
							) {
								return false;
							} else {
								return true;
							}
						} else {
							return false;
						}
				  });
		const source = keySearchUsed.length > 0 ? getSearchedResult(keySearchUsed, efiles) : folderAssets;
		let filteredData = [];
		filteredData = getFilterResult(filterState, source);
		const sortedData = getSortResult(filterState.SortBy ? filterState.SortBy : '', filteredData, filterState.SortOrder);
		return sortedData;
	}

	function waitingUploadFileConut(o) {
		setUploadingFileCount((prevCount) => {
			return prevCount + 1;
		});

		//let fileCount = o.fileList.length;

		//let completedCount = o.fileList.filter(x => x.status === 'done' || x.status === 'error').length;

		//if (o.file.status === 'done' || o.file.status === 'error') {
		//    if (fileCount === completedCount) {
		//        toggleBulkTagModal();
		//    }
		//}
	}

	async function handleUpload(option) {
		setIsUploading(true);
		setDropzoneShown(false);
		setSelectAll(false);
		console.log(option);
		const { file } = option;
		let base;

		let fileReader = new FileReader();
		console.log(file.size);
		console.log(file.size > 10485760);
		if (file.size > 10485760) {
			//chunk uploading when file is bigger than 10MB
			getFileContext(option);
		} else {
			fileReader.onload = async (e) => {
				base = e.target.result;

				var byteString = new Uint8Array(base);
				console.log('step1');
				// var code = CRC32.buf(base);
				// if (code < 0) {
				// 	code = 4294967296 + code;
				// }
				// var result = props.checkDuplicateAsset(code);
				// console.log(result);
				const data = {
					name: file.name,
					fileName: file.name,
					description: '',
					extension: file.name.split('.').pop(),
					fileBytes: Uint8ToBase64(byteString),
					fileType: file.type,
					folderId: touchFolder ? touchFolder : 0
				};
				console.log('step2');

				if (checkDuplicateFlag && file.size <= 10485760) {
					//checking duplicate when file is under 10MB
					var result = await props.checkDuplicateAsset(data);

					if (result) {
						var folderName = efolders.filter((f) => f.id === result.folderId);
						confirm({
							title: t('Messages.Check Duplicate Title', {
								assetName: result.name,
								folderName: folderName[0].folderName
							}),
							content: t('Messages.Check Duplicate Content'),
							icon: <ExclamationCircleOutlined />,
							async onOk() {
								var newAsset = await props.onUploadAsset(data);
								setUploadedFiles((files) => [...files, newAsset.asset]);
							},
							onCancel() {
								setUploadingFileCount((prevCount) => {
									return prevCount - 1;
								});
								setIsUploading(false);
							}
						});
					} else {
						var newAsset = await props.onUploadAsset(data);
						setUploadedFiles((files) => [...files, newAsset.asset]);
					}
				} else {
					var newAsset = await props.onUploadAsset(data);
					setUploadedFiles((files) => [...files, newAsset.asset]);
				}
			};
			fileReader.readAsArrayBuffer(file);
		}
	}

	// async function handleRevertAssetVersion(versionId) {
	// 	setIsUploading(true);
	// 	setDropzoneShown(false);
	// 	setIsUpdating(true);
	// 	const data = {
	// 		assetId: findFile ? findFile.id : findFileState.id,
	// 		versionId: versionId
	// 	};
	// 	await props.revertVersion(data).then((result) => {
	// 		setSelectedAssetsId(data.assetId);
	// 		onLoad(props);
	// 		setIsUploading(false);
	// 		setIsUpdating(false);
	// 	});
	// }

	// async function handleChangeVersion(versionId) {
	// 	const data = {
	// 		assetId: findFile ? findFile.id : findFileState.id,
	// 		versionId: versionId
	// 	};
	// 	await props.loadVersion(data).then((result) => {
	// 		setSelectedAssetsId(data.assetId);
	// 		modal().editDetails(result['asset'][0]);
	// 	});
	// }

	async function handleUploadVersion(option) {
		setDropzoneShown(false);

		const { file } = option;
		let base;

		let fileReader = new FileReader();
		fileReader.onload = async (e) => {
			base = e.target.result;

			var byteString = new Uint8Array(base);

			const data = {
				name: file.name,
				id: findFileState ? findFileState.id : findFile.id,
				fileName: file.name,
				description: '',
				extension: file.name.split('.').pop(),
				fileBytes: Uint8ToBase64(byteString),
				fileType: file.type,
				folderId: touchFolder ? touchFolder : 0,
				status: findFileState.status
			};

			setIsUpdating(true);
			await props.onUploadAssetVersion(data).then(async (result) => {
				setTouchFolder(touchFolder);
				setSelectedAssetsId(data.id);
				await props.loadFiles(true);
				setIsEditing(false);
				setIsUpdating(false);
			});
		};

		fileReader.readAsArrayBuffer(file);
	}

	async function handleAddFolder() {
		const data = {
			folderName: selectedFolderTitle,
			comments: selectedFolderComments,
			accounts: selectedFolderAccounts,
			countries: selectedFolderCountries,
			regions: selectedFolderRegions,
			parentFolderId: touchFolder ? touchFolder : 0
		};

		await props.onAddFolder(data).then(() => {
			console.log('data', data);
		});
	}

	async function handleDeleteFolder() {
		const folderData = [];
		folderData.push(selectedFolderIds);
		const folders = {
			folderIds: folderData,
			isSuccess: true
		};

		await props.onDeleteFolder(folders).then(() => {
			console.log('data', folders);
		});
	}

	function handleUpdatedFolder() {
		onLoad(true);
		setIsEditing(false);
		modalDispatch({});
	}

	async function handleMoveAssets(folderName) {
		const data = {
			assetIds: checkedAssets,
			folderId: touchMoveFolder
		};

		await props.onMoveAssets(data, folderName).then(() => {
			console.log('data', data);
		});
	}

	async function handleMoveFolder(folderName) {
		const data = {
			folderId: selectedFolderIds,
			parentFolderId: touchMoveFolder
		};

		await props.onMoveFolders(data, folderName).then(() => {
			console.log('data', data);
		});
	}

	async function handleCopyFolder(folderName) {
		const data = {
			folderId: selectedFolderIds,
			parentFolderId: touchMoveFolder
		};
		setIsUpdating(true);
		await props.onCopyFolders(data, folderName).then(() => {
			console.log('data', data);
		});
	}

	async function handleArchiveAssets() {
		const data = {
			assetIds: assetOpened.length > 0 ? assetOpened : checkedAssets,
			status: assetStatus['Archived'],
			folderId: selectedFolderIds
		};

		await props.onArchiveAssets(data).then(() => {
			console.log('data', data);
		});
	}

	async function handleUpdateAssets(option) {
		if (option === 'rename') {
			const data = {
				id: checkedAssets[0],
				name: selectedFileName,
				fileBytes: [],
				fileType: ''
			};
			await props.onUpdateAsset(data, 'rename').then(() => {
				setCroppedImage(null);
				modalDispatch({});
			});
		}
	}

	async function handleUploadToDynamics() {
		const urlParams = new URLSearchParams(document.location.search);
		const paramEntityName = urlParams.get('entity');
		const paramEntityId = urlParams.get('entityId');
		let entityName = '';
		let assetIds = [];

		if (paramEntityName === 'simple_job') {
			entityName = 'simple_jobs';
		} else if (paramEntityName === 'simple_campaign') {
			entityName = 'simple_campaigns';
		} else if (paramEntityName === 'simple_brief') {
			entityName = 'simple_briefs';
		} else if (paramEntityName === 'simple_initiative') {
			entityName = 'simple_initiatives';
		} else {
			entityName = '';
		}

		checkedAssets.forEach((assetId) => {
			assetIds.push({
				assetId: assetId,
				entity: entityName,
				entityId: paramEntityId
			});
		});

		setIsUploading(true);

		const resp = await props.onUploadToDynamics(assetIds);

		if (resp) {
			return resp;
		} else {
			return null;
		}
	}

	async function handlePromoteAsset(isMarketing, assetId) {
		let assetIds = [];

		assetIds.push({
			assetId: assetId,
			isMarketing: isMarketing,
			uploadedBy: currentUserId
		});

		setIsUploading(true);

		const resp = await props.onUploadToDynamics(assetIds);

		if (resp) {
			return resp;
		} else {
			return null;
		}
	}

	const uploaderBaseProps = {
		name: 'file',
		multiple: true,
		showUploadList: false,
		onChange: waitingUploadFileConut,
		customRequest: handleUpload
	};

	const dragUploaderProps = {
		openFileDialogOnClick: false,
		className: 'dragger',
		...uploaderBaseProps
	};

	const clickUploaderProps = {
		...uploaderBaseProps
	};

	const uploaderVersionBaseProps = {
		name: 'file',
		multiple: true,
		showUploadList: false,
		customRequest: handleUploadVersion
	};

	const clickUploaderVersionProps = {
		...uploaderVersionBaseProps
	};

	const onDropzoneToggle = () => {
		if (!isDynamicsAddFromDam) {
			if (isDropzoneShown) {
				setDropzoneShown(false);
			} else {
				setDropzoneShown(true);
			}
		}
	};

	const onDropDraggableFile = () => {
		if (!isDynamicsAddFromDam) {
			setDropzoneShown(false);
		}
	};

	const paginationSettings = {
		pageSizeOptions: [10, 24, 48, 96],
		showSizeChanger: true,
		defaultCurrent: 1,
		responsive: true,
		showQuickJumper: true,
		defaultPageSize: 48
	};
	function getGridLayout(gridSetting) {
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
			default:
				return {};
		}
	}
	//Azure Blob

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				{!isDynamicsAddFromDam && (
					<AssetsHeader
						setGridState={(mode) => {
							setGridState(mode);
						}}
						filterState={filterState}
						setFilterState={setFilterState}
						modal={modal}
						selectedCollection={selectedCollection}
					/>
				)}
				<Row className={`tabpane-layout dam ${isDynamicsAddFromDam ? 'add-from-dam' : ''}`}>
					<Col span={24}>
						<Row type="flex" className="folders-list-parent">
							<Col className="folders-container" xs={24} sm={24} md={24} lg={7} xl={7} xxl={7}>
								<Card
									title={''}
									bordered
									style={{
										marginLeft: 20,
										marginTop: 25,
										borderRadius: 8,
										marginRight: 20
									}}
								>
									{' '}
									<Tabs
										size="large"
										className="tab-panel fade-in"
										type="card"
										onChange={(e) => setSelectedViewTab(e)}
										activeKey={selectedViewTab}
									>
										<TabPane
											tab={
												<span>
													<FolderViewOutlined />
													{t('Tabs.Folders')}
												</span>
											}
											key="1"
										>
											<Folders
												modalState={modalState}
												expandThreeKeys={expandThreeKeys}
												treeState={treeState}
												checkKey={checkKey}
												selectTreeKey={selectTreeKey}
												userFolder={userFolder}
												currentUser={currentUser}
												touchFolder={touchFolder}
												setTouchFolder={setTouchFolder}
												treeDispatch={treeDispatch}
												folderId={props.match.params.folderId}
												folderData={efolders}
												efiles={efiles}
											/>
										</TabPane>
										<TabPane
											tab={
												<span>
													<ShoppingCartOutlined />
													{t('Tabs.Collections')}
												</span>
											}
											key="2"
										>
											<CollectionList
												setSelectedCollection={setSelectedCollection}
												userCollections={userCollections}
												loadCollections={loadCollections}
												efiles={efiles}
											/>
										</TabPane>
									</Tabs>
								</Card>
							</Col>

							<Col
								xs={24}
								sm={24}
								md={24}
								lg={17}
								xl={17}
								xxl={17}
								className="list-container fade-in"
								onDragEnter={onDropzoneToggle}
								onDragLeave={onDropzoneToggle}
								onDrop={onDropDraggableFile}
							>
								<Spin
									spinning={isUploading || props.fileLoading}
									size="large"
									tip={renderLoadingText(isUploading, props.fileLoading)}
								>
									<Row>
										<Card
											title={''}
											bordered
											style={{
												marginTop: 5,
												borderRadius: 8,
												width: '100%'
											}}
										>
											<ToolsMenu
												checkedAssets={checkedAssets}
												isDynamicsAddFromDam={isDynamicsAddFromDam}
												clickUploaderProps={clickUploaderProps}
												checkedAssetsItem={checkedAssetsItem}
												setCheckedAssetsItem={setCheckedAssetsItem}
												setCheckedAssets={setCheckedAssets}
												setShareModalState={setShareModalState}
												dataSource={dataSource}
												keySearchUsed={keySearchUsed}
												modalDispatch={modalDispatch}
												treeDispatch={treeDispatch}
												modal={modal}
												touchFolder={touchFolder}
												modalState={modalState}
												approvalFlag={approvalFlag}
												setSelectedFileKey={setSelectedFileKey}
												setSelectedAssetID={setSelectedAssetID}
												setSelectedAssetKey={setSelectedAssetKey}
												selectAll={selectAll}
												setSelectAll={setSelectAll}
												setSelectedShareAssets={setSelectedShareAssets}
												setShareFolderModalState={setShareFolderModalState}
												filterState={filterState}
												setFilterState={setFilterState}
												hasWatermark={hasWatermark}
												selectedCollection={selectedCollection}
											/>
										</Card>
									</Row>
									<Row type="flex" justify="space-between" align="middle">
										<Col span="auto" className="list-section">
											{keySearchUsed.length === 0 ? null : (
												<Row type="flex" justify="center" align="center" className="search-indicator">
													<Col align="center">
														{keySearchUsed.length === 0 ? null : dataSource().length}
														&nbsp;
														{dataSource().length > 1 ? 'Files Found' : 'File Found'}
													</Col>
												</Row>
											)}

											{dataSource().length ? (
												<>
													<Row
														id="dropzone-area"
														className={`${isDropzoneShown ? 'drop-upload-show' : 'drop-upload-hidden'}`}
													>
														<Upload.Dragger {...dragUploaderProps} className="drop-upload">
															<p className="ant-upload-drag-icon">
																<IoMdImages />
															</p>
															<p className="ant-upload-text">{'Drop to upload file(s)'}</p>
														</Upload.Dragger>
													</Row>

													{/*This is done like this because list - grid properties don't update when state is set as value,
													 * will look into this further*/}
													<List
														key={`Mode${gridState}`}
														pagination={paginationSettings}
														grid={getGridLayout(gridState)}
														dataSource={dataSource()}
														renderItem={(item) => (
															<AssetThumbnail
																item={item}
																mode={gridState}
																onCheckedAssets={onCheckedAssets}
																checkedAssets={checkedAssets}
																setVideoPreview={setVideoPreview}
																setVideoType={setVideoType}
																setModalType={setModalType}
																setImagePreview={setImagePreview}
																setImagePreviewType={setImagePreviewType}
																modal={modal}
																approvalFlag={approvalFlag}
																setSelectedFileCountries={setSelectedFileCountries}
																setSelectedFileRegions={setSelectedFileRegions}
																setSelectedFileAccounts={setSelectedFileAccounts}
																allCountries={allCountries}
																setRegionOptions={setRegionOptions}
																setSelectedAssetKey={setSelectedAssetKey}
																setSelectedAssetID={setSelectedAssetID}
																setCheckFile={setCheckFile}
																setTouchFile={setTouchFile}
																setSelectedShareFolders={setSelectedShareFolders}
																pinAssets={pinAssets}
																setPinAssets={setPinAssets}
															/>
														)}
													/>
												</>
											) : (
												<NoFiles
													dragUploaderProps={dragUploaderProps}
													clickUploaderProps={clickUploaderProps}
													keySearchUsed={keySearchUsed}
												/>
											)}
										</Col>
									</Row>
								</Spin>
							</Col>
						</Row>
					</Col>
				</Row>
			</Layout.Content>

			{modalState.isVisible && modalState.type === 'addToCart' && (
				<CartModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					setSelectedShareAssets={setSelectedShareAssets}
					setShareModalState={setShareModalState}
					checkedAssetsItem={checkedAssetsItem}
					setCheckedAssetsItem={setCheckedAssetsItem}
					setSelectedAssetID={setSelectedAssetID}
					checkedAssets={checkedAssets}
					setCheckedAssets={setCheckedAssets}
					efiles={efiles}
				/>
			)}

			{modalState.isVisible && modalState.type === 'add' && (
				<AddFolderModal
					form={form}
					modal={modal}
					modalState={modalState}
					canEditAccess={userRole.canEdit}
					canAddAccess={userRole.canAdd}
					canArchiveAccess={userRole.canArchive}
					isUpdating={isUpdating}
					onSubmit={onSubmit}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					selectOptionsType={selectOptionsType}
					regionOptions={regionOptions}
					compare={Compare}
					selectedFolderRegions={selectedFolderRegions}
					accounts={allAccounts}
					countries={allCountries}
					selectGroupOptionsType={selectGroupOptionsType}
				/>
			)}

			{modalState.isVisible && modalState.type === 'edit' && (
				<EditModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					onSubmit={onSubmit}
					selectOptionsType={selectOptionsType}
					touchFolder={touchFolder}
					selectedFolderParentId={selectedFolderParentId}
					handleUpdatedFolder={handleUpdatedFolder}
				/>
			)}
			{modalState.isVisible && modalState.type === 'newThumbnail' && (
				<UploadThumbnailModal
					modal={modal}
					modalState={modalState}
					findFileState={findFileState}
					refresh={() => props.loadFiles(true)}
				/>
			)}
			{modalState.isVisible && modalState.type === 'newPackage' && (
				<UploadPackageModal
					modal={modal}
					modalState={modalState}
					findFileState={findFileState}
					refresh={() => props.loadFiles(true)}
				/>
			)}
			{modalState.isVisible && modalState.type === 'move' && (
				<MoveAssetModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					setTouchMoveFolder={setTouchMoveFolder}
					combinedData={combinedData}
					nestedChild={NestedChild}
				/>
			)}

			{modalState.isVisible && modalState.type === 'move-folder' && (
				<MoveFolderModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					setTouchMoveFolder={setTouchMoveFolder}
					mFolders={mFolders}
					nestedChild={NestedChild}
				/>
			)}

			{modalState.isVisible && modalState.type === 'copy-folder' && (
				<CopyFolderModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					setTouchMoveFolder={setTouchMoveFolder}
					mFolders={mFolders}
					nestedChild={NestedChild}
				/>
			)}

			{modalState.isVisible && modalState.type === 'archive' && (
				<ArchiveModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
				/>
			)}

			{modalState.isVisible && modalState.type === 'drop-folder' && (
				<DropFolderModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
				/>
			)}
			{modalState.isVisible && modalState.type === 'approval' && (
				<ApprovalModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					modalDispatch={modalDispatch}
					findFileState={findFileState}
					isApprovalFromModal={isApprovalFromModal}
					folders={props.folderData}
					checkedAssetsItem={checkedAssetsItem}
					findFileState={findFileState}
				/>
			)}
			{modalState.isVisible && modalState.type === 'wopi' && (
				<WopiModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					fileInfo={fileInfo}
					setUploadUsingReplace={setUploadUsingReplace}
				/>
			)}
			{modalState.isVisible && modalState.type === 'rename' && (
				<RenameModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					findFile={findFile}
					checkedAssets={checkedAssets}
					assetsSlider={assetsSlider}
					onCurrentAssetsId={onCurrentAssetsId}
					setAssetSliderControls={setAssetSliderControls}
					selectedAssetsId={selectedAssetsId}
				/>
			)}
			{modalState.isVisible && (modalState.type === 'edit-meta-data' || modalState.type === 'edit-details') && (
				<EditMetaDetails
					modal={modal}
					modalState={modalState}
					findFileState={findFileState}
					setFindFileState={setFindFileState}
					numPages={numPages}
					setNumPages={setNumPages}
					setShareModalState={setShareModalState}
					isEditing={isEditing}
					setIsEditing={setIsEditing}
					efolders={efolders}
					setSelectedShareAssets={setSelectedShareAssets}
					listOnLoad={() => onLoad(false, true)}
					dataSource={dataSource}
					hasWatermark={hasWatermark}
					setCroppedImage={setCroppedImage}
					user={currentUser}
				/>
			)}
			{shareModalState && (
				<ShareModal
					shareModalState={shareModalState}
					setShareModalState={setShareModalState}
					selectedAssetID={selectedAssetID}
					selectedShareAssets={selectedShareAssets}
					setSelectedShareAssets={setSelectedShareAssets}
					efolders={efolders}
					onLoad={() => props.loadFiles(true)}
					hasWatermark={hasWatermark}
				/>
			)}
			{shareFolderModalState && (
				<ShareFolderModal
					folderId={touchFolder}
					shareFolderModalState={shareFolderModalState}
					setShareFolderModalState={setShareFolderModalState}
					efolders={efolders}
					hasWatermark={hasWatermark}
				/>
			)}

			<BulkTagUploadModal
				isOpen={isBulkTagOpen}
				onClose={handleBulkUploadModalClose}
				assets={uploadedFiles}
				setAssets={setUploadedFiles}
			/>
		</Layout>
	);
}

function mapStateToProps(state) {
	return {
		folderData: state.dam.folderData,
		fileData: state.dam.fileData,
		accounts: state.dam.accounts,
		countries: state.dam.countries,
		regions: state.dam.regions,
		wopiInfo: state.dam.params,
		fileLoading: state.dam.fileLoading,
		fileRefresh: state.dam.fileRefresh
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadFolders: (refresh) => dispatch(getFolders(refresh)),
		loadFiles: (refresh) => dispatch(getFiles(refresh)),
		loadVersion: (data) => dispatch(getFilesVersion(data)),
		revertVersion: (data) => dispatch(revertAssetVersion(data)),
		loadAccounts: () => dispatch(getAccounts()),
		loadCountries: () => dispatch(getCountries()),
		onUploadAsset: (data) => dispatch(uploadAsset(data)),
		onUploadAssetVersion: (data) => dispatch(uploadAssetVersion(data)),
		onUpdateAsset: (data, type) => dispatch(updateAsset(data, type)),
		onAddFolder: (data) => dispatch(addFolder(data)),
		onUpdateFolder: (data) => dispatch(updateFolder(data)),
		onMoveAssets: (data, folderName) => dispatch(moveAssets(data, folderName)),
		onArchiveAssets: (data) => dispatch(archiveAssets(data)),
		onDeleteFolder: (folders) => dispatch(deleteFolder(folders)),
		onMoveFolders: (data, folderName) => dispatch(moveFolder(data, folderName)),
		onCopyFolders: (data, folderName) => dispatch(copyFolder(data, folderName)),
		onUploadToDynamics: (data) => dispatch(uploadToDynamics(data)),
		getWopiParams: (id) => dispatch(getWopiParams(id)),
		loadUsers: (id) => dispatch(getUsers(id, false)),
		onAddUser: (data) => dispatch(addUser(data)),
		shareAsset: (assetKey, emailAddress) => dispatch(shareAsset(assetKey, emailAddress)),
		downloadAsset: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadAsset(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		downloadPDFAsImage: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadPDFAsImage(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		downloadOfficeAsPDF: (assetKey, userId, fileName) => dispatch(downloadOfficeAsPDF(assetKey, userId, fileName)),
		bulkDownloadAsset: (assetKey, userId, showWatermark) =>
			dispatch(bulkDownloadAsset(assetKey, userId, showWatermark)),
		authenticateForIndexing: () => dispatch(authenticateVideoForIndexing()),
		uploadVideoForIndexing: (fileName, data, config) => dispatch(uploadVideoForIndexing(fileName, data, config)),
		getIndexingState: (url, config) => dispatch(getIndexingState(url, config)),
		getFeatureFlag: () => dispatch(getFeatureFlag()),
		getCompanies: () => dispatch(getCompanies()),
		getUserPartner: (userId) => dispatch(getUserPartner(userId)),
		getUserFolder: (userId) => dispatch(getUserFolder(userId)),
		getAssetById: (data) => dispatch(getAssetById(data)),
		getPinAssets: () => dispatch(getPinAssets()),
		checkDuplicateAsset: (code) => dispatch(checkDuplicateAsset(code)),
		getDefaultWatermark: () => dispatch(getDefaultWatermark()),
		getCarts: () => dispatch(getCarts()),
		addCollection: (data) => dispatch(addCollection(data)),
		uploadChunk: (
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
			setUploadedFiles
		) =>
			dispatch(
				uploadChunk(
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
					null,
					setUploadedFiles
				)
			),
		uploadCompleted: (fileGuid) => dispatch(uploadCompleted(fileGuid)),
		getAssetContainer: () => dispatch(GetAssetContainer()),
		getAssetPreviewContainer: () => dispatch(GetAssetPreviewContainer()),
		removeThumbnail: (data) => dispatch(removeThumbnail(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Dam));

//<div className={`copylink-container ${showCopySuccess ? "copy-success" : ""}`}>
//    <Input ref={copylinkInput} addonAfter={copyLinkButton} value={`${copyUrl}`} title={`${copyUrl}`} />
//</div>
//{
//    showCopySuccess && (
//        <div className="alertsuccess-container">
//            <Alert message="Copied to clipboard." type="success" showIcon closable onClose={() => setShowCopySuccess(false)} />
//        </div>
//    )
//}

//<Input required addonAfter={emailSendButton}
//    type="email"
//    placeholder="Enter email address here..."
//    value={sendEmailInput}
//    onChange={(e) => setSendEmailInput(e.target.value)}
///>

//showEmailError && (
//<div className="alerterror-container">
//    <Alert message="Please enter email address." type="error" showIcon closable onClose={() => setShowEmailError(false)} />
//</div>
//)
