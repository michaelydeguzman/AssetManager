import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { Space, Card, Button, Form, Input, Select, message, TreeSelect, Layout } from 'antd';
import { inviteNewUser, getCompanies, getFolders, getUserRoles } from '../../../../actions';
import { LowFrequencyContext } from '@damcontext';
import { USER_ROLES } from '../../../../../constants';
import { NestedChild } from '@damhelper';
import { useTranslation } from 'react-i18next';

function InviteUser(props) {
	const { t } = useTranslation();
	const [form] = Form.useForm();
	const [companies, setCompanies] = useState({});
	const [companyOptions, setCompanyOptions] = useState([]);
	const [folderOptions, setFolderOptions] = useState([]);
	const [userRoles, setUserRoles] = useState([]);
	const [filteredUserRoles, setfilteredUserRoles] = useState([]);
	const [isFolderHidden, setFolderHidden] = useState(true);
	const [isCompanyHidden, setCompanyHidden] = useState(true);
	const [nestedFolders, setNestedFolders] = useState([]);
	const [isFolderAccessRequired, setIsFolderAccessRequired] = useState(true);
	const { currentUser } = useContext(LowFrequencyContext);

	useEffect(() => {
		const setupData = async () => {
			let companies = await props.getCompanies();
			let folders = await props.getFolders();
			let ur = await props.getUserRoles();
			getCompanyOptions(companies.data.companies);
			getFolderOptions(folders.data.folders);
			setUserRoles(ur.data.userRoles);
		};

		setupData();
	}, []);

	useEffect(() => {
		if (currentUser) {
			setIsFolderAccessRequired(!currentUser.userRoleId === 4);
		}
		let filteredRoles = [];
		if (userRoles.length > 0 && currentUser && currentUser.userRole.name != USER_ROLES.DAM_ADMIN) {
			filteredRoles = userRoles.filter((role) => role.name != USER_ROLES.DAM_ADMIN && role.name != USER_ROLES.USER);
		} else {
			filteredRoles = userRoles;
		}
		setfilteredUserRoles(filteredRoles);
	}, [currentUser, userRoles]);

	const getCompanyOptions = (companyList) => {
		let cOptions = [];

		if (companyList.length > 0) {
			companyList.map((company) => {
				if (company.status === true) {
					cOptions.push(
						<Select.Option value={company.id} key={company.id}>
							{company.companyName}
						</Select.Option>
					);
				}
			});
		}
		setCompanyOptions(cOptions);
		setCompanies(companyList);
	};

	const getFolderOptions = (folderList) => {
		const folderObj = [];

		folderList.map((folder) => {
			if (
				folder.company &&
				(folder.company.length === 0 || (folder.company.length > 0 && folder.company[0].status === true))
			) {
				folderObj.push({
					key: folder.id,
					id: folder.id,
					title: folder.folderName,
					parentFolderId: folder.parentFolderId,
					accounts: folder.accounts,
					countries: folder.countries,
					regions: folder.regions,
					comments: folder.comments
				});
			}
		});
		setFolderOptions(folderObj);
	};

	const onSubmit = (values) => {
		props.inviteNewUser(values);
		form.resetFields();
		setCompanyHidden(true);
		setFolderHidden(true);
		setNestedFolders([]);
	};

	const onUserRoleSelect = (value, option) => {
		let myUserRole = currentUser ? currentUser.userRole : null;
		switch (option.children) {
			case USER_ROLES.DAM_ADMIN:
				setCompanyHidden(true);
				setFolderHidden(true);
				setIsFolderAccessRequired(false);
				break;
			case USER_ROLES.COMPANY_ADMIN:
				setCompanyHidden(myUserRole && myUserRole.name != USER_ROLES.DAM_ADMIN);
				setFolderHidden(true);
				setIsFolderAccessRequired(false);
				break;
			case USER_ROLES.SUBSCRIBER:
				setCompanyHidden(myUserRole && myUserRole.name != USER_ROLES.DAM_ADMIN);
				setFolderHidden(false);
				setIsFolderAccessRequired(true);
				if (myUserRole && myUserRole.name != USER_ROLES.DAM_ADMIN) {
					onCompanyChange(currentUser.companyId);
				}
				break;
			case USER_ROLES.USER:
				setCompanyHidden(true);
				setFolderHidden(false);
				setNestedFolders(NestedChild(folderOptions, 1));
				setIsFolderAccessRequired(false);
				break;
			default:
				setCompanyHidden(true);
				setFolderHidden(true);
				break;
		}
	};

	const onCompanyChange = (value) => {
		var root = companies.find((company) => company.id === value);
		var folders = {
			key: root.rootFolderId,
			id: root.rootFolderId,
			title: root.rootFolderName,
			children: NestedChild(folderOptions, root.rootFolderId)
		};
		setNestedFolders([folders]);
	};

	const resetFields = () => {
		form.resetFields();
		setCompanyHidden(true);
		setFolderHidden(true);
		setNestedFolders([]);
	};

	const formItemLayout = {
		labelCol: {
			xs: { span: 8 },
			sm: { span: 8 }
		},
		wrapperCol: {
			xs: { span: 8 },
			sm: { span: 8 }
		}
	};
	const tailFormItemLayout = {
		wrapperCol: {
			xs: {
				span: 8,
				offset: 8
			},
			sm: {
				span: 8,
				offset: 8
			}
		},
		display: 'flex'
	};

	function handleFormValueChange(changedValues, allValues) {
		if (Object.keys(changedValues)[0] == 'companyId') {
			allValues['rootFolderIds'] = [];
		}
	}

	return (
		// <Layout className="dam-layout page-layout">
		//   <Layout.Content>
		<Card title={t('Slider.Invite User')} type="inner" className="card-container" style={{ margin: '15px' }}>
			<Form
				{...formItemLayout}
				// labelCol={{ xs: 8, sm: 8 }}
				// wrapperCol={{ xs: 8, sm: 8 }}
				form={form}
				onFinish={onSubmit}
				onValuesChange={handleFormValueChange}
			>
				<Form.Item
					name="displayName"
					label={t('ModalDetail.Name')}
					rules={[
						{
							required: true,
							message: t('Messages.Please input your Name')
						}
					]}
				>
					<Input placeholder={t('ModalDetail.Display Name')} />
				</Form.Item>

				<Form.Item
					name="emailAddress"
					label={t('ModalDetail.Email')}
					rules={[
						{
							type: 'email',
							message: t('Messages.Invalid email address')
						},
						{
							required: true,
							message: t('Messages.Please input your email address')
						}
					]}
				>
					<Input placeholder={t('ModalDetail.Enter email Address')} />
				</Form.Item>

				<Form.Item
					name="userRoleId"
					label={t('ModalDetail.User Role')}
					rules={[
						{
							required: true,
							message: t('Messages.Please select a user role')
						}
					]}
				>
					<Select placeholder={t('Messages.Select user role')} onSelect={onUserRoleSelect}>
						{filteredUserRoles.map((role) => {
							return (
								<Select.Option key={role.id} value={role.id}>
									{role.name}
								</Select.Option>
							);
						})}
					</Select>
				</Form.Item>
				{!isCompanyHidden && (
					<Form.Item
						name={'companyId'}
						label={t('ModalDetail.Company Name')}
						rules={[
							{
								required: !isCompanyHidden,
								message: t('Messages.Select Parent a company')
							}
						]}
					>
						<Select placeholder={t('Messages.Select')} onChange={onCompanyChange} dropdownStyle={{ position: 'fixed' }}>
							{companyOptions}
						</Select>
					</Form.Item>
				)}
				{!isFolderHidden && (
					<Form.Item
						name="rootFolderIds"
						label={t('ModalDetail.Folder Access')}
						rules={[
							{
								required: !isFolderHidden && isFolderAccessRequired,
								message: t('Messages.Please select a folder')
							}
						]}
					>
						<TreeSelect
							style={{ width: '100%' }}
							dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
							treeData={nestedFolders}
							placeholder={t('Messages.Select Root folder')}
							treeDefaultExpandAll
							multiple
							allowClear
							treeCheckable
							showCheckedStrategy={TreeSelect.SHOW_ALL}
						/>
					</Form.Item>
				)}
				<Form.Item {...tailFormItemLayout}>
					<Space>
						<Button type="primary" htmlType="submit">
							{t('Button.Invite User')}
						</Button>
						<Button type="secondary" onClick={resetFields}>
							{t('Button.Clear')}
						</Button>
					</Space>
				</Form.Item>
			</Form>
		</Card>
		//   {/* </Layout.Content>
		// </Layout> */}
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		inviteNewUser: (data) => dispatch(inviteNewUser(data)),
		getCompanies: () => dispatch(getCompanies()),
		getFolders: () => dispatch(getFolders()),
		getUserRoles: () => dispatch(getUserRoles())
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(InviteUser));
