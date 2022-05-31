import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import moment from 'moment';
import { Layout, Row, Col, Card, Button, List, Space, Checkbox, Spin, Avatar, Radio } from 'antd';

import useModal from '../../shared/useModal';
import { getApprovals, submitApprovals } from '../actions';
import { getAssetApprovals } from '../../dam/actions';
import { LowFrequencyContext } from '@damcontext';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEye } from '@fortawesome/free-solid-svg-icons';
import AssetThumbnail from '../../dam/components/assetThumbnail';
import { DefaultFilterState } from '../../constants';
import { useTranslation } from 'react-i18next';
import ReviewModal from './reviewModal';
import ContentHeader, { DropDownOptions } from '../../dam/components/contentHeader';

function Approvals(props) {
	const { t } = useTranslation();
	const { filterState } = useContext(LowFrequencyContext);
	// logged in user id
	const { Meta } = Card;
	const [filter, setFilter] = useState('');
	const [modalState, modalDispatch] = useModal();
	const [isLoading, setIsLoading] = useState(false);
	// placeholder for file state
	const [assetToApprove, setAssetToApprove] = useState({});
	const [keySearchUsed, dataSearched] =
		filterState.Search && filterState.Search.length > 1 ? filterState.Search : DefaultFilterState.Search;
	const [checkedAssetsType, setCheckedAssetsType] = useState(null);
	const parseCheckedAssetsType = JSON.parse(checkedAssetsType);
	const paginationSettings = {
		pageSizeOptions: [10, 24, 48, 96],
		showSizeChanger: true,
		defaultCurrent: 1,
		responsive: true,
		showQuickJumper: true
	};

	const [checkedAssets, setCheckedAssets] = useState([]);
	const [isReviewVisible, setIsReviewVisible] = useState(false);
	const [selectAll, setSelectAll] = useState(false);

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
				createdByName: file.createdByName,
				createdDate: file.createdDate,
				statusUpdatedDate: file.statusUpdatedDate,
				dueDate: file.dueDate,
				ownerPic: file.ownerProfilePic,
				levelNumber: file.currentApprovalLevelNumber,
				assetVersions: file.assetVersions,
				folderName: file.folderName
			};
		});

	useEffect(() => {
		onLoad();
	}, []);

	useEffect(() => {
		if (props.match.params.id && props.fileData && efiles.length > 0) {
			let filteredFile = efiles.filter((x) => x.id === parseInt(props.match.params.id));
			if (filteredFile.length > 0) {
				handleReviewClick(filteredFile[0]);
			}
		}
	}, [props.fileData]);

	function filterChange(value) {
		setFilter(value);
	}

	const getTitle = (item) => {
		return (
			<>
				<div className="approval-card-title">
					<span>
						{t('Label.Level')} {item.levelNumber}
					</span>
				</div>
				<Checkbox
					className="overlay-checkbox rounded-checkbox"
					onChange={(value) => {
						if (value.target.checked) {
							let newlist = checkedAssets;
							newlist.push(item);
							setCheckedAssets([...newlist]);
						} else {
							let newlist = checkedAssets.filter((asset) => asset.id !== item.id);
							setCheckedAssets([...newlist]);
						}
					}}
					checked={checkedAssets.find((asset) => asset.id === item.id) ? true : false}
				></Checkbox>
			</>
		);
	};

	const getDescription = (item) => {
		return (
			<Col className="approval-card-detail">
				<Row justify="end" style={{ marginBottom: '16px' }}>
					<Col xs={6} sm={6} md={6} lg={4}>
						<Avatar src={item.ownerPic}>{item.createdByName.substr(0, 1)}</Avatar>
					</Col>
					<Col xs={18} sm={18} md={18} lg={20}>
						<Row>
							<span>{`${t('ModalDetail.Created By')}: `}</span>
						</Row>
						<Row>
							<span>{item.createdByName}</span>
						</Row>
					</Col>
				</Row>
				{item.dueDate && (
					<Row>
						<span>
							{t('Label.Due On') + ' ' + (item.dueDate ? moment(item.dueDate).format('DD/MM/YYYY HH:mm A') : 'N/A')}
						</span>
					</Row>
				)}
			</Col>
		);
	};

	async function onLoad() {
		setIsLoading(true);
		await props.loadAssetsForApproval();
		setIsLoading(false);
		setCheckedAssets([]);
	}

	function modal() {
		return {
			selectMultiData() {},
			closeModal() {
				modalDispatch({});
				setIsReviewVisible(false);
				setCheckedAssets([]);
				setSelectAll(false);
			},
			approval(item) {
				setAssetToApprove(item);
				setIsReviewVisible(true);

				modalDispatch({
					type: 'BOTH',
					payload: 'approval'
				});
			}
		};
	}

	function dataSource() {
		const data =
			keySearchUsed.length > 0
				? dataSearched
				: filter === 'name'
				? efiles.sort((a, b) => a.name.localeCompare(b.name, undefined, { numeric: true, sensitivity: 'base' }))
				: efiles;

		const source = data;

		return parseCheckedAssetsType
			? source.filter((row) => parseCheckedAssetsType.accept.includes(row.fileType.split('/').shift()))
			: source;
	}

	const data = [
		{ slug: 'All', accept: ['video', 'image', 'application'], type: 'assets_type' },
		{ slug: 'Photos', accept: ['image'], type: 'assets_type' },
		{ slug: 'Videos', accept: ['video'], type: 'assets_type' },
		{ slug: 'Audio Files', accept: ['audio'], type: 'assets_type' },
		{ slug: 'Others', accept: ['application'], type: 'assets_type' }
	];
	const assetsTypeData = data
		.filter((row) => row.type === 'assets_type')
		.map((row) => {
			delete row.type;
			return row;
		});
	const changeAssetsType = (slug) => setCheckedAssetsType(slug);

	const handleReviewClick = async (item) => {
		if (checkedAssets.length === 0 && checkedAssets.find((a) => a.id === item.id) == undefined) {
			let newList = checkedAssets;
			newList.push(item);
			setCheckedAssets([...newList]);
		}
		modal().approval(item);
	};

	function handleSelectAll() {
		if (selectAll) {
			setCheckedAssets([]);
		} else {
			let tempItems = dataSource();
			setCheckedAssets(tempItems);
		}
		setSelectAll(!selectAll);
	}

	return (
		<Spin spinning={isLoading} size="large">
			<Layout className="approval-layout page-layout">
				<Layout.Content>
					<Row className="approval">
						<Col span={24}>
							<ContentHeader
								title={t('Slider.Approvals')}
								extraButtons={
									<>
										{dataSource().length > 0 && (
											<Checkbox
												style={{ marginLeft: '8px' }}
												className="rounded-checkbox"
												checked={selectAll}
												onChange={handleSelectAll}
											>
												{t('ToolMenu.Select All')}
											</Checkbox>
										)}
										<DropDownOptions
											title={t('Button.Sort By')}
											subtitle={t('Sort By.Sort Options')}
											onClear={() => {
												filterChange(null);
											}}
											content={
												<Row style={{ display: 'flex', flexWrap: 'nowrap', alignItems: 'center' }}>
													<Col>
														<Radio.Group
															style={{ display: 'flex', flexDirection: 'column' }}
															value={filter}
															onChange={(e) => {
																filterChange(e.target.value);
															}}
														>
															<Radio value="name">{t('ModalDetail.Name')}</Radio>;
														</Radio.Group>
													</Col>
												</Row>
											}
										/>
										<DropDownOptions
											title={t('Button.Filter By')}
											subtitle={t('Filter By.Filter Options')}
											onClear={() => {
												setCheckedAssetsType(null);
											}}
											content={
												<Row type="flex" align="top" style={{ marginTop: 10 }}>
													<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
														{t('File Types')}
													</Col>
													<Col span={24} align="left">
														{assetsTypeData.map((row) => (
															<Col key={row.slug}>
																<Space>
																	<Radio
																		onChange={() => changeAssetsType(JSON.stringify(row))}
																		checked={JSON.stringify(row) === checkedAssetsType}
																	>
																		{row.slug}
																	</Radio>
																</Space>
															</Col>
														))}
													</Col>
												</Row>
											}
										/>
									</>
								}
							/>
							<Row type="flex" className="folders-list-parent">
								<Col span="auto" className="list-container fade-in">
									<Row type="flex" justify="space-between" align="middle">
										<Col span="auto" className="list-section">
											{keySearchUsed.length === 0 ? null : (
												<Row type="flex" justify="center" align="center" className="search-indicator">
													<Col align="center">
														{keySearchUsed.length === 0 ? null : dataSource().length}
														&nbsp;
														{dataSource().length > 1 ? 'Files Found' : 'File Found'}
													</Col>
												</Row>
											)}

											{dataSource().length > 0 && (
												<List
													pagination={paginationSettings}
													grid={{
														gutter: 16,
														xs: 2,
														sm: 2,
														md: 3,
														lg: 3,
														xl: 5,
														xxl: 5
													}}
													dataSource={dataSource()}
													renderItem={(item) => (
														<AssetThumbnail
															item={item}
															title={getTitle(item)}
															mode={0}
															metaDescription={
																<Meta
																	//avatar={getCreatorAvatar(item)}
																	title={item.fileName}
																	description={getDescription(item)}
																/>
															}
															actionButtons={[
																<Button
																	className="option-button"
																	onClick={() => handleReviewClick(item)}
																	icon={<FontAwesomeIcon icon={faEye} />}
																>
																	{t('Button.Review')}
																</Button>
															]}
															onClickAction={() => handleReviewClick(item)}
														/>
													)}
												/>
											)}
										</Col>
									</Row>
								</Col>
							</Row>
						</Col>
					</Row>
				</Layout.Content>

				<ReviewModal
					modal={modal}
					isVisible={isReviewVisible}
					onLoad={() => onLoad()}
					assetToApprove={assetToApprove}
					setAssetToApprove={setAssetToApprove}
					checkedAssets={checkedAssets}
				></ReviewModal>
			</Layout>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {
		fileData: state.approvals.fileData
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadAssetsForApproval: () => dispatch(getApprovals()),
		submitApprovals: (data) => dispatch(submitApprovals(data)),
		getAssetApprovals: (id, verId) => dispatch(getAssetApprovals(id, verId))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Approvals));
