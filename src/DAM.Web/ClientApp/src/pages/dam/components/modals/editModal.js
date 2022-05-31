import React, { memo, useContext, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Input, Select, Button, Form, Modal, Spin } from 'antd';
import { useTranslation } from 'react-i18next';
import { LowFrequencyContext } from '@damcontext';
import { getUserRole } from '@damtoken';
import { getFolderDetail, updateFolder } from '../../actions';

function EditModal(props) {
	const { t } = useTranslation();
	const {
		modal,
		modalState,
		isUpdating,
		selectOptionsType,
		touchFolder,
		selectedFolderParentId,
		handleUpdatedFolder
	} = props;

	const { allCountries, allAccounts } = useContext(LowFrequencyContext);
	// get role access
	const userRole = getUserRole();
	const [form] = Form.useForm();

	useEffect(() => {
		async function loadFolderDetail() {
			const folder = await props.getFolderDetail(touchFolder ? touchFolder : 1);
			console.log(folder);
			form.setFieldsValue({
				folder_name: folder.folders[0].folderName,
				accounts: folder.folders[0].folderAccounts.map((row) => row.accountId),
				//country: folder.folders[0].folderCountryRegions.map((row) => row.CountryId),
				comments: folder.folders[0].comments
			});
		}
		loadFolderDetail();
	}, [touchFolder]);

	async function handleUpdateFolder(values) {
		console.log(values);
		const data = {
			id: touchFolder,
			folderName: values.folder_name,
			comments: values.comments,
			accounts: values.accounts.map((o) => {
				return { id: o };
			}),
			//countries: selectedFolderCountries,
			//regions: selectedFolderRegions,
			parentFolderId: selectedFolderParentId
		};
		console.log(data);
		await props.onUpdateFolder(data).then(() => {
			console.log('data', data);
			handleUpdatedFolder();
		});
	}
	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'edit'}
			onCancel={modal().closeModal}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Spin spinning={isUpdating} size="large">
				<Form
					form={form}
					key={'edit'}
					layout="horizontal"
					name="edit"
					onFinish={handleUpdateFolder}
					scrollToFirstError
					className="dam-form"
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<Col>
						<Form.Item
							name="folder_name"
							label={t('ModalDetail.Folder Name')}
							className="folder-chip"
							rules={[
								{
									required: true,
									message: t('Messages.Please input folder name')
								}
							]}
						>
							<Input width="auto" autoFocus onChange={modal().setTitle} />
						</Form.Item>

						<Form.Item name="accounts" label={t('ModalDetail.Account')}>
							<Select
								mode="multiple"
								placeholder={t('Messages.Select')}
								dropdownStyle={{ position: 'fixed' }}
								onChange={modal().selectAccount}
							>
								{selectOptionsType(allAccounts)}
							</Select>
						</Form.Item>

						{/* since remove the region long long ago the feature is broken, remove it as well */}
						{/* <Form.Item name="country" label="Country">
							<Select
								mode="multiple"
								placeholder="Please select"
								dropdownStyle={{ position: 'fixed' }}
								onChange={modal().selectCountry}
							>
								{selectOptionsType(allCountries)}
							</Select>
						</Form.Item> */}

						{/* Temporarily remove it */}
						{/* <Form.Item name="region" label={t('ModalDetail.Region')}>
							<Select
								mode="multiple"
								placeholder={t('Messages.Select')}
								onChange={modal().selectRegion}
								dropdownStyle={{ position: 'fixed' }}
							>
								{selectGroupOptionsType(regionOptions.sort(compare))}
							</Select>
						</Form.Item> */}

						<Form.Item label={t('ModalDetail.Comments')} name="comments">
							<Input.TextArea placeholder={t('Messages.Comments here')} onChange={modal().setComments} />
						</Form.Item>

						<Row type="flex" className="form-actions">
							<Col
								xs={24}
								className="form-update-actions"
								style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
							>
								<Button type="secondary" onClick={modal().closeModal}>
									{t('Button.Cancel')}
								</Button>
								{(userRole.canEdit && !modal().isReadOnly()) || (userRole.canArchive && !modal().isReadOnly()) ? (
									<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
										{modal().submitText()}
									</Button>
								) : (
									''
								)}
							</Col>
						</Row>
					</Col>
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
		getFolderDetail: (id) => dispatch(getFolderDetail(id)),
		onUpdateFolder: (data) => dispatch(updateFolder(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(EditModal));
