import React, { useContext } from 'react';
import { Form, Input, Button, Card, Row, Col, message } from 'antd';
import { connect } from 'react-redux';
import { useLocation } from 'react-router-dom';
import { submitCreatePassword } from '../actions';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';

function CreatePassword(props) {
	const { t } = useTranslation();
	const { search } = useLocation();
	const queryString = require('query-string');
	const parsed = queryString.parse(search.slice(1));
	const { email } = parsed;

	const { themeLogo } = useContext(LowFrequencyContext);
	const [form] = Form.useForm();
	async function handleSubmit(data) {
		props.onSubmitCreatePassword(data).then(() => {});
	}

	const { creatingPassword } = props;

	return (
		<Row className="login-container" type="flex" justify="center" align="middle">
			<Col>
				<Card className="login-card">
					<Row type="flex" justify="center" align="middle">
						<Col align="center" span={24}>
							<img src={themeLogo} className="logo" alt="simple" />
						</Col>
						<Col span={24}>
							<h2 className="description">{t('Create Password')}</h2>
						</Col>
					</Row>
					<Form
						form={form}
						layout="vertical"
						name="basic"
						initialValues={{
							email: email,
							remember: true
						}}
						onFinish={handleSubmit}
						hideRequiredMark
						scrollToFirstError
					>
						<Form.Item
							name="email"
							rules={[
								{
									required: true,
									message: t('Messages.Please input a valid email!'),
									type: 'email'
								}
							]}
						>
							<Input placeholder={t('ModalDetail.Email')} width="auto" disabled />
						</Form.Item>

						<Form.Item name="password" rules={[{ required: true, message: t('Messages.Enter a new password') }]}>
							<Input.Password placeholder={t('Login Page.New Password')} />
						</Form.Item>

						<Form.Item
							name="confirmPassword"
							dependencies={['password']}
							rules={[
								{ required: true, message: t('Login Page.Confirm Password') },
								({ getFieldValue }) => ({
									validator(_, value) {
										if (!value || getFieldValue('password') === value) {
											return Promise.resolve();
										}
										return Promise.reject(new Error(t('Messages.The two passwords that you entered do not match!')));
									}
								})
							]}
						>
							<Input.Password placeholder={t('Login Page.Confirm Password')} />
						</Form.Item>

						<Form.Item>
							<Button htmlType="submit" type="primary" loading={creatingPassword}>
								{t('Button.Submit')}
							</Button>
						</Form.Item>
					</Form>
				</Card>
			</Col>
			<Col className="login-footer">
				&#169; 2021 <a href="https://simple.io/">Simple</a>. All Rights Reserved.
			</Col>
		</Row>
	);
}

function mapStateToProps(state) {
	return {
		creatingPassword: state.createPassword.creatingPassword
	};
}

function mapDispatchToProps(dispatch) {
	return {
		onSubmitCreatePassword: (data) => dispatch(submitCreatePassword(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(CreatePassword);
