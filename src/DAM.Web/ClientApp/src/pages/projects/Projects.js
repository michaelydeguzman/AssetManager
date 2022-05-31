import React, { useState, useEffect, useRef, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { ExclamationCircleOutlined } from '@ant-design/icons';
import { Layout, Row, Col, Card, Button, List, Form, Spin, Tooltip, Checkbox, Modal, Tag, Select } from 'antd';
import { v4 as uuidv4 } from 'uuid';
import moment from 'moment';
import useModal from '../shared/useModal';

import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import { getUser, getUserRole } from '@damtoken';

import { NestedChild, getSelectedCountriesRegions, Compare, Uint8ToBase64 } from '@damhelper';

import {
	getFolders,
	getFiles,
	moveAssets,
	getUsers,
	downloadAsset,
	bulkDownloadAsset,
	downloadPDFAsImage,
	downloadOfficeAsPDF,
	archiveAssets,
	getProjects,
	getDefaultWatermark,
	addFolder,
	exportToFolder,
	getImportedAssetsToProject,
	getProjectUploads,
	uploadAsset,
	uploadChunk,
	removeAssetsFromProject,
	getTeams
} from '../dam/actions';

import AddFolderModal from '../dam/components/modals/addModal';
import ShareModal from '../dam/components/modals/shareModal';
import DownloadModal from '../dam/components/modals/downloadModal';
import EditMetaDetails from '../dam/components/modals/editMetaDetails';
import ProjectList from './projectList';
import ExportCollectionModal from '../dam/components/modals/exportCollectionModal';

import ProjectDetails from './projectDetails';
import ProjectToolsMenu from './projectToolsMenu';
import ContentHeader from '../dam/components/contentHeader';

function Projects(props) {
	const { t } = useTranslation();
	const { confirm } = Modal;
	const [modalState, modalDispatch] = useModal();
	const [form] = Form.useForm();
	const [isAssetUrl, setIsAssetUrl] = useState(false);
	const [regionOptions, setRegionOptions] = useState([]);
	const [users, setUsers] = useState([]);
	const [teams, setTeams] = useState([]);
	const { currentUser, allCountries, allAccounts } = useContext(LowFrequencyContext);

	const [createMode, setCreateMode] = useState(false);
	const [editMode, setEditMode] = useState(false);

	const [isLoading, setIsLoading] = useState(true);
	const [isExporting, setIsExporting] = useState(false);
	//const [selectedCart, setSelectedCart] = useState({});
	const [selectedProjectAssets, setSelectedProjectAssets] = useState([]);

	//share modal
	const [shareModalState, setShareModalState] = useState(false);
	const [selectedAssetID, setSelectedAssetID] = useState('');
	const [selectedShareAssets, setSelectedShareAssets] = useState([]);

	//MoveAssetModal
	const [touchMoveFolder, setTouchMoveFolder] = useState(null);
	const [checkedAssets, setCheckedAssets] = useState([]);
	const [checkedAssetsItem, setCheckedAssetsItem] = useState([]);
	const userRole = getUserRole();
	const user = getUser();
	const assetsSlider = useRef();
	const [assetsSliderControls, setAssetSliderControls] = useState([false, false]);
	const [assetsSliderPrev, assetsSliderNext] = assetsSliderControls;
	const [touchFolder, setTouchFolder] = useState(null);
	const [touchFile, setTouchFile] = useState(null);
	const [checkFile, setCheckFile] = useState([]);
	const [isEditing, setIsEditing] = useState(false);

	//toolsMenu
	const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
	const { isDynamicsAddFromDam, approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const [selectAll, setSelectAll] = useState(false);

	//archive modal
	const [findFileState, setFindFileState] = useState({});

	//approval modal
	const [isApprovalFromModal, setIsApprovalFromModal] = useState(false);

	const [requiresCartUpdate, setRequiresCartUpdate] = useState(false);
	const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
	const [assetIdToDelete, setAssetIdToDelete] = useState([]);

	//edit modal
	const [numPages, setNumPages] = useState(null);
	const [videoPreview, setVideoPreview] = useState(null);
	const [videoType, setVideoType] = useState(null);
	const [modalType, setModalType] = useState('');
	// For image viewer
	const [imagePreview, setImagePreview] = useState(null);
	const [imagePreviewType, setImagePreviewType] = useState(null);
	const [selectedFileIsReadOnly, setSelectedFileIsReadOnly] = useState(false);

	// Selected Baskets
	const [selectedProject, setSelectedProject] = useState(null);
	const [userProjects, setUserProjects] = useState([]);

	// Watermark flagging
	const [hasWatermark, setHasWatermark] = useState(false);

	// export to folder state
	const [selectedFolderTitle, setSelectedFolderTitle] = useState('');
	const [selectedFolderAccounts, setSelectedFolderAccounts] = useState([]);
	const [selectedFolderCountries, setSelectedFolderCountries] = useState([]);
	const [selectedFolderRegions, setSelectedFolderRegions] = useState([]);
	const [selectedFolderComments, setSelectedFolderComments] = useState('');
	const [selectedFolderParentId, setSelectedFolderParentId] = useState(0);

	const [downloadExt, setDownloadExt] = useState('');

	const [uploadingFileCount, setUploadingFileCount] = useState(0);
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
	const [uploadedFiles, setUploadedFiles] = useState([]);

	useEffect(() => {
		loadProjectsAndUsers();
		checkIfHasWatermark();
	}, []);

	useEffect(() => {
		setSelectedProjectAssets([]);
		if (selectedProject) {
			loadData();
		}
	}, [selectedProject]);

	useEffect(() => {
		if (uploadedFiles && uploadedFiles.length > 0) {
			// Launch bulk tagging modal
			if (uploadedFiles.length === uploadingFileCount) {
				setIsLoading(false);
				setUploadingFileCount(0);
				loadData();
				loadProjectsAndUsers();
			}
		}
	}, [uploadedFiles]);

	async function loadProjectsAndUsers() {
		var result = await props.getProjects();

		if (result) {
			var projects = result.data.projects;
			setUserProjects(projects);

			if (selectedProject && projects.filter((project) => project.id == selectedProject.id)[0]) {
				setSelectedProject(projects.filter((project) => project.id == selectedProject.id)[0]);
			} else if (props.match.params.projectId) {
				// if from route
				setSelectedProject(projects.filter((project) => project.id == props.match.params.projectId)[0]);
				var li = document.getElementById(`${props.match.params.projectId}-li`);
				if (li) {
					li.style.backgroundColor = '#c6c8c540';
				}
			}
		}

		var userResult = await props.loadUsers();

		if (userResult) {
			setUsers(userResult.data.users);
		}

		let fetchTeamsResult = await props.loadTeams();

		setTeams(fetchTeamsResult.data.teams.filter((team) => team.status === 1));

		setIsLoading(false);
	}

	async function loadfilesAndFolders(props) {
		await props.loadFolders();
		await props.loadFiles();
	}

	const customTreeTitle = (folderName, assetCount) => {
		return (
			<Tooltip color="#000" placement="right" title={folderName + ' (' + assetCount + ')'}>
				<span>{folderName}</span>
			</Tooltip>
		);
	};

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

	const combinedData = [...efolders];

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
				projectId: file.projectId
			};
		});

	//useEffect(() => {

	//    setSelectedCart({});
	//    setSelectedCartAssets([]);
	//    loadData();

	//}, [props.match.params.cartID]);

	useEffect(() => {
		if (efiles.length > 0 && selectedProject && (selectedProject.projectItems || selectedProject.projectUploads)) {
			var importAssetIds = selectedProject.projectItems.map((item) => {
				return item.assetId;
			});
			var uploadAssetIds = selectedProject.projectUploads;

			var assetIds = importAssetIds.concat(uploadAssetIds);

			let files = efiles.filter((file) => assetIds.includes(file.id));
			setSelectedProjectAssets([...files]);
		}
	}, [props.fileData]);

	async function checkIfHasWatermark() {
		var result = await props.getDefaultWatermark();
		if (result && result.data.watermark) {
			var wm = result.data.watermark;
			if (wm.id) {
				setHasWatermark(true);
			}
		}
	}

	async function loadData(unselectItems = true) {
		if (selectedProject) {
			setIsLoading(true);

			var importAssetIds = selectedProject.projectItems.map((item) => {
				return item.assetId;
			});

			var uploadAssetIds = selectedProject.projectUploads;

			var assetIds = importAssetIds.concat(uploadAssetIds);

			await loadfilesAndFolders(props);

			let files = efiles.filter((file) => assetIds.includes(file.id));
			setSelectedProjectAssets([...files]);

			if (unselectItems) {
				setCheckedAssets([]);
				setCheckedAssetsItem([]);
				setSelectAll(false);
				setSelectedAssetID('');
			}
			setTouchMoveFolder(null);

			setIsLoading(false);
		}
	}

	function modal() {
		return {
			closeModal(e, reloadData = false) {
				if (reloadData) {
					loadData();
				}
				console.log(modalState.type);
				modalDispatch({});
				form.resetFields();
				setSelectedShareAssets([]);

				if (modalState.type === 'add') {
					modal().exportBasket();
				} else {
					setTouchMoveFolder(null);
				}
			},
			header() {
				switch (modalState.type) {
					case 'add':
						return t('ModalDetail.Add Folder'); //'Add Folder'
					case 'export-basket':
						return t('ModalDetail.Export Basket');
					case 'edit-meta-data':
					case 'edit-details':
						return t('ModalDetail.Asset Preview'); //'Asset Preview'
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
					case 'edit-meta-data':
					case 'edit-details':
						return t('Button.Update'); //'Update'
					default:
						return '';
				}
			},
			submitDisabled() {
				var isDisabled = modalState.type === 'export-basket' ? (touchMoveFolder == null ? true : false) : false;

				return isDisabled;
			},
			isReadOnly() {
				return true;
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
							currentUser.id,
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
						await props.downloadOfficeAsPDF(files[0].id, currentUser.id, fileName);
					} else {
						await props.downloadAsset(
							files[0].id,
							currentUser.id,
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
								currentUser.id,
								fileName,
								files.extension,
								showWatermark,
								extension
							);
						} else if (
							(files.extension.includes('doc') || files.extension.includes('xls') || files.extension.includes('ppt')) &&
							extension === 'pdf'
						) {
							await props.downloadOfficeAsPDF(files.id, currentUser.id, fileName);
						} else {
							await props.downloadAsset(files.id, currentUser.id, fileName, files.extension, showWatermark, extension);
						}
					} else {
						await props.downloadAsset(files.id, currentUser.id, fileName, files.extension, showWatermark);
					}
				}
				if (files.length > 1) {
					var ids = [];
					files.map((f) => {
						ids.push(f.id);
					});
					await props.bulkDownloadAsset(ids.toString(), currentUser.id, showWatermark);
				}
			},
			editDetails(item) {
				let selectedFile;
				if (checkedAssets.length > 0) {
					setSelectedAssetID(checkedAssets[0]);
					selectedFile = selectedProjectAssets && selectedProjectAssets.find((row) => row.id === checkedAssets[0]);
				} else {
					if (item) {
						selectedFile = item;
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

					if (
						(selectedFile.createdById == currentUser.id || userRole.canEdit) &&
						(!approvalFlag || selectedFile.statusName !== 'Submitted For Review')
					) {
						setSelectedFileIsReadOnly(false);
					} else {
						setSelectedFileIsReadOnly(true);
					}
					setFindFileState(selectedFile);

					modalDispatch({
						type: 'BOTH',
						payload: 'edit-details'
					});
				}
			},
			exportBasket() {
				modalDispatch({
					type: 'BOTH',
					payload: 'export-basket'
				});
			},
			addFolder() {
				//setSelectedFolderTitle('');
				//setSelectedFolderAccounts([]);
				//setSelectedFolderCountries([]);
				//setSelectedFolderRegions([]);
				//setSelectedFolderComments('');
				modalDispatch({
					type: 'BOTH',
					payload: 'add'
				});
			},
			selectAccount(e) {
				var selectedAccountOptions = allAccounts.filter((c) => {
					if (e.filter((x) => x == c.id).length > 0) {
						return true;
					} else {
						return false;
					}
				});

				if (modalState.type === 'add') {
					setSelectedFolderAccounts(selectedAccountOptions);
				}
			},
			setTitle(e) {
				if (modalState.type === 'add') {
					setSelectedFolderTitle(e.target.value);
				}
			},
			setComments(e) {
				if (modalState.type === 'add') {
					setSelectedFolderComments(e.target.value);
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

				if (modalState.type === 'add') {
					var updateFolderRegions = selectedFolderRegions.filter((t) => {
						if (regionOptions.filter((x) => x.id == t.id).length > 0) {
							return true;
						}
					});
					setSelectedFolderCountries(selectedCountryOptions);
					setSelectedFolderRegions(updateFolderRegions);
				}
			}
		};
	}

	function renderLoadingText(isLoading) {
		if (isLoading) return 'Loading...';
	}

	function onCheckedAssets(item) {
		let tempIds = checkedAssets;
		let tempItems = checkedAssetsItem;

		const isChecked = tempIds.indexOf(item.id);

		if (isChecked < 0) {
			tempIds.push(item.id);
			tempItems.push(item);
			setCheckedAssets(tempIds);
			setCheckedAssetsItem(tempItems);

			if (tempIds.length === selectedProjectAssets.length) {
				setSelectAll(true);
			} else {
				setSelectAll(false);
			}

			return;
		}

		tempIds = tempIds.filter((fId) => fId !== item.id);
		tempItems = tempItems.filter((fId) => fId.id !== item.id);

		setCheckedAssets(tempIds);
		setCheckedAssetsItem(tempItems);

		if (tempIds.length === selectedProjectAssets.length) {
			setSelectAll(true);
		} else {
			setSelectAll(false);
		}
	}

	async function onSubmit(values) {
		if (modalState.type === 'add') {
			handleAddFolder().then(() => {
				form.resetFields();
				modalDispatch({});
				loadData(false);
				modal().exportBasket();
			});
		} else if (modalState.type === 'export-basket') {
			handleExportToFolder().then(() => {
				form.resetFields();
				modalDispatch({});
				loadData(false);
			});
		}
	}

	const getAddDate = (assetId) => {
		var result = '';
		if (selectedProject) {
			var filter = selectedProject.projectItems.filter((x) => x.assetId === assetId);

			if (filter.length === 1) {
				result = moment(filter[0].createdDate).format('MM/DD/YYYY');
			}
		}
		return result;
	};

	async function handleAddFolder() {
		const data = {
			folderName: selectedFolderTitle,
			comments: selectedFolderComments,
			accounts: selectedFolderAccounts,
			countries: selectedFolderCountries,
			regions: selectedFolderRegions,
			parentFolderId: touchMoveFolder ? touchMoveFolder : 0
		};

		await props.onAddFolder(data);
	}

	async function handleExportToFolder() {
		setIsExporting(true);
		const data = {
			assetIds: checkedAssets,
			folderId: touchMoveFolder
		};

		await props.exportToFolder(data);

		setIsExporting(false);
	}

	function onConfirmDelete() {
		let newAssetList = selectedProjectAssets.filter((asset) => asset.id !== assetIdToDelete);
		setSelectedProjectAssets([...newAssetList]);
		setRequiresCartUpdate(true);
		setIsDeleteModalVisible(false);
	}

	function waitingUploadFileConut(o) {
		setUploadingFileCount((prevCount) => {
			return prevCount + 1;
		});
	}

	//upload big file
	useEffect(() => {
		if (fileSize > 0) {
			fileUpload();
		}
		if (progress === 100) {
			setIsLoading(false);
			loadData();
			setProgress(0);
			setFileSize(0);
			setCounter(1);
			setBeginingOfTheChunk(0);
			setEndOfTheChunk(chunkSize);
			setFileToBeUpload({});
		}
	}, [fileToBeUpload, progress]);

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
	const uploaderBaseProps = {
		name: 'file',
		multiple: true,
		showUploadList: false,
		onChange: waitingUploadFileConut,
		customRequest: handleUpload
	};

	const clickUploaderProps = {
		...uploaderBaseProps
	};

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

	async function handleUpload(option) {
		setIsLoading(true);

		setSelectAll(false);

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
				const data = {
					name: file.name,
					fileName: file.name,
					description: '',
					extension: file.name.split('.').pop(),
					fileBytes: Uint8ToBase64(byteString),
					fileType: file.type,
					folderId: 0,
					projectId: selectedProject.id
				};

				var newAsset = await props.onUploadAsset(data);
				setUploadedFiles((files) => [...files, newAsset.asset]);
			};
			fileReader.readAsArrayBuffer(file);
		}
	}

	//async function updateCart() {
	//    let newAssetIds = selectedProjectImportedAssets.map((asset) => asset.id);

	//    const data = {
	//        id: selectedCart.id,
	//        isCurrentCart: false,
	//        UserID: user.id,
	//        assetIds: [...newAssetIds],
	//        name: selectedCart.name
	//    };

	//    if (newAssetIds.length === 0) {
	//        await deleteBasket();
	//    } else {
	//        await props.addCart(data).then(() => {
	//            console.log('updateCart_data', data);
	//        });
	//    }
	//}

	async function removeLibraryAsset(id, items = []) {
		setIsLoading(true);

		var assetIdsToRemove = [];

		if (items.length > 0) {
			assetIdsToRemove = items.map((item) => {
				return item.id;
			});
		} else {
			assetIdsToRemove.push(id);
		}

		var projectIds = [];
		projectIds.push(selectedProject.id);

		if (assetIdsToRemove.length > 0) {
			const data = {
				projectIds: projectIds,
				assetIds: assetIdsToRemove
			};

			await props.removeAssetsFromProject(data);
			setIsLoading(false);
			loadProjectsAndUsers();
		}
	}

	const onClickCard = async (item) => {
		if (item.fileType && item.fileType.includes('video')) {
			setVideoPreview(item.originalUrl);
			setVideoType(item.fileType);
			setModalType('video');
		} else if (item.fileType && item.fileType.includes('officedocument')) {
			setModalType('wopi');
		} else {
			setImagePreview(item.originalUrl);
			setImagePreviewType(item.extension);
			setModalType('image');
		}
		setSelectedAssetID(item.id);
		modal().editDetails(item);
	};

	const dataSource = () => {
		return selectedProjectAssets ? selectedProjectAssets : [];
	};

	const deleteBasket = async (projectId) => {
		setIsLoading(true);
		const data = {
			id: projectId
		};
		await props.deleteCart(data);
		setIsLoading(false);
		loadProjectsAndUsers();
		setSelectedProject(null);
	};

	const showRemoveConfirm = (e, id) => {
		confirm({
			title: t('Messages.Confirm Remove Title'),
			content: t('Messages.Confirm Remove 2'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				removeLibraryAsset(id);
			},
			onCancel() {}
		});
	};

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

	function toggleCreateMode() {
		setSelectedProject(null);

		if (!createMode) {
			setEditMode(false);
		}
		setCreateMode(!createMode);
	}

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content className="pinned-layout">
				<Row className={`dam ${isDynamicsAddFromDam ? 'add-from-dam' : ''}`}>
					<ContentHeader title={t('Projects.Header Text')} />

					<Row type="flex" className="folders-list-parent">
						<Col className="folders-container" xs={24} sm={24} md={24} lg={6} xl={6} xxl={6} style={{ marginTop: 3 }}>
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
								<ProjectList
									createMode={createMode}
									setCreateMode={setCreateMode}
									toggleCreateMode={toggleCreateMode}
									setEditMode={setEditMode}
									selectedProject={selectedProject}
									setSelectedProject={setSelectedProject}
									userProjects={userProjects}
									setUserProjects={setUserProjects}
									loadProjects={loadProjectsAndUsers}
									isArchiveMode={false}
								/>
							</Card>
						</Col>
						<Col xs={24} sm={24} md={24} lg={18} xl={18} xxl={18} className="list-container fade-in">
							{(selectedProject || createMode) && (
								<Spin spinning={isLoading} size="large" tip={renderLoadingText(isLoading)}>
									<Row style={{ marginBottom: 12 }}>
										<Card
											title={''}
											bordered
											style={{
												marginTop: 5,
												borderRadius: 8,
												width: '100%'
											}}
										>
											<ProjectDetails
												createMode={createMode}
												editMode={editMode}
												toggleCreateMode={toggleCreateMode}
												setEditMode={setEditMode}
												setCreateMode={setCreateMode}
												selectedProject={selectedProject}
												setSelectedProject={setSelectedProject}
												users={users}
												loadProjectsAndUsers={loadProjectsAndUsers}
												isArchiveMode={false}
												teams={teams}
											/>
										</Card>
									</Row>

									{!createMode && !editMode && (
										<>
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
													<ProjectToolsMenu
														modal={modal}
														isDynamicsAddFromDam={isDynamicsAddFromDam}
														clickUploaderProps={clickUploaderProps}
														checkedAssets={checkedAssets}
														setCheckedAssets={setCheckedAssets}
														checkedAssetsItem={checkedAssetsItem}
														setCheckedAssetsItem={setCheckedAssetsItem}
														setShareModalState={setShareModalState}
														setSelectedAssetID={setSelectedAssetID}
														setSelectedShareAssets={setSelectedShareAssets}
														setIsDownloadModalOpen={setIsDownloadModalOpen}
														selectedProjectImportedAssets={selectedProjectAssets}
														selectAll={selectAll}
														setSelectAll={setSelectAll}
														hasWatermark={hasWatermark}
														selectedProject={selectedProject}
														removeLibraryAsset={removeLibraryAsset}
														setDownloadExt={setDownloadExt}
													/>
												</Card>
											</Row>
											<List
												dataSource={selectedProjectAssets}
												renderItem={(item) => (
													<List.Item className="cart-list-container list-view-card">
														<Card
															cover={
																<>
																	<img alt="example" src={item.thumbnail} />
																	<Checkbox.Group
																		className="overlay-checkbox rounded-checkbox"
																		onChange={(value) => {
																			setCheckFile(value);
																			setTouchFile(...value);
																			onCheckedAssets(item);
																		}}
																		value={checkedAssets.indexOf(item.id) >= 0 ? [item.id] : []}
																	>
																		<Checkbox value={item.id} />
																	</Checkbox.Group>
																</>
															}
														>
															<div className="card-body-container">
																<div className="card-body">
																	<div className="card-details">
																		{!item.projectId && (
																			<>
																				<Row>
																					<Col align="center">
																						<Tag className="project-asset-type">Library</Tag>
																					</Col>
																				</Row>
																				<br />
																			</>
																		)}
																		<Row>
																			<div className="card-text title">
																				<h3>{item.name}</h3>
																			</div>
																		</Row>
																		<Row>
																			<div className="card-text">{`${t('ModalDetail.Total Versions')}: ${
																				item.assetVersions.length
																			}`}</div>
																		</Row>
																		<Row>
																			<div className="card-text">{`${t('ModalDetail.File Size')}: ${
																				item.fileSizeText
																			}`}</div>
																		</Row>
																	</div>
																	<div className="card-details">
																		<Row justify="center">
																			<Col align="center">{t(`Status.${item.statusName}`)}</Col>
																		</Row>
																	</div>
																	<div className="card-details">
																		<Row>
																			<div className="card-text">{`${t('ModalDetail.Created')}: ${new Date(
																				item.createdDate
																			).toLocaleDateString()}`}</div>
																		</Row>
																		{!item.projectId && (
																			<Row>
																				<div className="card-text">{`Add Date: ${getAddDate(item.id)}`}</div>
																			</Row>
																		)}
																		<Row>
																			<div className="card-text">{`${t('ModalDetail.Expiry2')}: ${
																				item.expiryDate || t('ModalDetail.None')
																			}`}</div>
																		</Row>
																	</div>
																	<div className="card-details">
																		<Row>
																			<div className="card-text title">{t('ModalDetail.Tags')}:</div>
																		</Row>
																		<Row>
																			{item.tags.map((tag) => (
																				<Col key={tag.id} className="cognitive-tags-list">
																					<Tag
																						style={{ borderRadius: 40, marginTop: 5 }}
																						closable={false}
																						className={tag.isCognitive ? 'cognitive-tag' : 'user-tag'}
																						visible
																					>
																						{tag.name}
																					</Tag>
																				</Col>
																			))}
																		</Row>
																	</div>
																	<div>
																		<Button
																			className="option-button"
																			onClick={(e) => {
																				onClickCard(item);
																			}}
																		>
																			{t('Button.View')}
																		</Button>
																	</div>
																	<div>
																		<Button
																			className="option-button"
																			onClick={(e) => {
																				showRemoveConfirm(e, item.id);
																			}}
																		>
																			{t('Button.Remove')}
																		</Button>
																	</div>
																</div>
															</div>
														</Card>
													</List.Item>
												)}
											></List>
										</>
									)}
								</Spin>
							)}
						</Col>
					</Row>
				</Row>
			</Layout.Content>

			<Modal
				visible={isDeleteModalVisible}
				okText={'Delete'}
				onOk={(e) => onConfirmDelete()}
				onCancel={(e) => setIsDeleteModalVisible(false)}
			>
				<div>Remove item from basket?</div>
			</Modal>

			{shareModalState && (
				<ShareModal
					shareModalState={shareModalState}
					setShareModalState={setShareModalState}
					selectedAssetID={selectedAssetID}
					selectedShareAssets={selectedShareAssets}
					setSelectedShareAssets={setSelectedShareAssets}
					efolders={efolders}
					onLoad={() => props.loadFiles()}
					hasWatermark={hasWatermark}
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
					isUpdating={isLoading}
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
					isFromBasket={true}
				/>
			)}

			{isDownloadModalOpen && (
				<DownloadModal
					isDownloadModalOpen={isDownloadModalOpen}
					setIsDownloadModalOpen={setIsDownloadModalOpen}
					modal={modal}
					checkedAssetsItem={checkedAssetsItem}
					downloadExt={downloadExt}
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
					listOnLoad={() => loadData()}
					dataSource={dataSource}
					hasWatermark={hasWatermark}
				/>
			)}

			{modalState.isVisible && modalState.type === 'export-basket' && (
				<ExportCollectionModal
					modal={modal}
					modalState={modalState}
					isUpdating={isExporting}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={userRole.canEdit}
					canArchiveAccess={userRole.canArchive}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					touchMoveFolder={touchMoveFolder}
					setTouchMoveFolder={setTouchMoveFolder}
					combinedData={combinedData}
					nestedChild={NestedChild}
				/>
			)}
		</Layout>
	);
}

