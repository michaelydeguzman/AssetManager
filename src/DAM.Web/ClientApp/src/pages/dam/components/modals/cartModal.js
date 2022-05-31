import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Spin, Input, List, Space, Checkbox } from 'antd';
import { FullscreenOutlined } from '@ant-design/icons';
import { getUserRole, getUser } from '@damtoken';
import { addCollection, getCarts, getCurrentCart, deleteCollection } from '../../actions';

function CartModal(props) {
	const {
		modal,
		modalState,
		form,
		setSelectedAssetID,
		setSelectedShareAssets,
		setShareModalState,
		setCheckedAssetsItem,
		checkedAssets,
		setCheckedAssets,
		efiles,
		selectedCart,
		carts,
		setCarts
	} = props;

	const userRole = getUserRole();
	const user = getUser();
	const [isLoading, setIsLoading] = useState(true);

	//IDs
	const [cartID, setCartID] = useState(0);
	const [cartAsset, setCartAsset] = useState([]);
	const [cartCheckedAsset, setCartCheckedAsset] = useState([]);

	//actual file lists
	const [assetItems, setAssetItems] = useState([]);
	const [cartCheckedAssetItem, setCartCheckedAssetItem] = useState([]);

	const [cartName, setCartName] = useState(null);

	const [isNameModalVisible, setIsNameModalVisible] = useState(false);

	const handleNameChange = (e) => {
		setCartName(e.target.value);
	};

	useEffect(() => {
		if (selectedCart) {
			//load the cart passed in.
			let savedAssets = selectedCart.assetIds;
			let id = selectedCart.id;
			setCartID(id);
			setCartName(selectedCart.name);

			let items = efiles.filter((f) => savedAssets.includes(f.id));
			let fieldValues = {};

			setAssetItems([...items]);
			setCartCheckedAssetItem([...items]);

			setCartCheckedAsset([...savedAssets]);
			setCartAsset([...savedAssets]);

			items.forEach((file) => {
				fieldValues[file.id] = file.name;
			});

			fieldValues['cart-name'] = selectedCart.name;

			form.setFieldsValue(fieldValues);

			setIsLoading(false);
		} //load the users current cart.
		else {
			props.getCurrentCart().then((cartDto) => {
				let assets;
				let fieldValues = {};
				if (cartDto) {
					let savedAssets = cartDto.assetIds;
					let id = cartDto.id;
					assets = arrayUnique(checkedAssets.concat(savedAssets));
					setCartID(id);
					setCartName(cartDto.name);
					fieldValues['cart-name'] = cartDto.name;
				} else {
					assets = checkedAssets;
				}

				let items = efiles.filter((f) => assets.includes(f.id));

				setAssetItems([...items]);
				setCartCheckedAssetItem([...items]);

				setCartCheckedAsset([...assets]);
				setCartAsset([...assets]);

				items.forEach((file) => {
					fieldValues[file.id] = file.name;
				});

				form.setFieldsValue(fieldValues);

				setIsLoading(false);
			});
		}
	}, [modalState.type, form]);

	function removeItemFromCart(id) {
		let newAssetItems = assetItems.filter((f) => f.id !== id);
		setAssetItems([...newAssetItems]);

		let newCartCheckedAssetItems = cartCheckedAssetItem.filter((f) => f.id !== id);
		setCartCheckedAssetItem([...newCartCheckedAssetItems]);

		let newCartCheckedAsset = cartCheckedAsset.filter((assetID) => assetID !== id);
		setCartCheckedAsset([...newCartCheckedAsset]);

		let newCartAsset = cartAsset.filter((assetID) => assetID !== id);
		setCartAsset([...newCartAsset]);
	}

	function removeAllItemsFromCart() {
		setAssetItems([]);
		setCartCheckedAssetItem([]);
		setCartCheckedAsset([]);
		setCartAsset([]);
	}

	const download = () => {
		let cartCheckedItem = cartCheckedAssetItem.filter((item) => cartCheckedAsset.includes(item.id));

		if (cartCheckedItem.length > 0) {
			modal().download(null, cartCheckedItem);
		}

		modal().closeModal();
	};

	async function handleSaveCart() {
		const data = {
			UserID: user.id,
			assetIds: [...cartCheckedAsset],
			name: cartName
		};

		await props.addCollection(data).then(() => {
			console.log('addCollection_data', data);
		});

		modal().closeModal();
		window.location.reload(); //not sure how else to refresh the sider's cart list from here
	}

	async function updateCart() {
		let isCurrent = selectedCart ? false : true;

		const data = {
			id: cartID,
			isCurrentCart: isCurrent,
			UserID: user.id,
			assetIds: [...cartAsset],
			name: cartName
		};

		if (cartID === 0 && cartAsset.length === 0) {
			return;
		}

		if (!isCurrent && cartAsset.length === 0) {
			await deleteCollection();
		} else {
			await props.addCollection(data).then(() => {
				console.log('updateCart_data', data);
			});
		}
	}

	async function handleGetCarts() {
		const data = {
			UserID: user.id
		};

		await props.getCarts(data).then(() => {
			console.log('handleGetCarts_data', data);
		});
	}

	async function deleteCollection() {
		const data = {
			id: cartID
		};

		await props.deleteCollection(data).then(() => {
			console.log('deleteCollection_data', data);
		});

		let newCartSet = carts.filter((cart) => cart.id !== selectedCart.id);

		setCarts([...newCartSet]);

		modal().closeModal();
	}

	const onCancel = () => {
		updateCart().then(() => modal().closeModal());
	};

	function clearOrDeleteButton() {
		if (selectedCart) {
			return (
				<Button
					type="primary"
					style={{ width: '96px' }}
					onClick={() => {
						deleteCollection();
					}}
				>
					Delete
				</Button>
			);
		} else {
			return (
				<Button
					type="primary"
					style={{ width: '96px' }}
					onClick={() => {
						removeAllItemsFromCart();
					}}
				>
					Clear
				</Button>
			);
		}
	}

	function nameModalCancel() {
		setIsNameModalVisible(false);
		setCartName(null);
	}

	const collectionTitle = () => {
		return (
			<Row>
				<Col span={6}>
					<h1>Collection</h1>
				</Col>
				<Space>
					{cartCheckedAsset.length > 0 && (
						<>
							<Col>
								{userRole.canMove && (
									<Col>
										<Button
											className="option-button"
											onClick={() => {
												setCheckedAssets([...cartCheckedAsset]);
												setCheckedAssetsItem([...cartCheckedAssetItem]);
												modal().move();
											}}
										>
											<FullscreenOutlined rotate={45} />
											Move
										</Button>
									</Col>
								)}
							</Col>
							<Col>
								<Button onClick={download} type="primary" style={{ width: '96px' }}>
									Download
								</Button>
							</Col>
							<Col>
								{userRole.canShare && (
									<Button
										type="primary"
										style={{ width: '96px' }}
										onClick={() => {
											setSelectedAssetID(cartCheckedAssetItem[0].id);
											let shareAssets = cartCheckedAssetItem.filter((item) => cartCheckedAsset.includes(item.id));
											setSelectedShareAssets([...shareAssets]);
											setShareModalState(true);
										}}
									>
										Share
									</Button>
								)}
							</Col>

							<Col>
								<Col>
									<Button
										type="primary"
										onClick={() => {
											setIsNameModalVisible(true);
										}}
										style={{ width: '96px' }}
									>
										Save
									</Button>
								</Col>
							</Col>

							<Col>
								<Col>{clearOrDeleteButton()}</Col>
							</Col>
						</>
					)}
				</Space>
			</Row>
		);
	};

	return (
		<Spin spinning={isLoading} size="large">
			<Form form={form} layout="horizontal">
				<Modal
					title={collectionTitle()}
					visible={modalState.isVisible && modalState.type === 'addToCart'}
					onCancel={onCancel}
					centered={true}
					width={580}
					footer={false}
					getContainer={false}
					closable={false}
					className={`${modalState.type}-modal`}
				>
					<List
						bordered
						dataSource={assetItems}
						renderItem={(item) => (
							<Row type="flex" justify="space-between" align="middle" style={{ padding: '8px' }}>
								<Form.Item>
									<Checkbox
										checked={cartCheckedAsset.includes(item.id)}
										onChange={(e) => {
											if (!cartCheckedAsset.includes(item.id)) {
												let newCartChecked = cartCheckedAsset;
												newCartChecked.push(item.id);
												setCartCheckedAsset([...newCartChecked]);
											} else {
												let newCartChecked = cartCheckedAsset.filter((id) => id !== item.id);
												setCartCheckedAsset([...newCartChecked]);
											}
										}}
									></Checkbox>
								</Form.Item>

								<Col span={6}>{<img alt="simple file" loading="lazy" src={item.thumbnail} width={'100%'} />}</Col>

								<Col span={14}>
									<Form.Item
										label="File Name:"
										name={item.id}
										rules={[
											{
												required: true,
												message: 'Please input display name!'
											}
										]}
									>
										<Input disabled={true} />
									</Form.Item>
									<Button
										onClick={(e) => {
											removeItemFromCart(item.id);
										}}
									>
										{' '}
										Remove
									</Button>
								</Col>
							</Row>
						)}
					/>
				</Modal>

				<Modal
					title="Input name of Collection"
					visible={isNameModalVisible}
					onOk={handleSaveCart}
					onCancel={nameModalCancel}
				>
					<Form.Item
						name={'cart-name'}
						value={cartName}
						onChange={handleNameChange}
						rules={[
							{
								required: true,
								message: 'Please input display name!'
							}
						]}
					>
						<Input />
					</Form.Item>
				</Modal>
			</Form>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		addCollection: (data) => dispatch(addCollection(data)),
		getCarts: (data) => dispatch(getCarts(data)),
		getCurrentCart: () => dispatch(getCurrentCart()),
		deleteCollection: (data) => dispatch(deleteCollection(data))
	};
}

function arrayUnique(array) {
	var a = array.concat();
	for (var i = 0; i < a.length; ++i) {
		for (var j = i + 1; j < a.length; ++j) {
			if (a[i] === a[j]) a.splice(j--, 1);
		}
	}

	return a;
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(CartModal));
