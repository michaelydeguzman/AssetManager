import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import moment from 'moment';

import { Layout, Row, Col, Card, Button, List, Checkbox, Form, Modal, Spin, Radio } from 'antd';

import useModal from '../../shared/useModal';

import { getArchive, updateAssetStatus } from '../actions';
import { LowFrequencyContext } from '@damcontext';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faUndo } from '@fortawesome/free-solid-svg-icons';
import AssetThumbnail from '../../dam/components/assetThumbnail';
import { DefaultFilterState } from '../../constants';
import { useTranslation } from 'react-i18next';
import ContentHeader, { DropDownOptions } from '../../dam/components/contentHeader';

function Archive(props) {
	const { t } = useTranslation();
	const { filterState } = useContext(LowFrequencyContext);

	// logged in user id
	const [form] = Form.useForm();
	const { Meta } = Card;

	const [filter, setFilter] = useState('');
	const [modalState, modalDispatch] = useModal();
	const [isLoading, setIsLoading] = useState(false);

	// placeholder for file state

	const [assetsSliderControls, setAssetSliderControls] = useState([false, false]);
	const [assetsSliderPrev, assetsSliderNext] = assetsSliderControls;

	const [showPreview, setPreview] = useState(false);

	// Asset Status enum
	const assetStatus = Object.freeze({
		Active: 0,
		Archived: 1,
		Deleted: 2
	});

	const [assetId, setAssetId] = useState(0);

	// edit files data
	let efiles =
		props.fileData &&
		props.fileData.map((file) => {
			const {
				id,
				name,
				fileName,
				extension,
				folderId,
				accounts,
				countries,
				regions,
				thumbnail,
				downloadUrl,
				comments,
				originalUrl,
				fileType,
				tags,
				fileSizeText,
				createdByName,
				createdDate,
				statusUpdatedDate,
				assetVersions
			} = file;
			return {
				key: JSON.stringify(folderId),
				id,
				name,
				fileName,
				extension,
				folderId,
				type: 'file',
				accounts,
				countries,
				regions,
				thumbnail,
				downloadUrl,
				comments,
				originalUrl,
				fileType,
				tags,
				fileSizeText,
				createdByName,
				createdDate,
				statusUpdatedDate,
				assetVersions
			};
		});

	useEffect(() => {
		onLoad(props);
	}, []);

	// Event handlers

	function filterChange(value) {
		setFilter(value);
	}

	async function onSubmit(values) {
		handleAssetStatus(modalState.type).then(() => {
			modalDispatch({});
			onLoad(props);
			setAssetId(0);
		});
	}

	async function onLoad(props) {
		props.loadArchivedFiles();
	}

	function modal() {
		return {
			header() {
				return modalState.type === 'Restore'
					? t('Restore Asset.Title')
					: modalState.type === 'Delete' && t('Delete Asset Permanently.Title');
			},
			selectMultiData() {},
			closeModal() {
				modalDispatch({});
				form.resetFields();
			},
			restore(id) {
				setAssetId(id);
				modalDispatch({
					type: 'BOTH',
					payload: 'Restore'
				});
			},
			delete(id) {
				setAssetId(id);
				modalDispatch({
					type: 'BOTH',
					payload: 'Delete'
				});
			}
		};
	}

	const [keySearchUsed, dataSearched] =
		filterState.Search && filterState.Search.length > 1 ? filterState.Search : DefaultFilterState.Search;

	const [checkedAssetsType, setCheckedAssetsType] = useState([]);

	const typeData = [
		{
			slug: 'All',
			accept: ['video', 'image', 'application'],
			type: 'assets_type'
		},
		{ slug: 'Photos', accept: ['image'], type: 'assets_type' },
		{ slug: 'Videos', accept: ['video'], type: 'assets_type' },
		{ slug: 'Audio Files', accept: ['audio'], type: 'assets_type' },
		{ slug: 'Others', accept: ['application'], type: 'assets_type' }
	];

	function dataSource() {
		const data =
			keySearchUsed.length > 0
				? dataSearched
				: filter === 'name'
				? efiles.sort((a, b) =>
						a.name.localeCompare(b.name, undefined, {
							numeric: true,
							sensitivity: 'base'
						})
				  )
				: efiles;

		const source = data;

		const acceptedData =
			checkedAssetsType.length > 0
				? typeData.filter((row) => checkedAssetsType.includes(row.slug)).flatMap((row) => row.accept)
				: [];

		return checkedAssetsType.length > 0
			? source.filter((row) => acceptedData.includes(row.fileType.split('/').shift()))
			: source;
	}

	async function handleAssetStatus(status) {
		var assetList = [];
		assetList.push(assetId);
		const data = {
			assetIds: assetList,
			status: assetStatus[status.includes('Delete') ? 'Deleted' : status.includes('Restore') && 'Active']
		};

		await props.onUpdateAssetStatus(data, status).then(() => {});
	}

	const assetsTypeData = typeData.filter((row) => row.type === 'assets_type');
	const assetTypeSlug = assetsTypeData.map((row) => row.slug);

	const changeAssetsType = (slug) => setCheckedAssetsType(slug);

	return (
		<Spin spinning={isLoading} size="large">
			<Layout className="archive-layout page-layout">
				<Layout.Content>
					<Row className="tabpane-layout">
						<Col span={24}>
							<Row className="archive">
								<Col span={24}>
									<ContentHeader
										title={t('Slider.Archive')}
										extraButtons={
											<>
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
														setCheckedAssetsType([]);
													}}
													content={
														<Row type="flex" align="top" style={{ marginTop: 10 }}>
															<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
																{t('File Types')}
															</Col>
															<Col span={24} align="left">
																<Checkbox.Group
																	style={{ display: 'flex', flexDirection: 'column' }}
																	options={assetTypeSlug}
																	value={checkedAssetsType}
																	onChange={changeAssetsType}
																/>
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
													{keySearchUsed.length !== 0 && (
														<Row type="flex" justify="center" align="center" className="search-indicator">
															<Col align="center">
																{keySearchUsed.length === 0 ? null : dataSource().length}
																&nbsp;
																{dataSource().length > 1 ? 'Files Found' : 'File Found'}
															</Col>
														</Row>
													)}

													{dataSource().length !== 0 && (
														<List
															pagination={{
																pageSize: 12
															}}
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
																	mode={0}
																	previewThumbnail={
																		showPreview
																			? item.fileType && item.fileType.includes('image')
																				? item.originalUrl
																				: item.thumbnail
																			: item.thumbnail
																	}
																	metaDescription={
																		<Meta
																			title={t('ModalDetail.Archive Date')}
																			description={moment(item.statusUpdatedDate).format('DD/MM/YYYY HH:mm A')}
																		/>
																	}
																	actionButtons={[
																		<Button
																			type="link"
																			className="success"
																			onClick={() => {
																				modal().restore(item.id);
																			}}
																			icon={<FontAwesomeIcon icon={faUndo} />}
																		>
																			{t('Button.Restore')}
																		</Button>,
																		<Button
																			type="link"
																			className="error"
																			onClick={() => {
																				modal().delete(item.id);
																			}}
																			icon={<FontAwesomeIcon icon={faTrashAlt} />}
																		>
																			{t('Button.Delete')}
																		</Button>
																	]}
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
						</Col>
					</Row>
				</Layout.Content>

				<Modal
					title={modal().header()}
					visible={modalState.isVisible}
					onCancel={modal().closeModal}
					centered={true}
					width={500}
					footer={false}
					getContainer={false}
					closable={false}
					className={`${modalState.type}-modal`}
				>
					<Form
						form={form}
						layout="vertical"
						name="archive"
						onFinish={onSubmit}
						scrollToFirstError
						className={`archive-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					>
						{modalState.type === 'Restore' && <p>{t('Restore Asset.Body')}</p>}

						{modalState.type === 'Delete' && <p>{t('Delete Asset Permanently.Body')}</p>}

						<Form.Item>
							<Row type="flex" className="form-actions">
								<Col xs={24} md={24} className="form-update-actions">
									<Button onClick={modal().closeModal}>{t('Button.Cancel')}</Button>
									<Button
										type="primary"
										htmlType="submit"
										className={modalState.type === 'Restore' ? 'success' : 'error'}
										icon={<FontAwesomeIcon icon={modalState.type === 'Restore' ? faUndo : faTrashAlt} />}
									>
										{t(`Button.${modalState.type}`)}
									</Button>
								</Col>
							</Row>
						</Form.Item>
					</Form>
				</Modal>
			</Layout>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {
		fileData: state.archive.fileData
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadArchivedFiles: () => dispatch(getArchive()),
		onUpdateAssetStatus: (data, status) => dispatch(updateAssetStatus(data, status))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Archive));
