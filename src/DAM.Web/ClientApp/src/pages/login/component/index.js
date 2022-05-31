import React, { useEffect, useState, useContext } from 'react';
import { Form, Input, Button, Card, Row, Col } from 'antd';
import { connect } from 'react-redux';
import { submitLogin, sendRequestPasswordRequest, sendRequestResendConfirm } from '../actions';
import { getUsers, getPinAssets } from '../../dam/actions';
import { getUser, setUserRole } from '@damtoken';
import { history } from '../../../utilities/history';
import { LowFrequencyContext } from '@damcontext';

import { useTranslation } from 'react-i18next';


function Login(props) {
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const [toggleForgotPassword, setToggleForgotPassword] = useState(false);
    const [toggleResendConfirmation, setToggleResendConfirmation] = useState(false);
    const { themeLogo, isAdministrator } = useContext(LowFrequencyContext);

    const {
        loggingIn,
        loginRejected,
        loginMessage,
        requestingPassword,
        requestPasswordSuccess,
        requestPasswordFailed,
        requestMessage
    } = props;

    const isDynamics =
        props.location &&
        props.location.state &&
        props.location.state.from &&
        props.location.state.from.pathname &&
        props.location.state.from.pathname.includes('dynamics-');

    async function handleAfterLogin(user) {
        var response = await props.getUsers(user.id);
        var userRole = response.data.users.userRole;
        setUserRole(userRole);

        if (userRole.id === 1) {
            history.push('/home');
        } else {
            // check if user has pinned assets
            var pinnedAssets = await props.getPinAssets();

            if (pinnedAssets && pinnedAssets.length > 0) {
                history.push('/pinned');
            } else {
                history.push('/assets');
            }
        }
    };


    async function autoLoginForDynamics() {
        await props.onSubmitLogin({ IsDynamics: true });
        const user = getUser();
        if (user) {
            handleAfterLogin(user);
        }
    }

    if (isDynamics) {
        autoLoginForDynamics();
    }

    async function handleSubmit(data) {
        await props.onSubmitLogin(data);
        const user = getUser();
        if (user) {
            handleAfterLogin(user);
        }
    }

    async function handleRequestPassword(data) {
        await props.onResetRequest(data);
    }

    async function handleRequestResend(data) {
        await props.onResendRequest(data);
    }

    useEffect(() => {
        //TODO: Figure Out
        if (loginRejected) {
            // message.error(loginMessage);
        }

        if (requestPasswordFailed) {
            // message.error(requestMessage);
        }

        if (requestPasswordSuccess) {
            // message.success('A password reset request confirmation has been sent to your email address.');
            setToggleForgotPassword(!toggleForgotPassword);
        }
    }, [props]);

    return (
        <Row className="login-container" type="flex" justify="center" align="middle">
            <Col>
                <Card className="login-card">
                    {toggleForgotPassword ? (
                        <div>
                            <Row type="flex" justify="center" align="middle">
                                <Col align="center" span={24}>
                                    <img src={themeLogo} className="logo" loading="lazy" alt="logo" />
                                </Col>
                                <Col span={24}>
                                    <h2 className="description">{t('Login Page.Enter email to request password reset')}</h2>
                                </Col>
                            </Row>
                            <Form
                                form={form}
                                layout="vertical"
                                name="basic"
                                initialValues={{
                                    remember: false
                                }}
                                onFinish={handleRequestPassword}
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
                                    <Input placeholder={t('ModalDetail.Email')} width="auto" autoFocus />
                                </Form.Item>

                                <Form.Item>
                                    <Button htmlType="submit" type="primary" loading={requestingPassword}>
                                        {t('Button.Reset')}
                                    </Button>

                                    <a className="login-form-forgot" onClick={() => setToggleForgotPassword(!toggleForgotPassword)}>
                                        {t('Button.Back to Login')}
                                    </a>
                                </Form.Item>
                            </Form>
                        </div>
                    ) : toggleResendConfirmation ? (
                        <div>
                            <Row type="flex" justify="center" align="middle">
                                <Col align="center" span={24}>
                                    <img src={themeLogo} className="logo" loading="lazy" alt="logo" />
                                </Col>
                                <Col span={24}>
                                    <h2 className="description">{t('Login Page.Resend')}</h2>
                                </Col>
                            </Row>
                            <Form
                                form={form}
                                layout="vertical"
                                name="basic"
                                initialValues={{
                                    remember: false
                                }}
                                onFinish={handleRequestResend}
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
                                    <Input placeholder={t('ModalDetail.Email')} width="auto" autoFocus />
                                </Form.Item>

                                <Form.Item>
                                    <Button htmlType="submit" type="primary" loading={requestingPassword}>
                                        {t('Button.Resend')}
                                    </Button>

                                    <a
                                        className="login-form-forgot"
                                        onClick={() => setToggleResendConfirmation(!toggleResendConfirmation)}
                                    >
                                        {t('Button.Back to Login')}
                                    </a>
                                </Form.Item>
                            </Form>
                        </div>
                    ) : (
                        <div>
                            <Row type="flex" justify="center" align="middle">
                                <Col align="center" span={24}>
                                    <img src={themeLogo} className="logo" loading="lazy" alt="logo" />
                                </Col>
                                <Col span={24}>
                                    <h2 className="description">{t('Login Page.Log in with your existing account')}</h2>
                                </Col>
                            </Row>
                            <Form
                                form={form}
                                layout="vertical"
                                name="basic"
                                initialValues={{
                                    remember: true,
                                    IsDynamics: isDynamics
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
                                    <Input placeholder={t('ModalDetail.Email')} width="auto" autoFocus />
                                </Form.Item>

                                <Form.Item
                                    name="password"
                                    rules={[{ required: true, message: t('Messages.Please input your password') }]}
                                >
                                    <Input.Password placeholder={t('ModalDetail.Password')} />
                                </Form.Item>
                                <a className="login-form-resend" onClick={() => setToggleResendConfirmation(!toggleResendConfirmation)}>
                                    {t('Already have an account?')}
                                </a>

                                <Form.Item name="IsDynamics">
                                    <Input hidden={true} placeholder="IsDynamics" />
                                </Form.Item>

                                <Form.Item>
                                    <Button htmlType="submit" type="primary" loading={loggingIn}>
                                        {t('Button.Log In')}
                                    </Button>

                                    <a className="login-form-forgot" onClick={() => setToggleForgotPassword(!toggleForgotPassword)}>
                                        {t('Login Page.Forgot Password')}
                                    </a>
                                </Form.Item>
                            </Form>
                        </div>
                    )}
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
        loginRejected: state.login.loginRejected,
        loggingIn: state.login.loggingIn,
        loginMessage: state.login.message,

        requestPasswordSuccess: state.login.requestPasswordSuccess,
        requestPasswordFailed: state.login.requestPasswordFailed,
        requestingPassword: state.login.requestingPassword,
        requestMessage: state.login.requestMessage
    };
}

function mapDispatchToProps(dispatch) {
    return {
        onSubmitLogin: (data) => dispatch(submitLogin(data)),
        onResetRequest: (data) => dispatch(sendRequestPasswordRequest(data)),
        onResendRequest: (data) => dispatch(sendRequestResendConfirm(data)),
        getUsers: (id) => dispatch(getUsers(id)),
        getPinAssets: () => dispatch(getPinAssets())
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(Login);
