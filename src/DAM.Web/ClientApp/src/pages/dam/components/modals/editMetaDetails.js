import React, { memo, useState, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import {
	Row,
	Col,
	Button,
	Form,
	Modal,
	Upload,
	Spin,
	Input,
	Select,
	DatePicker,
	notification,
	Tag,
	Tooltip,
	Switch,
	TreeSelect,
	Badge,
	Timeline,
	Dropdown,
	Menu,
	Space
} from 'antd';
import {
	FileDoneOutlined,
	ShareAltOutlined,
	DownloadOutlined,
	RadiusUprightOutlined,
	InboxOutlined,
	UploadOutlined,
	StockOutlined,
	FundOutlined,
	CheckCircleFilled,
	CloseCircleFilled,
	FileWordOutlined,
	PlusOutlined
} from '@ant-design/icons';
import Avatar from 'antd/lib/avatar/avatar';
import PicDimension from './picDimension';
import { ASSET_STATUS } from '../../../constants';
import MetaDetailsPreview from './metaDetailsPreview';
import {
	getAssetById,
	getAssetApprovals,
	authenticateVideoForIndexing,
	uploadVideoForIndexing,
	getIndexingState,
	getFilesVersion,
	revertAssetVersion,
	updateAsset,
	uploadAssetVersion,
	downloadAssetPackage
} from '../../actions';

import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { NestedChild, getSelectedCountriesRegions, Compare, Uint8ToBase64 } from '@damhelper';
import moment from 'moment';
import { useTranslation } from 'react-i18next';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleDown, faChevronLeft, faChevronRight, faPlus } from '@fortawesome/free-solid-svg-icons';
import { CustomIcons } from '@damCustomIcons';
import DownloadModal from './downloadModal';
import Cropper from '../../../../utilities/cropper';

