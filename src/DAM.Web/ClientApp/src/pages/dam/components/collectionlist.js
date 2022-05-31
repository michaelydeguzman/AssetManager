import React, { useState, memo } from 'react';
import { connect } from 'react-redux';
import { addCollection, deleteCollection } from '../actions';
import { Row, Col, Modal, Button, Tooltip, Spin, Space, List, Input, message, Badge, Popover } from 'antd';
import { ExclamationCircleOutlined, PlusOutlined, DeleteOutlined, EyeOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

function CollectionList(props) {
	const { t } = useTranslation();
	const [isLoading, setIsLoading] = useState(false);
	const { confirm } = Modal;

	const [createNew, setCreateNew] = useState(false);
	const [newCollectionName, setNewCollectionName] = useState('');
	const { setSelectedCollection, userCollections, loadCollections, efiles } = props;

	const saveNewCollection = async () => {
		let collections = userCollections.filter((x) => x.name == newCollectionName);

		if (collections.length > 0) {
			message.error(t('Messages.Collection Exists'));
			return;
		}

		setIsLoading(true);

		const data = {
			assetIds: [],
			name: newCollectionName
		};

		await props.addCollection(data);

		setIsLoading(false);
		loadCollections();
		clear();
	};

	const deleteCollection = async (collectionId) => {
		setIsLoading(true);
		const data = {
			id: collectionId
		};
		await props.deleteCollection(data);
		setIsLoading(false);
		loadCollections();
		setSelectedCollection(null);
	};

	const showConfirm = (e, collectionId) => {
		confirm({
			title: t('Confirm Delete'),
			content: t('Messages.Delete Collection'),
			icon: <ExclamationCircleOutlined />,
			okButtonProps: { typeof: 'primary', className: 'error' },
			onOk() {
				deleteCollection(collectionId);
			},
			onCancel() {}
		});
	};

	const clear = () => {
		setCreateNew(false);
		setNewCollectionName('');
	};

	const selectCollection = (item) => {
		setSelectedCollection(item);

		userCollections.forEach((collection) => {
			if (item.id !== collection.id) {
				var li = document.getElementById(`${collection.id}-li`);
				li.style.backgroundColor = 'white';
			}
		});

		var li = document.getElementById(`${item.id}-li`);
		li.style.backgroundColor = '#c6c8c540';
	};

	const renderPopoverContent = (assetIds) => {
		if (efiles) {
			var filterAssets = efiles.filter((x) => {
				if (assetIds.filter((id) => id === x.id).length > 0) {
					return x;
				}
			});

			return (
				<List
					dataSource={filterAssets}
					grid={{ gutter: 16, column: 4 }}
					renderItem={(item) => (
						<List.Item style={{ width: 200 }}>
							<Row>
								<img alt="example" src={item.thumbnail} style={{ width: '100%' }} />
							</Row>
							<Row>
								<span style={{ fontSize: 11 }}>{item.name}</span>
							</Row>
						</List.Item>
					)}
				></List>
			);
		} else {
			return '';
		}
	};

	return (
		<Spin spinning={isLoading}>
			{!createNew && (
				<Row style={{ marginTop: 10 }}>
					<Button className="option-button" onClick={() => setCreateNew(true)} icon={<PlusOutlined />}>
						{t('Collection.Create')}
					</Button>
				</Row>
			)}
			{createNew && (
				<Row>
					<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={14} align="left" style={{ marginTop: 10 }}>
						<Input
							type="text"
							placeholder={t('Collection.CreatePlaceholder')}
							value={newCollectionName}
							onChange={(e) => setNewCollectionName(e.target.value)}
						/>
					</Col>
					<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={10} align="right" style={{ marginTop: 10 }}>
						<Space>
							<Button type="secondary" onClick={clear}>
								{t('Button.Cancel')}
							</Button>
							<Button type="primary" onClick={saveNewCollection} disabled={newCollectionName.length === 0}>
								{t('Button.Save')}
							</Button>
						</Space>
					</Col>
				</Row>
			)}
			<List
				size="small"
				style={{ marginTop: 20 }}
				bordered
				dataSource={userCollections}
				renderItem={(item) => (
					<List.Item id={item.id + '-li'} onClick={(e) => selectCollection(item)} className="collection-list-item">
						<Col>
							<Popover
								title={'Items in Cart (' + item.assetIds.length + ')'}
								trigger={efiles ? 'hover' : ''}
								placement="right"
								content={() => renderPopoverContent(item.assetIds)}
								overlayStyle={{
									width: '470px'
								}}
							>
								<Badge
									count={item.assetIds.length}
									className="badge-collection"
									size="small"
									style={{
										backgroundColor: 'green'
									}}
								>
									<span id={item.id + '-span'} style={{ marginRight: 12 }}>
										{item.name}
									</span>
								</Badge>
							</Popover>
						</Col>
						<Col>
							<Space>
								{efiles && (
									<Button
										type="link"
										onClick={(e) => {
											window.open(window.location.origin + '/collections/' + item.id);
										}}
										icon={<EyeOutlined />}
									/>
								)}
								<Button
									type="link"
									className="error"
									onClick={(e) => showConfirm(e, item.id)}
									icon={<DeleteOutlined id={item.id + '-delete'} />}
								/>
							</Space>
						</Col>
					</List.Item>
				)}
			/>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {
		folderRefresh: state.dam.folderRefresh
	};
}

function mapDispatchToProps(dispatch) {
	return {
		addCollection: (data) => dispatch(addCollection(data)),
		deleteCollection: (data) => dispatch(deleteCollection(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(CollectionList));
