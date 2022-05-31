import React, { useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { USER_ROLES } from '../constants';
import { NestedChildUser } from '@damhelper';
import { Row, Tree } from 'antd';
import { FolderOutlined, FolderOpenOutlined, BankOutlined } from '@ant-design/icons';
import useTree from '@damhookuseTree';
import { history } from '@damhistory';

function PinFolders(props) {
	const { efolders, currentUser, folderIdList } = props;
	const [treeState, treeDispatch] = useTree();
	const [folderDataSource, setFolderDataSource] = useState();
	const unique = (value, index, self) => {
		return self.indexOf(value) === index;
	};

	useEffect(() => {
		var result = mainNestedChild(efolders);
		var allKeys = nestedSubFolders(efolders, 0);
		treeDispatch({ type: 'EXPANDED_KEYS', payload: allKeys.flat(99) });
		setFolderDataSource(result);
	}, [efolders]);

	function mainNestedChild(array) {
		const uniqFolderIdList = folderIdList.filter(unique);

		let filterFolderIds = [];
		if (currentUser && currentUser.userRole.name === USER_ROLES.USER && currentUser.userFolders.length > 0) {
			filterFolderIds = uniqFolderIdList.filter((id) => currentUser.userFolders.find((u) => u.folderId === id));
		} else if (
			currentUser &&
			currentUser.userRole.name === USER_ROLES.SUBSCRIBER &&
			currentUser.userFolders.length > 0
		) {
			filterFolderIds = uniqFolderIdList.filter((id) => currentUser.userFolders.find((u) => u.folderId === id));
		} else if (
			currentUser &&
			currentUser.userRole.name === USER_ROLES.COMPANY_ADMIN &&
			currentUser.userFolders.length > 0
		) {
			var companyFolder = efolders.find((f) => f.id === currentUser.companyId);
			var companySubFolderIds = nestedSubFolders(efolders, companyFolder.Id);
			filterFolderIds = uniqFolderIdList.filter((id) => companySubFolderIds.find((u) => u === id));
		} else {
			filterFolderIds = uniqFolderIdList;
		}
		let root = 0;
		let result = [];
		if (currentUser) {
			root = currentUser.rootFolderId;
			for (let arrayKey in array) {
				if (uniqFolderIdList.length > 0) {
					if (array[arrayKey].parentFolderId === root) {
						const children = NestedChildUser(array, array[arrayKey].key, filterFolderIds);
						if (children.length) {
							array[arrayKey].children = children;
						}

						result.push(array[arrayKey]);
					}
				}
			}
		}
		return result;
	}

	function nestedSubFolders(folderlist, rootFolderId) {
		var list = [];
		for (let key in folderlist) {
			if (folderlist[key].parentFolderId === rootFolderId) {
				var ids = nestedSubFolders(folderlist, folderlist[key].id);
				if (ids.length) {
					list.push(ids);
				}
				list.push(folderlist[key].id);
			}
		}
		return list;
	}

	function selectFolderTreeNode(o) {
		history.push(`/assets/folder/${o}`);
	}

	return (
		<Row style={{ marginTop: 20 }}>
			<Tree
				icon={(folderNode) => {
					if (folderNode.company && folderNode.company.length > 0) {
						return <BankOutlined />;
					} else {
						return folderNode.expanded ? <FolderOpenOutlined /> : <FolderOutlined />;
					}
				}}
				onSelect={selectFolderTreeNode}
				treeData={folderDataSource}
				style={{ backgroundColor: 'white', width: '100%' }}
				showIcon
				expandedKeys={treeState.expandedKeys.length > 0 ? treeState.expandedKeys : [1]}
				autoExpandParent
			/>
		</Row>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(PinFolders));
