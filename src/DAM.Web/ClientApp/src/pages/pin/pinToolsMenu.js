import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Menu, Dropdown } from 'antd';
import { FullscreenOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
	faPen,
	faShareAlt,
	faDownload,
	faArrowRight,
	faFile,
	faFileArchive,
	faCheckCircle
} from '@fortawesome/free-solid-svg-icons';
import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { ASSET_STATUS } from '../constants';
import { useTranslation } from 'react-i18next';
import { FaAngleDown } from 'react-icons/fa';

function PinToolsMenu(props) {
	const { t } = useTranslation();
	const {
		modal,
		checkedAssetsItem,
		setShareModalState,
		setSelectedAssetID,
		setSelectedShareAssets,
		setIsDownloadModalOpen,
		savePinOrder,
		assetsCount,
		hasWatermark,
		setDownloadExt
	} = props;
	const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
	const userRole = getUserRole();
	const { confirm } = Modal;

	function showConfirm() {
		confirm({
			title: t('Leave Order Assets.Title'),
			content: t('Leave Order Assets.Body'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				setIsOrdering(!isOrdering);
			},
			onCancel() {}
		});
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

	return (
		<Row>
			<Col span={22}>
				<Space className="toolMenu">
					{userRole.canPinAsset && (
						<Col>
							<Button
								className="option-button"
								onClick={() => {
									if (isOrdering) {
										showConfirm();
									} else {
										if (assetsCount > 0) setIsOrdering(!isOrdering);
									}
								}}
							>
								<FullscreenOutlined rotate={45} style={{ marginRight: '8px' }} />
								{t('Button.Order')}
							</Button>
						</Col>
					)}
					{/* {userRole.canEdit && checkedAssetsItem.length === 1 && (
                    <Col>
                        <Button className="option-button" onClick={rename}>
                            <span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
                                <FontAwesomeIcon icon={faFile} size="lg" />
                                <FontAwesomeIcon icon={faPen} color="white" transform="shrink-6.4 down-2" />
                            </span>
                            {t('Button.Rename')}
                        </Button>
                    </Col>
                )} */}
					{checkedAssetsItem.length > 0 && userRole.canArchive && !isOrdering && (
						<Col>
							<Button className="option-button" onClick={modal().archive}>
								<FontAwesomeIcon icon={faFileArchive} size="lg" style={{ marginRight: '8px' }} />
								{t('Button.Archive')}
							</Button>
						</Col>
					)}
					{/* {userRole.canMove && (
                    <Col>
                        <Button className="option-button" onClick={modal().move}>
                            <span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
                                <FontAwesomeIcon icon={faFile} size="lg" />
                                <FontAwesomeIcon icon={faArrowRight} color="white" transform="shrink-6.4 down-2" />
                            </span>
                            {t('Button.Move')}
                        </Button>
                    </Col>
                )} */}
					{checkedAssetsItem.length > 0 && !isOrdering && (
						<Col>
							{checkedAssetsItem.length === 1 &&
							(checkedAssetsItem[0].fileType.includes('image') ||
								checkedAssetsItem[0].extension.includes('pdf') ||
								checkedAssetsItem[0].extension.includes('doc') ||
								checkedAssetsItem[0].extension.includes('xls') ||
								checkedAssetsItem[0].extension.includes('ppt')) ? (
								<Dropdown overlay={downloadMenu} placement="bottomRight">
									<Button className="option-button" className="ant-dropdown-link">
										<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
											<FontAwesomeIcon icon={faFile} size="lg" />
											<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
										</span>

										<Space>
											{t('Button.Download')}
											<FaAngleDown className="icons" onClick={(e) => e.preventDefault()} style={{ marginTop: 5 }} />
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
					{checkedAssetsItem.length > 0 && userRole.canShare && !isOrdering && (
						<Col>
							<Button
								className="option-button"
								onClick={() => {
									setSelectedAssetID(checkedAssetsItem[0].id);
									setSelectedShareAssets(checkedAssetsItem);
									setShareModalState(true);
								}}
							>
								<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
									<FontAwesomeIcon icon={faFile} size="lg" />
									<FontAwesomeIcon icon={faShareAlt} color="white" transform="shrink-6.4 down-2" />
								</span>
								{t('Button.Share')}
							</Button>
						</Col>
					)}
					{checkedAssetsItem.length >= 1 && userRole.canApprove && approvalFlag && !isOrdering && (
						<Col>
							<Button className="option-button" onClick={() => modal().approval(checkedAssetsItem[0])}>
								<span className="fa-layers" style={{ marginRight: '8px' }}>
									<FontAwesomeIcon icon={faFile} size="lg" />
									<FontAwesomeIcon icon={faCheckCircle} color="white" transform="shrink-5 down-4 right-2" />
								</span>
								{t('Button.Send for Approval')}
							</Button>
						</Col>
					)}
				</Space>
			</Col>
			<Col span={2}>
				{userRole.canPinAsset && isOrdering && (
					<Button type="primary fade-in" style={{ width: '7vw' }} onClick={savePinOrder}>
						{t('Button.Save')}
					</Button>
				)}
			</Col>
		</Row>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(PinToolsMenu));
