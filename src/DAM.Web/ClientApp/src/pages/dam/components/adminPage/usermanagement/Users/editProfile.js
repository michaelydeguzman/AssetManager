import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Card, Input, Button, Form, message, Upload, Space, Tabs, Divider, Calendar, Badge, Select, Typography, Radio } from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { getBase64, Uint8ToBase64 } from '../../../../../../utilities/index';
import { changePassword, uploadUserImage, getUserOutOfOffice, updateUserProfile } from '../../../../actions';
import { LoadingOutlined, PlusOutlined, LeftOutlined, RightOutlined } from '@ant-design/icons';
import { FeedbackMessage, TYPE } from '../../../../../messageTextConstants';
import { useTranslation } from 'react-i18next';
import moment from 'moment';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretLeft, faCaretRight, faPlus } from '@fortawesome/free-solid-svg-icons';
import OOOModal from './oooModal';

function EditProfile(props) {
    const { t } = useTranslation();
    const { currentUser, setCurrentUser } = useContext(LowFrequencyContext);
    const [form] = Form.useForm();
    const [formPasswordReset] = Form.useForm();
    const [imageUrl, setImgUrl] = useState(currentUser ? currentUser.imageUrl : '');
    const [selectedFile, setSelectedFile] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const { TabPane } = Tabs;
    const formItemLayout = {
        labelCol: {
            xs: {
                span: 24
            },
            sm: {
                span: 24
            },
            md: {
                span: 10
            },
            lg: {
                span: 10
            },
            xl: {
                span: 10
            }
        },
        wrapperCol: {
            xs: {
                span: 24
            },
            sm: {
                span: 24
            },
            md: {
                span: 20
            },
            lg: {
                span: 20
            },
            xl: {
                span: 20
            }
        }
    };
    const tailFormItemLayout = {
        wrapperCol: {
            sm: {
                span: 24
            },
            md: {
                offset: 10,
                span: 16
            },
            lg: {
                offset: 10,
                span: 16
            }
        },
        display: 'flex'
    };
    const [activeTab, setActiveTab] = useState('1');

    const [isOOOOpen, setIsOOOOpen] = useState(false);
    const [userOOO, setUserOOO] = useState([]);
    const [selectedOOO, setSelectedOOO] = useState(null);
    const [optionalStartDate, setOptionalStartDate] = useState(null);
    const [isEdit, setIsEdit] = useState(false);

    useEffect(() => {
        if (currentUser) {
            form.setFieldsValue({
                userName: currentUser.userName,
                oldPassword: '',
                email: currentUser.email
            });
            setImgUrl(currentUser.imageUrl);
        }
    }, [currentUser]);

    useEffect(() => {
        if (activeTab === '3') {
            setupCalendarData();
        }
    }, [activeTab]);

    function onError(e) {
        e.target.src = imageUrl;
    }

    const setupCalendarData = async () => {
        var result = await props.getUserOutOfOffice(currentUser.id);

        if (result && result.data.userOOO) {
            setUserOOO(result.data.userOOO);
        }
    };

    const editOOO = async (date) => {
        let oooToEdit = userOOO.filter((o) => {
            if (moment(o.startDate) <= date && date <= moment(o.endDate)) {
                return o;
            }
        });

        if (oooToEdit.length > 0 && oooToEdit[0]) {
            setSelectedOOO(oooToEdit[0]);
            setIsEdit(true);
            setIsOOOOpen(true);
        }
    };

    const openCreateNew = () => {
        setIsEdit(false);
        setIsOOOOpen(true);
        setOptionalStartDate(null);
    };

    const openCreateNewWithStartDate = (startDate) => {
        setIsEdit(false);
        setIsOOOOpen(true);
        setOptionalStartDate(startDate);
    };

    function dateCellRender(value) {
        const cellDate = value.date();
        if (userOOO) {
            var dateHits = userOOO.filter((ooo) => {
                if (moment(ooo.startDate) <= value && value <= moment(ooo.endDate)) {
                    return ooo;
                }
            });

            if (dateHits.length > 0) {
                return (
                    <div className="calendarCell">
                        <Row>
                            <Col span={12}>
                            </Col>
                            <Col align="right" span={12} className={cellDate == moment().date() ? 'calendarDateToday' : 'calendarDate'}>
                                <span>{cellDate}</span>
                            </Col>
                        </Row>
                        <Row gutter={8}>
                            <div className="ooo-cell" onDoubleClick={(e) => editOOO(value)}>
                                {cellDate == moment(dateHits[0].startDate).date() && (
                                    <div >
                                        <Row className="ooo-text">
                                            <h5> {dateHits[0].title} </h5>
                                        </Row>
                                        <Row className="ooo-text">
                                            <span className="ooo-cell-span">{dateHits[0].description ? ' ' + dateHits[0].description : ''}</span>
                                        </Row>
                                    </div>
                                )}
                            </div>
                        </Row>
                    </div>
                );
            } else {
                return (
                    <div className="calendarCell" onDoubleClick={(e) => openCreateNewWithStartDate(value)}>
                        <Row>
                            <Col span={12}>
                            </Col>
                            <Col align="right" span={12} className={cellDate == moment().date() ? 'calendarDateToday' : 'calendarDate'}>
                                <span>{cellDate}</span>
                            </Col>
                        </Row>
                    </div>
                );
            }
        }
    }

    async function saveUserProfile(values) {
        await props.updateUserProfile(values);

        let newCurrentUser = currentUser;
        newCurrentUser.userName = values.userName;

        setCurrentUser(newCurrentUser);
    }

    function changePassword(values) {
        values.email = currentUser.email;
        values.displayName = currentUser.userName;
        props.changePassword(values);
        formPasswordReset.resetFields();
    }

    function beforeUpload(file) {
        const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/jpg';
        if (!isJpgOrPng) {
            FeedbackMessage(TYPE._REJECTED, 'You can only upload JPG/PNG files!');
        }
        const isLt2M = file.size / 1024 / 1024 < 2;
        if (!isLt2M) {
            FeedbackMessage(TYPE._REJECTED, 'Image must be smaller than 2MB!');
        }
        setSelectedFile(file);
        return isJpgOrPng && isLt2M;
    }
    function handleChange(info) {
        if (info.file.status === 'uploading') {
            setIsLoading(true);
            getBase64(info.file.originFileObj, async (imageUrl) => {
                setImgUrl(imageUrl);
                setIsLoading(false);
            });

            if (selectedFile.name) {
                const file = selectedFile;
                let base;

                let fileReader = new FileReader();

                fileReader.onload = async (e) => {
                    base = e.target.result;
                    var byteString = new Uint8Array(base);
                    var fileBytes = Uint8ToBase64(byteString);
                    const data = {
                        Id: currentUser.id,
                        name: selectedFile.name,
                        fileName: selectedFile.name,
                        description: '',
                        ImageFileExtension: selectedFile.name.split('.').pop(),
                        fileBytes: fileBytes,
                        fileType: selectedFile.type,
                        folderId: 0,
                        createdBy: props
                    };
                    await props.onUploadAsset(data);
                };

                fileReader.readAsArrayBuffer(file);
            }
            return;
        }
        if (info.file.status === 'done') {
            // Get this url from response in real world.
            getBase64(info.file.originFileObj, (imageUrl) => {
                setImgUrl(imageUrl);
                setIsLoading(false);
            });
        }
    }

    return (
        <Card title={t('ModalDetail.Profile')} type="inner" className="admin-title">
            <Tabs activeKey={activeTab} onChange={(e) => setActiveTab(e)}>
                <TabPane tab={t('Profile.Details')} key="1">
                    {activeTab === '1' && (
                        <Form {...formItemLayout} onFinish={saveUserProfile} scrollToFirstError form={form}>
                            <Row flex={5}>
                                <Col xs={24} sm={8} md={8} lg={8} xl={4} xxl={4} align="center">

                                    <Row align="center" style={{ marginTop: '5px' }}>
                                        <Upload
                                            name="avatar"
                                            listType="picture-card"
                                            showUploadList={false}
                                            beforeUpload={beforeUpload}
                                            onChange={handleChange}
                                            className="profilepic"
                                        >
                                            {imageUrl && <img src={imageUrl} onError={onError} alt="avatar" />}
                                            <div className="hover-box">
                                                {isLoading ? <LoadingOutlined /> : <PlusOutlined />}
                                                <div>{t('Button.Upload')}</div>
                                            </div>
                                        </Upload>
                                    </Row>

                                    <Row align="center" style={{ marginTop: '5px' }}>
                                        {t('Profile.Profile Picture')}
                                    </Row>

                                </Col>
                                <Col xs={24} sm={14} md={12} lg={10} xl={8} xxl={6} style={{ marginTop: '50px' }}>
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
                                    {currentUser && (
                                        <Form.Item name="email" label={t('ModalDetail.Email Address')}>
                                            <label>{currentUser.email}</label>
                                        </Form.Item>
                                    )}
                                    <Form.Item {...tailFormItemLayout}>
                                        <Button type="primary" htmlType="submit">
                                            {t('Button.Update')}
                                        </Button>
                                    </Form.Item>
                                </Col>
                            </Row>
                        </Form>
                    )}
                </TabPane>
                <TabPane tab={t('Profile.Update Password')} key="2">
                    {activeTab === '2' && (
                        <Form {...formItemLayout} onFinish={changePassword} scrollToFirstError form={formPasswordReset}>
                            <Row flex={5}>
                                <Col xs={24} sm={16} md={14} lg={12} xl={8} style={{ marginTop: '50px' }}>
                                    <Form.Item
                                        name="oldPassword"
                                        label={t('Login Page.Current Password')}
                                        hasFeedback
                                        rules={[
                                            {
                                                required: true,
                                                message: t('Messages.Enter Current Password')
                                            }
                                        ]}
                                    >
                                        <Input.Password />
                                    </Form.Item>
                                    <Form.Item
                                        name="password"
                                        label={t('Login Page.New Password')}
                                        hasFeedback
                                        rules={[
                                            {
                                                required: true,
                                                message: t('Messages.Enter a New Password!')
                                            }
                                        ]}
                                    >
                                        <Input.Password />
                                    </Form.Item>
                                    <Form.Item
                                        name="confirm"
                                        label={t('Login Page.Confirm Password')}
                                        dependencies={['password']}
                                        hasFeedback
                                        rules={[
                                            {
                                                required: true,
                                                message: t('Messages.Please confirm your password')
                                            },
                                            ({ getFieldValue }) => ({
                                                validator(_, value) {
                                                    if (!value || getFieldValue('password') === value) {
                                                        return Promise.resolve();
                                                    }
                                                    return Promise.reject(new Error('The two passwords that you entered do not match!'));
                                                }
                                            })
                                        ]}
                                    >
                                        <Input.Password />
                                    </Form.Item>
                                    <Form.Item {...tailFormItemLayout}>
                                        <Button type="primary" htmlType="submit">
                                            {t('Button.Update Password')}
                                        </Button>
                                    </Form.Item>
                                </Col>
                            </Row>
                        </Form>
                    )}
                </TabPane>
                <TabPane tab={t('Profile.Out of Office')} key="3">
                    {activeTab === '3' && (
                        <>
                            <Row>
                                <h3>{t('Profile.OOOHeader')}</h3>
                            </Row>
                            <Row>
                                <Col span={12}>
                                    <p>{t('Profile.OOOSubHeader')}</p>
                                </Col>
                                <Col span={12} align="right">
                                    <Button type="primary" onClick={openCreateNew}>
                                        <FontAwesomeIcon icon={faPlus} />
                                        {t('Button.Add Out of Office')}
                                    </Button>
                                </Col>
                            </Row>
                            <br />
                            <Calendar
                                locale={moment.locale()}
                                dateFullCellRender={dateCellRender}
                                headerRender={({ value, type, onChange, onTypeChange }) => {
                                    const start = 0;
                                    const end = 12;
                                    const monthOptions = [];

                                    const current = value.clone();
                                    const localeData = value.localeData();
                                    const months = [];
                                    for (let i = 0; i < 12; i++) {
                                        current.month(i);
                                        months.push(localeData.monthsShort(current));
                                    }

                                    for (let index = start; index < end; index++) {
                                        monthOptions.push(
                                            <Select.Option className="month-item" key={`${index}`}>
                                                {months[index]}
                                            </Select.Option>,
                                        );
                                    }
                                    const month = value.month();

                                    const year = value.year();
                                    const options = [];
                                    for (let i = year - 10; i < year + 10; i += 1) {
                                        options.push(
                                            <Select.Option key={i} value={i} className="year-item">
                                                {i}
                                            </Select.Option>,
                                        );
                                    }

                                    const leftArrowClick = () => {
                                        const newValue = value.clone();
                                        newValue.month(value.month() - 1, 10);
                                        onChange(newValue);
                                    }

                                    const rightArrowClick = () => {
                                        const newValue = value.clone();
                                        newValue.month(value.month() + 1, 10);
                                        onChange(newValue);
                                    }

                                    return (
                                        <div style={{ padding: 8 }}>
                                            <Row gutter={8} justify="end">
                                                <Col>
                                                    <Button style={{ paddingTop: 5 }} onClick={leftArrowClick}>

                                                        <FontAwesomeIcon icon={faCaretLeft} />
                                                    </Button>
                                                </Col>
                                                <Col>
                                                    <Button style={{ paddingTop: 5 }} onClick={rightArrowClick}>

                                                        <FontAwesomeIcon icon={faCaretRight} />
                                                    </Button>
                                                </Col>
                                                <Col>
                                                    <Select
                                                        dropdownMatchSelectWidth={false}
                                                        value={String(month)}
                                                        onChange={selectedMonth => {
                                                            const newValue = value.clone();
                                                            newValue.month(parseInt(selectedMonth, 10));
                                                            onChange(newValue);
                                                        }}
                                                    >
                                                        {monthOptions}
                                                    </Select>
                                                </Col>
                                                <Col>
                                                    <Select
                                                        dropdownMatchSelectWidth={false}
                                                        className="my-year-select"
                                                        onChange={newYear => {
                                                            const now = value.clone().year(newYear);
                                                            onChange(now);
                                                        }}
                                                        value={String(year)}
                                                    >
                                                        {options}
                                                    </Select>
                                                </Col>
                                            </Row>
                                        </div>
                                    );
                                }}
                            />
                            <OOOModal
                                open={isOOOOpen}
                                close={() => setIsOOOOpen(false)}
                                isEdit={isEdit}
                                currentUser={currentUser}
                                reloadCalendar={setupCalendarData}
                                selectedOOO={selectedOOO}
                                startDateOptional={optionalStartDate}
                            />
                        </>
                    )}
                </TabPane>
            </Tabs>
        </Card>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        changePassword: (data) => dispatch(changePassword(data)),
        onUploadAsset: (data) => dispatch(uploadUserImage(data)),
        getUserOutOfOffice: (id) => dispatch(getUserOutOfOffice(id)),
        updateUserProfile: (data) => dispatch(updateUserProfile(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(EditProfile));
