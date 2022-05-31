import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Input, Divider } from 'antd';
import {
	FullscreenOutlined,
	ExclamationCircleOutlined,
	DownOutlined,
	DeleteOutlined,
	EditOutlined
} from '@ant-design/icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faShareAlt, faDownload, faFile, faFolder, faPlusCircle, faMinus } from '@fortawesome/free-solid-svg-icons';
import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { ASSET_STATUS } from '../../constants';
import { useTranslation } from 'react-i18next';
import { FaAngleDown } from 'react-icons/fa';

function CartToolsMenu(props) {
	const { t } = useTranslation();
	const { confirm } = Modal;
	const {
		modal,
		checkedAssetsItem,
		setCheckedAssetsItem,
		setShareModalState,
		setSelectedAssetID,
		setSelectedShareAssets,
		setIsDownloadModalOpen,
		deleteCollection,
		selectedCartAssets,
		checkedAssets,
		setCheckedAssets,
		selectAll,
		setSelectAll,
		hasWatermark,
		selectedCollection,
		renameCollection,
		removeCollectionItem,
		setDownloadExt
	} = props;
	const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const userRole = getUserRole();

	const [isdeleteCollectionModalVisible, setIsdeleteCollectionModalVisible] = useState(false);
	const [isEditingName, setisEditingName] = useState(false);
	const [selectedCollectionName, setSelectedCollectionName] = useState('');

	useEffect(() => {
		setisEditingName(false);
		setSelectedCollectionName('');
	}, [selectedCollection]);

	function handleSelectAll() {
		if (selectAll) {
			setCheckedAssets([]);
			setCheckedAssetsItem([]);
			setSelectedShareAssets([]);
		} else {
			let tempItems = selectedCartAssets;

			if (tempItems && tempItems.length > 0) {
				let tempIds = tempItems.map((d) => {
					return d.id;
				});
				setCheckedAssets(tempIds);
				setCheckedAssetsItem(tempItems);
			}
		}
		setSelectAll(!selectAll);
	}

	const downloadMenu = () => {
		return (
			<Menu>
				<Menu.Item onClick={handleDownloadClick}>Original</Menu.Item>

				{checkedAssetsItem[0].extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('png')))
					<Menu.Item onClick={() => handleDownloadClick('png')}>PNG</Menu.Item>
				)}

				{checkedAssetsItem[0].extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('jpg') && !checkedAssetsItem[0].extension.includes('jpeg')))
					<Menu.Item onClick={() => handleDownloadClick('jpeg')}>JPEG</Menu.Item>
				)}

				{(checkedAssetsItem[0].fileType.includes('image') ||
					checkedAssetsItem[0].extension.includes('doc') ||
					checkedAssetsItem[0].extension.includes('ppt') ||
					checkedAssetsItem[0].extension.includes('xls')) && (
					<Menu.Item onClick={() => handleDownloadClick('pdf')}>PDF</Menu.Item>
				)}
			</Menu>
		);
	};

	function handleDownloadClick(ext) {
		var filterList = checkedAssetsItem.filter(
			(x) => x.fileType.includes('image') || (x.extension.includes('pdf') && ext.length > 0)
		);

		if (hasWatermark && filterList.length > 0) {
			setIsDownloadModalOpen(true);
			setDownloadExt(ext);
		} else {
			downloadAssets(checkedAssetsItem, ext);
		}
	}

	function downloadAssets(checkedAssetsItem, ext) {
		modal().download(checkedAssetsItem, false, ext);
	}

	const showRemoveConfirm = () => {
		confirm({
			title: t('Confirm Remove'),
			content: t('Are you sure you want to remove these item(s) from the collection?'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				removeCollectionItem(0, checkedAssetsItem);
			},
			onCancel() {}
		});
	};

	const menu = (
		<Menu>
			<Menu.Item key="1">Export to Folder</Menu.Item>
			<Menu.Item key="2">Export to LinkedIn</Menu.Item>
			<Menu.Item key="3">Export to Twitter</Menu.Item>
			<Menu.Item key="4">Export to Facebook</Menu.Item>
		</Menu>
	);

	const showConfirm = (e, collectionId) => {
		confirm({
			title: t('Confirm Delete'),
			content: t('Are you sure you want to delete this collection?'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				deleteCollection(collectionId);
			},
			onCancel() {}
		});
	};

	const handleEditClick = () => {
		setisEditingName(true);
		setSelectedCollectionName(selectedCollection.name);
	};

	return (
		<>
			{selectedCollection && (
				<Row>
					{!isEditingName ? (
						<Space>
							<h2>{selectedCollection.name}</h2>
							<Space style={{ marginBottom: 5 }}>
								<EditOutlined onClick={handleEditClick} />
								<DeleteOutlined onClick={(e) => showConfirm(e, selectedCollection.id)} />
							</Space>
						</Space>
					) : (
						<>
							<Col span={8}>
								<Input value={selectedCollectionName} onChange={(e) => setSelectedCollectionName(e.target.value)} />
							</Col>
							<Col span={10} align="left" style={{ marginLeft: 10 }}>
								<Space>
									<Button type="secondary" onClick={(e) => setisEditingName(false)}>
										{t('Button.Cancel')}
									</Button>
									<Button
										onClick={(e) => {
											renameCollection(selectedCollectionName);
											setisEditingName(false);
											setSelectedCollectionName('');
										}}
									>
										{t('Button.Save')}
									</Button>
								</Space>
							</Col>
						</>
					)}
				</Row>
			)}
			<Row style={{ display: 'flex', justifyContent: 'space-between', marginTop: 10 }} id="cartToolsMenu-toolBar">
				<Col>
					<Space>
						{checkedAssets.length > 0 ? (
							<Tag
								closable
								visible
								onClose={() => {
									setCheckedAssets([]);
									setCheckedAssetsItem([]);
									setSelectedShareAssets([]);
									setSelectAll(false);
								}}
								style={{ marginRight: '4px' }}
							>
								{t('ToolMenu.Selected', { number: `${checkedAssets.length}/${selectedCartAssets.length}` })}
							</Tag>
						) : (
							<Tag style={{ marginRight: '4px' }}>
								{t('ToolMenu.Total Assets', { number: selectedCartAssets.length })}
							</Tag>
						)}
						{selectedCartAssets.length > 0 ? (
							<Checkbox
								style={{ marginLeft: '8px' }}
								className="select-all-checkbox"
								checked={selectAll}
								onChange={handleSelectAll}
							>
								{t('ToolMenu.Select All')}
							</Checkbox>
						) : (
							''
						)}

						{checkedAssetsItem.length > 0 && !isEditingName && (
							<Col>
								{checkedAssetsItem.length === 1 &&
								(checkedAssetsItem[0].fileType.includes('image') ||
									checkedAssetsItem[0].extension.includes('pdf') ||
									checkedAssetsItem[0].extension.includes('doc') ||
									checkedAssetsItem[0].extension.includes('xls') ||
									checkedAssetsItem[0].extension.includes('ppt')) ? (
									<Dropdown overlay={downloadMenu} placement="bottomRight">
										<Button
											className="option-button"
											icon={
												<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
													<FontAwesomeIcon icon={faFile} size="lg" />
													<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
												</span>
											}
										>
											<Space>
												{t('Button.Download')}
												<FaAngleDown className="icons" onClick={(e) => e.preventDefault()} style={{ marginTop: 10 }} />
											</Space>
										</Button>
									</Dropdown>
								) : (
									<Button
										className="option-button"
										onClick={handleDownloadClick}
										icon={
											<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
											</span>
										}
									>
										{t('Button.Download')}
									</Button>
								)}
							</Col>
						)}
						{checkedAssetsItem.length > 0 && userRole.canShare && !isEditingName && (
							<Col>
								<Button
									className="option-button"
									onClick={() => {
										setSelectedAssetID(checkedAssetsItem[0].id);
										setSelectedShareAssets(checkedAssetsItem);
										setShareModalState(true);
									}}
									icon={
										<span className="fa-layers fa-fw" style={{ marginRight: 8 }}>
											<FontAwesomeIcon icon={faFile} size="lg" />
											<FontAwesomeIcon icon={faShareAlt} color="white" transform="shrink-6.4 down-2" />
										</span>
									}
								>
									{t('Button.Share')}
								</Button>
							</Col>
						)}

						{checkedAssetsItem.length > 0 && !isEditingName && (
							<Col>
								<Button
									className="option-button"
									onClick={modal().exportCollection}
									icon={
										<span className="fa-layers" style={{ marginRight: 8 }}>
											<FontAwesomeIcon icon={faFolder} size="lg" />
											<FontAwesomeIcon icon={faPlusCircle} color="white" transform="shrink-5 down-4 right-2" />
										</span>
									}
								>
									{t('Button.Export To Folder')}
								</Button>
							</Col>
						)}

						{checkedAssetsItem.length > 0 && !isEditingName && (
							<Col>
								<Button
									className="option-button"
									onClick={showRemoveConfirm}
									icon={
										<span className="fa-layers" style={{ marginRight: 8 }}>
											<FontAwesomeIcon icon={faFile} size="lg" />
											<FontAwesomeIcon icon={faMinus} color="white" transform="shrink-5 down-4 right-2" />
										</span>
									}
								>
									{t('Button.Remove')}
								</Button>
							</Col>
						)}
					</Space>
				</Col>
			</Row>
		</>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(CartToolsMenu));
