import React, { useState, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Upload } from 'antd';
import { ExclamationCircleOutlined } from '@ant-design/icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faShareAlt, faDownload, faFile, faFolder, faPlusCircle, faMinus } from '@fortawesome/free-solid-svg-icons';
import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import { FaAngleDown } from 'react-icons/fa';

function ProjectToolsMenu(props) {
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
		savePinOrder,
		assetsCount,
		deleteBasket,
		selectedProjectImportedAssets,
		checkedAssets,
		setCheckedAssets,
		selectAll,
		setSelectAll,
		hasWatermark,
		removeLibraryAsset,
		setDownloadExt,
		clickUploaderProps
	} = props;
	const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const userRole = getUserRole();

	const [isEditingName, setisEditingName] = useState(false);

	function handleSelectAll() {
		if (selectAll) {
			setCheckedAssets([]);
			setCheckedAssetsItem([]);
			setSelectedShareAssets([]);
		} else {
			let tempItems = selectedProjectImportedAssets;

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
			content: t('Are you sure you want to remove these library asset(s) from the project?'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				removeLibraryAsset(0, checkedAssetsItem);
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

	return (
		<>
			<Row style={{ display: 'flex', justifyContent: 'space-between' }} id="cartToolsMenu-toolBar">
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
								{t('ToolMenu.Selected', { number: `${checkedAssets.length}/${selectedProjectImportedAssets.length}` })}
							</Tag>
						) : (
							<Tag style={{ marginRight: '4px' }}>
								{t('ToolMenu.Total Assets', { number: selectedProjectImportedAssets.length })}
							</Tag>
						)}
						{selectedProjectImportedAssets.length > 0 ? (
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
										<Button className="option-button">
											<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
											</span>

											<Space>
												{t('Button.Download')}
												<FaAngleDown className="icons" onClick={(e) => e.preventDefault()} style={{ marginTop: 10 }} />
											</Space>
										</Button>
									</Dropdown>
								) : (
									<Button className="option-button" onClick={handleDownloadClick}>
										<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
											<FontAwesomeIcon icon={faFile} size="lg" />
											<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
										</span>
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
								>
									<span className="fa-layers fa-fw" style={{ marginRight: 8 }}>
										<FontAwesomeIcon icon={faFile} size="lg" />
										<FontAwesomeIcon icon={faShareAlt} color="white" transform="shrink-6.4 down-2" />
									</span>
									{t('Button.Share')}
								</Button>
							</Col>
						)}

						{checkedAssetsItem.length > 0 && !isEditingName && (
							<Col>
								<Button className="option-button" onClick={modal().exportBasket}>
									<span className="fa-layers" style={{ marginRight: 8 }}>
										<FontAwesomeIcon icon={faFolder} size="lg" />
										<FontAwesomeIcon icon={faPlusCircle} color="white" transform="shrink-5 down-4 right-2" />
									</span>
									{t('Button.Export To Folder')}
								</Button>
							</Col>
						)}

						{checkedAssetsItem.length > 0 && !isEditingName && (
							<Col>
								<Button className="option-button" onClick={showRemoveConfirm}>
									<span className="fa-layers" style={{ marginRight: 8 }}>
										<FontAwesomeIcon icon={faFile} size="lg" />
										<FontAwesomeIcon icon={faMinus} color="white" transform="shrink-5 down-4 right-2" />
									</span>
									{t('Button.Remove')}
								</Button>
							</Col>
						)}
					</Space>
				</Col>
				<Col align="right">
					<Upload {...clickUploaderProps}>
						<Button type="primary" style={{ width: '100%' }}>
							{t('Button.Upload')}
						</Button>
					</Upload>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectToolsMenu));
