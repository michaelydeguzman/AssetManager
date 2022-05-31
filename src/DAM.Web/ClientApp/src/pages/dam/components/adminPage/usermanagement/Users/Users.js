import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { Card, Input, Tag, Table, Avatar, Button, Spin } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import UserProfile from './UserProfile';
import { getUsers, getUsersByCompany } from '../../../../actions';
import { ROLES } from '../../../../../constants';
import _ from 'lodash';
import { getUser, getUserRole } from '@damtoken';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, fas } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';

function Users(props) {
	const { t } = useTranslation();
	const [isUserProfile, setIsUserProfile] = useState(false);
	const [selectedUser, setSelectedUser] = useState([]);
	const [userList, setUserList] = useState([]);
	const [filtereduserList, setFilteredUserList] = useState([]);
	const [tableFilter, setTableFilter] = useState([]);
	const [companyFilter, setCompanyFilter] = useState([]);
	const [search, setSearch] = useState('');
	const [userRole, setUserRole] = useState(getUserRole());
	const [isLoading, setIsLoading] = useState(false);

	let user = getUser();

	useEffect(() => {
		setupData();
	}, []);

	useEffect(() => {
		if (search.trim() == '') {
			setFilteredUserList(userList);
		} else {
			const filteredList = filterResultsBySearchText(userList, search.trim());
			setFilteredUserList(filteredList);
		}
	}, [search]);

	function filterResultsBySearchText(userList, keyword) {
		return userList.filter(
			(user) =>
				user.userName.toLowerCase().includes(keyword.toLowerCase()) ||
				user.email.toLowerCase().includes(keyword.toLowerCase())
		);
	}
	const setDataSourceAndFilters = (data) => {
		let dataSource = [];
		let userRoles = [];
		let companyList = [];
		data.map((user) => {
			let folders = [];
			let folderIds = [];

			if (parseInt(user.userRoleId) == 1) {
				folders.push(user.companyName);
			} else if (parseInt(user.userRoleId) == 2) {
				folders.push(user.companyName);
			} else {
				user.userFolders.map((x) => {
					folders.push(x.folder.folderName);
					folderIds.push(x.folder.id);
				});
			}

			dataSource.push({
				id: user.id,
				userName: user.userName,
				email: user.email,
				active: user.active ? 'Active' : 'Inactive',
				userRoleId: user.userRoleId,
				userRole: ROLES[user.userRoleId - 1],
				password: user.password,
				imageUrl: user.imageUrl,
				imageFileExtension: user.imageFileExtension,
				companyName: user.companyName,
				companyId: user.companyId,
				userFolders: folders,
				userFolderIds: folderIds
			});
			if (companyList.indexOf(user.companyName) === -1) {
				companyList.push(user.companyName);
			}
		});
		ROLES.forEach((role) =>
			userRoles.push({
				text: role,
				value: role
			})
		);
		setUserList(dataSource);
		setFilteredUserList(dataSource);
		setTableFilter(userRoles);

		setCompanyFilter(
			companyList.map((company) => {
				return {
					text: company,
					value: company
				};
			})
		);
	};

	async function setupData() {
		// Get current login user company
		setIsLoading(true);
		let loggedInUser = await props.getUsers(user.id, window.location.href.includes('/admin/'));
		if (loggedInUser) {
			let userDetails = loggedInUser.data.users;

			// Fetch users
			if (userRole.id == 1) {
				loggedInUser = await props.getUsers();
			} else if (userRole.id == 2) {
				loggedInUser = await props.getUsersByCompany(userDetails.companyId);
			}

			setDataSourceAndFilters(loggedInUser.data.users);
		}
		setIsLoading(false);
	}

	const columns = [
		{
			width: 100,
			dataIndex: 'imageUrl',
			key: 'imageUrl',
			render: (imageUrl) => <Avatar size={44} src={imageUrl} icon={<UserOutlined />} />
		},
		{
			width: 100,
			title: 'Username',
			dataIndex: 'userName',
			key: 'userName',
			sorter: (a, b) =>
				a.userName.localeCompare(b.userName, undefined, {
					numeric: true,
					sensitivity: 'base'
				}),
			sortDirections: ['ascend', 'descend']
		},
		{
			width: 200,
			title: 'Email',
			dataIndex: 'email',
			key: 'email'
		},
		{
			width: 100,
			title: 'Company',
			dataIndex: 'companyName',
			key: 'companyName',
			filters: companyFilter,
			onFilter: (value, record) => record.companyName.indexOf(value) === 0
		},
		{
			width: 200,
			title: 'Folder Access',
			dataIndex: 'userFolders',
			key: 'userFolders',
			render: (data) => (
				<>
					{data.map((tag) => {
						return (
							<Tag className="cognitive-tag" key={tag}>
								{tag}
							</Tag>
						);
					})}
				</>
			)
		},

		{
			width: 100,
			title: 'Status',
			dataIndex: 'active',
			key: 'active',
			filters: [
				{
					text: 'Active',
					value: 'Active'
				},
				{
					text: 'Inactive',
					value: 'Inactive'
				}
			],
			onFilter: (value, record) => record.active.indexOf(value) === 0
		},
		{
			width: 150,
			title: 'User Role',
			dataIndex: 'userRole',
			key: 'userRole',
			filters: tableFilter,
			onFilter: (value, record) => record.userRole.indexOf(value) === 0
		},
		{
			width: 100,
			fixed: 'right',
			render: (record) => (
				<div>
					<Button className="option-button" onClick={() => onTableDblClick(record)}>
						<FontAwesomeIcon icon={faEdit} />
						{t('Button.Edit')}
					</Button>
				</div>
			)
		}
	];

	const onTableDblClick = (record) => {
		userList.map((user) => {
			if (user.id === record.id) {
				setIsUserProfile(true);
				setSelectedUser(user);
			}
		});
	};

	return (
		<>
			{isUserProfile ? (
				<UserProfile
					user={selectedUser}
					handleBackButton={(toggle) => {
						setIsUserProfile(toggle);
						setupData();
					}}
				/>
			) : (
				<Spin spinning={isLoading} size="large">
					<Card
						bordered
						style={{
							marginLeft: '5%',
							marginTop: 25,
							borderRadius: 8,
							marginRight: '5%'
						}}
					>
						<Input.Search
							name="search"
							id="search"
							placeholder={t('ModalDetail.Search')}
							value={search}
							onChange={(e) => setSearch(e.target.value)}
							style={{ width: '300px' }}
						/>
					</Card>
					<Table
						onRow={(record, rowIndex) => {
							return {
								onDoubleClick: (event) => {
									onTableDblClick(record);
								} // double click row
							};
						}}
						columns={columns}
						dataSource={filtereduserList}
						style={{ marginLeft: '5%', marginRight: '5%', marginBottom: '5%', marginTop: '3%' }}
						scroll={{ x: 400 }}
					/>
				</Spin>
			)}
		</>
	);
}

function mapStateToProps(state) {
	return {
		users: state.dam.users
	};
}

function mapDispatchToProps(dispatch) {
	return {
		getUsers: (id) => dispatch(getUsers(id)),
		getUsersByCompany: (id) => dispatch(getUsersByCompany(id))
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(Users));