function mapStateToProps(state) {
	return {
		folderData: state.dam.folderData,
		fileData: state.dam.fileData,
		fileLoading: state.dam.fileLoading,
		fileRefresh: state.dam.fileRefresh
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadUsers: () => dispatch(getUsers(false)),
		loadTeams: () => dispatch(getTeams()),
		downloadAsset: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadAsset(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		bulkDownloadAsset: (assetKey, userId, showWatermark) =>
			dispatch(bulkDownloadAsset(assetKey, userId, showWatermark)),
		downloadPDFAsImage: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadPDFAsImage(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		downloadOfficeAsPDF: (assetKey, userId, fileName) => dispatch(downloadOfficeAsPDF(assetKey, userId, fileName)),
		loadFiles: () => dispatch(getFiles()),
		loadFolders: () => dispatch(getFolders()),
		onMoveAssets: (data, folderName) => dispatch(moveAssets(data, folderName)),
		onArchiveAssets: (data) => dispatch(archiveAssets(data)),
		removeAssetsFromProject: (data) => dispatch(removeAssetsFromProject(data)),
		getProjects: () => dispatch(getProjects()),
		getImportedAssetsToProject: (id) => dispatch(getImportedAssetsToProject(id)),
		getProjectUploads: (id) => dispatch(getProjectUploads(id)),
		getDefaultWatermark: () => dispatch(getDefaultWatermark()),
		onAddFolder: (data) => dispatch(addFolder(data)),
		exportToFolder: (data) => dispatch(exportToFolder(data)),
		onUploadAsset: (data) => dispatch(uploadAsset(data)),
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
			projectId,
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
					0,
					projectId,
					setUploadedFiles
				)
			)
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Projects));
