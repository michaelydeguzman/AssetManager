import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Card, Input, Select, Button, Form, Radio, TreeSelect } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import { ROLES, USER_ROLES } from '../../../../../constants';
import { updateUserByAdmin, uploadUserImage, getCompanies, getFolders, getUserRoles } from '../../../../actions';
import { LowFrequencyContext } from '@damcontext';
import { NestedFoldersChild, NestedChild } from '@damhelper';
import { useTranslation } from 'react-i18next';

function UserProfile(props) {
	const { t } = useTranslation();
	const { setIsUserUpdated, currentUser } = useContext(LowFrequencyContext);
	const [companies, setCompanies] = useState([]);
	const [companyOptions, setCompanyOptions] = useState([]);
	const [folderOptions, setFolderOptions] = useState([]);
	const [userRoles, setUserRoles] = useState([]);
	const [filteredUserRoles, setfilteredUserRoles] = useState([]);
	const [isFolderHidden, setFolderHidden] = useState(
		props.user.userRole == USER_ROLES.DAM_ADMIN || props.user.userRole == USER_ROLES.COMPANY_ADMIN
	);
	const [isCompanyHidden, setCompanyHidden] = useState(true);
	const [nestedFolders, setNestedFolders] = useState([]);
	const [form] = Form.useForm();
	const formItemLayout = {
		labelCol: {
			xs: {
				span: 16
			},
			sm: {
				span: 4
			}
		},
		wrapperCol: {
			xs: {
				span: 16
			},
			sm: {
				span: 8
			}
		}
	};
	const tailFormItemLayout = {
		wrapperCol: {
			xs: {
				span: 16,
				offset: 16
			},
			sm: {
				span: 8,
				offset: 24
			}
		},
		display: 'flex'
	};

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
			setCompanyHidden(
				currentUser.userRoleId != 1 ||
					props.user.userRole == USER_ROLES.DAM_ADMIN ||
					props.user.userRole == USER_ROLES.USER
			);
		}
		let filteredRoles = [];
		if (userRoles.length > 0 && currentUser) {
			if (currentUser.userRole.name == USER_ROLES.COMPANY_ADMIN) {
				filteredRoles = userRoles.filter((role) => role.name != USER_ROLES.DAM_ADMIN);
			} else {
				filteredRoles = userRoles;
			}
		}
		setfilteredUserRoles(filteredRoles);
	}, [currentUser, userRoles]);

	useEffect(() => {
		// handle initial folder values for subscribers
		if (companies.length > 0 && folderOptions) {
			if (props.user.userRole === USER_ROLES.SUBSCRIBER) {
				var root = companies.find((company) => company.id === props.user.companyId);

				let nestFolder = NestedFoldersChild(folderOptions, root.rootFolderId);
				var folders = {
					key: root.rootFolderId,
					id: root.rootFolderId,
					title: root.rootFolderName,
					children: nestFolder
				};
				setNestedFolders([folders]);
			}
			if (props.user.userRole === USER_ROLES.USER) {
				setNestedFolders(NestedChild(folderOptions, 1));
			}
		}
	}, [props.user, folderOptions, companies]);

	const onUserRoleSelect = (value, option) => {
		switch (option.children) {
			case USER_ROLES.DAM_ADMIN:
				setCompanyHidden(true);
				setFolderHidden(true);
				break;
			case USER_ROLES.COMPANY_ADMIN:
				setCompanyHidden(false);
				setFolderHidden(true);
				break;
			case USER_ROLES.SUBSCRIBER:
				setCompanyHidden(false);
				setFolderHidden(false);
				break;
			case USER_ROLES.USER:
				setCompanyHidden(true);
				setFolderHidden(false);
				break;
			default:
				setCompanyHidden(true);
				setFolderHidden(true);
				break;
		}
	};

	const getCompanyOptions = (companyList) => {
		let cOptions = [];

		if (companyList.length > 0) {
			companyList.map((company) => {
				if (company.status == true) {
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

	function saveUserData(values) {
		let userData = values;
		userData.status = userData.status === 'Active' ? 1 : 0;
		userData.userRoleId = ROLES.indexOf(userData.userRole) + 1;

		props.updateUserByAdmin(userData);
		if (userData.id == currentUser.Id) setIsUserUpdated(true);
		props.handleBackButton(false);
	}

	function handleFormValueChange(changedValues, allValues) {
		if (Object.keys(changedValues)[0] === 'company') {
			var root = companies.find((company) => company.id === changedValues.company);
			if (root) {
				const nestFolder = NestedFoldersChild(folderOptions, root.rootFolderId);
				var folders = {
					key: root.rootFolderId,
					id: root.rootFolderId,
					title: root.rootFolderName,
					children: nestFolder
				};
				setNestedFolders([folders]);
			}
			allValues['rootFolderIds'] = [];
		}
		if (Object.keys(changedValues)[0] === 'userRole') {
			allValues['company'] = [];
			allValues['rootFolderIds'] = [];
			setNestedFolders([]);
			if (changedValues.userRole === 'User') {
				setNestedFolders(NestedChild(folderOptions, 1));
			}
		}
	}

	return (
		<>
			<Card
				title={t('ModalDetail.Update User Profile')}
				type="inner"
				className="card-container"
				style={{ margin: '15px' }}
			>
				<Row>
					<Col span={16}>
						<ArrowLeftOutlined
							onClick={() => {
								props.handleBackButton(false);
							}}
						/>
						<Form
							{...formItemLayout}
							form={form}
							name="userProfile"
							onFinish={saveUserData}
							scrollToFirstError
							onValuesChange={handleFormValueChange}
							initialValues={{
								id: props.user.id,
								userName: props.user.userName,
								userRole: props.user.userRole,
								email: props.user.email,
								password: props.user.password,
								company: props.user.companyId,
								confirm: props.user.password,
								status: props.user.active,
								imageFileExtension: props.user.imageFileExtension,
								rootFolderIds: props.user.userFolderIds
							}}
						>
							<Form.Item
								name="userName"
								label={t('ModalDetail.Name')}
								rules={[
									{
										required: true,
										message: t('Messages.Please input your Name')
									}
								]}
							>
								<Input placeholder={t('Messages.Enter Name')} />
							</Form.Item>

							<Form.Item
								name="email"
								label={t('ModalDetail.Email')}
								rules={[
									{
										type: 'email',
										message: t('Messages.Invalid email address')
									},
									{
										required: false,
										message: t('Messages.Please input your email address')
									}
								]}
							>
								<Input placeholder={t('ModalDetail.Enter email Address')} disabled />
							</Form.Item>
							<Form.Item name="status" label={t('ModalDetail.Status')}>
								<Radio.Group buttonStyle="solid">
									<Radio value="Active">{t('ModalDetail.Active')}</Radio>
									<Radio value="Inactive">{t('ModalDetail.Inactive')}</Radio>
								</Radio.Group>
							</Form.Item>
							<Form.Item hidden="true" name="imageFileExtension" label="">
								<Input placeholder={t('ModalDetail.Enter email Address')} />
							</Form.Item>
							<Form.Item
								name="userRole"
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
											<Select.Option key={role.id} value={role.name}>
												{role.name}
											</Select.Option>
										);
									})}
								</Select>
							</Form.Item>
							{!isCompanyHidden && (
								<Form.Item
									name="company"
									label={t('ModalDetail.Company Name')}
									rules={[
										{
											required: !isCompanyHidden,
											message: t('Messages.Select a region')
										}
									]}
								>
									<Select placeholder={t('Messages.Select')} dropdownStyle={{ position: 'fixed' }}>
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
											required: !isFolderHidden,
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

							<Row>
								<Form.Item {...tailFormItemLayout}>
									<Button type="primary" htmlType="submit">
										{t('Button.Update')}
									</Button>
								</Form.Item>
							</Row>
						</Form>
					</Col>
				</Row>
			</Card>
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
		updateUserByAdmin: (data) => dispatch(updateUserByAdmin(data)),
		onUploadAsset: (data) => dispatch(uploadUserImage(data)),
		getCompanies: () => dispatch(getCompanies()),
		getFolders: () => dispatch(getFolders()),
		getUserRoles: () => dispatch(getUserRoles())
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(UserProfile));