function EditMetaDetails(props) {
	const { t } = useTranslation();
	const {
		modal,
		modalState,
		findFileState,
		setFindFileState,
		numPages,
		setNumPages,
		setShareModalState,
		isEditing,
		setIsEditing,
		efolders,
		setSelectedShareAssets,
		listOnLoad,
		dataSource,
		hasWatermark,
		setCroppedImage,
		user
	} = props;

	const {
		allCountries,
		allAccounts,
		approvalFlag,
		assetVersionFlag,
		videoIndexerFlag,
		promoteMRMFlag,
		assetContainer,
		assetPreviewContainer,
		isAdministrator
	} = useContext(LowFrequencyContext);

	const [tagInputVisible, setIsTagInputVisible] = useState(false);
	const [tagInputValue, setTagInputValue] = useState('');
	const [currentVersion, setCurrentVersion] = useState(0);
	const [activeVersion, setActiveVersion] = useState(0);
	const [assetDetail, setAssetDetail] = useState();
	const [regionOptions, setRegionOptions] = useState([]);
	const [assetApprovals, setAssetApprovals] = useState([]);
	const [tags, setTags] = useState([]);
	const [isUpdating, setIsUpdating] = useState(false);
	const [expiryDate, setExpiryDate] = useState();
	const [assetTimeLine, setAssetTimeLine] = useState([]);
	const [allApprovals, setAllApprovals] = useState([]);
	const [allAssets, setAssets] = useState(dataSource());
	const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
	const [isCropping, setIsCropping] = useState(false);
	const [cropResult, setCropResult] = useState(undefined);
	const [textAnalytics, setTextAnalytics] = useState(undefined);

	const [form] = Form.useForm();
	const userRole = getUserRole();
	const perCol = {
		marginRight: 5
	};

	const { BlobServiceClient } = require('@azure/storage-blob');
	const accountName = 'damblob1';
	const sasToken =
		'sv=2019-12-12&ss=b&srt=sco&sp=rwdx&se=2025-02-17T03:11:27Z&st=2021-02-16T19:11:27Z&spr=https,http&sig=mOZOdHWx3EmPz%2F39JAF%2BnA1kZIOLVtxNjzxnsRnYvtw%3D';
	const destContainerName = 'videoindexercontainer';
	const sourceContainerName = assetContainer;
	const clickUploaderVersionProps = {
		name: 'file',
		multiple: false,
		showUploadList: false,
		customRequest: handleUploadVersion
	};
	const [downloadExt, setDownloadExt] = useState('');

	useEffect(() => {
		onLoad();
	}, [findFileState.id]);

	useEffect(() => {
		if (assetDetail) {
			var sharefolders = [];
			if (assetDetail.shareFolderIds && assetDetail.shareFolderIds.length > 0) {
				const folders = assetDetail.shareFolderIds.split(',');
				folders.map((c) => {
					sharefolders.push(Number(c));
				});
			}

			var regions = getSelectedCountriesRegions(allCountries, assetDetail.countries);
			setRegionOptions(regions);
			setTags(assetDetail.tags);
			const activeVersionId = assetDetail.assetVersions.filter((x) => x.activeVersion === 1)[0].id;
			setActiveVersion(activeVersionId);
			form.setFieldsValue({
				filename: assetDetail.fileName,
				name: assetDetail.name,
				accounts: assetDetail.accounts.map((row) => row.id),
				country: [...new Set(assetDetail.countries.map((row) => row.id))],
				region: assetDetail.regions.map((row) => row.id),
				folder_name: [assetDetail.folderId],
				projectId: assetDetail.projectId,
				comments: assetDetail.comments,
				extension: assetDetail.extension,
				filesize: assetDetail.fileSizeText,
				uploadedby: assetDetail.createdByName,
				uploaddate: moment(assetDetail.createdDate).format('DD/MM/YYYY HH:mm A'),
				expirydate: assetDetail.expiryDate ? moment(assetDetail.expiryDate) : null,
				expirydatereadOnly: assetDetail.expiryDate ? moment(assetDetail.expiryDate).format('DD/MM/YYYY HH:mm A') : null,
				downloadcount: assetDetail.downloadCount,
				statusname: t(`Status.${assetDetail.statusName}`),
				assetVersions: assetDetail.assetVersions,
				share_folders: sharefolders,
				totalversions: assetDetail.assetVersions.length,
				currentVersions: currentVersion > 0 ? currentVersion : activeVersionId
			});
		}
	}, [assetDetail]);

	useEffect(() => {
		if (currentVersion > 0) {
			if (allApprovals && allApprovals.length > 0) {
				let activeVersionApprovals = [];
				allApprovals.map((a) => {
					if (a.assetVersionId == currentVersion) {
						activeVersionApprovals.push(a);
					}
				});
				console.log(activeVersionApprovals);
				setAssetApprovals([...activeVersionApprovals]);
			}
		}
	}, [currentVersion]);

	async function onLoad(refresh = false) {
		var asset = await props.getAssetById(findFileState.id);
		setAssetDetail(asset[0]);
		renderTextAnalytics(asset[0].key, asset[0].extension);

		let applvls = await props.getAssetApprovals(findFileState.id, 0, refresh);
		if (applvls.approvalLevels && applvls.approvalLevels.length > 0) {
			setAllApprovals(applvls.approvalLevels);
			const activeVersionId = asset[0].assetVersions.filter((x) => x.activeVersion === 1)[0].id;
			let activeVersionApprovals = [];
			applvls.approvalLevels.map((a) => {
				if (a.assetVersionId == activeVersionId) {
					activeVersionApprovals.push(a);
				}
			});
			setAssetApprovals([...activeVersionApprovals]);
		}
		//set the timeline of asset's lifecycle
		let assetTimeLineList = [];
		if (asset[0].assetVersions) {
			const assetCreatedItem = {
				label: asset[0].createdDate,
				user: asset[0].createdByName,
				action: 'Created the asset',
				comments: ''
			};
			assetTimeLineList.push(assetCreatedItem);
			if (asset[0].modifiedDate) {
				const timelineItem = {
					label: asset[0].modifiedDate,
					user: asset[0].modifiedByName,
					action: 'Lastest modified',
					comments: ''
				};
				assetTimeLineList.push(timelineItem);
			}
			asset[0].assetVersions.map((v) => {
				const timelineItem = {
					label: v.createdDate,
					user: v.createdByName,
					action: 'Uploaded a version, version name: ' + v.fileName,
					comments: ''
				};
				assetTimeLineList.push(timelineItem);
			});
		}
		if (applvls && applvls.approvalLevels && applvls.approvalLevels.length > 0) {
			applvls.approvalLevels.map((level) => {
				let approverList = level.approvers.map((a) => {
					return a.approverName;
				});
				approverList = approverList.join(', ');
				const approvalTimeLineItem = {
					label: level.createdDate,
					user: level.createdByName,
					action: `Sent a Level ${level.levelNumber} Approval Request to ${approverList}`,
					comments: ''
				};
				assetTimeLineList.push(approvalTimeLineItem);
				if (level.approvers) {
					level.approvers.map((a) => {
						if (a.approvalStatus > 0) {
							const approverTimeLineItem = {
								label: a.reviewDate,
								user: a.approverName,
								action:
									a.approvalStatus === 1
										? 'Approved the request' +
										  (level.completedDate && level.completedDate === a.reviewDate
												? ' and completed Approval Level ' + level.levelNumber
												: '')
										: 'Rejected the request',
								comments: a.comments ? a.comments : ''
							};
							assetTimeLineList.push(approverTimeLineItem);
						}
					});
				}
			});
		}
		setAssetTimeLine([...assetTimeLineList]);
	}

	function showInputTag() {
		setIsTagInputVisible(true);
	}
	const onSelectAsset = (value, option) => {
		//changing assetversion
		handleChangeVersion(value);
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
	function selectOptionsVersionType(type = []) {
		return type.map((row) => {
			return (
				<Select.Option value={row.id} key={row.id}>
					{row.fileName}
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
				<Select.OptGroup key={country} label={country}>
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
						{region.description}
					</Select.Option>
				);
			}
		});
	}
	async function handleChangeVersion(versionId) {
		const data = {
			assetId: findFileState.id,
			versionId: versionId
		};
		await props.loadVersion(data).then((result) => {
			setCurrentVersion(versionId);
			setAssetDetail(result['asset'][0]);
		});
	}

	async function onSubmit(value) {
		setIsUpdating(true);
		if (currentVersion > 0 && activeVersion != currentVersion) {
			await handleRevertAssetVersion(currentVersion);
		}
		await handleUpdateAssets(value);

		setCropResult(null);
		onLoad(true);

		setIsEditing(false);
		setIsUpdating(false);
		listOnLoad();
	}
	async function handleRevertAssetVersion(versionId) {
		const data = {
			assetId: assetDetail.id,
			versionId: versionId
		};
		await props.revertVersion(data);
	}
	async function handleUpdateAssets(value) {
		var selectedAccountOptions = allAccounts.filter((c) => {
			if (value.accounts.filter((x) => x == c.id).length > 0) {
				return true;
			} else {
				return false;
			}
		});
		var selectedCountryOptions = allCountries.filter((c) => {
			if (value.country.filter((x) => x == c.id).length > 0) {
				return true;
			} else {
				return false;
			}
		});
		let regionOptions = [];
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
		var selectedRegionOptions = regionOptions.filter((c) => {
			if (value.region.filter((x) => x == c.id).length > 0) {
				return true;
			} else {
				return false;
			}
		});

		var fileType = '';
		var byteString = [];
		if (cropResult) {
			byteString = unescape(cropResult.split(',')[1]);
			fileType = cropResult.split(',')[0].split(':')[1].split(';')[0];
		}

		const data = {
			id: findFileState.id,
			name: value.name,
			accounts: selectedAccountOptions,
			countries: selectedCountryOptions,
			regions: selectedRegionOptions,
			description: value.comments,
			fileBytes: cropResult ? byteString : [],
			fileType: fileType,
			extension: value.name.split('.').pop(),

			tags: tags,
			expiryDate: expiryDate ? expiryDate : null,
			shareFolderIds: value.share_folders.length > 0 ? String(value.share_folders) : ''
		};
		await props.onUpdateAsset(data, 'update');
	}

	async function handleUploadVersion(option) {
		//setDropzoneShown(false);
		const { file } = option;
		let base;
		let fileReader = new FileReader();
		fileReader.onload = async (e) => {
			base = e.target.result;
			var byteString = new Uint8Array(base);
			const data = {
				name: file.name,
				id: findFileState.id,
				fileName: file.name,
				description: '',
				extension: file.name.split('.').pop(),
				fileBytes: Uint8ToBase64(byteString),
				fileType: file.type,
				folderId: findFileState.folderId,
				status: assetDetail.status
			};
			setIsUpdating(true);
			await props.onUploadAssetVersion(data).then(async (result) => {
				//setTouchFolder(touchFolder);
				onLoad();
				listOnLoad();
				setIsEditing(false);
				setIsUpdating(false);
			});
		};
		fileReader.readAsArrayBuffer(file);
	}

	function selectCountry(e) {
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
	}

	function renderApprovers() {
		if (assetApprovals && assetApprovals.length > 0) {
			let allApprovers = assetApprovals.map((approvalLevel) => {
				let approvers = '';
				if (approvalLevel.approvers && approvalLevel.approvers.length > 0) {
					approvers = approvalLevel.approvers.map((approver) => {
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
				}
				return (
					<React.Fragment key={approvalLevel.id}>
						<Row style={{ marginLeft: 10, marginBottom: 5 }}>
							<Col span={20}>
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
							{approvalLevel.isActiveLevel === true &&
								approvalLevel.dueDate &&
								approvalLevel.approvers.filter((a) => a.approvalStatus === 2).length === 0 && (
									<Col span={4}>{checkStatus(approvalLevel)}</Col>
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

	function renderAssetTimeLine() {
		if (assetTimeLine) {
			let timeLineList = assetTimeLine;
			timeLineList.sort((a, b) =>
				a.label.localeCompare(b.label, undefined, {
					numeric: true,
					sensitivity: 'base'
				})
			);
			return (
				<>
					<Row
						style={{
							borderTop: '2px solid #c6c8c5',
							padding: '20px 0px',
							marginTop: 20
						}}
					>
						<h4>{t('ModalDetail.Asset Detail History')}</h4>
					</Row>
					<Row justify="start">
						<Timeline className="asset-history-time-line" mode="left">
							{timeLineList.map((a) => {
								return (
									<Timeline.Item
										label={moment(a.label).format('DD/MM/YYYY HH:mm A')}
										key={a.label}
										className="asset-history"
										// color="#5f41d2"
									>
										<Row align="middle">
											<Avatar>{a?.user?.substr(0, 1)}</Avatar>
											<span style={{ marginLeft: '10px', wordWrap: 'break-word' }}>
												{a.user} -- {a.action}
											</span>
										</Row>
										{a.comments && <Row style={{ marginLeft: '40px' }}>Comment: {a.comments}</Row>}
									</Timeline.Item>
								);
							})}
						</Timeline>
					</Row>
				</>
			);
		}
	}

	function promoteToAdobe(assetVersionId, fileExtension) {
		return new Promise((resolve, reject) => {
			let xhr = new XMLHttpRequest();
			xhr.open(
				'POST',
				'https://prod-03.australiasoutheast.logic.azure.com:443/workflows/a782170ba5d24349af8bb2a84a7d62c8/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=u_Y-Y6QeaWiDMAfCeoEbfvjLJOxIOb2YiWDCBaCmfKY'
			);
			xhr.setRequestHeader('Content-Type', 'application/json');
			xhr.onload = function () {
				if (this.status >= 200 && this.status < 300) {
					resolve(xhr.response);
					notification.success({
						message: 'Promote',
						description: 'Successfully promoted to Adobe'
					});
				} else {
					reject(this.responseText);
					notification.warning({
						message: 'Promote',
						description: this.responseText
					});
				}
			};
			xhr.onerror = function () {
				reject(this.responseText);
				notification.error({
					message: 'Promote',
					description: this.responseText
				});
			};
			xhr.send(
				JSON.stringify({
					assetVersionId: String(assetVersionId),
					fileExtension: fileExtension,
					container_name: assetContainer
				})
			);
		});
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

	function shareToLinkedIn(assetVersionId, fileName, fileExtension) {
		return new Promise((resolve, reject) => {
			let xhr = new XMLHttpRequest();
			xhr.open(
				'POST',
				'https://prod-00.australiasoutheast.logic.azure.com:443/workflows/303d31d01c284930934548d2e7e5cf7e/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=A8j9_iqqbojLAA5CC01oEaCb6a0k35ptrBUBgvHJpn0'
			);
			xhr.setRequestHeader('Content-Type', 'application/json');
			xhr.onload = function () {
				if (this.status >= 200 && this.status < 300) {
					resolve(xhr.response);
					notification.success({
						message: 'Share',
						description: 'Successfully shared to LinkedIn'
					});
				} else {
					reject(this.responseText);
					notification.warning({
						message: 'Share',
						description: this.responseText
					});
				}
			};
			xhr.onerror = function () {
				reject(this.responseText);
				notification.error({
					message: 'Share',
					description: this.responseText
				});
			};
			xhr.send(
				JSON.stringify({
					asset_id: `${assetVersionId}.${fileExtension}`,
					filename: fileName,
					container_name: assetContainer
				})
			);
		});
	}

	function shareToTwitter(assetVersionId, fileName, fileExtension) {
		return new Promise((resolve, reject) => {
			let xhr = new XMLHttpRequest();
			xhr.open(
				'POST',
				'https://prod-28.australiasoutheast.logic.azure.com:443/workflows/c8afa145c42b4bf7b7df3d05936d4e79/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=sA0K1Q38tqlIiYp0kzVTGCK4BRRQV0NOSSJ1Qc6pxBU'
			);
			xhr.setRequestHeader('Content-Type', 'application/json');
			xhr.onload = function () {
				if (this.status >= 200 && this.status < 300) {
					resolve(xhr.response);
					notification.success({
						message: 'Share',
						description: 'Successfully shared to Twitter'
					});
				} else {
					reject(this.responseText);
					notification.warning({
						message: 'Share',
						description: this.responseText
					});
				}
			};
			xhr.onerror = function () {
				reject(this.responseText);
				notification.error({
					message: 'Share',
					description: this.responseText
				});
			};
			xhr.send(
				JSON.stringify({
					asset_id: `${assetVersionId}.${fileExtension}`,
					filename: fileName,
					container_name: assetContainer
				})
			);
		});
	}

	function openOfficeOnline(assetDetail) {
		console.log(assetDetail);

		var action = assetDetail.status !== ASSET_STATUS.SUBMITTED && userRole.canEdit ? 'edit' : 'view';

		var url = `${'https://wopi.simplemrm.com/Home/DAM'}/${assetDetail.key}/${assetDetail.extension}/${assetDetail.fileName}/${currentVersion}/${assetDetail.size}/${assetDetail.createdByName}/${user.id}/${user.userName}/${action}/${assetContainer}`;
		window.open(url, '_blank', 'height=' + window.outerHeight + ',width=' + window.outerWidth + ',resizable=yes,scrollbars=yes,toolbar=yes,menubar=yes,location=yes');
	}

	function renderTextAnalytics(assetVersionId, fileExtension) {
		new Promise((resolve, reject) => {
			let xhr = new XMLHttpRequest();
			xhr.open(
				'POST',
				'https://prod-00.australiasoutheast.logic.azure.com:443/workflows/6516615e73f34df2824c0b5558bc4e02/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=MussAkePiqxQXDWuNltCFzwjPq-KmkVnncqaLBMXbCE'
			);
			xhr.setRequestHeader('Content-Type', 'application/json');
			xhr.onload = function () {
				if (this.status >= 200 && this.status < 300) {
					const result = JSON.parse(xhr.response);
					resolve(result);
				} else {
					resolve(false);
				}
			};
			xhr.onerror = function () {
				resolve(false);
			};
			if (fileExtension.includes('pdf') || fileExtension.includes('doc') || fileExtension.includes('ppt')) {
				xhr.send(
					JSON.stringify({ asset_id: `${assetVersionId}.${fileExtension}`, container_name: assetPreviewContainer })
				);
			} else {
				xhr.send(JSON.stringify({ asset_id: `${assetVersionId}.${fileExtension}`, container_name: assetContainer }));
			}
		}).then((response) => {
			if (response !== false) {
				setTextAnalytics(
					<>
						<Row>Language</Row>
						<Row style={{ marginLeft: 15 }}>
							{response.DetectedLanguage.map((dl) => {
								return (
									<Tag style={{ borderRadius: 40, marginTop: 5 }} className={'cognitive-tag'} visible>
										{dl.name} - {dl.iso6391Name}
									</Tag>
								);
							})}
						</Row>
						<Row>Key Phrases</Row>
						<Row style={{ marginLeft: 15 }}>
							{response.KeyPhrases.map((kp) => {
								return (
									<Tag style={{ borderRadius: 40, marginTop: 5 }} className={'cognitive-tag'} visible>
										{kp}
									</Tag>
								);
							})}
						</Row>
						<Row>Named Entities</Row>
						<Row style={{ marginLeft: 15 }}>
							{response.NamedEntities.map((ne) => {
								return (
									<Tag style={{ borderRadius: 40, marginTop: 5 }} className={'cognitive-tag'} visible>
										{Math.round(ne.confidenceScore * 100).toString()}% - {ne.text}
									</Tag>
								);
							})}
						</Row>
					</>
				);
			} else {
				setTextAnalytics(
					<Row style={{ marginLeft: 15 }}>
						<h5>No Text Analytics</h5>
					</Row>
				);
			}
		});
	}

	function onSlide(delta) {
		let currentIndex = allAssets.findIndex((item) => item.id === findFileState.id);

		let newIndex = null;
		if (currentIndex + delta > allAssets.length - 1) {
			newIndex = allAssets.length - (currentIndex + delta);
		} else if (currentIndex + delta < 0) {
			newIndex = allAssets.length + (currentIndex + delta);
		} else {
			newIndex = currentIndex + delta;
		}
		setFindFileState(allAssets[newIndex]);
	}

	const handleDownload = (ext) => {
		if (
			hasWatermark &&
			(assetDetail.fileType.includes('image') || (assetDetail.extension.includes('pdf') && ext.length > 0))
		) {
			setIsDownloadModalOpen(true);
			setDownloadExt(ext);
		} else {
			downloadAsset(ext);
		}
	};

	const downloadAsset = (ext) => {
		let downloadsCount = form.getFieldValue('downloadcount');
		form.setFieldsValue({
			downloadcount: downloadsCount + 1
		});
		setIsDownloadModalOpen(false);
		modal().download(assetDetail, false, ext);
	};

	const addThumbnail = () => {
		modal().addNewThumbnail();
	};

	const addPackage = () => {
		modal().addNewPackage();
	};

	const downloadPackage = () => {
		console.log('download package');
		console.log(assetDetail);
		props.downloadAssetPackage(assetDetail.id, assetDetail.packageName, assetDetail.packageExtension);
	};

	const downloadMenu = () => {
		return (
			<Menu>
				<Menu.Item onClick={handleDownload}>Original</Menu.Item>

				{assetDetail.extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('png')))
					<Menu.Item onClick={() => handleDownload('png')}>PNG</Menu.Item>
				)}

				{assetDetail.extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('jpg') && !checkedAssetsItem[0].extension.includes('jpeg')))
					<Menu.Item onClick={() => handleDownload('jpeg')}>JPEG</Menu.Item>
				)}

				{(assetDetail.fileType.includes('image') ||
					assetDetail.extension.includes('doc') ||
					assetDetail.extension.includes('ppt') ||
					assetDetail.extension.includes('xls')) && <Menu.Item onClick={() => handleDownload('pdf')}>PDF</Menu.Item>}
				{assetDetail.packageUrl.includes('https') && <Menu.Item onClick={() => downloadPackage()}>Package</Menu.Item>}
			</Menu>
		);
	};
	const addMenu = () => {
		return (
			<Menu>
				<Menu.Item onClick={addThumbnail}>Thumbnail</Menu.Item>
				<Menu.Item onClick={addPackage}>Package</Menu.Item>
			</Menu>
		);
	};
	const promoteMenu = () => {
		return (
			<Menu>
				{promoteMRMFlag && (
					<Menu.Item
						onClick={() => {
							modal().promoteAsset(false, assetDetail.id);
						}}
					>
						<Space>
							<StockOutlined />
							Simple Marketing
						</Space>
					</Menu.Item>
				)}
				<Menu.Item
					onClick={(e) => {
						modal().promoteAsset(true, assetDetail.id);
					}}
				>
					<Space>
						<FundOutlined />
						Marketing
					</Space>
				</Menu.Item>
				<Menu.Item
					onClick={(e) => {
						promoteToAdobe(assetDetail.key, assetDetail.extension);
					}}
				>
					<Space>
						<FontAwesomeIcon icon={CustomIcons.AdobeCC} />
						Adobe CC
					</Space>
				</Menu.Item>

				<Menu.Item>
					<Space>
						<FontAwesomeIcon icon={CustomIcons.Salesforce} />
						SalesForce
					</Space>
				</Menu.Item>
			</Menu>
		);
	};
	const shareToSocialMediaMenu = () => {
		return (
			<Menu>
				<Menu.Item
					onClick={(e) => {
						shareToLinkedIn(assetDetail.key, assetDetail.name, assetDetail.extension);
					}}
				>
					<Space>
						<FontAwesomeIcon icon={CustomIcons.LinkedIn} />
						LinkedIn
					</Space>
				</Menu.Item>
				{assetDetail.size <= 5242880 && !assetDetail.fileType.includes('application') && (
					<Menu.Item
						onClick={(e) => {
							shareToTwitter(assetDetail.key, findFileState.name, assetDetail.extension);
						}}
					>
						<Space>
							<FontAwesomeIcon icon={CustomIcons.Twitter} />
							Twitter
						</Space>
					</Menu.Item>
				)}
				<Menu.Item>
					<Space>
						<FontAwesomeIcon icon={CustomIcons.Facebook} />
						Facebook
					</Space>
				</Menu.Item>
				<Menu.Item>
					<Space>
						<FontAwesomeIcon icon={CustomIcons.Instagram} />
						Instagram
					</Space>
				</Menu.Item>
			</Menu>
		);
	};
	return (
		<>
			<Modal
				title={modal().header()}
				visible={
					modalState.isVisible &&
					(modalState.type === 'edit-meta-data' || modalState.type === 'edit-details') &&
					assetDetail
				}
				onCancel={modal().closeModal}
				centered={true}
				width={'95%'}
				footer={false}
				getContainer={false}
				closable={true}
				className={`${modalState.type}-modal`}
				keyboard={false}
				destroyOnClose
			>
				<Spin spinning={isUpdating} size="large" tip="Updating...">
					<Form
						form={form}
						key={'edit-meta-details'}
						layout="horizontal"
						name="edit-meta-details"
						onFinish={onSubmit}
						scrollToFirstError
						className="dam-form"
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
						{assetDetail && (
							<Row>
								<Col xs={24} md={24} lg={16}>
									<Row style={{ marginBottom: 20 }}>
										<Col xs={24} md={24} lg={24}>
											<div
												className={`preview-title ${allAssets.length < 2 && 'title-center'}`}
												title={assetDetail.name}
											>
												{allAssets.length > 1 && (
													<FontAwesomeIcon
														size="2x"
														icon={faChevronLeft}
														onClick={(e) => {
															onSlide(-1);
														}}
													/>
												)}
												<h3>{assetDetail.name}</h3>
												{allAssets.length > 1 && (
													<FontAwesomeIcon
														size="2x"
														icon={faChevronRight}
														onClick={(e) => {
															onSlide(1);
														}}
													/>
												)}
											</div>
										</Col>
									</Row>
									<Row type="flex" justify="space-between" align="start" gutter={20}>
										<Col xs={24} md={24} className="edit-assets-preview-section">
											<MetaDetailsPreview assetFile={assetDetail} numPages={numPages} setNumPages={setNumPages} />
										</Col>
									</Row>
									<Row type="flex" className="form-actions" align="middle" justify="center">
										<Col className="cognitive-tags">
											{(modalState.type === 'edit-details' || modalState.type === 'edit-meta-data') && (
												<>
													<Row type="flex" align="middle">
														{userRole.canApprove && approvalFlag && (
															<Col span="auto" style={perCol}>
																<Button className="option-button" onClick={() => modal().approval(assetDetail, true)}>
																	<FileDoneOutlined />
																	{t('Button.Send for Approval')}
																</Button>
															</Col>
														)}

														{userRole.canShare && (
															<>
																<Col span="auto" style={perCol}>
																	<Button
																		className="option-button"
																		onClick={() => {
																			setSelectedShareAssets([assetDetail]);
																			setShareModalState(true);
																		}}
																	>
																		<ShareAltOutlined />
																		{t('Button.Share')}
																	</Button>
																</Col>
																<Col span="auto" style={perCol}>
																	<Dropdown overlay={shareToSocialMediaMenu}>
																		<Button className="option-button">
																			<Space>
																				<ShareAltOutlined />
																				Share to Social Media
																				<FontAwesomeIcon icon={faAngleDown} />
																			</Space>
																		</Button>
																	</Dropdown>
																</Col>
															</>
														)}
														<Col span="auto" style={perCol}>
															{assetDetail.fileType.includes('image') ||
															assetDetail.extension.includes('pdf') ||
															assetDetail.extension.includes('doc') ||
															assetDetail.extension.includes('xls') ||
															assetDetail.extension.includes('ppt') ||
															(assetDetail.packageUrl && assetDetail.packageUrl.includes('https')) ? (
																<Dropdown overlay={downloadMenu} placement="bottomRight">
																	<Button className="option-button">
																		<Space>
																			<DownloadOutlined />
																			{t('Button.Download')}
																			<FontAwesomeIcon icon={faAngleDown} />
																		</Space>
																	</Button>
																</Dropdown>
															) : (
																<Button
																	className="option-button"
																	onClick={(e) => {
																		handleDownload();
																	}}
																>
																	<DownloadOutlined />
																	{t('Button.Download')}
																</Button>
															)}
														</Col>
														{isAdministrator && (
															<Col span="auto" style={perCol}>
																<Dropdown overlay={addMenu}>
																	<Button className="option-button">
																		<Space>
																			<PlusOutlined />
																			Add
																			<FontAwesomeIcon icon={faAngleDown} />
																		</Space>
																	</Button>
																</Dropdown>
															</Col>
														)}
														{isEditing ? (
															<Col span="auto" style={perCol}>
																{assetDetail.fileType.includes('image') ? (
																	<Button
																		className="option-button"
																		onClick={(e) => {
																			setIsCropping(true);
																		}}
																	>
																		<RadiusUprightOutlined />
																		{t('Button.Crop')}
																	</Button>
																) : (
																	<></>
																)}
															</Col>
														) : (
															<></>
														)}

														{assetDetail.fileType.includes('application') && (assetDetail.extension.includes('docx') || assetDetail.extension.includes('pptx') || 
														assetDetail.extension.includes('xlsx')) && (
															<Col span="auto" style={perCol}> 
																<Button
																	className="option-button"
																	onClick={(e) => {
																		openOfficeOnline(assetDetail);
																	}}
																>
																	<FileWordOutlined />
																	Open Office Online
																</Button>
															</Col>
														)}

														{userRole.canArchive && (
															<Col span="auto" style={perCol}>
																<Button className="option-button" onClick={modal().archive}>
																	<InboxOutlined />
																	{t('Button.Archive')}
																</Button>
															</Col>
														)}
														{promoteMRMFlag && (
															<>
																<Col span="auto" style={perCol}>
																	<Dropdown overlay={promoteMenu}>
																		<Button className="option-button">
																			<Space>
																				<FundOutlined />
																				Promote
																				<FontAwesomeIcon icon={faAngleDown} />
																			</Space>
																		</Button>
																	</Dropdown>
																</Col>
															</>
														)}
														{!assetDetail.fileType.includes('video') && !assetDetail.fileType.includes('audio') && (
															<Col span="auto" style={perCol}>
																<Button
																	className="option-button"
																	onClick={(e) => {
																		markup(assetDetail.originalUrl, assetDetail.extension, assetDetail.fileType);
																	}}
																>
																	<FontAwesomeIcon icon={CustomIcons.markup} />
																	Mark Up
																</Button>
															</Col>
														)}

														{videoIndexerFlag &&
															assetDetail.fileType.includes('video') &&
															!window.location.pathname.includes('dynamics') && (
																<Col span="auto" style={perCol}>
																	<Button
																		className="option-button"
																		onClick={async (e) => {
																			await props.authenticateForIndexing().then(async (response) => {
																				let videoId = '';
																				const videoAccessToken = response.data;
																				const videoUrl = await copyToVideoIndexerContainer(
																					assetDetail.assetVersions[0].key.concat('.', assetDetail.extension)
																				);

																				let config = {
																					headers: {
																						'Ocp-Apim-Subscription-Key': '058b08005c2c44c39a062a4cb1d621b2',
																						'x-ms-client-request-id': '',
																						'Content-Type': 'multipart/form-data'
																					}
																				};

																				let querString = `accessToken=${videoAccessToken}&location=trial&name=${assetDetail.fileName}&privacy=Public&partition=parition&description=${assetDetail.fileName}&videoUrl=${videoUrl}`;
																				let url = `trial/Accounts/06f672dd-a001-45fb-919c-29771f87f85f/Videos?${querString}`;

																				await props
																					.uploadVideoForIndexing(url, '', config)
																					.then(async (uploadResponse) => {
																						console.log('Upload', uploadResponse);
																						videoId = uploadResponse.data.id;

																						// Check Index Status
																						url = `trial/Accounts/06f672dd-a001-45fb-919c-29771f87f85f/Videos/${videoId}/Index?accessToken=${videoAccessToken}&language=English`;
																						config = {
																							headers: {
																								'Ocp-Apim-Subscription-Key': '058b08005c2c44c39a062a4cb1d621b2'
																							}
																						};
																						let processingState = '';

																						while (true) {
																							await sleep(4000);
																							await props.getIndexingState(url, config).then((result) => {
																								console.log(result.data.videos[0].state);

																								processingState = result.data.videos[0].state;
																								notification.info({
																									message: t('Messages.Video Indexer'),
																									description: `Indexing Progress at ${result.data.videos[0].processingProgress}`
																								});

																								console.log(result.data.videos[0].processingProgress);
																							});

																							if (processingState != 'Uploaded' && processingState != 'Processing') {
																								break;
																							}
																						}

																						// Check Index Status
																					});
																			});
																		}}
																	>
																		<UploadOutlined />
																		{t('Button.Upload to Video Indexer')}
																	</Button>
																</Col>
															)}
													</Row>
												</>
											)}
										</Col>
									</Row>
									{renderAssetTimeLine()}
								</Col>
								<Col xs={24} md={24} lg={1}>
									<Col xs={12} md={12} lg={12} className="vertical-divider"></Col>
								</Col>
								<Col xs={24} md={24} lg={7}>
									<Row justify="start" style={{ marginBottom: 16 }}>
										<Col xs={12} md={12} lg={12}>
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
												{t('ModalDetail.File Details')}
											</div>
										</Col>

										{!modal().isReadOnly() && (
											<Col xs={12} md={12} lg={12} style={{ textAlign: 'end', padding: '4px' }} className="edit-switch">
												<Switch
													checkedChildren={t('Button.Done')}
													unCheckedChildren={t('Button.Edit')}
													checked={isEditing}
													onChange={() => {
														setIsEditing(!isEditing);
													}}
												/>
											</Col>
										)}
									</Row>

									<Row type="flex" justify="space-between" align="end">
										<Col xs={24} md={24}>
											<Form.Item
												name="name"
												label={t('ModalDetail.Display Name')} //"Display Name"
												rules={[
													{
														required: true,
														message: t('Messages.Please input file display name')
													}
												]}
											>
												<Input style={{ width: '100%' }} readOnly={modal().isReadOnly() || !isEditing} />
											</Form.Item>
											<Form.Item name="filename" label={t('ModalDetail.File Name')}>
												<Input style={{ width: '100%' }} disabled />
											</Form.Item>
											<Form.Item name="extension" label={t('ModalDetail.File Extension')}>
												<Input disabled style={{ width: '100%' }} />
											</Form.Item>
											<Form.Item name="folder_name" label={t('ModalDetail.Main Folder')} initialValue={['Root']}>
												<Select
													className="folder-chip"
													disabled
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													bordered={false}
												>
													{selectOptionsType(efolders)}
												</Select>
											</Form.Item>
											<Form.Item name="share_folders" label={t('ModalDetail.Share Folders')}>
												<TreeSelect
													disabled={!userRole.canShareFolders || !isEditing}
													bordered={isEditing}
													className={isEditing ? '' : 'folder-chip'}
													mode="multiple"
													placeholder={t('Messages.Select')}
													treeData={efolders ? NestedChild(efolders, 1) : []}
													allowClear
													treeCheckable
												></TreeSelect>
											</Form.Item>
											<Form.Item
												name="filesize"
												label={t('ModalDetail.File Size')}
												style={{ display: 'flex', flexFlow: 'wrap' }}
											>
												<Input disabled style={{ width: '100%' }} />
											</Form.Item>
											<Form.Item
												name="dimensions"
												label={t('ModalDetail.Dimensions')}
												style={{ display: 'flex', flexFlow: 'wrap' }}
											>
												<PicDimension url={assetDetail.originalUrl} fileType={assetDetail.extension} />
											</Form.Item>
											{approvalFlag && (
												<Form.Item name="statusname" label={t('ModalDetail.Status')}>
													<Input disabled style={{ width: '100%' }} />
												</Form.Item>
											)}
											<Form.Item name="uploadedby" label={t('ModalDetail.Uploaded By')}>
												<Input disabled style={{ width: '100%' }} />
											</Form.Item>
											<Form.Item name="uploaddate" label={t('ModalDetail.Upload Date')}>
												<Input disabled style={{ width: '100%' }} />
											</Form.Item>

											{modal().isReadOnly() || !isEditing ? (
												<Form.Item
													label={t('ModalDetail.Expiry')}
													name="expirydatereadOnly"
													style={{ width: '100%' }}
													help={modal().isReadOnly() || !isEditing ? '' : '*File will expire on selected date.'}
												>
													<Input readOnly style={{ width: '100%' }} />
												</Form.Item>
											) : (
												<Form.Item
													label={t('ModalDetail.Expiry')}
													name="expirydate"
													style={{ width: '100%' }}
													help={modal().isReadOnly() || !isEditing ? '' : '*File will expire on selected date.'}
												>
													<DatePicker
														disabledDate={disabledDate}
														onChange={(a, c) => {
															setExpiryDate(c);
														}}
														readOnly={modal().isReadOnly()}
													/>
												</Form.Item>
											)}
											<Form.Item label={t('ModalDetail.Description')} name="comments">
												{modal().isReadOnly() || !isEditing ? (
													<Input readOnly />
												) : (
													<Input.TextArea placeholder={t('Messages.Comments here')} readOnly={modal().isReadOnly()} />
												)}
											</Form.Item>
											<Form.Item name="country" label={t('ModalDetail.Country')}>
												<Select
													readOnly={modal().isReadOnly() || !isEditing}
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													onChange={selectCountry}
													disabled={modal().isReadOnly() || !isEditing}
													className={isEditing ? '' : 'select-tag'}
													bordered={false}
												>
													{selectOptionsType(allCountries)}
												</Select>
											</Form.Item>
											<Form.Item name="region" label={t('ModalDetail.Region')}>
												<Select
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													disabled={modal().isReadOnly() || !isEditing}
													className={isEditing ? '' : 'select-tag'}
													bordered={false}
												>
													{selectGroupOptionsType(regionOptions.sort(Compare))}
												</Select>
											</Form.Item>
											<Form.Item name="accounts" label={t('ModalDetail.Account')}>
												<Select
													readOnly={modal().isReadOnly() || !isEditing}
													mode="multiple"
													placeholder={t('Messages.Select')}
													dropdownStyle={{ position: 'fixed' }}
													disabled={modal().isReadOnly() || !isEditing}
													className={isEditing ? '' : 'select-tag'}
													bordered={false}
												>
													{selectOptionsType(allAccounts)}
												</Select>
											</Form.Item>
											<Row
												style={{
													borderTop: '2px solid #c6c8c5',
													padding: '10px 0px'
												}}
											></Row>
											<Form.Item name="tags" label={t('ModalDetail.Tags')}>
												<Row>
													{tags.map((row) => (
														<Col key={row.id} className="cognitive-tags-list">
															<Tag
																style={{ borderRadius: 40, marginTop: 5 }}
																closable={!modal().isReadOnly() && isEditing}
																onClose={() => removeTag(row.name)}
																className={row.isCognitive ? 'cognitive-tag' : 'user-tag'}
																visible
															>
																{row.name}
															</Tag>
														</Col>
													))}
													{tagInputVisible && (
														<Input
															type="text"
															size="small"
															style={{ width: 78 }}
															value={tagInputValue}
															onChange={handleTagInputChange}
															onBlur={handleTagInputConfirm}
															onPressEnter={handleTagInputConfirm}
														/>
													)}
												</Row>
												<Row style={{ marginTop: 5 }}>
													{isEditing && (
														<Button size="small" type="dashed" onClick={showInputTag}>
															{t('Button.New Tag')}
														</Button>
													)}
												</Row>
											</Form.Item>
											{(assetDetail.fileType.includes('image') ||
												assetDetail.fileType.includes('ppt') ||
												assetDetail.fileType.includes('pdf') ||
												assetDetail.fileType.includes('doc')) && (
												<Row
													style={{
														borderTop: '2px solid #c6c8c5',
														padding: '10px 0px'
													}}
												>
													<Col span={24}>
														<Row>Text Analytics</Row>
														{textAnalytics}
													</Col>
												</Row>
											)}
											{approvalFlag && (
												<Row
													style={{
														borderTop: '2px solid #c6c8c5',
														padding: '10px 0px'
													}}
												>
													<Col span={24}>
														<Row>{t('ModalDetail.Approvers')}</Row>
														{renderApprovers()}
													</Col>
												</Row>
											)}
											{assetVersionFlag && (
												<Row
													style={{
														borderTop: '2px solid #c6c8c5',
														padding: '10px 0px'
													}}
												>
													<Col span={24}>
														<Row>
															<Col span={24}>
																<Form.Item label={t('ModalDetail.Current Version')} name="currentVersions">
																	<Select
																		dropdownStyle={{ position: 'fixed' }}
																		disabled={!isEditing}
																		onSelect={onSelectAsset}
																	>
																		{selectOptionsVersionType(assetDetail.assetVersions)}
																	</Select>
																</Form.Item>
															</Col>
														</Row>
														<Row>
															<Col span={24}>
																<Form.Item label={t('ModalDetail.Total Versions')} name="totalversions">
																	<Input style={{ width: '100%' }} disabled />
																</Form.Item>
															</Col>
															{assetDetail.status !== ASSET_STATUS.SUBMITTED && userRole.canEdit && isEditing && (
																<Col style={{ paddingleft: '12px' }}>
																	<Upload {...clickUploaderVersionProps}>
																		<Button icon={<UploadOutlined />} type="primary">
																			{t('ModalDetail.Upload New Version')}
																		</Button>
																	</Upload>
																</Col>
															)}
														</Row>
													</Col>
												</Row>
											)}
											<Row
												style={{
													borderTop: '2px solid #c6c8c5',
													padding: '10px 0px'
												}}
											>
												<Row type="flex" className="form-actions">
													<Col
														xs={24}
														className="form-update-actions"
														style={{
															textAlign: 'right'
														}}
													>
														{modalState.type !== 'wopi' &&
															!(
																modalState.type === 'edit-meta-data' ||
																modalState.type === 'edit-details' ||
																modalState.type === 'approval'
															) && (
																<Button type="secondary" onClick={modal().closeModal}>
																	{t('Button.Cancel')}
																</Button>
															)}
														{userRole.canEdit && isEditing && (
															<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
																{t('Button.Update Asset')}
															</Button>
														)}
													</Col>
												</Row>
											</Row>
										</Col>
									</Row>
								</Col>
							</Row>
						)}
					</Form>
				</Spin>
			</Modal>

			<DownloadModal
				isDownloadModalOpen={isDownloadModalOpen}
				setIsDownloadModalOpen={setIsDownloadModalOpen}
				modal={modal}
				checkedAssetsItem={assetDetail}
				downloadExt={downloadExt}
			/>
			<Cropper
				initialImg={assetDetail ? assetDetail.thumbnail : null}
				imageToCrop={assetDetail ? assetDetail.originalUrl : null}
				// uploadUsingReplace={uploadUsingReplace}
				cropResult={cropResult}
				setCropResult={(cropResult) => {
					applyCroppedImage(cropResult);
				}}
				//imageFileType={assetDetail ? assetDetail.thumbnail : null}
				readOnly={modal().isReadOnly()}
				isCropping={isCropping}
				setCropping={setIsCropping}
				handleCropped={setCroppedImage}
			/>
		</>
	);

	function applyCroppedImage(cropResult) {
		setCropResult(cropResult);
		var asset = assetDetail;
		asset.originalUrl = cropResult;
		setAssetDetail(asset);
	}
	function sleep(ms) {
		return new Promise((resolve) => setTimeout(resolve, ms));
	}

	async function copyToVideoIndexerContainer(blobName) {
		const blobService = new BlobServiceClient(`https://${accountName}.blob.core.windows.net?${sasToken}`);

		const sourceContainer = blobService.getContainerClient(sourceContainerName);
		const destContainer = blobService.getContainerClient(destContainerName);
		const sourceBlob = sourceContainer.getBlobClient(blobName);

		if (await sourceBlob.exists()) {
			let sourceProperties = await sourceBlob.getProperties();

			const lease = sourceBlob.getBlobLeaseClient();

			if (sourceProperties.leaseState == 'leased') {
				await lease.breakLease();
			}

			await lease.acquireLease(-1);

			const destBlob = destContainer.getBlobClient(sourceBlob.name);

			await destBlob.deleteIfExists();

			await destBlob.syncCopyFromURL(sourceBlob.url);

			const destProperties = await destBlob.getProperties();

			console.log(`Copy status: ${destProperties.copyStatus}`);
			console.log(`Copy progress: ${destProperties.copyProgress}`);
			console.log(`Completion time: ${destProperties.copyCompletedOn}`);
			console.log(`Total bytes: ${destProperties.contentLength}`);

			sourceProperties = await sourceBlob.getProperties();

			if (sourceProperties.leaseState == 'leased') {
				// Break the lease on the source blob.
				await lease.breakLease();

				// Update the source blob's properties to check the lease state.
				sourceProperties = await sourceBlob.getProperties();
				console.log(`Lease state: ${sourceProperties.leaseState}`);
			}

			return destBlob.url;
		}
	}

	function disabledDate(current) {
		// Can not select days before today and today
		return current && current < moment().endOf('day');
	}

	function handleTagInputChange(e) {
		setTagInputValue(e.target.value);
	}
	function handleTagInputConfirm() {
		const inputValue = tagInputValue;
		let newTags = tags;
		if (inputValue && tags.filter((t) => t.name === inputValue).length === 0) {
			let newTag = {
				id: Math.floor(Math.random() * 100),
				name: inputValue,
				isCognitive: false
			};
			newTags.push(newTag);
		}
		setTags(newTags);
		setIsTagInputVisible(false);
		setTagInputValue('');
	}
	function removeTag(tag) {
		const newTags = tags.filter((t) => t.name !== tag);
		setTags(newTags);
	}
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getAssetApprovals: (id, verId, refresh) => dispatch(getAssetApprovals(id, verId, refresh)),
		getAssetById: (id) => dispatch(getAssetById(id)),
		authenticateForIndexing: () => dispatch(authenticateVideoForIndexing()),
		uploadVideoForIndexing: (fileName, data, config) => dispatch(uploadVideoForIndexing(fileName, data, config)),
		getIndexingState: (url, config) => dispatch(getIndexingState(url, config)),
		loadVersion: (data) => dispatch(getFilesVersion(data)),
		revertVersion: (data) => dispatch(revertAssetVersion(data)),
		onUpdateAsset: (data, type) => dispatch(updateAsset(data, type)),
		onUploadAssetVersion: (data) => dispatch(uploadAssetVersion(data)),
		downloadAssetPackage: (assetKey, fileName, fileExt) => dispatch(downloadAssetPackage(assetKey, fileName, fileExt))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(EditMetaDetails));
