import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { Layout, Row, Col, Card, Spin, List, Form, Empty, Radio } from 'antd';
import PinAssetThumbnail from './pinAssetThumbnail';
import EditMetaDetails from '../dam/components/modals/editMetaDetails';
import ToolsMenu from './pinToolsMenu';
import PinFolders from './pinFolders';
import DownloadModal from '../dam/components/modals/downloadModal';
import ArchiveModal from '../dam/components/modals/archiveModal';
import ShareModal from '../dam/components/modals/shareModal';
import ApprovalModal from '../dam/components/modals/approvalModal';
import {
	getPinAssetsDetail,
	getPinAssets,
	getFolders,
	uploadAsset,
	updateAsset,
	getFilesVersion,
	revertAssetVersion,
	downloadAsset,
	bulkDownloadAsset,
	downloadPDFAsImage,
	downloadOfficeAsPDF,
	archiveAssets,
	orderPinAssets,
	getUsers,
	getCompanies,
	getDefaultWatermark
} from '../dam/actions';
import { getAppMode, getUser, getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { getPinSortResult, getGridLayout } from '@damhelper';
import { OPTIONS, USER_ROLES } from '../constants';
import useModal from '../shared/useModal';
import { GridContextProvider, GridDropZone, GridItem, swap } from 'react-grid-dnd';
import { useWindowWidth } from '@damhookuseWindowWidth';
import { useTranslation } from 'react-i18next';
import ContentHeader, { DropDownOptions } from '../dam/components/contentHeader';

function PinContainer(props) {
	const { t } = useTranslation();
	const [modalState, modalDispatch] = useModal();
	const [form] = Form.useForm();
	const width = useWindowWidth();
	const rowNumber = width > 767 ? (width > 1920 ? 4 : 3) : 2;

	const [currentUserId, setCurrentUserId] = useState('');
	const userRole = getUserRole();

	const [assetList, setAssetList] = useState([]);
	const [orderingAssetList, setOrderingAssetList] = useState([]);
	const [shareModalState, setShareModalState] = useState(false);

	const [selectedShareAssets, setSelectedShareAssets] = useState();

	const [checkedAssets, setCheckedAssets] = useState([]);
	const [checkedAssetsItem, setCheckedAssetsItem] = useState([]);
	const [videoPreview, setVideoPreview] = useState(null);
	const [videoType, setVideoType] = useState(null);
	const [modalType, setModalType] = useState('');
	// For image viewer
	const [imagePreview, setImagePreview] = useState(null);
	const [imagePreviewType, setImagePreviewType] = useState(null);

	const [selectedAssetID, setSelectedAssetID] = useState();
	const [selectedFileIsReadOnly, setSelectedFileIsReadOnly] = useState(false);

	const [findFileState, setFindFileState] = useState({});

	const [touchFile, setTouchFile] = useState(null);
	const [checkFile, setCheckFile] = useState([]);

	const [pinAssets, setPinAssets] = useState([]);

	const [isUpdating, setIsUpdating] = useState(false);
	const [isEditing, setIsEditing] = useState(false);
	const [gridSettings, setGridSettings] = useState(getAppMode().CardGridPreference);

	const [assetsSliderControls, setAssetSliderControls] = useState([false, false]);
	const [assetsSliderPrev, assetsSliderNext] = assetsSliderControls;
	const [numPages, setNumPages] = useState(null);

	const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
	const [isApprovalFromModal, setIsApprovalFromModal] = useState(false);
	const [hasWatermark, setHasWatermark] = useState(false);

	const { isDynamicsAddFromDam, approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const [selectedSortbyOpt, setSelectedSortbyOpt] = useState(OPTIONS.SORTBY.Default.label);
	const [currentUser, setCurrentUser] = useState();
	const [folderIdList, setFolderIdList] = useState([]);

	const [downloadExt, setDownloadExt] = useState('');

	const SortByOptions = approvalFlag
		? [
				OPTIONS.SORTBY.Default.label,
				OPTIONS.SORTBY.DisplayName.label,
				OPTIONS.SORTBY.DateCreated.label,
				OPTIONS.SORTBY.DateApproved.label,
				OPTIONS.SORTBY.DateRejected.label,
				OPTIONS.SORTBY.DateSubmitted.label
		  ]
		: [OPTIONS.SORTBY.DisplayName.label, OPTIONS.SORTBY.DateCreated.label];

	const efolders =
		props.folderData &&
		props.folderData.map((folder) => {
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

	useEffect(() => {
		async function getCurrentUser() {
			let user = await getUser();
			let rootfolder = 0;
			if (user) {
				var userObj = await props.loadUsers(user.id);
				var companyObj = await props.getCompanies();
				const myCompanyId = userObj.data.users.companyId;
				const myCompany = companyObj ? companyObj.data.companies.find((c) => c.id === myCompanyId) : null;
				if (userObj) {
					if (userObj.data.users.userRole.Name !== USER_ROLES.DAM_ADMIN && myCompanyId) {
						rootfolder = myCompany.rootFolderId;
					}
					userObj.data.users.rootFolderId = rootfolder;
					userObj.data.users.company = myCompany;
					setCurrentUser(userObj.data.users);
				}
				setCurrentUserId(user.id);
			}
		}
		async function loadFolders() {
			await props.loadFolders();
		}
		getCurrentUser();
		onLoad();
		loadFolders();
	}, []);

	useEffect(() => {
		if (modalState.type === 'edit-details') {
			let filteredFile = assetList.filter((x) => x.id === parseInt(selectedAssetID));
			if (filteredFile.length > 0) {
				var file = filteredFile[0];
				//setIsUploading(false);
				modal().editDetails(file);
			}
		}
	}, [assetList]);

	useEffect(() => {
		if (props.fileRefresh) {
			onLoad();
		}
	}, [props.fileRefresh]);

	async function onLoad() {
		setIsUpdating(true);
		var pinAssets = await props.getPinAssets();
		var list = await props.getPinAssetsDetail();
		if (list && list.length > 0) {
			let folderIdList = [];
			list.forEach((p) => {
				var orderNumber = pinAssets.find((a) => a.assetId === p.id).orderNumber;
				p.orderNumber = orderNumber;
				folderIdList.push(p.folderId);
			});

			var sortedList = getPinSortResult(OPTIONS.SORTBY.Default.label, list);
			setAssetList(sortedList);
			setOrderingAssetList(sortedList);
			setPinAssets(pinAssets);
			setFolderIdList(folderIdList);
		} else {
			setAssetList([]);
			setOrderingAssetList([]);
			setFolderIdList([]);
		}
		setSelectedSortbyOpt(OPTIONS.SORTBY.Default.label);
		setIsUpdating(false);
		checkIfHasWatermark();
		setCheckedAssets([]);
		setCheckedAssetsItem([]);
	}

	const dataSource = () => {
		return assetList ? assetList : [];
	};

	async function checkIfHasWatermark() {
		var result = await props.getDefaultWatermark();
		if (result && result.data.watermark) {
			var wm = result.data.watermark;
			if (wm.id) {
				setHasWatermark(true);
			}
		}
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

			// if (tempIds.length === dataSource().length) {
			// 	setSelectAll(true);
			// } else {
			// 	setSelectAll(false);
			// }

			return;
		}

		tempIds = tempIds.filter((fId) => fId !== item.id);
		tempItems = tempItems.filter((fId) => fId.id !== item.id);

		setCheckedAssets(tempIds);
		setCheckedAssetsItem(tempItems);

		// if (tempIds.length === dataSource().length) {
		// 	setSelectAll(true);
		// } else {
		// 	setSelectAll(false);
		// }
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
					case 'edit-meta-data':
					case 'edit-details':
						return t('Button.Update'); //'Update'
					default:
						return '';
				}
			},
			submitDisabled() {
				// var isDisabled =
				// 	modalState.type === 'move' ||
				// 	modalState.type === 'move-folder' ||
				// 	modalState.type === 'video'
				// modalState.type === 'wopi'
				// 	? touchMoveFolder == null
				// 		? true
				// 		: false
				// 	: false;

				// return isDisabled;
				return false;
			},
			isReadOnly() {
				var isReadOnly = false;

				// if (modalState.type === 'rename' || modalState.type === 'edit-metadata' || modalState.type === 'edit-details') {
				// 	isReadOnly = selectedFileIsReadOnly;
				// }
				return isReadOnly;
			},
			archive() {
				modalDispatch({
					type: 'BOTH',
					payload: 'archive'
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
			closeModal() {
				modalDispatch({});
				form.resetFields();
				setFindFileState({});
				setIsEditing(false);
				setCheckedAssets([]);
				setCheckedAssetsItem([]);
				setNumPages(null);
			},
			isReadOnly() {
				var isReadOnly = false;

				if (modalState.type === 'rename' || modalState.type === 'edit-metadata' || modalState.type === 'edit-details') {
					isReadOnly = selectedFileIsReadOnly;
				}
				return isReadOnly;
			},
			submitDisabled() {
				var isDisabled = modalState.type === 'move' || modalState.type === 'move-folder' || modalState.type === 'video';
				return isDisabled;
			},
			editDetails(item) {
				let selectedFile;
				if (checkedAssets.length > 0) {
					setSelectedAssetID(checkedAssets[0]);
					selectedFile = assetList && assetList.find((row) => row.id === checkedAssets[0]);
				} else {
					if (item) {
						selectedFile = item;
						// let assetOpened = [];
						// assetOpened.push(item.id);
						// setAssetOpened(assetOpened);
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
						(selectedFile.createdById == currentUserId || userRole.canEdit) &&
						(!approvalFlag || selectedFile.statusName !== 'Submitted For Review')
					) {
						setSelectedFileIsReadOnly(false);
					} else {
						setSelectedFileIsReadOnly(true);
					}
					setFindFileState(selectedFile);

					// let docViewerDocList = [...docViewerDoc];
					// docViewerDocList = [];
					// docViewerDocList.push({
					//   uri: selectedFile.originalUrl
					// });
					// setDocViewerDoc(docViewerDocList);
					modalDispatch({
						type: 'BOTH',
						payload: 'edit-details'
					});
				}
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
			}
		};
	}

	async function onSubmit(values) {
		if (modalState.type === 'archive') {
			handleArchiveAssets().then(() => {
				modalDispatch({});
				onLoad();
				//setTouchFolder(touchFolder);
				//setTouchMoveFolder(null);
				//setAssetOpened([]);
				setCheckedAssets([]);
				//setIsEditing(false);
			});
		}
		// }
	}

	async function handleArchiveAssets() {
		const data = {
			assetIds: checkedAssets.length > 0 ? checkedAssets : [selectedAssetID],
			status: 1, //Archived
			folderId: findFileState.folderId
		};
		await props.onArchiveAssets(data);
	}

	function renderLoadingText(loading) {
		if (loading) return 'Loading...';
	}

	function dragOnChange(sourceId, sourceIndex, targetIndex, targetId) {
		const result = swap(orderingAssetList, sourceIndex, targetIndex);
		return setOrderingAssetList(result);
	}

	function renderAssetsList() {
		if (dataSource().length > 0 && !isOrdering) {
			return (
				<>
					<Col className="folders-container fade-in" xs={24} sm={24} md={24} lg={7} xl={7} xxl={7}>
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
							<PinFolders efolders={efolders} currentUser={currentUser} folderIdList={folderIdList} />
						</Card>
					</Col>
					<Col span="auto" className="list-section" xs={24} sm={24} md={24} lg={17} xl={17} xxl={17}>
						<List
							key={'Mode1'}
							pagination={false}
							grid={getGridLayout(1)}
							dataSource={dataSource()}
							className="fade-in"
							renderItem={(item) => (
								<PinAssetThumbnail
									item={item}
									mode={gridSettings}
									onCheckedAssets={onCheckedAssets}
									checkedAssets={checkedAssets}
									setVideoPreview={setVideoPreview}
									setVideoType={setVideoType}
									setModalType={setModalType}
									setImagePreview={setImagePreview}
									setImagePreviewType={setImagePreviewType}
									modal={modal}
									setSelectedAssetID={setSelectedAssetID}
									setCheckFile={setCheckFile}
									setTouchFile={setTouchFile}
									pinAssets={pinAssets}
									setPinAssets={setPinAssets}
									isOrdering={isOrdering}
									listOnLoad={onLoad}
								/>
							)}
						/>
					</Col>
				</>
			);
		} else if (dataSource().length > 0 && isOrdering) {
			return (
				<Col span="auto" className="list-section">
					<GridContextProvider onChange={dragOnChange}>
						<GridDropZone
							id="assetList"
							boxesPerRow={rowNumber}
							rowHeight={((width * 0.87) / rowNumber) * 1.02}
							style={{
								height: `${((width * 0.87) / rowNumber) * 1.02 * Math.ceil(dataSource().length / rowNumber)}px`
							}}
							className="fade-in"
						>
							{orderingAssetList.map((item) => (
								<GridItem key={item.id}>
									<PinAssetThumbnail
										item={item}
										mode={gridSettings}
										onCheckedAssets={onCheckedAssets}
										checkedAssets={checkedAssets}
										setVideoPreview={setVideoPreview}
										setVideoType={setVideoType}
										setModalType={setModalType}
										setImagePreview={setImagePreview}
										setImagePreviewType={setImagePreviewType}
										modal={modal}
										setSelectedAssetID={setSelectedAssetID}
										setCheckFile={setCheckFile}
										setTouchFile={setTouchFile}
										pinAssets={pinAssets}
										setPinAssets={setPinAssets}
										isOrdering={isOrdering}
										listOnLoad={onLoad}
									/>
								</GridItem>
							))}
						</GridDropZone>
					</GridContextProvider>
				</Col>
			);
		} else {
			return (
				<Row className="empty-folder-container" type="flex" justify="center" align="middle">
					<Col span={24} align="center">
						<Empty image="https://gw.alipayobjects.com/zos/antfincdn/ZHrcdLPrvN/empty.svg" />
					</Col>
				</Row>
			);
		}
	}

	function savePinOrder() {
		if (orderingAssetList.length > 0) {
			var idList = [];
			orderingAssetList.forEach((a) => {
				idList.push(a.id);
			});
			const data = {
				orderedAssetIds: idList
			};
			props.orderPinAssets(data).then(async () => {
				await onLoad();
				setIsOrdering(false);
			});
		}
	}

	function sortAssetsList(value) {
		const result = getPinSortResult(value, assetList);
		setSelectedSortbyOpt(value);
		setAssetList([...result]);
		setOrderingAssetList([...result]);
	}

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content className="pinned-layout">
				<Row className={`dam ${isDynamicsAddFromDam ? 'add-from-dam' : ''}`}>
					<ContentHeader
						title={t('Slider.Pinned Assets')}
						extraButtons={
							<>
								<DropDownOptions
									title={t('Button.Sort By')}
									subtitle={t('Sort By.Sort Options')}
									onClear={() => {
										sortAssetsList(OPTIONS.SORTBY.Default.label);
									}}
									content={
										<Row style={{ display: 'flex', flexWrap: 'nowrap', alignItems: 'center' }}>
											<Col>
												<Radio.Group
													style={{ display: 'flex', flexDirection: 'column' }}
													value={selectedSortbyOpt}
													onChange={(e) => {
														sortAssetsList(e.target.value);
													}}
													options={SortByOptions}
												/>
											</Col>
										</Row>
									}
								/>
							</>
						}
					/>
					<Row type="flex" className="folders-list-parent">
						<Col span={24} className="list-container fade-in">
							<Spin spinning={isUpdating} size="large" tip={renderLoadingText(isUpdating)}>
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
											checkedAssetsItem={checkedAssetsItem}
											setShareModalState={setShareModalState}
											// keySearchUsed={keySearchUsed}
											// searchState={searchState}
											// modalDispatch={modalDispatch}
											// treeDispatch={treeDispatch}

											// touchFolder={touchFolder}
											// modalState={modalState}
											// approvalFlag={approvalFlag}
											// setSelectedFileKey={setSelectedFileKey}
											setSelectedAssetID={setSelectedAssetID}
											// setSelectedAssetKey={setSelectedAssetKey}
											// selectAll={selectAll}
											// setSelectAll={setSelectAll}
											setSelectedShareAssets={setSelectedShareAssets}
											// setShareFolderModalState={setShareFolderModalState}
											setIsDownloadModalOpen={setIsDownloadModalOpen}
											savePinOrder={savePinOrder}
											assetsCount={assetList ? assetList.length : 0}
											hasWatermark={hasWatermark}
											setDownloadExt={setDownloadExt}
										/>
									</Card>
								</Row>
								<Row type="flex" justify="center">
									{renderAssetsList()}
								</Row>
							</Spin>
						</Col>
					</Row>
				</Row>
			</Layout.Content>
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
					listOnLoad={() => onLoad(props)}
					dataSource={dataSource}
					hasWatermark={hasWatermark}
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
			{modalState.isVisible && modalState.type === 'archive' && (
				<ArchiveModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
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
					onLoad={() => {}}
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
				/>
			)}
			{/*{modalState.isVisible && modalState.type === 'move' && (
				<MoveAssetModal
					modal={modal}
					modalState={modalState}
					isUpdating={isUpdating}
					form={form}
					onSubmit={onSubmit}
					canEditAccess={canEditAccess}
					canArchiveAccess={canArchiveAccess}
					assetsSliderPrev={assetsSliderPrev}
					assetsSliderNext={assetsSliderNext}
					setTouchMoveFolder={setTouchMoveFolder}
					combinedData={combinedData}
					nestedChild={NestedChild}
				/>
			)}
			{shareFolderModalState && (
				<ShareFolderModal
					folderId={touchFolder}
					shareFolderModalState={shareFolderModalState}
					setShareFolderModalState={setShareFolderModalState}
					efolders={efolders}
				/>
			)} */}
		</Layout>
	);
}

function mapStateToProps(state) {
	return {
		folderData: state.dam.folderData,
		fileLoading: state.dam.fileLoading,
		fileRefresh: state.dam.fileRefresh
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadUsers: (id) => dispatch(getUsers(id, false)),
		getCompanies: () => dispatch(getCompanies()),
		loadFolders: () => dispatch(getFolders()),
		getPinAssetsDetail: () => dispatch(getPinAssetsDetail()),
		getPinAssets: () => dispatch(getPinAssets()),
		orderPinAssets: (data) => dispatch(orderPinAssets(data)),
		onArchiveAssets: (data) => dispatch(archiveAssets(data)),
		onUploadAsset: (data) => dispatch(uploadAsset(data)),
		onUpdateAsset: (data, type) => dispatch(updateAsset(data, type)),
		loadVersion: (data) => dispatch(getFilesVersion(data)),
		revertVersion: (data) => dispatch(revertAssetVersion(data)),
		downloadAsset: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadAsset(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		bulkDownloadAsset: (assetKey, userId, showWatermark) =>
			dispatch(bulkDownloadAsset(assetKey, userId, showWatermark)),
		downloadPDFAsImage: (assetKey, userId, fileName, fileExt, showWatermark, extension) =>
			dispatch(downloadPDFAsImage(assetKey, userId, fileName, fileExt, showWatermark, extension)),
		downloadOfficeAsPDF: (assetKey, userId, fileName) => dispatch(downloadOfficeAsPDF(assetKey, userId, fileName)),
		getDefaultWatermark: () => dispatch(getDefaultWatermark())
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(PinContainer));
