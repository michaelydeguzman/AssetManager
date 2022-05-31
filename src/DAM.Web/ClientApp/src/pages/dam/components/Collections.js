import React, { useState, useEffect, useRef, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { ExclamationCircleOutlined } from '@ant-design/icons';
import { Layout, Row, Col, Card, Button, List, Form, Spin, Tooltip, Checkbox, Modal, Tag, Select } from 'antd';

import useModal from '../../shared/useModal';

import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import { getUser, getUserRole } from '@damtoken';

import { NestedChild, Compare } from '@damhelper';

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
	deleteCollection,
	addCollection,
	getCart,
	getCarts,
	getDefaultWatermark,
	addFolder,
	exportToFolder
} from '../actions';

import AddFolderModal from './modals/addModal';
import ShareModal from './modals/shareModal';
import DownloadModal from './modals/downloadModal';
import EditMetaDetails from './modals/editMetaDetails';
import CollectionList from './collectionlist';
import ExportCollectionModal from './modals/exportCollectionModal';

import ToolsMenu from './collectionToolsMenu';
import ContentHeader from './contentHeader';

function Carts(props) {
	const { t } = useTranslation();
	const { confirm } = Modal;
	const [modalState, modalDispatch] = useModal();
	const [form] = Form.useForm();
	const [regionOptions, setRegionOptions] = useState([]);

	const { currentUser, allCountries, allAccounts } = useContext(LowFrequencyContext);

	const [isLoading, setIsLoading] = useState(true);
	const [isExporting, setIsExporting] = useState(false);
	const [selectedCart, setSelectedCart] = useState({});
	const [selectedCartAssets, setSelectedCartAssets] = useState([]); //selected cart files

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
	const [assetsSliderControls, setAssetSliderControls] = useState([false, false]);
	const [assetsSliderPrev, assetsSliderNext] = assetsSliderControls;
	const [touchFile, setTouchFile] = useState(null);
	const [checkFile, setCheckFile] = useState([]);
	const [isEditing, setIsEditing] = useState(false);

	//toolsMenu
	const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
	const { isDynamicsAddFromDam, approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const [selectAll, setSelectAll] = useState(false);

	//archive modal
	const [findFileState, setFindFileState] = useState({});

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

	// Selected Collections
	const [selectedCollection, setSelectedCollection] = useState(null);
	const [userCollections, setUserCollections] = useState([]);

	// Watermark flagging
	const [hasWatermark, setHasWatermark] = useState(false);

	// export to folder state
	const [selectedFolderTitle, setSelectedFolderTitle] = useState('');
	const [selectedFolderAccounts, setSelectedFolderAccounts] = useState([]);
	const [selectedFolderCountries, setSelectedFolderCountries] = useState([]);
	const [selectedFolderRegions, setSelectedFolderRegions] = useState([]);
	const [selectedFolderComments, setSelectedFolderComments] = useState('');

	const [downloadExt, setDownloadExt] = useState('');

	useEffect(() => {
		loadCollections();
		checkIfHasWatermark();
	}, []);

	useEffect(() => {
		loadData();
	}, [selectedCollection]);

	async function loadCollections() {
		var result = await props.getCarts();

		if (result) {
			setUserCollections(result);

			if (selectedCollection && result.filter((collection) => collection.id == selectedCollection.id)[0]) {
				setSelectedCollection(result.filter((collection) => collection.id == selectedCollection.id)[0]);
			} else if (props.match.params.collectionId) {
				// if from route
				setSelectedCollection(result.filter((collection) => collection.id == props.match.params.collectionId)[0]);
				var li = document.getElementById(`${props.match.params.collectionId}-li`);
				if (li) {
					li.style.backgroundColor = '#c6c8c540';
				}
			}
		}

		setIsLoading(false);
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
				shareFolderIds: file.shareFolderIds
			};
		});

	//useEffect(() => {

	//    setSelectedCart({});
	//    setSelectedCartAssets([]);
	//    loadData();

	//}, [props.match.params.cartID]);

	useEffect(() => {
		if (efiles.length > 0 && selectedCart.assetIds) {
			let files = efiles.filter((file) => selectedCart.assetIds.includes(file.id));
			setSelectedCartAssets([...files]);
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
		if (selectedCollection) {
			setIsLoading(true);

			let cart = await props.getCart(selectedCollection.id);

			setSelectedCart(cart);

			await loadfilesAndFolders(props);

			let files = efiles.filter((file) => cart.assetIds.includes(file.id));
			setSelectedCartAssets([...files]);

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
					modal().exportCollection();
				} else {
					setTouchMoveFolder(null);
				}
			},
			header() {
				switch (modalState.type) {
					case 'add':
						return t('ModalDetail.Add Folder'); //'Add Folder'
					case 'export-collection':
						return t('ModalDetail.Export Collection');
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
				var isDisabled = modalState.type === 'export-collection' ? (touchMoveFolder == null ? true : false) : false;

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
					selectedFile = selectedCartAssets && selectedCartAssets.find((row) => row.id === checkedAssets[0]);
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
			exportCollection() {
				modalDispatch({
					type: 'BOTH',
					payload: 'export-collection'
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

	async function loadfilesAndFolders(props) {
		await props.loadFolders();
		props.loadFiles();
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

			if (tempIds.length === selectedCartAssets.length) {
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

		if (tempIds.length === selectedCartAssets.length) {
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
				modal().exportCollection();
			});
		} else if (modalState.type === 'export-collection') {
			handleExportToFolder().then(() => {
				form.resetFields();
				modalDispatch({});
				loadData(false);
			});
		}
	}

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
		let newAssetList = selectedCartAssets.filter((asset) => asset.id !== assetIdToDelete);
		setSelectedCartAssets([...newAssetList]);
		setRequiresCartUpdate(true);
		setIsDeleteModalVisible(false);
	}

	async function updateCart() {
		let newAssetIds = selectedCartAssets.map((asset) => asset.id);

		const data = {
			id: selectedCart.id,
			isCurrentCart: false,
			UserID: user.id,
			assetIds: [...newAssetIds],
			name: selectedCart.name
		};

		if (newAssetIds.length === 0) {
			await deleteCollection();
		} else {
			await props.addCollection(data).then(() => {
				console.log('updateCart_data', data);
			});
		}
	}

	async function removeCollectionItem(id = 0, items = []) {
		setIsLoading(true);

		let filteredIds = [];
		let copyIds = selectedCollection.assetIds;

		if (items.length > 0) {
			for (let i = 0; i < items.length; i++) {
				filteredIds = copyIds.filter((x) => x !== items[i].id);
				copyIds = filteredIds;
			}
		} else {
			filteredIds = selectedCollection.assetIds.filter((x) => x !== id);
		}

		const data = {
			id: selectedCollection.id,
			assetIds: filteredIds,
			name: selectedCollection.name
		};

		await props.addCollection(data);
		setIsLoading(false);
		loadCollections();
	}

	async function renameCollection(newName) {
		setIsLoading(true);
		const data = {
			id: selectedCollection.id,
			assetIds: selectedCollection.assetIds,
			name: newName
		};

		await props.addCollection(data);
		setIsLoading(false);
		loadCollections();
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
		return selectedCartAssets ? selectedCartAssets : [];
	};

	const deleteCollection = async (collectionId) => {
		setIsLoading(true);
		const data = {
			id: collectionId
		};
		await props.deleteCollection(data);
		setIsLoading(false);
		loadCollections();
		setSelectedCollection(null);
	};

	const showRemoveConfirm = (e, id) => {
		confirm({
			title: t('Messages.Confirm Remove Title'),
			content: t('Messages.Confirm Remove'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				removeCollectionItem(id);
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

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content className="pinned-layout">
				<Row className={`dam ${isDynamicsAddFromDam ? 'add-from-dam' : ''}`}>
					<ContentHeader title={t('Collections')} />

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
								<CollectionList
									setSelectedCollection={setSelectedCollection}
									userCollections={userCollections}
									loadCollections={loadCollections}
								/>
							</Card>
						</Col>
						<Col xs={24} sm={24} md={24} lg={18} xl={18} xxl={18} className="list-container fade-in">
							{selectedCollection && (
								<Spin spinning={isLoading} size="large" tip={renderLoadingText(isLoading)}>
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
												modal={modal}
												isDynamicsAddFromDam={isDynamicsAddFromDam}
												checkedAssets={checkedAssets}
												setCheckedAssets={setCheckedAssets}
												checkedAssetsItem={checkedAssetsItem}
												setCheckedAssetsItem={setCheckedAssetsItem}
												setShareModalState={setShareModalState}
												setSelectedAssetID={setSelectedAssetID}
												setSelectedShareAssets={setSelectedShareAssets}
												setIsDownloadModalOpen={setIsDownloadModalOpen}
												deleteCollection={deleteCollection}
												updateCart={updateCart}
												renameCollection={renameCollection}
												selectedCartAssets={selectedCartAssets}
												selectAll={selectAll}
												setSelectAll={setSelectAll}
												hasWatermark={hasWatermark}
												selectedCollection={selectedCollection}
												removeCollectionItem={removeCollectionItem}
												setDownloadExt={setDownloadExt}
											/>
										</Card>
									</Row>

									<List
										dataSource={selectedCartAssets}
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
																<Row>
																	<div className="card-text title">{item.name}</div>
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
				<div>Remove item from collection?</div>
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
					isFromCollection={true}
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

			{modalState.isVisible && modalState.type === 'export-collection' && (
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
		loadUsers: (id) => dispatch(getUsers(id, false)),
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
		deleteCollection: (data) => dispatch(deleteCollection(data)),
		addCollection: (data) => dispatch(addCollection(data)),
		getCart: (data) => dispatch(getCart(data)),
		getCarts: () => dispatch(getCarts()),
		getDefaultWatermark: () => dispatch(getDefaultWatermark()),
		onAddFolder: (data) => dispatch(addFolder(data)),
		exportToFolder: (data) => dispatch(exportToFolder(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Carts));
