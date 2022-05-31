import React, { memo, useEffect, useState } from 'react';
import { connect } from 'react-redux';
import { Modal, Form, Row, Col, Input, Select, Button, Tag, Tooltip, Badge, Avatar, List } from 'antd';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import useModal from '../../shared/useModal';
import { getAssetApprovals } from '../../dam/actions';
import { submitApprovals } from '../actions';

import { faThumbsDown, faThumbsUp } from '@fortawesome/free-solid-svg-icons';
import { CustomIcons } from '@damCustomIcons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import MetaDetailsPreview from '../../dam/components/modals/metaDetailsPreview';
import PicDimension from '../../dam/components/modals/picDimension';
import { CheckCircleFilled, CloseCircleFilled } from '@ant-design/icons';
import { useWindowHeight } from '@damhookuseWindowHeight';

function ReviewModal(props) {
	const { t } = useTranslation();
	const height = useWindowHeight() * 0.8;
	const { TextArea } = Input;
	const { modal, isVisible, onLoad, checkedAssets } = props;
	const [form] = Form.useForm();
	const [modalState, modalDispatch] = useModal();
	const [numPages, setNumPages] = useState(null);
	const [comments, setComments] = useState('');
	const [assetApprovals, setAssetApprovals] = useState([]);
	const [currentAsset, setCurrentAsset] = useState();

	useEffect(() => {
		setCurrentAsset(checkedAssets[0]);
	}, [checkedAssets]);

	useEffect(() => {
		if (currentAsset) {
			getApprovals();
			form.setFieldsValue({
				statusUpdatedDate: moment(currentAsset.statusUpdatedDate).format('DD/MM/YYYY HH:mm A'),
				name: currentAsset.name,
				owner: currentAsset.createdByName,
				filesize: currentAsset.fileSizeText,
				extension: currentAsset.extension,
				filename: currentAsset.fileName,
				expirydatereadOnly: currentAsset.expiryDate
					? moment(currentAsset.expiryDate).format('DD/MM/YYYY HH:mm A')
					: null,
				folder_name: currentAsset.folderName ? currentAsset.folderName : '',
				currentversion: currentAsset.assetVersions ? currentAsset.assetVersions.length : ''
			});
		}
	}, [currentAsset]);

	async function getApprovals() {
		let currentVersionId = currentAsset.assetVersions.filter((x) => x.activeVersion === 1)[0].id;
		let applvls = await props.getAssetApprovals(currentAsset.id, currentVersionId);
		setAssetApprovals(applvls);
	}

	function markup(url, fileExtension, fileType) {
		let queryString = '';
		if (fileExtension === 'pdf') queryString = `?file=${encodeURIComponent(url)}`;
		if (fileType.includes('video')) queryString = `video?file=${encodeURIComponent(url)}`;
		if (fileType.includes('image')) queryString = `?file=${encodeURIComponent(url)}`;

		if (fileType.includes('video')) {
			window.open(`http://simplepowerannotate.simplemrm.com.s3-website-ap-southeast-1.amazonaws.com/${queryString}`);
		} else {
			window.open(`https://simplepowerannotate.simplemrm.com/${queryString}`);
		}
	}

	// function onFieldsChange(changedFields) {
	// 	if (changedFields.length > 0) {
	// 		let asset = {};

	// 		if (changedFields[0].name.length === 1 && changedFields[0].name[0] === 'comments') {
	// 			asset = {
	// 				...assetToApprove,
	// 				comments: changedFields[0].value
	// 			};
	// 		}

	// 		setAssetToApprove(asset);
	// 	}
	// }

	function getDueDate(level) {
		if (level.isActiveLevel === true && level.dueDate) {
			return ` ${t('Label.by')} ` + moment(level.dueDate).format('DD/MM/YYYY HH:mm A');
		} else {
			return '';
		}
	}

	function checkStatus(level) {
		if (level.isActiveLevel === true && level.dueDate) {
			if (moment(level.dueDate) > moment()) {
				return <Tag color="green">{t('Label.On-track')}</Tag>;
			} else {
				return <Tag color="orange">{t('Label.Overdue')}</Tag>;
			}
		} else {
			return '';
		}
	}

	async function onApproval(isApproved) {
		//let asset = assetToApprove;

		let dto = checkedAssets.map((asset) => {
			return {
				assetId: asset.id,
				assetVersionId: asset.assetVersions.filter((x) => x.activeVersion === 1)[0].id,
				isApproved: isApproved,
				comment: comments
			};
		});
		await props.submitApprovals(dto, isApproved).then(() => {
			onLoad();
			modal().closeModal();
			modalDispatch({});
			form.resetFields();
			setCurrentAsset();
		});
	}

	function renderApprovers() {
		if (assetApprovals.approvalLevels && assetApprovals.approvalLevels.length > 0) {
			let allApprovers = assetApprovals.approvalLevels.map((approvalLevel) => {
				let approvers = approvalLevel.approvers.map((approver) => {
					switch (approver.approvalStatus) {
						case 1:
							return (
								<Col key={approver.id}>
									<Tooltip
										title={`Approved -- ${approver.approverName}${approver.comments ? ': ' + approver.comments : ''}`}
										placement="top"
									>
										<Badge count={<CheckCircleFilled style={{ color: 'green' }} />} offset={[-5, 5]} size="large">
											<Avatar>{approver.approverName.substr(0, 1)}</Avatar>
										</Badge>
									</Tooltip>
								</Col>
							);
							break;
						case 2:
							return (
								<Col key={approver.id}>
									<Tooltip
										title={`Rejected -- ${approver.approverName}${approver.comments ? ': ' + approver.comments : ''}`}
										placement="top"
									>
										<Badge count={<CloseCircleFilled style={{ color: 'red' }} />} offset={[-5, 5]} size="large">
											<Avatar>{approver.approverName.substr(0, 1)}</Avatar>
										</Badge>
									</Tooltip>
								</Col>
							);
							break;
						default:
							return (
								<Col key={approver.id}>
									<Tooltip title={`Waiting -- ${approver.approverName}`} placement="top">
										<Avatar>{approver.approverName.substr(0, 1)}</Avatar>
									</Tooltip>
								</Col>
							);
							break;
					}
				});
				return (
					<React.Fragment key={approvalLevel.id}>
						<Row style={{ marginLeft: 10, marginBottom: 5 }}>
							<Col xs={24} sm={20} md={20} lg={20} xl={20}>
								{approvalLevel.completedDate ? (
									<h5>{`${t('Label.Level')} ${approvalLevel.levelNumber} - ${t(
										'Label.All approvals completed on'
									)} ${moment(approvalLevel.completedDate).format('DD/MM/YYYY HH:mm A')}`}</h5>
								) : (
									<h5>
										{`${t('Label.Level')} ${approvalLevel.levelNumber} - ${t('Label.All approvals required')}` +
											getDueDate(approvalLevel)}
									</h5>
								)}
							</Col>
							{approvalLevel.isActiveLevel === true && approvalLevel.dueDate && (
								<Col xs={24} sm={4} md={4} lg={4} xl={4} align="right">
									{checkStatus(approvalLevel)}
								</Col>
							)}
						</Row>
						<Row style={{ marginLeft: 15, marginBottom: 10 }}>{approvers}</Row>
					</React.Fragment>
				);
			});
			return <>{allApprovers}</>;
		} else {
			return (
				<Row style={{ marginLeft: 15 }}>
					<h5>{t('ModalDetail.No Approvers')}</h5>
				</Row>
			);
		}
	}

	return (
		<Modal
			visible={isVisible}
			onCancel={() => {
				setComments('');
				form.resetFields();
				modal().closeModal();
			}}
			centered={true}
			width={'85%'}
			footer={false}
			getContainer={false}
			className="approval-modal"
		>
			<Form
				form={form}
				layout="horizontal"
				name="approval"
				scrollToFirstError
				labelAlign="left"
				// onFieldsChange={onFieldsChange}
				className="approval-form"
				labelCol={{
					xs: 24,
					sm: 24,
					md: 10,
					lg: 12,
					xl: 12,
					xxl: 12,
					style: { ...{ lineHeight: 2.2, flexFlow: 'wrap' } }
				}}
				wrapperCol={{
					xs: 24,
					sm: 24,
					md: 8,
					lg: 10,
					xl: 10,
					xxl: 10,
					style: { ...{ lineHeight: 2 } }
				}}
			>
				{isVisible && currentAsset && (
					<Row type="flex" justify="space-between" align="end" style={{ marginBottom: 20 }}>
						<Col xs={24} sm={24} md={24} lg={16} xl={16} xxl={16}>
							<Row style={{ marginBottom: 20 }}>
								<Col xs={24} md={24} lg={24}>
									<div
										style={{
											fontSize: 24,
											fontWeight: 500,
											textOverflow: 'ellipsis',
											whiteSpace: 'nowrap',
											overflowX: 'hidden'
										}}
										title={currentAsset.name}
									>
										{currentAsset.name}
									</div>
								</Col>
							</Row>
							<Row type="flex" justify="space-between" align="start">
								<Col xs={8} md={6} lg={4} style={{ marginLeft: -10 }}>
									<List
										header="Assets List"
										bordered
										size="small"
										split
										itemLayout="horizontal"
										style={{ height: height, overflowY: 'scroll' }}
										dataSource={checkedAssets}
										renderItem={(item) => (
											<List.Item onClick={() => setCurrentAsset(item)} style={{ cursor: 'pointer' }}>
												<List.Item.Meta avatar={<Avatar src={item.thumbnail} />} />
												{item.name}
											</List.Item>
										)}
									/>
								</Col>
								<Col xs={16} md={18} lg={20} className="approve-assets-detail-preview-section" align="center">
									<MetaDetailsPreview assetFile={currentAsset} numPages={numPages} setNumPages={setNumPages} />
								</Col>
							</Row>
							<Row style={{ marginTop: 16 }}>
								<Col span="24" align="center">
									<Button
										className="option-button"
										onClick={(e) => {
											markup(currentAsset.originalUrl, currentAsset.extension, currentAsset.fileType);
										}}
										icon={<FontAwesomeIcon icon={CustomIcons.markup} />}
									>
										Mark Up
									</Button>
								</Col>
							</Row>

							<Row type="flex" justify="space-between" align="end" style={{ marginTop: 16 }}>
								<Col xs={8} md={8}>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item name="filename" label={t('ModalDetail.File Name')}>
												<Input style={{ width: '100%' }} disabled size="small" bordered={false} />
											</Form.Item>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item label={t('ModalDetail.Current Version')} name="currentversion">
												<Input style={{ width: '100%' }} disabled size="small" bordered={false} />
											</Form.Item>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item name="extension" label={t('ModalDetail.File Extension')}>
												<Input disabled style={{ width: '100%' }} size="small" bordered={false} />
											</Form.Item>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item
												name="filesize"
												label={t('ModalDetail.File Size')}
												style={{ display: 'flex', flexFlow: 'wrap' }}
											>
												<Input disabled style={{ width: '100%' }} size="small" bordered={false} />
											</Form.Item>
										</Col>
									</Row>
								</Col>
								<Col xs={8} md={8}>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item name="folder_name" label={t('ModalDetail.Main Folder')}>
												<Select
													className="folder-chip"
													disabled
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													bordered={false}
													size="small"
												></Select>
											</Form.Item>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item
												name="dimensions"
												label={t('ModalDetail.Dimensions')}
												style={{ display: 'flex', flexFlow: 'wrap' }}
											>
												<PicDimension
													url={currentAsset.originalUrl}
													fileType={currentAsset.extension}
													size={'small'}
													bordered={false}
												/>
											</Form.Item>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item label={t('ModalDetail.Expiry')} name="expirydatereadOnly" style={{ width: '100%' }}>
												<Input disabled style={{ width: '100%' }} size="small" bordered={false} />
											</Form.Item>
										</Col>
									</Row>
								</Col>
								<Col xs={8} md={8}>
									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
											<Row>{t('ModalDetail.Tags') + ':'}</Row>
										</Col>
										<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
											<Row>
												{currentAsset.tags.map((row) => (
													<Col key={row.id} className="cognitive-tags-list">
														<Tag
															style={{ borderRadius: 40, marginTop: 5 }}
															closable={false}
															className={row.isCognitive ? 'cognitive-tag' : 'user-tag'}
															visible
															size="small"
														>
															{row.name}
														</Tag>
													</Col>
												))}
											</Row>
										</Col>
									</Row>
								</Col>
							</Row>
						</Col>
						<Col xs={24} sm={24} md={24} lg={1} xl={1} xxl={1}>
							<Col xs={12} md={12} lg={12} className="vertical-divider"></Col>
						</Col>

						<Col xs={24} sm={24} md={24} lg={7} xl={7} xxl={7} className="approve-assets-detail-section">
							<Row justify="start" style={{ marginBottom: 16 }}>
								<Col xs={24} md={24}>
									<div
										style={{
											fontSize: 24,
											fontWeight: 500,
											textOverflow: 'ellipsis',
											whiteSpace: 'nowrap',
											overflowX: 'hidden'
										}}
										title={'File Details'}
									>
										{t('ModalDetail.Approve Asset')}
									</div>
								</Col>
							</Row>

							<Row
								style={{
									padding: '10px 0px'
								}}
							>
								<Col span={12}>
									<Row>{t('ModalDetail.Submission Date') + ':'}</Row>
								</Col>
								<Col span={12}>
									<Row>{moment(currentAsset.statusUpdatedDate).format('DD/MM/YYYY HH:mm A')} </Row>
								</Col>
							</Row>
							<Row style={{ marginBottom: '16px' }}>
								<Col span={12}>
									<Row>{t('ModalDetail.Owner') + ':'} </Row>
								</Col>
								<Col span={12}>
									<Row>{currentAsset.createdByName} </Row>
								</Col>
							</Row>
							<Row
								style={{
									borderTop: '2px solid #c6c8c5',
									padding: '10px 0px'
								}}
							>
								<Col span={24}>
									<Row style={{ marginBottom: '10px' }}>{t('ModalDetail.Approvals')}</Row>
									{renderApprovers()}
								</Col>
							</Row>

							<Row
								type="flex"
								justify="space-between"
								align="middle"
								style={{
									borderTop: '2px solid #c6c8c5',
									padding: '10px 0px'
								}}
							>
								<Col xs={24} md={24}>
									<Row style={{ marginBottom: '10px' }}>
										<span>{t('Label.Enter comments or feedback')}</span>
									</Row>
									<Row type="flex" justify="space-between" align="middle">
										<TextArea
											placeholder={t('ModalDetail.Comments')}
											value={comments}
											onChange={(e) => setComments(e.target.value)}
										/>
									</Row>
								</Col>
							</Row>

							<Row type="flex" justify="space-between" align="middle">
								<Col xs={24} md={24}>
									<Row type="flex" className="form-actions form-update-actions">
										<Button
											type="primary"
											onClick={() => {
												onApproval(true);
											}}
											className={'success'}
											icon={<FontAwesomeIcon icon={faThumbsUp} />}
										>
											{t(`Button.Approve`)}
										</Button>
										<Button
											type="primary"
											onClick={() => {
												onApproval(false);
											}}
											className={'error'}
											icon={<FontAwesomeIcon icon={faThumbsDown} />}
										>
											{t('Button.Reject')}
										</Button>
									</Row>
								</Col>
							</Row>
						</Col>
					</Row>
				)}
			</Form>
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		submitApprovals: (data, isApproved) => dispatch(submitApprovals(data, isApproved)),
		getAssetApprovals: (id, verId) => dispatch(getAssetApprovals(id, verId))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ReviewModal));
