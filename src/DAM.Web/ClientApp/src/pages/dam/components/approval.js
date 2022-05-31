import React, { useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import {
	Row,
	Col,
	Input,
	Select,
	Button,
	Form,
	Spin,
	InputNumber,
	Tag,
	Radio,
	Space,
	Badge,
	Avatar,
	Tooltip,
	Modal,
	List
} from 'antd';
import { getApprovers, createApprovals, getAssetApprovals, getApprovalTemplates, getUserOutOfOffice } from '../actions';
import MetaDetailsPreview from './modals/metaDetailsPreview';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMinus, faPaperPlane, faPlus } from '@fortawesome/free-solid-svg-icons';
import PicDimension from './modals/picDimension';
import { ASSET_STATUS } from '../../constants';
import { CheckCircleFilled, CloseCircleFilled } from '@ant-design/icons';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { useWindowHeight } from '@damhookuseWindowHeight';

function Approval(props) {
	const { t } = useTranslation();
	const height = useWindowHeight() * 0.8;
	const { Option } = Select;
	const [approvalLevels, setApprovalLevels] = useState([]);
	const [approvalTemplates, setApprovalTemplates] = useState([]);
	const [currentTemplateId, setCurrentTemplateId] = useState(null);
	const [approverOptions, setApproverOptions] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const [numPages, setNumPages] = useState(null);
	const [selectedLevelIndex, setSelectedLevelIndex] = useState(0);
	const [selectedApprovalTemplateLevels, setSelectedApprovalTemplateLevels] = useState([]);
	const [isDisabled, setIsDisabled] = useState(true);
	const [tags, setTags] = useState([]);
	const [message, setMessage] = useState('');
	const [prevVersionLevels, setPrevVersionLevels] = useState([]);
	const [mode, setMode] = useState(1);
	const [copyMode, setCopyMode] = useState(1);
	const [assetlistData, setAssetlistData] = useState([]);
	const [currentAsset, setCurrentAsset] = useState();

	const [oooHits, setOOOHits] = useState([]);
	const [showOOOModal, setShowOOOModal] = useState(false);
	const [form] = Form.useForm();

	useEffect(() => {
		let assetsList = [];
		if (props.checkedAssetsItem.length > 1) {
			//assetsList = props.checkedAssetsItem.filter((item) => item.status == ASSET_STATUS.DRAFT);
			assetsList = props.checkedAssetsItem;
			if (assetsList.length === 0) {
				props.close();
			}
		} else {
			assetsList = props.isApprovalFromModal ? [props.findFileState] : props.checkedAssetsItem;
		}
		setAssetlistData(assetsList);
		setCurrentAsset(assetsList[0]);
	}, []);

	useEffect(() => {
		if (currentAsset) {
			setupData(false);
			form.setFieldsValue({
				name: currentAsset.name,
				owner: currentAsset.createdByName,
				filesize: currentAsset.fileSizeText,
				extension: currentAsset.extension,
				filename: currentAsset.fileName,
				expirydatereadOnly: currentAsset.expiryDate
					? moment(currentAsset.expiryDate).format('DD/MM/YYYY HH:mm A')
					: null,
				folder_name: currentAsset.folderId,
				currentversion: currentAsset.assetVersions.length
			});
		}
	}, [currentAsset]);

	useEffect(() => {
		checkIfCanSave();
		checkForApproverOOO();
	}, [selectedApprovalTemplateLevels]);

	async function setupData(refresh) {
		setIsLoading(true);

		setTags(currentAsset.tags);
		let currentVersionId = currentAsset.assetVersions.filter((x) => x.activeVersion === 1)[0].id;
		// setCurrentVersionId(currentVersionId);

		var prevVersions = currentAsset.assetVersions.filter(
			(x) => x.activeVersion === 0 && x.id < currentVersionId && x.status >= ASSET_STATUS.SUBMITTED
		);
		var prevVersionId = 0;

		if (prevVersions.length > 0) {
			var verIds = prevVersions.map((x) => {
				return x.id;
			});
			prevVersionId = Math.max.apply(null, verIds);

			let prevVersionAppLvls = await props.getAssetApprovals(currentAsset.id, prevVersionId, refresh);

			if (props.checkedAssetsItem.length === 1 && prevVersionAppLvls && prevVersionAppLvls.approvalLevels.length > 0) {
				setPrevVersionLevels(prevVersionAppLvls.approvalLevels);
				setMode(2);

				if (prevVersionAppLvls.approvalLevels[prevVersionAppLvls.approvalLevels.length - 1].completedDate) {
					setCopyMode(2);
				}
			}
		}

		var assetFolder = props.folders.filter((f) => f.id === currentAsset.folderId)[0];
		var companyId = assetFolder && assetFolder.company && assetFolder.company[0] ? assetFolder.company[0].id : 0;

		// fetch approvers
		let approverResponse = await props.getApprovers();
		let approversList = [];

		let filterApproversByCompany = approverResponse.data.users.filter((u) => {
			if (u.companyId === companyId || (u.companyId == null && u.userRole.canApprove)) {
				return u;
			}
		});
		approversList = filterApproversByCompany.map((approver) => ({
			value: approver.id,
			label: approver.userName
		}));
		setApproverOptions(approversList);

		// fetch approval templates
		let approvalTemplates = await props.getApprovalTemplates(companyId);
		if (approvalTemplates && approvalTemplates.data.approvalTemplates) {
			let templates = approvalTemplates.data.approvalTemplates;
			templates.push({
				id: 0,
				templateName: 'None',
				deleted: false,
				companyId: null,
				approverTemplateLevels: []
			});
			templates.sort(function (a, b) {
				if (a.id !== b.id) {
					return a.id - b.id;
				}
			});
			setApprovalTemplates(templates);
		}

		setIsLoading(false);
	}

	function renderApprovers() {
		if (prevVersionLevels && prevVersionLevels.length > 0) {
			let allApprovers = prevVersionLevels.map((approvalLevel) => {
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
							<Col span={12}>{`${t('Label.Level')} ${approvalLevel.levelNumber}`}</Col>
							{approvalLevel.duration && (
								<Col span={12}>
									<h5>{`${t('Label.Duration')}: ${approvalLevel.duration} ${t('Label.Business Days')}`}</h5>
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

	function renderExistingPreviousApproval() {
		if (prevVersionLevels.length > 0) {
			return (
				<Row justify="start" style={{ marginBottom: 16 }}>
					<Col xs={24} md={24}>
						<Radio.Group
							onChange={(e) => setCopyMode(e.target.value)}
							value={copyMode}
							style={{ marginTop: 16, width: '100%' }}
						>
							{!prevVersionLevels[prevVersionLevels.length - 1].completedDate && (
								<Radio.Button value={1}>{t('Label.Continue Approval')}</Radio.Button>
							)}

							<Radio.Button value={2}>{t('Label.Restart Approval')}</Radio.Button>
						</Radio.Group>
					</Col>
				</Row>
			);
		} else {
			return <></>;
		}
	}

	const saveAndSendApproval = (overrideOOOCheck = false) => {
		let approvalLevels = [];
		let currentVersion = currentAsset.assetVersions.filter((x) => x.activeVersion === 1)[0];
		let delegateIfOOO = false;

		if (mode === 1) {
			// new
			assetlistData.forEach((asset) => {
				selectedApprovalTemplateLevels.forEach((level, index) => {
					let approvers = level.approvers.map((a) => {
						return {
							approverId: a.approverId
						};
					});
					let order = index + 1;
					let appLvl = {
						assetId: asset.id,
						LevelNumber: order,
						isActiveLevel: order == 1 ? true : false,
						approvers: approvers,
						duration: level.duration,
						assetVersionId: asset.assetVersions.filter((x) => x.activeVersion === 1)[0].id
					};
					approvalLevels.push(appLvl);
				});
			});
		} else {
			// copy
			prevVersionLevels.forEach((level, index) => {
				let approvers = level.approvers.map((a) => {
					if (copyMode === 1 && a.approvalStatus !== 2) {
						return {
							approverId: a.approverId,
							approvalStatus: a.approvalStatus,
							reviewDate: a.reviewDate,
							comments: a.comments
						};
					} else {
						return {
							approverId: a.approverId
						};
					}
				});

				let order = index + 1;

				let appLvl = {
					assetId: currentAsset.id,
					LevelNumber: order,
					isActiveLevel: copyMode === 1 ? level.isActiveLevel : order == 1 ? true : false,
					approvers: approvers,
					duration: level.duration,
					assetVersionId: currentVersion.id,
					completedDate: copyMode === 1 ? level.completedDate : null,
					dueDate: copyMode === 1 ? (level.completedDate ? level.dueDate : null) : null
				};
				approvalLevels.push(appLvl);
			});
			delegateIfOOO = true;
		}

		let dto = {
			assetId: currentAsset.id,
			assetVersionId: currentVersion.id,
			approvalLevels: approvalLevels,
			isSubmitted: true,
			delegateIfOOO: delegateIfOOO
		};
		props.createApprovals(dto).then(() => {
			setupData(true);
			props.modalDispatch({});
		});
	};

	const handleCancelClick = () => {
		if (props.isApprovalFromModal) {
			props.modalDispatch({
				type: 'BOTH',
				payload: 'edit-details'
			});
		} else {
			props.close();
		}
	};

	const handleTemplateSelect = (e) => {
		if (e > 0) {
			let filteredTemplates = approvalTemplates.filter((t) => t.id === e);
			if (filteredTemplates.length > 0) {
				setCurrentTemplateId(e);
				setSelectedApprovalTemplateLevels(filteredTemplates[0].approvalTemplateLevels);
			}
		} else {
			setSelectedApprovalTemplateLevels([]);
			setCurrentTemplateId(null);
		}
	};

	const handleAddNewLevelClick = () => {
		let levelCount = selectedApprovalTemplateLevels.length;
		setSelectedLevelIndex(levelCount);

		let currApprovalLevels = [...selectedApprovalTemplateLevels];

		var newLevel = {
			id: null,
			levelOrderNumber: levelCount + 1,
			approvers: [],
			duration: null
		};
		currApprovalLevels.push(newLevel);
		setSelectedApprovalTemplateLevels(currApprovalLevels);
	};

	const handleRemoveLevelClick = () => {
		let lastLevel = [...selectedApprovalTemplateLevels];

		setSelectedLevelIndex(lastLevel.length);
		lastLevel.pop();
		setSelectedApprovalTemplateLevels(lastLevel);
	};

	function selectApproverOptions(level) {
		return approverOptions.map((user, index) => {
			return (
				<Select.Option value={user.value} key={index} level={level}>
					{user.label}
				</Select.Option>
			);
		});
	}

	const addDuration = (e, index) => {
		var duration = e;
		if (duration === 0) {
			duration = null;
		}

		let levelupdate = [...selectedApprovalTemplateLevels];
		var approvalLevelToModify = levelupdate[index];
		approvalLevelToModify.duration = duration;

		setSelectedApprovalTemplateLevels(levelupdate);
	};

	const mapLevelApprovers = (approvers, index) => {
		let selectedApproversInLevel = [];
		if (approvers) {
			selectedApproversInLevel = approvers.map((user) => {
				return user.approverId;
			});
		}
		return selectedApproversInLevel;
	};

	function selectOptionsType(type = []) {
		return type.map((row) => {
			return (
				<Select.Option value={row.id} key={row.id}>
					{row.name ? row.name : row.folderName ? row.folderName : row.description}
				</Select.Option>
			);
		});
	}

	const handleApproverChanges = (item, options) => {
		var index = selectedLevelIndex;
		var updateApprovalLevel = [...selectedApprovalTemplateLevels];

		var approversToModify = [];

		options.forEach((option) => {
			var newApprover = {
				approverId: option.value
			};
			approversToModify.push(newApprover);
		});

		var approvalLevelToModify = selectedApprovalTemplateLevels[index];
		approvalLevelToModify.approvers = approversToModify;

		setSelectedApprovalTemplateLevels(updateApprovalLevel);
	};

	function checkIfCanSave() {
		var disableFl = true;
		var levelCount = selectedApprovalTemplateLevels.length;
		var hasApprovers = selectedApprovalTemplateLevels.filter((x) => x.approvers.length > 0).length;
		if (levelCount > 0 && levelCount === hasApprovers) {
			disableFl = false;
			setMessage('');
		} else if (levelCount > 0 && levelCount != hasApprovers) {
			setMessage(t('Messages.Need Approval Levels'));
		} else {
			setMessage(t('Messages.Add Approval Levels'));
		}
		setIsDisabled(disableFl);
	}

	const checkForApproverOOO = () => {
		let updateDelegationWarning = [...oooHits];
		updateDelegationWarning = [];
		if (selectedApprovalTemplateLevels.length > 0) {
			selectedApprovalTemplateLevels.forEach((level) => {
				if (level.levelOrderNumber === 1 && selectedLevelIndex === level.levelOrderNumber - 1) {
					let approvers = level.approvers;
					let dueDate = level.duration ? addBusinessDays(moment(), level.duration) : 0;

					if (approvers.length > 0 && dueDate > 0) {
						approvers.forEach(async (approver) => {
							var result = await props.getUserOutOfOffice(approver.approverId);

							if (result && result.data.userOOO) {
								var userOOOs = result.data.userOOO;
								userOOOs.forEach((ooo) => {
									//check if user has ooo on following dates
									if (dueDate >= moment(ooo.startDate) && dueDate <= moment(ooo.endDate)) {
										updateDelegationWarning.push(ooo);
										setShowOOOModal(true);
									}
								});
							}
							setOOOHits(updateDelegationWarning);
						});
					} else {
						setOOOHits([]);
					}
				}
			});
		}
	};

	const closeOOOModal = () => {
		setShowOOOModal(false);
	};

	const delegateApproversField = () => {
		let ooos = oooHits;
		let updateApprovalLevel = [...selectedApprovalTemplateLevels];
		let approvalLevelToModify = updateApprovalLevel[0];

		ooos.forEach((ooo) => {
			if (ooo.defaultDelegateUser && ooo.defaultDelegateUser.length > 0) {
				let approversToModifyRemovedOOO = approvalLevelToModify.approvers.filter((a) => a.approverId !== ooo.userId);
				if (approversToModifyRemovedOOO.filter((u) => u.approverId === ooo.defaultDelegateUser).length === 0) {
					var newApprover = {
						approverId: ooo.defaultDelegateUser
					};
					approversToModifyRemovedOOO.push(newApprover);
				}
				approvalLevelToModify.approvers = approversToModifyRemovedOOO;
			}
		});

		setSelectedApprovalTemplateLevels(updateApprovalLevel);
		setShowOOOModal(false);
	};

	function addBusinessDays(date, days) {
		let daysToAdd = days;
		const today = moment(date);
		const nextWeekStart = today.clone().add(1, 'week').weekday(1);
		const weekEnd = today.clone().weekday(5);

		const daysTillWeekEnd = Math.max(0, weekEnd.diff(today, 'days'));
		if (daysTillWeekEnd >= daysToAdd) return today.clone().add(daysToAdd, 'days');

		daysToAdd = daysToAdd - daysTillWeekEnd - 1;

		return nextWeekStart.add(Math.floor(daysToAdd / 5), 'week').add(daysToAdd % 5, 'days');
	}

	return (
		<Spin spinning={isLoading} size="large">
			<Form
				form={form}
				className={`dam-form`}
				labelCol={{
					xs: 24,
					sm: 24,
					md: 24,
					lg: 24,
					xl: 24,
					xxl: 12,
					style: { ...{ lineHeight: 2.2, flexFlow: 'wrap' } }
				}}
				wrapperCol={{
					xs: 24,
					sm: 24,
					md: 24,
					lg: 24,
					xl: 24,
					xxl: 10,
					style: { ...{ lineHeight: 2 } }
				}}
			>
				{currentAsset && (
					<Row type="flex" justify="space-between" align="start">
						<Col xs={24} sm={24} md={24} lg={16} xl={16} xxl={16}>
							<Row style={{ marginBottom: 20 }}>
								<Col xs={1} md={1} lg={1}>
									<FontAwesomeIcon
										size="2x"
										icon={faChevronLeft}
										onClick={handleCancelClick}
										style={{ marginTop: 8 }}
									/>
								</Col>
								<Col xs={23} md={23} lg={23}>
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
										dataSource={assetlistData}
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
											<Form.Item name="folder_name" label={t('ModalDetail.Main Folder')} initialValue={['Root']}>
												<Select
													className="folder-chip"
													disabled
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													bordered={false}
													size="small"
												>
													{selectOptionsType(props.folders)}
												</Select>
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
											<Form.Item name="owner" label={t('ModalDetail.Owner')}>
												<Input placeholder={t('ModalDetail.Owner')} disabled size="small" bordered={false} />
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
												{tags.map((row) => (
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
						<Col xs={1} md={1} lg={1} xl={1} xxl={1}>
							<Col xs={12} md={12} lg={12} xl={12} xxl={12} className="vertical-divider"></Col>
						</Col>
						<Col xs={24} sm={24} md={24} lg={7} xl={7} xxl={7} className="approve-assets-detail-section">
							<Row justify="start" style={{ marginBottom: 16 }}>
								<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
									<div
										style={{
											fontSize: 24,
											fontWeight: 500,
											textOverflow: 'ellipsis',
											whiteSpace: 'nowrap',
											overflowX: 'hidden'
										}}
										title={t('ModalDetail.Submit for Review')}
									>
										{t('ModalDetail.Submit for Review')}
									</div>
								</Col>
							</Row>

							{approvalLevels && approvalLevels.length > 0 ? (
								approvalLevels.map((approvalLevel) => {
									let approvers = approvalLevel.approvers.map((approver) => {
										const textColor = {
											color:
												approver.approvalStatus === 0
													? '#d99a26'
													: approver.approvalStatus === 1
													? '#1d631d'
													: '#cc3333'
										};
										return (
											<>
												<h4 style={textColor}>{approver.approverName}</h4>
												<h5>
													{approver.comments} - {moment(approver.reviewDate).format('DD/MM/YYYY HH:mm A')}
												</h5>
											</>
										);
									});

									return (
										<>
											<h2>{t('Label.Approval Level') + ' ' + approvalLevel.levelNumber}</h2>
											{approvers}
										</>
									);
								})
							) : (
								<>
									<Row justify="start" style={{ marginBottom: 16, marginTop: 32 }}>
										<Col xs={24} md={24}>
											<Radio.Group
												onChange={(e) => {
													setMode(e.target.value);
													setSelectedApprovalTemplateLevels([]);
													setCurrentTemplateId(null);
													setOOOHits([]);
												}}
												value={mode}
											>
												{prevVersionLevels.length > 0 && (
													<Radio value={2}>{t('Label.Copy from Previous Version')}</Radio>
												)}
												<Radio value={1}>{t('Label.Create New')}</Radio>
											</Radio.Group>
										</Col>
									</Row>

									<Row
										style={{
											borderTop: '2px solid #c6c8c5',
											padding: '10px 0px'
										}}
									></Row>

									{mode === 1 ? (
										<>
											<Row justify="start" style={{ marginBottom: 16 }}>
												<Col xs={12} sm={12} md={12} lg={24} xl={24} xxl={12}>
													<span className="approval-label">{t('Label.Approval Template (Optional)')}:</span>
												</Col>
												<Col xs={12} sm={12} md={12} lg={24} xl={24} xxl={12}>
													<Select
														label="Approval Template"
														className="approval-template-select"
														placeholder={t('Messages.Select')}
														value={currentTemplateId}
														onSelect={handleTemplateSelect}
													>
														{approvalTemplates.map((item) => (
															<Option key={item.id} value={item.id}>
																{item.templateName}
															</Option>
														))}
													</Select>
												</Col>
											</Row>
											<Row justify="start" style={{ marginBottom: 16 }}>
												<Col xs={24} md={24}>
													<Form.Item name="owner" label={t('Label.Approval Levels')}>
														<Row gutter={8}>
															<Col xs={12} md={12} justify="start">
																<Button
																	type="dashed"
																	className="link-button level-btn"
																	onClick={handleAddNewLevelClick}
																	size="small"
																	icon={<FontAwesomeIcon icon={faPlus} size="small" />}
																>
																	{t('Button.Add')}
																</Button>
															</Col>
															<Col xs={12} md={12} justify="end">
																{selectedApprovalTemplateLevels.length > 0 && (
																	<Button
																		type="dashed"
																		className="link-button level-btn"
																		onClick={handleRemoveLevelClick}
																		size="small"
																		icon={<FontAwesomeIcon icon={faMinus} size="small" />}
																	>
																		{t('Button.Remove')}
																	</Button>
																)}
															</Col>
														</Row>
													</Form.Item>
												</Col>
											</Row>
										</>
									) : (
										<>
											{renderApprovers()}
											{renderExistingPreviousApproval()}
										</>
									)}

									{selectedApprovalTemplateLevels.map((level, index) => (
										<>
											<Row
												style={{
													borderTop: '2px solid #c6c8c5',
													padding: '10px 0px'
												}}
											></Row>

											<Row justify="start" style={{ marginBottom: 16 }}>
												<Col xs={12} sm={12} md={12} lg={24} xl={12} xxl={12}>
													<label className="approver-level-header approval-label">
														{t('Label.Level') + ' ' + (index + 1) + ':'}
													</label>
												</Col>
												<Col xs={12} sm={12} md={12} lg={24} xl={12} xxl={12}>
													<Select
														className="approval-template-select level"
														key={'select' + (index + 1)}
														mode="multiple"
														size="small"
														placeholder={t('Messages.Select')}
														dropdownStyle={{ position: 'fixed' }}
														value={mapLevelApprovers(level.approvers, index)}
														onMouseEnter={() => setSelectedLevelIndex(index)}
														onChange={handleApproverChanges}
													>
														{selectApproverOptions(index)}
													</Select>
												</Col>
											</Row>
											<Row justify="start" style={{ marginBottom: 16 }}>
												<Col xs={12} md={12}>
													<div className="approval-label-large">
														<span>{t('Messages.Duration Message')}</span>
													</div>
												</Col>
												<Col xs={12} md={12}>
													<InputNumber
														min={0}
														defaultValue={0}
														onChange={(e) => addDuration(e, index)}
														size="small"
														onMouseEnter={() => setSelectedLevelIndex(index)}
													/>
													<span className="approval-label-small"> {t('Label.Business Days')}</span>
												</Col>
											</Row>
										</>
									))}

									{oooHits.length > 0 && mode === 1 && (
										<Row justify="start" style={{ marginBottom: 16, marginTop: 40 }}>
											<Col xs={24} md={24} align="left">
												<h5>{t('Messages.OOO Warning')}</h5>
												<ul>
													{oooHits.map((ooo) => {
														return (
															<li>
																<h5>
																	{approverOptions.filter((u) => u.value === ooo.userId)[0].label} {t('Label.From')}{' '}
																	{moment(ooo.startDate).format('DD/MM/YYYY')} {t('Label.To')}{' '}
																	{moment(ooo.endDate).format('DD/MM/YYYY')}
																</h5>
															</li>
														);
													})}
												</ul>
											</Col>
										</Row>
									)}

									<Row justify="start" style={{ marginBottom: 16, marginTop: 32 }}>
										<Col xs={12} md={12} align="left">
											<Button
												type="primary"
												htmlType="submit"
												disabled={isDisabled && mode === 1}
												//className="save-approval-level-btn"
												onClick={saveAndSendApproval}
												icon={<FontAwesomeIcon icon={faPaperPlane} />}
											>
												{t('Button.Submit Levels')}
											</Button>
										</Col>
										{message.length > 0 && mode === 1 && (
											<Col xs={12} md={12} align="left">
												<div className="approval-message-large">
													<span style={{ fontColor: 'red' }}> {message}</span>
												</div>
											</Col>
										)}
									</Row>
								</>
							)}
						</Col>
					</Row>
				)}
			</Form>

			<Modal
				title={t('Messages.OOO Modal Warning')}
				visible={showOOOModal}
				centered={true}
				width={'500px'}
				footer={false}
				getContainer={false}
				closable={false}
				keyboard={false}
				destroyOnClose
			>
				<Row>
					<h4>{t('Messages.OOO Warning 2')}</h4>
					<ul>
						{oooHits.map((ooo) => {
							return (
								<li>
									<span>
										{approverOptions.filter((u) => u.value === ooo.userId)[0].label} {t('Label.From')}{' '}
										{moment(ooo.startDate).format('DD/MM/YYYY')} {t('Label.To')}{' '}
										{moment(ooo.endDate).format('DD/MM/YYYY')}
									</span>
								</li>
							);
						})}
					</ul>
				</Row>
				<br />
				<Row>
					<Col xs={24} sm={24} md={24} lg={8} xl={8} xxl={8} align="left">
						<Button type="secondary" onClick={closeOOOModal}>
							{t('Button.Assign Anyway')}
						</Button>
					</Col>
					<Col xs={24} sm={24} md={24} lg={16} xl={16} xxl={16} align="right">
						<Space>
							<Button type="primary" onClick={delegateApproversField}>
								{t('Button.Delegate to Set Users')}
							</Button>
						</Space>
					</Col>
				</Row>
			</Modal>
		</Spin>
	);
}
function mapStateToProps(state) {
	return {
		assets: state.dam.assets
	};
}

function mapDispatchToProps(dispatch) {
	return {
		getApprovers: () => dispatch(getApprovers()),
		createApprovals: (data) => dispatch(createApprovals(data)),
		getAssetApprovals: (id, prevVerId, refresh) => dispatch(getAssetApprovals(id, prevVerId, refresh)),
		getApprovalTemplates: (id) => dispatch(getApprovalTemplates(id)),
		getUserOutOfOffice: (id) => dispatch(getUserOutOfOffice(id))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Approval));
