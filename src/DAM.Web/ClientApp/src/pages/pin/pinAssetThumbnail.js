import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Card, List, Checkbox, Button } from 'antd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEye } from '@fortawesome/free-solid-svg-icons';
import { PushpinOutlined, FullscreenOutlined, PushpinFilled } from '@ant-design/icons';
import { addPinAssets, removePinAssets, replacePinAssets, getPinAssetsDetail } from '../dam/actions';

import PreviewAsset from '../dam/components/previewAsset.js';
import ReplacePinModal from '../dam/components/modals/replacePinModal';

import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';

function PinAssetThumbnail(props) {
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
		isOrdering,
		listOnLoad
	} = props;

	const userRole = getUserRole();
	const { approvalFlag } = useContext(LowFrequencyContext);
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
		listOnLoad();
	};

	const replacePin = async (data) => {
		var replaceResult = await props.replacePinAssets(data);
		setPinAssets(replaceResult.assets);
		setShowReplacePinModal(false);
	};

	return (
		<>
			<List.Item className={'asset-list-container' + (isOrdering ? ' pin-ordering-container' : '')}>
				{mode != 0 ? (
					<>
						{isOrdering && (
							<div className="pin-order-hover-box">
								<FullscreenOutlined rotate={45} style={{ color: 'white', fontSize: '4em', opacity: '1' }} />
							</div>
						)}
						<Card
							cover={
								<>
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
								<div>
									<Button className="option-button" onClick={onClickCard}>
										<FontAwesomeIcon icon={faEye} /> {t('Button.Preview')}
									</Button>
								</div>
							</div>
						</Card>
					</>
				) : (
					<Card
						hoverable
						className="card-title"
						title={item.name}
						onClick={onClickAction && onClickAction}
						cover={<PreviewAsset item={item} />}
						actions={actionButtons}
						className="list-item-card fade-in"
					>
						{metaDescription}
					</Card>
				)}
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
		getPinAssetsDetail: () => dispatch(getPinAssetsDetail()),
		addPinAssets: (data) => dispatch(addPinAssets(data)),
		removePinAssets: (data) => dispatch(removePinAssets(data)),
		replacePinAssets: (data) => dispatch(replacePinAssets(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(PinAssetThumbnail));
