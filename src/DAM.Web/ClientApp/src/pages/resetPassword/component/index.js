import React, { useContext } from 'react';
import { Form, Input, Button, Card, Row, Col, message } from 'antd';
import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { submitResetPassword } from '../actions';
import { LowFrequencyContext } from '@damcontext';

import { useTranslation } from 'react-i18next';

function ResetPassword(props) {
	const { t } = useTranslation();
	const [form] = Form.useForm();
	const { search } = useLocation();
	const queryString = require('query-string');
	const { themeLogo } = useContext(LowFrequencyContext);

	async function handleSubmit(data) {
		props.onSubmitCreatePassword(data).then(() => {});
	}

	const { resetting } = props;
	const parsed = queryString.parse(search.slice(1));
	const { email, token } = parsed;

	return (
		<Row className="login-container" type="flex" justify="center" align="middle">
			<Col>
				<Card className="login-card">
					<Row type="flex" justify="center" align="middle">
						<Col align="center" span={24}>
							<img src={themeLogo} className="logo" loading="lazy" alt="logo" />
						</Col>
						<Col span={24}>
							<h2 className="description">Enter new password</h2>
						</Col>
					</Row>
					<Form
						form={form}
						layout="vertical"
						name="basic"
						initialValues={{
							remember: false,
							email: email,
							token: token
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
							<Input placeholder={t('ModalDetail.Email')} width="auto" autoFocus value={email} disabled={true} />
						</Form.Item>

						<Form.Item name="password" rules={[{ required: true, message: t('Messages.Enter a new password') }]}>
							<Input.Password placeholder={t('Login Page.New Password')} />
						</Form.Item>

						<Form.Item
							name="confirmPassword"
							dependencies={['password']}
							rules={[
								{ required: true, message: t('Messages.Confirm the new password') },
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

						<Form.Item name="token">
							<Input.TextArea placeholder="Token" hidden={true} />
						</Form.Item>

						<Form.Item>
							<Button htmlType="submit" type="primary" loading={resetting}>
								{t('Button.Submit')}
							</Button>
						</Form.Item>
					</Form>
				</Card>
			</Col>
			<Col className="login-footer">
				&#169; 2022. All Rights Reserved.
			</Col>
		</Row>
	);
}

function mapStateToProps(state) {
	return {
		resetting: state.resetPassword.resetting
	};
}

function mapDispatchToProps(dispatch) {
	return {
		onSubmitCreatePassword: (data) => dispatch(submitResetPassword(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(ResetPassword);
