import React, { memo, useState, useEffect } from 'react';
import { connect } from 'react-redux';
import {
	Row,
	Col,
	Button,
	Form,
	Modal,
	Tree,
	Spin,
	Space,
	message,
	List,
	Card,
	Tag,
	Checkbox,
	Input,
	Tooltip
} from 'antd';
import { useTranslation } from 'react-i18next';
import { PlusOutlined } from '@ant-design/icons';
import { bulkUpdateAssetTags } from '../../actions';

function BulkTagUploadModal(props) {
	const { t } = useTranslation();
	const [form] = Form.useForm();
	const [isUpdating, setIsUpdating] = useState(false);
	const [checkedAssets, setCheckedAssets] = useState([]);
	const [bulkTagValue, setBulkTagValue] = useState('');
	const [selectAll, setSelectAll] = useState(false);
	const [canApply, setCanApply] = useState(false);

	const { assets, setAssets, isOpen, onClose } = props;

	function renderLoadingText(isLoading) {
		if (isLoading) return t('Messages.Exporting');
	}

	useEffect(() => {
		if (assets.length > 0) {
			checkIfCanApply();
		}
	}, [checkedAssets, bulkTagValue]);

	function checkIfCanApply() {
		if (checkedAssets.length > 0 && bulkTagValue.length > 0) {
			setCanApply(true);
		} else {
			setCanApply(false);
		}
	}

	function onCheckedAsset(item) {
		let tempItems = checkedAssets;
		const isChecked = tempItems.filter((tempItem) => tempItem.id === item.id);
		var added = true;

		if (isChecked.length === 0) {
			setCheckedAssets((ca) => [...ca, item]);
			added = true;
		} else {
			tempItems = tempItems.filter((fId) => fId.id !== item.id);
			setCheckedAssets(tempItems);
			added = false;
		}

		if (tempItems.length === assets.length || (added && tempItems.length === 0 && assets.length === 1)) {
			setSelectAll(true);
		} else {
			setSelectAll(false);
		}
	}

	function handleSelectAll() {
		if (selectAll) {
			setCheckedAssets([]);
		} else {
			let tempItems = assets;

			if (tempItems && tempItems.length > 0) {
				setCheckedAssets(tempItems);
			}
		}
		setSelectAll(!selectAll);
	}

	async function onSubmit() {
		var updateData = {
			assetsToUpdate: assets
		};
		// call update asset tags api
		await props.bulkUpdateAssetTags(updateData);
		setSelectAll(false);
		onClose();
	}

	function applyBulkTag() {
		var tagApplied = 0;
		var checkedIds = checkedAssets.map((a) => {
			return a.id;
		});

		// filter assets
		var assetsToAddTag = [...assets];

		assetsToAddTag.forEach((a) => {
			if (checkedIds.filter((id) => id === a.id).length > 0) {
				if (a.tags.filter((tag) => tag.name === bulkTagValue).length === 0) {
					let newTag = {
						id: null,
						name: bulkTagValue,
						isCognitive: false,
						assetId: a.assetVersions[0].id
					};
					a.tags.push(newTag);
					tagApplied = tagApplied + 1;
				} else {
					message.warning(`${t('Messages.TagExists')} ${a.fileName}.`);
				}
			}
		});

		if (tagApplied > 0) {
			message.success(`${t('Messages.TagApplied')}`);
			setAssets(assetsToAddTag);
			setBulkTagValue('');
		}
	}

	function removeTag(assetId, name) {
		// filter assets
		var assetsToUpdate = [...assets];
		assetsToUpdate.forEach((a) => {
			if (a.id === assetId) {
				var tags = a.tags.filter((t) => t.name !== name);
				a.tags = tags;
			}
		});

		setAssets(assetsToUpdate);
	}

	return (
		<Modal
			title="Bulk asset tagging"
			visible={isOpen}
			centered={true}
			width={600}
			footer={false}
			getContainer={false}
			className={''}
			closable={isUpdating ? false : true}
			keyboard
			onCancel={() => {
				setSelectAll(false);
				onClose();
			}}
			maskClosable={false}
		>
			<Spin spinning={isUpdating} size="large" tip={renderLoadingText(isUpdating)}>
				<Form
					form={form}
					key={'export'}
					layout="horizontal"
					name="export"
					onFinish={onSubmit}
					scrollToFirstError
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{
						xs: 24,
						sm: 16,
						md: 16,
						lg: 16,
						xl: 16,
						xxl: 16,
						span: 16,
						style: { ...{ lineHeight: 2.2 } }
					}}
				>
					<Row>
						<h4>{t('Messages.BulkTagUploadHeaderModal')}</h4>
					</Row>

					<Row style={{ marginTop: 16 }}>
						<Col xs={8} sm={8} md={8} lg={8} xl={8} xxl={8} style={{ paddingTop: 8 }}>
							<Checkbox checked={selectAll} onClick={handleSelectAll}>
								Select All
							</Checkbox>
						</Col>

						<Col xs={16} sm={16} md={16} lg={16} xl={16} xxl={16} align="right">
							<Space>
								<Input
									value={bulkTagValue}
									onChange={(e) => setBulkTagValue(e.target.value)}
									placeholder="New Tag"
								></Input>
								<Button type="primary" disabled={!canApply} onClick={() => applyBulkTag()}>
									<PlusOutlined />
									Apply
								</Button>
							</Space>
						</Col>
					</Row>

					{assets.length > 0 && (
						<Row style={{ marginTop: 16 }}>
							<List
								className="bulk-tag-list"
								dataSource={assets}
								renderItem={(item) => (
									<List.Item className="bulk-tag-list-container list-view-card">
										<Space>
											<Checkbox.Group
												className="overlay-checkbox rounded-checkbox"
												onChange={(value) => {
													onCheckedAsset(item);
												}}
												value={checkedAssets.filter((ca) => ca.id === item.id).length > 0 ? [item.id] : []}
											>
												<Checkbox value={item.id}></Checkbox>
											</Checkbox.Group>
											<Card
												cover={
													<>
														<img alt="example" src={item.thumbnail} />
													</>
												}
											>
												<div className="card-body-container">
													<div className="card-body">
														<div className="card-details">
															<Row>
																<div className="card-text title" style={{ width: 150 }}>
																	<Tooltip title={item.name}>
																		<span>{item.name}</span>
																	</Tooltip>
																</div>
															</Row>
															<Row>
																<div className="card-text">{`${t('ModalDetail.File Extension')}: ${
																	item.extension
																}`}</div>
															</Row>
															<Row>
																<div className="card-text">{`${t('ModalDetail.File Size')}: ${item.fileSizeText}`}</div>
															</Row>
														</div>

														<div className="card-details">
															<Row>
																<div className="card-text title">{t('ModalDetail.Tags')}:</div>
															</Row>
															<Row>
																{item.tags.map((tag) => (
																	<Col key={tag.id} className="cognitive-tags-list">
																		<Tag
																			style={{ borderRadius: 40, marginTop: 5 }}
																			closable={true}
																			onClose={() => removeTag(item.id, tag.name)}
																			className={tag.isCognitive ? 'cognitive-tag' : 'user-tag'}
																			visible
																		>
																			{tag.name}
																		</Tag>
																	</Col>
																))}
															</Row>
														</div>
													</div>
												</div>
											</Card>
										</Space>
									</List.Item>
								)}
							></List>
						</Row>
					)}

					<Row type="flex" className="form-actions" style={{ marginTop: 16 }}>
						<Col xs={24} className="form-update-actions" align="right">
							<Space>
								<Button type="secondary" onClick={onClose}>
									{t('Button.Cancel')}
								</Button>

								<Button htmlType="submit" type="primary">
									{t('Button.Save')}
								</Button>
							</Space>
						</Col>
					</Row>
				</Form>
			</Spin>
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		bulkUpdateAssetTags: (data) => dispatch(bulkUpdateAssetTags(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(BulkTagUploadModal));
