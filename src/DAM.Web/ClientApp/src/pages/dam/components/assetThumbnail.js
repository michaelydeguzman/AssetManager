import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Card, List, Checkbox, Button, Row, Col } from 'antd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEye } from '@fortawesome/free-solid-svg-icons';
import { PushpinOutlined, PushpinFilled, PlusOutlined, CloseOutlined } from '@ant-design/icons';
import { getAssetById, addPinAssets, removePinAssets, replacePinAssets, getPinAssetsDetail } from '../actions';

import PreviewAsset from './previewAsset.js';
import ReplacePinModal from './modals/replacePinModal';

import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import moment from 'moment';

function AssetThumbnail(props) {
	const { t } = useTranslation();

	const {
		item,
		mode,
		onCheckedAssets,
		checkedAssets,
		setVideoPreview,
		setVideoType,
		setModalType,
		setImagePreview,
		setImagePreviewType,
		modal,
		setSelectedAssetID,
		metaDescription,
		actionButtons,
		onClickAction,
		setCheckFile,
		setTouchFile,
		pinAssets,
		setPinAssets,
		title
	} = props;

	const userRole = getUserRole();
	const { approvalFlag, isAdministrator } = useContext(LowFrequencyContext);
	const [pinList, setPinList] = useState([]);
	const [showReplacePinModal, setShowReplacePinModal] = useState(false);
	const [replaceList, setReplaceList] = useState([]);

	useEffect(() => {
		setPinList(pinAssets);
	}, [pinAssets]);

	function approvalStatus(statusName) {
		switch (statusName) {
			case 'Draft':
				return '#8a8a8a';
			case 'Approved':
				return 'green';
			case 'Rejected':
				return 'red';
			default:
				return 'black';
		}
	}
	const onClickCard = async () => {
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

	const renderPinIcon = () => {
		if (userRole.canPinAsset) {
			if (pinList) {
				var pin = pinList.find((p) => p.assetId === item.id);
				if (pin) {
					return (
						<div className="pin-corner-active fade-in" onClick={onClickRemovePin}>
							<PushpinFilled className="pin-corner-icon-active" />
						</div>
					);
				} else {
					return (
						<div className="pin-corner-inactive fade-in" onClick={onClickPin}>
							<PushpinOutlined className="pin-corner-icon-inactive" />
						</div>
					);
				}
			} else {
				return (
					<div className="pin-corner-inactive fade-in" onClick={onClickPin}>
						<PushpinOutlined className="pin-corner-icon-inactive" />
					</div>
				);
			}
		}
	};

	const renderAddThumbnailIcon = (item) => {
		if (item.preview && item.preview.includes('https')) {
			return (
				<div className="add-asset-thumbnail">
					<CloseOutlined className="add-asset-plus-icon" onClick={() => modal().removeThumbnail(item.id)} />
				</div>
			);
		} else {
			return (
				<div className="add-asset-thumbnail">
					<PlusOutlined className="add-asset-plus-icon" onClick={() => modal().addNewThumbnail(item.id)} />
				</div>
			);
		}
	};

	const onClickPin = async () => {
		var data = {
			assetId: item.id
		};
		var result = await props.addPinAssets(data);
		if (result.message) {
			var list = await props.getPinAssetsDetail();
			var replaceList =
				list &&
				list.map((pin) => {
					return {
						key: pin.id,
						id: pin.id,
						title: pin.name
					};
				});
			setReplaceList(replaceList);
			setShowReplacePinModal(true);
		} else {
			setPinList(result.assets);
		}
	};

	const onClickRemovePin = async () => {
		var data = {
			assetId: item.id
		};
		var newPinList = await props.removePinAssets(data);
		setPinList(newPinList);
	};

	const replacePin = async (data) => {
		var replaceResult = await props.replacePinAssets(data);
		setPinAssets(replaceResult.assets);
		setShowReplacePinModal(false);
	};
	function CardView() {
		switch (mode) {
			case 1:
			case 2:
				return (
					<Card
						cover={
							<>
								{isAdministrator && renderAddThumbnailIcon(item)}
								{renderPinIcon()}
								<PreviewAsset item={item} onClickCard={onClickCard} />
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
						<div className="card-body" onClick={onClickCard}>
							<div className="card-details">
								<div className="card-text">{item.name}</div>
								{approvalFlag && (
									<div className="card-approval-status" style={{ color: approvalStatus(item.statusName) }}>
										{t(`Status.${item.statusName}`)}
									</div>
								)}
							</div>
						</div>
					</Card>
				);
			case 3:
				return (
					<Card
						cover={
							<>
								{renderAddThumbnailIcon(item)}
								{renderPinIcon()}
								<PreviewAsset item={item} onClickCard={onClickCard} />
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
						<div className="card-body-container" onClick={onClickCard}>
							<div className="card-body">
								<div className="card-details">
									<div className="card-text title">{item.name}</div>
								</div>
								<div className="card-details">
									<div className="card-text">{`File Size: ${item.fileSizeText}`}</div>
									<div className="card-text">{`${t('ModalDetail.Total Versions')}: ${item.assetVersions.length}`}</div>
								</div>
								<div className="card-details">
									<div className="card-text">{`Expiry Date : ${item.expiryDate || t('ModalDetail.None')}`}</div>
									<div className="card-text">{`Last Updated: ${moment(item.createdDate).format('DD/MM/YYYY')}`}</div>
								</div>
								<div>
									<Button className="option-button" onClick={onClickCard} icon={<FontAwesomeIcon icon={faEye} />}>
										{t('Button.Preview')}
									</Button>
								</div>
							</div>
							<Col>
								{approvalFlag && (
									<div className="card-approval-status" style={{ color: approvalStatus(item.statusName) }}>
										{t(`Status.${item.statusName}`)}
									</div>
								)}
							</Col>
						</div>
					</Card>
				);
			default:
				return (
					<Card
						className="card-title"
						title={title}
						cover={<PreviewAsset item={item} onClick={onClickAction && onClickAction} />}
						actions={actionButtons}
						className="list-item-card fade-in"
					>
						{metaDescription}
					</Card>
				);
		}
	}

	return (
		<>
			<List.Item
				className={'asset-list-container' + (mode == 3 ? ' list-view-card' : mode == 2 ? ' small-thumbnail-view' : '')}
			>
				{CardView()}
			</List.Item>
			<ReplacePinModal
				showReplacePinModal={showReplacePinModal}
				setShowReplacePinModal={setShowReplacePinModal}
				newAssetId={item.id}
				replacePin={replacePin}
				replaceList={replaceList}
			/>
		</>
	);
}
function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getAssetById: (data) => dispatch(getAssetById(data)),
		getPinAssetsDetail: () => dispatch(getPinAssetsDetail()),
		addPinAssets: (data) => dispatch(addPinAssets(data)),
		removePinAssets: (data) => dispatch(removePinAssets(data)),
		replacePinAssets: (data) => dispatch(replacePinAssets(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(AssetThumbnail));
