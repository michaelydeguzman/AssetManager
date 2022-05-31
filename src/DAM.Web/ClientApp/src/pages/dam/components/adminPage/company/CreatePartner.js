import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { Radio, Card, Button, Form, Input, Select, message } from 'antd';
import { addCompany } from '../../../actions';
import { useTranslation } from 'react-i18next';

function CreatePartner(props) {
	const { t } = useTranslation();
	const [form] = Form.useForm();
	const { userList, partnerList, setupData } = props;
	const [userOptions, setUserOptions] = useState([]);
	const [partnerOptions, setPartnerOptions] = useState([]);
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

	useEffect(() => {
		const getUserAndPartnerOptions = async () => {
			let uOptions = [];
			let pOptions = [];
			if (userList.length > 0) {
				userList.map((user) => {
					uOptions.push(
						<Select.Option value={user.id} key={user.id}>
							{user.userName}
						</Select.Option>
					);
				});
			}

			if (partnerList.length > 0) {
				partnerList.map((partner) => {
					pOptions.push(
						<Select.Option value={partner.id} key={partner.id}>
							{partner.companyName}
						</Select.Option>
					);
				});
			}
			setPartnerOptions(pOptions);
			setUserOptions(uOptions);
		};
		getUserAndPartnerOptions();
	}, [userList, partnerList]);

	const onUserChange = () => {};

	const onPartnerChange = () => {};

	const onSubmit = async (values) => {
		let partner = values;
		partner.createdDate = new Date();
		await props.addPartner(partner);
		await setupData();
		form.resetFields();
	};

	return (
		<Card title={t('ModalDetail.Create Company')} type="inner" className="card-container" style={{ margin: '15px' }}>
			<Form {...formItemLayout} form={form} onFinish={onSubmit}>
				<Form.Item
					name={'companyName'}
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

				<Form.Item
					name={'status'}
					label={t('ModalDetail.Status')}
					rules={[
						{
							required: true,
							message: t('Messages.Select partner status!')
						}
					]}
				>
					<Radio.Group buttonStyle="solid">
						<Radio value={true}>{t('ModalDetail.Active')}</Radio>
						<Radio value={false}>{t('ModalDetail.Inactive')}</Radio>
					</Radio.Group>
				</Form.Item>

				<Form.Item {...tailFormItemLayout}>
					<Button type="primary" htmlType="submit">
						{t('Button.Create')}
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
		addPartner: (data) => dispatch(addCompany(data))
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(CreatePartner));
