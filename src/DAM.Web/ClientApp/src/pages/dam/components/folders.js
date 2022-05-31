import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import lodash from 'lodash';
import { USER_ROLES } from '../../constants';
import { bulkMoveFolders, orderFolders, getFolders, getPinFolders, addPinFolders, removePinFolders } from '../actions';
import { NestedChild, NestedChildUser } from '@damhelper';
import { Row, Tree, Col, Modal, Button, Tooltip, Spin } from 'antd';
import {
	FolderOutlined,
	FolderOpenOutlined,
	BankOutlined,
	ExclamationCircleOutlined,
	FullscreenOutlined,
	PushpinOutlined,
	PushpinFilled
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { LowFrequencyContext } from '@damcontext';
function Folders(props) {
	const { pinnedFolders, setPinnedFolders } = useContext(LowFrequencyContext);

	const { t } = useTranslation();
	const {
		expandThreeKeys,
		treeState,
		selectTreeKey,
		currentUser,
		touchFolder,
		setTouchFolder,
		treeDispatch,
		folderId,
		folderData,
		efiles
	} = props;
	const [folderDataSource, setFolderDataSource] = useState([]);
	const [folderOrderDataSource, setFolderOrderDataSource] = useState([]);
	const [isOrdering, setIsOrdering] = useState(false);
	const [sortedData, setSortedData] = useState([]);
	const [moveList, setMoveList] = useState([]);
	const [orderList, setOrderList] = useState([]);
	const [isLoading, setIsLoading] = useState(false);

	const { confirm } = Modal;

	useEffect(() => {
		if (currentUser) {
			loadFoldersAndFiles();
		}
	}, [currentUser, folderId, pinnedFolders, folderData]);

	useEffect(() => {
		if (props.folderRefresh) {
			loadFoldersAndFiles(true);
		}
	}, [props.folderRefresh]);

	async function loadFoldersAndFiles(refresh = false) {
		setIsLoading(true);
		var tempFolders = folderData ? { data: { folders: folderData } } : await props.loadFolders(refresh);
		const data =
			tempFolders.data.folders &&
			tempFolders.data.folders.map((folder) => {
				return {
					key: folder.id,
					id: folder.id,
					title: customTreeTitle(
						folder.id,
						folder.folderName,
						efiles.filter(
							(file) =>
								file.folderId === folder.id ||
								(file.shareFolderIds && folder.id && file.shareFolderIds.split(',').includes(folder.id.toString()))
						).length
					),
					parentFolderId: folder.parentFolderId,
					accounts: folder.accounts,
					countries: folder.countries,
					regions: folder.regions,
					comments: folder.comments,
					folderName: folder.folderName,
					company: folder.company,
					orderNumber: folder.orderNumber
				};
			});
		var tempData = lodash.cloneDeep(data);
		const result = mainNestedChild(tempData);
		setFolderDataSource(result);
		setFolderOrderDataSource(result);
		setSortedData(data);
		setIsLoading(false);
		if (folderId > 0) {
			treeDispatch({ type: 'EXPANDED_KEYS', payload: [Number(folderId)] });
			selectTreeKey([Number(folderId)]);
		}
	}
	async function putPinnedFolder(item) {
		var data = {
			folderId: item.id
		};
		await props.addPinFolders(data);
	}
	async function deletePinnedFolder(item) {
		var data = {
			folderId: item.id
		};
		await props.removePinFolders(data);
	}
	function idExists(id, object) {
		return object.some((el) => {
			return el.id === id;
		});
	}
	const customTreeTitle = (id, folderName, assetCount) => {
		return (
			<>
				<Tooltip color="#000" placement="right" title={folderName + ' (' + assetCount + ')'}>
					<span style={{ textTransform: 'none' }}>{folderName}</span>
				</Tooltip>
				<a
					style={{ position: 'absolute', right: '0', paddingRight: '12px', paddingTop: '2px' }}
					onClick={() => {
						setPinnedFolders((state) => {
							if (idExists(id, [...state])) {
								var prevState = [...state];
								var index = prevState.findIndex((x) => x.id === id);
								if (index > -1) {
									prevState.splice(index, 1);
								}
								deletePinnedFolder({ id, folderName });
								return prevState;
							} else {
								if ([...state].length < 5) {
									putPinnedFolder({ id, folderName });
									return [...state, { id, folderName }];
								} else {
									Modal.error({
										title: 'Limit of Pinned Folders reached',
										content: 'A maximum of 5 folders can be pinned. Please unpin to add more.'
									});
									return [...state];
								}
							}
						});
					}}
				>
					{idExists(id, pinnedFolders) ? <PushpinFilled /> : <PushpinOutlined />}
				</a>
			</>
		);
	};

	function showConfirm() {
		confirm({
			title: t('Leave Order Folder.Title'),
			content: t('Leave Order Folder.Body'),
			icon: <ExclamationCircleOutlined />,
			onOk() {
				setIsOrdering(!isOrdering);
				expandThreeKeys([1]);
			},
			onCancel() {}
		});
	}

	function mainNestedChild(array) {
		setIsLoading(true);
		let root = 0;
		let result = [];
		if (currentUser) {
			root = currentUser.rootFolderId;
			for (let arrayKey in array) {
				//for Admin and User which didn't set specific folder access
				if (
					(currentUser && currentUser.userRole.name === USER_ROLES.DAM_ADMIN) ||
					(currentUser && currentUser.userRole.name === USER_ROLES.USER && currentUser.userFolders.length === 0)
				) {
					if (array[arrayKey].parentFolderId === root) {
						const children = NestedChild(array, array[arrayKey].key);
						if (children.length) {
							children.sort((a, b) => {
								return a.orderNumber - b.orderNumber;
							});
							array[arrayKey].children = children;
						}
						result.push(array[arrayKey]);
					}
					//for User which set the specific folder access
				} else if (currentUser && currentUser.userRole.name === USER_ROLES.USER && currentUser.userFolders.length > 0) {
					const userFoldersList = currentUser.userFolders.map((item) => {
						return item.folderId;
					});
					if (array[arrayKey].parentFolderId === root) {
						const children = NestedChildUser(array, array[arrayKey].key, userFoldersList);
						if (children.length) {
							var unique = Array.from(new Set(children));
							unique.sort((a, b) => {
								return a.orderNumber - b.orderNumber;
							});
							array[arrayKey].children = unique;
						}

						result.push(array[arrayKey]);
					}
					//for Subscriber
				} else if (
					currentUser &&
					currentUser.userRole.name === USER_ROLES.SUBSCRIBER &&
					currentUser.userFolders.find((folder) => folder.folderId === root) &&
					array[arrayKey].id === root
				) {
					result.push(array[arrayKey]);
					//for Company Admin
				} else {
					let idToBeUsed =
						currentUser && currentUser.userRole.name === USER_ROLES.COMPANY_ADMIN
							? array[arrayKey].id
							: array[arrayKey].parentFolderId;
					if (idToBeUsed === root) {
						const children = NestedChild(array, array[arrayKey].key);
						if (children.length) {
							children.sort((a, b) => {
								return a.orderNumber - b.orderNumber;
							});
							array[arrayKey].children = children;
						}

						result.push(array[arrayKey]);
					}
				}
			}
			if (currentUser && currentUser.userRole.name === USER_ROLES.SUBSCRIBER) {
				let userFolders = currentUser.userFolders.map((f) => {
					return f.folderId;
				});
				var filteredResult = result.filter((arr) => {
					return userFolders.includes(arr.id);
				});
				result = filteredResult;
				if (touchFolder === root) {
					setTouchFolder(result[0].id);
				}
			} else {
				if (result.length > 0) {
					if (touchFolder === root) {
						setTouchFolder(result[0].id);
					}
				}
			}
		}
		if (result.length > 0 && result[0].children) {
			result[0].children.map((item) => {
				if (item.id === 2) {
					if (item.children) {
						item.children.sort((a, b) => {
							if (a.orderNumber !== b.orderNumber) {
								return a.orderNumber - b.orderNumber;
							}
							var nameA = a.folderName.toLowerCase(); // ignore upper and lowercase
							var nameB = b.folderName.toLowerCase(); // ignore upper and lowercase
							if (nameA < nameB) {
								return -1;
							}
							if (nameA > nameB) {
								return 1;
							}
							// names must be equal
							return 0;
						});
					} else {
						return 0;
					}
				}
			});
		}
		setIsLoading(false);
		return result;
	}

	function onDragEnd(o) {}

	function onDragEnter(o) {
		expandThreeKeys(o.expandedKeys);
	}

	function onDragLeave(o) {}

	function onDragOver(o) {}

	function onDragStart(o) {}

	function onDrop(o) {
		console.log('onDrop');
		console.log(o);
		setIsLoading(true);
		const dragPos = o.dragNode.pos.split('-');
		const nodePos = o.node.pos.split('-');
		if (o.dragNode.id > 2 && o.node.id !== 1) {
			var originalData = sortedData;
			if (originalData.find((f) => f.id === o.dragNode.id)) {
				//move across levels
				if (o.node.dragOver) {
					console.log('dragOver');
					var newElement = originalData.find((f) => f.id === o.dragNode.id);
					newElement.parentFolderId = o.node.id;
					originalData.splice(originalData.indexOf(newElement), 1, newElement);
				}
				if (!o.node.dragOver && checkMoveAcrossLevels(dragPos, nodePos)) {
					console.log('moveAcross');
					var newElement = originalData.find((f) => f.id === o.dragNode.id);
					newElement.parentFolderId = o.node.parentFolderId;
					originalData.splice(originalData.indexOf(newElement), 1, newElement);
				}
				//order
				var subNodes = originalData.filter((f) => f.parentFolderId === o.node.parentFolderId);
				if (o.node.parentFolderId === 2) {
					subNodes.sort((a, b) => {
						if (a.orderNumber !== b.orderNumber) {
							return a.orderNumber - b.orderNumber;
						}
						var nameA = a.folderName.toLowerCase(); // ignore upper and lowercase
						var nameB = b.folderName.toLowerCase(); // ignore upper and lowercase
						if (nameA < nameB) {
							return -1;
						}
						if (nameA > nameB) {
							return 1;
						}
						// names must be equal
						return 0;
					});
				} else {
					subNodes.sort((a, b) => {
						return a.orderNumber - b.orderNumber;
					});
				}
				if (o.node.dragOverGapTop) {
					console.log('dragTop');
					console.log(o.node.id);
					var newElement = subNodes.find((f) => f.id === o.dragNode.id);
					var beforeElement = subNodes.find((f) => f.id === o.node.id);
					var insertPosition = beforeElement.orderNumber;
					var index = subNodes.indexOf(beforeElement);
					subNodes.forEach((n, i) => {
						if (i >= index) {
							n.orderNumber += 1;
						}
					});
					newElement.orderNumber = insertPosition;
					originalData.splice(originalData.indexOf(newElement), 1, newElement);
				}
				if (o.node.dragOverGapBottom) {
					console.log('dragBottom');
					console.log(o.node.id);
					var newElement = subNodes.find((f) => f.id === o.dragNode.id);
					var beforeElement = subNodes.find((f) => f.id === o.node.id);
					var insertPosition = beforeElement.orderNumber + 1;
					var index = subNodes.indexOf(beforeElement);
					subNodes.forEach((n, i) => {
						if (i > index) {
							n.orderNumber += 2;
						}
					});
					newElement.orderNumber = insertPosition;
					originalData.splice(originalData.indexOf(newElement), 1, newElement);
				}
				var tempData = lodash.cloneDeep(originalData);
				const result = mainNestedChild(tempData);
				setSortedData(originalData);
				setFolderOrderDataSource(result);
			}
			//如果是移动层级，执行move文件夹操作API的list
			if (o.node.dragOver || checkMoveAcrossLevels(dragPos, nodePos)) {
				var newElement = {
					folderId: o.dragNode.id,
					parentFolderId: o.node.dragOver ? o.node.id : o.node.parentFolderId
				};
				setMoveList((prevList) => {
					var newList = prevList;
					if (newList.find((m) => m.folderId === o.dragNode.id)) {
						var prevElement = newList.find((m) => m.folderId === o.dragNode.id);
						newList.splice(newList.indexOf(prevElement), 1);
					}
					newList.push(newElement);
					return newList;
				});
			}
			console.log(subNodes);
			//做orderlist的保存，去重
			if (orderList.length === 0) {
				let list = [];
				subNodes.forEach((n) => {
					const element = {
						folderId: n.id,
						orderNumber: n.orderNumber
					};
					list.push(element);
				});
				setOrderList(list);
			} else {
				let list = orderList;
				subNodes.forEach((n) => {
					if (list.find((f) => f.folderId === n.id)) {
						let newElement = list.find((f) => f.folderId === n.id);
						newElement.orderNumber = n.orderNumber;
						list.splice(list.indexOf(newElement), 1, newElement);
					} else {
						const element = {
							folderId: n.id,
							orderNumber: n.orderNumber
						};
						list.push(element);
					}
				});
				setOrderList(list);
			}
		}
		setIsLoading(false);
	}

	async function saveFolderOrder() {
		console.log('move list:');
		console.log(moveList);
		console.log('order list:');
		console.log(orderList);
		setIsLoading(true);
		//如果有movelist做后端保存
		if (moveList.length > 0) {
			await props.bulkMoveFolders(moveList);
		}
		//如果有orderlist做后端保存;
		if (orderList.length > 0) {
			await props.orderFolders(orderList);
		}
		//刷新
		setFolderOrderDataSource([]);
		setSortedData([]);
		setMoveList([]);
		setOrderList([]);
		loadFoldersAndFiles(true);
		setIsOrdering(false);
		setIsLoading(false);
	}

	function checkMoveAcrossLevels(dragNode, node) {
		if (dragNode.length !== node.length) {
			return true;
		}
		let levelsFlag = false;
		const positionLevels = dragNode.length;
		for (var i = 0; i < positionLevels - 1; i++) {
			if (dragNode[i] !== node[i]) {
				levelsFlag = true;
			}
		}
		return levelsFlag;
	}

	return (
		<Spin spinning={isLoading}>
			<Row justify="space-between">
				<Col>
					<Button
						className="option-button"
						onClick={() => {
							if (isOrdering) {
								showConfirm();
							} else {
								setIsOrdering(!isOrdering);
							}
						}}
						icon={<FullscreenOutlined rotate={45} />}
					>
						{t('Button.Order')}
					</Button>
				</Col>
				<Col>
					{isOrdering && (
						<Button type="primary fade-in" onClick={saveFolderOrder}>
							{t('Button.Save')}
						</Button>
					)}
				</Col>
			</Row>
			<Row style={{ marginTop: 10 }}>
				<Tree
					icon={(folderNode) => {
						if (isOrdering) {
							return <FullscreenOutlined rotate={45} className="folder-order-icon" />;
						}
						if (folderNode.company && folderNode.company.length > 0) {
							return <BankOutlined />;
						} else {
							return folderNode.expanded ? <FolderOpenOutlined /> : <FolderOutlined />;
						}
					}}
					onExpand={expandThreeKeys}
					expandedKeys={treeState.expandedKeys && treeState.expandedKeys.length > 0 ? treeState.expandedKeys : [1]}
					onSelect={isOrdering ? () => {} : selectTreeKey}
					selectedKeys={treeState.selectedKeys}
					treeData={isOrdering ? folderOrderDataSource : folderDataSource}
					style={{ backgroundColor: 'white', width: '100%' }}
					autoExpandParent={treeState.autoExpandParent}
					showIcon
					draggable={isOrdering}
					onDragEnd={onDragEnd}
					onDragEnter={onDragEnter}
					onDragLeave={onDragLeave}
					onDragOver={onDragOver}
					onDragStart={onDragStart}
					onDrop={onDrop}
				/>
			</Row>
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
		bulkMoveFolders: (data) => dispatch(bulkMoveFolders(data)),
		orderFolders: (data) => dispatch(orderFolders(data)),
		loadFolders: (refresh) => dispatch(getFolders(refresh)),
		addPinFolders: (data) => dispatch(addPinFolders(data)),
		removePinFolders: (data) => dispatch(removePinFolders(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Folders));
