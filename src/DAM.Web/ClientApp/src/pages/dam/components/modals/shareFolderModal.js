import React, { useState, memo, useRef, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Button, Modal, Alert, Input, Select, TreeSelect, Form, Checkbox } from 'antd';
import { CloseOutlined } from '@ant-design/icons';
import { shareBulkAssets, getUsers, shareFoldertoUser } from '../../actions';
import { CheckCompanyAccess } from '@damhelper';
import { getUserRole } from '@damtoken';
import { useTranslation } from 'react-i18next';

function ShareFolderModal(props) {
	const { t } = useTranslation();
	const { shareFolderModalState, setShareFolderModalState, folderId, efolders, hasWatermark } = props;
	const userRole = getUserRole();
	const [form] = Form.useForm();
	const [showCopySuccess, setShowCopySuccess] = useState(false);
	const [publicLink, setPublicLink] = useState('');
	const [userData, setUserData] = useState([]);

	const [shareFolderSelect, setShareFolderSelect] = useState(false);
	const [selectedOption, setSelectedOption] = useState('');

	useEffect(() => {
		setPublicLink(`${window.location.origin}/api/Folders/Download/${folderId}/public/false`);
		async function getUserSelectOption() {
			var userList = await props.getUsers();
			var eligibleUser = [];
			userList.data &&
				userList.data.users.map((user) => {
					if (user.userRoleId === 4) {
						var userOption = {
							title: user.userName,
							value: user.id
						};
						eligibleUser.push(userOption);
					}
					if (user.userRoleId === 2 || user.userRoleId === 3) {
						if (CheckCompanyAccess(props.efolders, folderId, user)) {
							var userOption = {
								title: user.userName,
								value: user.id
							};
							eligibleUser.push(userOption);
						}
					}
				});
			setUserData(eligibleUser);
		}
		getUserSelectOption();
	}, []);

	const copyLinkToClipboard = () => {
		var link = document.getElementById('downLoadLink');
		link.type = 'text';
		link.select();
		document.execCommand('copy', false, null);
		link.type = 'hidden';
		setShowCopySuccess(true);
	};

	function handleShareChange(value) {
		if (value == 1) {
			setShareFolderSelect(true);
		} else {
			setShareFolderSelect(false);
		}
		setSelectedOption(value);
	}

	async function submitInternalShare(values) {
		var data = {
			FolderId: folderId,
			UserIds: values.userIds
		};
		props.shareFoldertoUser(data);
	}

	function handleApplywmcheck(e) {
		var newLink = publicLink;
		if (e.target.checked) {
			newLink = newLink.replace('/false', '');
			newLink = newLink + '/true';
		} else {
			newLink = newLink.replace('/true', '');
			newLink = newLink + '/false';
		}
		setPublicLink(newLink);
	}

	return (
		<Modal
			title={<Row>{t('Share Message.Folder')}</Row>}
			visible={shareFolderModalState}
			onCancel={() => {
				setShareFolderModalState(false);
			}}
			centered={true}
			footer={false}
			getContainer={false}
			closable={true}
			closeIcon={<CloseOutlined />}
			className={`share-modal`}
		>
			<Row>
				<div>{t('Share Message.Body')}</div>
			</Row>
			<Row>
				<Select
					style={{ width: '350px', marginTop: '12px' }}
					onChange={handleShareChange}
					placeholder={t('Messages.Select')}
				>
					<Select.Option key={1}>{t('Share Message.With Access')}</Select.Option>
					<Select.Option key={2}>{t('Share Message.With Anyone')}</Select.Option>
				</Select>
			</Row>
			{userRole.canShareFolders && shareFolderSelect && (
				<Form form={form} onFinish={submitInternalShare} layout={'vertical'}>
					<Form.Item name="userIds" label={t('Share Message.Folder to')} style={{ marginTop: '12px' }}>
						<TreeSelect
							multiple
							placeholder={t('Messages.Select Users')}
							treeData={userData}
							allowClear
							style={{ width: '350px', marginTop: '8px' }}
						></TreeSelect>
					</Form.Item>
					<Row>
						<Button htmlType="submit" className="share-modal-copy-button">
							{t('Share Message.Folder')}
						</Button>
					</Row>
				</Form>
			)}

			{hasWatermark && selectedOption === '2' && (
				<Row style={{ marginTop: 16 }}>
					<Checkbox onChange={handleApplywmcheck}>{t('Messages.ApplyWM')}</Checkbox>
				</Row>
			)}

			{!shareFolderSelect && (
				<Row>
					<Button onClick={() => copyLinkToClipboard()} className="share-modal-copy-button">
						{t('Share Message.Copy Link')}
					</Button>
				</Row>
			)}
			<>
				<Input id={'downLoadLink'} type="hidden" value={publicLink} title="download" />
				{showCopySuccess && (
					<Row>
						<Alert
							message={t('Share Message.Copied Message')}
							type="success"
							showIcon
							closable
							onClose={() => setShowCopySuccess(false)}
							style={{ marginTop: '12px', width: '100%' }}
						/>
					</Row>
				)}
			</>
		</Modal>
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
		onShareBulkAssets: (data, type) => dispatch(shareBulkAssets(data, type)),
		shareFoldertoUser: (data) => dispatch(shareFoldertoUser(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ShareFolderModal));
