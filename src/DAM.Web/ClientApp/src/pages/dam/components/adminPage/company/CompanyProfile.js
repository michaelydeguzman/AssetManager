import React, { memo, useState } from 'react';
import { connect } from 'react-redux';
import { updateCompany } from '../../../actions';
import { ArrowLeftOutlined, ExclamationCircleOutlined } from '@ant-design/icons';

import { Card, Input, Button, Form, Radio, Modal } from 'antd';
import { useTranslation } from 'react-i18next';

function CompanyProfile(props) {
	const { t } = useTranslation();
	const { confirm } = Modal;
	const formItemLayout = {
		labelCol: {
			span: 4
		},
		wrapperCol: {
			span: 14
		},
		layout: 'horizontal'
	};

	const onSubmit = async (values) => {
		var { company } = props;
		company.companyName = values.companyName;
		if (values.status !== undefined && props.company.status !== values.status) {
			confirm({
				title: t("Do you want to update the status of all compnay's users as well?"),
				//content: t(''),
				icon: <ExclamationCircleOutlined />,
				okText: 'Yes',
				cancelText: 'No',
				async onOk() {
					company.status = values.status;
					company.updateUsers = true;
					await props.updateCompany(company);
					props.setupData();
					props.handleBackButton(false);
				},
				async onCancel() {
					company.status = values.status;
					await props.updateCompany(company);
					props.setupData();
					props.handleBackButton(false);
				}
			});
		} else {
			company.status = values.status;
			await props.updateCompany(company);
			props.setupData();
			props.handleBackButton(false);
		}
	};

	return (
		<Card title={t('ModalDetail.Edit Company')} type="inner" className="card-container" style={{ margin: '15px' }}>
			<ArrowLeftOutlined
				onClick={() => {
					props.handleBackButton(false);
				}}
			/>
			<Form
				{...formItemLayout}
				name="companyProfile"
				onFinish={onSubmit}
				scrollToFirstError
				initialValues={{
					companyName: props.company.companyName,
					rootFolder: props.company.rootFolderId,
					status: props.company.status
				}}
			>
				<Form.Item
					name="companyName"
					label={t('ModalDetail.Company Name')}
					rules={[
						{
							required: true,
							message: t('Messages.Please enter a company name!')
						}
					]}
				>
					<Input placeholder={t('Messages.Enter company name')} />
				</Form.Item>

				<Form.Item name="status" label={t('ModalDetail.Status')}>
					<Radio.Group buttonStyle="solid">
						<Radio value={true}>{t('ModalDetail.Active')}</Radio>
						<Radio value={false}>{t('ModalDetail.Inactive')}</Radio>
					</Radio.Group>
				</Form.Item>

				<Form.Item>
					<Button type="primary" htmlType="submit">
						{t('Button.Save')}
					</Button>
				</Form.Item>
			</Form>
		</Card>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		updateCompany: (data) => dispatch(updateCompany(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(CompanyProfile));
