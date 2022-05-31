import React, { useState, useEffect, memo } from 'react';
import { Layout, Row, Col, Card, Input, Table, Avatar, Button, Spin, Switch, Tooltip } from 'antd';
import { Link } from 'react-router-dom';
import _ from 'lodash';
import { connect } from 'react-redux';
import { getUsers, getUsersByCompany, confirmEmail, resetPassword } from '../../actions';
import { ROLES } from '../../../constants';
import { UserOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { getUser, getUserRole } from '@damtoken';

function SuperAdmin(props) {
	const { t } = useTranslation();
	const [userList, setUserList] = useState([]);
	const [filtereduserList, setFilteredUserList] = useState([]);
	const [search, setSearch] = useState('');
	const [userRole] = useState(getUserRole());
	const [isLoading, setIsLoading] = useState(false);
	const [resetPWDinput, setResetPWDinput] = useState([]);
	const [newPWD, setNewPWD] = useState('');

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
			title: 'EmailConfirmed',
			dataIndex: 'emailConfirmed',
			key: 'emailConfirmed',
			render: (value, record) =>
				value ? (
					'true'
				) : (
					<div className="option-button">
						<Tooltip title="Click to confirm the account without emailing the user">
							<Link style={{ textDecoration: 'underline', color: 'blue' }} onClick={() => onConfirmEmail(record)}>
								false
							</Link>
						</Tooltip>
					</div>
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
			width: 200,
			fixed: 'right',
			title: 'Resetpassword',
			render: (record) => (
				<Row type="flex" justify="space-between" align="center">
					<Col>
						<Tooltip title="Switch to open the reset password input">
							<Switch
								checked={resetPWDinput.includes(record.id)}
								onClick={(checked) => {
									if (checked) {
										setResetPWDinput(() => [record.id]);
									} else {
										setResetPWDinput((resetPWDinput) => resetPWDinput.filter((id) => id !== record.id));
									}
								}}
							/>
						</Tooltip>
					</Col>
					{resetPWDinput.includes(record.id) && (
						<>
							<Col className="fade-in">
								<Input onChange={(e) => setNewPWD(e.target.value)}></Input>
							</Col>
							<Col className="fade-in">
								<Button onClick={() => onResetPassword(record)}>Reset</Button>
							</Col>
						</>
					)}
				</Row>
			)
		}
	];
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
				userFolderIds: folderIds,
				emailConfirmed: user.emailConfirmed
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
	};

	async function setupData(refresh = false) {
		// Get current login user company
		setIsLoading(true);
		let loggedInUser = await props.getUsers(user.id);
		if (loggedInUser) {
			let userDetails = loggedInUser.data.users;

			// Fetch users
			if (userRole.id == 1) {
				loggedInUser = await props.getUsers(0, refresh);
			} else if (userRole.id == 2) {
				loggedInUser = await props.getUsersByCompany(userDetails.companyId);
			}
			console.log(loggedInUser);
			setDataSourceAndFilters(loggedInUser.data.users);
		}
		setIsLoading(false);
	}

	function filterResultsBySearchText(userList, keyword) {
		return userList.filter(
			(user) =>
				user.userName.toLowerCase().includes(keyword.toLowerCase()) ||
				user.email.toLowerCase().includes(keyword.toLowerCase())
		);
	}

	const onConfirmEmail = (record) => {
		props.confirmEmail(record.id).then(() => {
			setupData(true);
		});
	};

	const onResetPassword = (record) => {
		console.log(record);
		props
			.resetPassword({
				Email: record.email,
				Password: newPWD
			})
			.then(() => {
				setNewPWD('');
				setResetPWDinput([]);
			});
	};

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				<Row className="tabpane-layout">
					<Col span={24}>
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
								columns={columns}
								dataSource={filtereduserList}
								style={{ marginLeft: '5%', marginRight: '5%', marginBottom: '5%', marginTop: '3%' }}
								scroll={{ x: 400 }}
							/>
						</Spin>
					</Col>
				</Row>
			</Layout.Content>
		</Layout>
	);
}

function mapStateToProps(state) {
	return {
		users: state.dam.users
	};
}

function mapDispatchToProps(dispatch) {
	return {
		getUsers: (id, refresh) => dispatch(getUsers(id, refresh)),
		getUsersByCompany: (id) => dispatch(getUsersByCompany(id)),
		confirmEmail: (id) => dispatch(confirmEmail(id)),
		resetPassword: (data) => dispatch(resetPassword(data))
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(SuperAdmin));
