import React, { memo, useContext, useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { DatePicker, Modal, Row, Col, Form, Input, Button, Space, Select, Checkbox } from 'antd';
import { addUserOutOfOffice, editUserOutOfOffice, getAssetsForApprovalOnOOO, delegateApprovals, getApprovers } from '../../../../actions';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import moment from 'moment';

function OOOModal(props) {
    const {
        approvalFlag
    } = useContext(LowFrequencyContext);

    const [defaultDelegateOptions, setDefaultDelegateOptions] = useState([]);
    const [assetsForApproval, setAssetsForApproval] = useState([]);
    const { t } = useTranslation();
    const [form] = Form.useForm();
    const {
        open,
        close,
        isEdit,
        currentUser,
        reloadCalendar,
        selectedOOO,
        startDateOptional
    } = props;
    const userRole = currentUser.userRole;
    const [approvers, setApprovers] = useState([]);
    const [defaultDelegateUser, setDefaultDelegateUser] = useState('');
    const [approvalsToDelegate, setApprovalsToDelegate] = useState([]);
    const [canDelegate, setCanDelegate] = useState(false);
    const [startDate, setStartDate] = useState(null);
    const [endDate, setEndDate] = useState(null);

    useEffect(() => {
        setupData();
    }, [props]);

    useEffect(() => {
        if (approvalFlag && userRole.canApprove && isEdit) {
            checkIfCanDelegate();
        }
    }, [approvalsToDelegate])

    useEffect(() => {
        if (approvalFlag && userRole.canApprove && isEdit) {
            retrieveApprovalsOnOO(currentUser.id, startDate, endDate);
        }
    }, [startDate, endDate])

    const setupData = () => {
        if (open) {
            if (userRole.canApprove) {
                retrieveDefaultUserDelegateOptions();
            }

            if (isEdit && selectedOOO && form) {
                form.setFieldsValue({
                    title: selectedOOO.title,
                    startDate: moment(selectedOOO.startDate),
                    endDate: moment(selectedOOO.endDate),
                    defaultDelegateUser: selectedOOO.defaultDelegateUser,
                    description: selectedOOO.description
                });
                setDefaultDelegateUser(selectedOOO.defaultDelegateUser);
                setStartDate(moment(selectedOOO.startDate).toISOString());
                setEndDate(moment(selectedOOO.endDate).toISOString());

                if (approvalFlag && userRole.canApprove) {
                    retrieveApprovalsOnOO(currentUser.id, moment(selectedOOO.startDate).toISOString(), moment(selectedOOO.endDate).toISOString());
                }
            } else {
                if (startDateOptional) {
                    form.setFieldsValue({
                      
                        startDate: moment(startDateOptional)
                       
                    });
                    setStartDate(moment(startDateOptional).toISOString());
                }
            }
        }
    }

    const checkIfCanDelegate = () => {
        var canDelegate = false;
        if (approvalsToDelegate.length > 0) {
            let filteredWithDelegatees = approvalsToDelegate.filter(a => {
                if (a.delegateId.length > 0) {
                    return a;
                }
            });

            if (approvalsToDelegate.length === filteredWithDelegatees.length) {
                canDelegate = true;
            }
        }

        setCanDelegate(canDelegate);
    }

    const retrieveDefaultUserDelegateOptions = async () => {
        let approversList = []

        // fetch approvers
        let approverResponse = await props.getApprovers();

        setApprovers(approverResponse.data.users);

        let filterApproversByCompany = approverResponse.data.users.filter(u => {
            if (((currentUser.companyId && u.companyId === currentUser.companyId) || (currentUser.companyId === null && u.companyId == null && u.userRole.canApprove)) && u.id !== currentUser.id) {
                return u;
            }
        })
        approversList = filterApproversByCompany.map(approver => ({
            value: approver.id,
            label: approver.userName
        }));
        setDefaultDelegateOptions(approversList);
    }

    const retrieveAssetDelegationOptions = (companyId) => {
        let delegateeOptions = [];

        if (companyId) {
            let source = approvers.filter(a => (a.companyId === companyId || (a.userRole.canApprove && a.companyId == null)) && a.id !== currentUser.id);

            delegateeOptions = source.map(approver => ({
                value: approver.id,
                label: approver.userName
            }));
        } else {
            delegateeOptions = defaultDelegateOptions;
        }

        return (
            delegateeOptions.map((user, index) => {
                return (
                    <Select.Option value={user.value} key={index}>
                        {user.label}
                    </ Select.Option>
                )
            })
        )
    }

    const retrieveApprovalsOnOO = async (userId, startDate, endDate) => {
        if (startDate && endDate) {
            let input = {
                approverId: userId,
                startDate: startDate,
                endDate: endDate
            }
            var result = await props.getAssetsForApprovalOnOOO(input);

            if (result && result.data.assetsForApproval) {
                setAssetsForApproval(result.data.assetsForApproval);
            }
        }
    }

    function disabledDate(endDate) {
        var startDate = form.getFieldValue('startDate');

        if (startDate) {
            return endDate && endDate <= moment(startDate);
        }
    }

    function handleStartDateChange(startDate) {
        var endDate = form.getFieldValue('endDate');
        if (endDate) {
            if (startDate >= moment(endDate)) {
                form.setFieldsValue({
                    endDate: ''
                })
            }
        }
        setStartDate(startDate.toISOString());
    }

    function handleEndDateChange(endDate) {
        setEndDate(endDate.toISOString());
    }

    function handleCloseModal() {
        form.resetFields();
        setAssetsForApproval([]);
        setApprovalsToDelegate([]);
        close();
    }

    const saveOOO = async (value) => {
        let result = null;
        let saveOOO = {
            id: isEdit ? selectedOOO.id : null,
            userId: currentUser.id,
            title: value.title,
            startDate: value.startDate.set("hour", 0).set("minute", 0).set("second", 0).toISOString(),
            endDate: value.endDate.set("hour", 23).set("minute", 59).set("second", 59).toISOString(),
            description: value.description,
            deleted: false,
            defaultDelegateUser: value.defaultDelegateUser
        };

        if (isEdit) {
            result = await props.editUserOutOfOffice(saveOOO);
        } else {
            result = await props.addUserOutOfOffice(saveOOO);
        }

        if (result.status && result.status === 200) {
            reloadCalendar();
            handleCloseModal();
        }
    }

    const deleteOOO = async () => {
        let result = null;
        let deleteOOO = {
            id: selectedOOO.id,
            userId: selectedOOO.userId,
            title: selectedOOO.title,
            startDate: selectedOOO.startDate,
            endDate: selectedOOO.endDate,
            description: selectedOOO.description,
            deleted: true,
            defaultDelegateUser: selectedOOO.defaultDelegateUser
        };

        if (isEdit) {
            result = await props.editUserOutOfOffice(deleteOOO);
        }

        if (result.status && result.status === 200) {
            reloadCalendar();
            handleCloseModal();
        }
    }

    const handleGoToAssetReview = (assetId) => {
        window.open(`${window.location.origin}/approvals/${assetId}`, "_blank");
    }

    function renderOOOForm() {
        return (<Form
            form={form}
            layout="vertical"
            onFinish={saveOOO}
            scrollToFirstError
        >
            <Row>
                <Col span={24}>
                    <Form.Item
                        name="title"
                        label={t('OOOModal.Title')}
                        rules={[
                            {
                                required: true,
                                message: t('OOOModal.Title required')
                            }
                        ]}
                    >
                        <Input />
                    </Form.Item>
                </Col>
            </Row>
            <Row>
                <Col span={12}>
                    <Form.Item
                        name="startDate"
                        label={t('OOOModal.Start Date')}
                        rules={[
                            {
                                required: true,
                                message: t('OOOModal.Start Date required')
                            }
                        ]}
                    >
                        <DatePicker onChange={handleStartDateChange} />
                    </Form.Item>
                </Col>
                <Col span={12}>
                    <Form.Item
                        name="endDate"
                        label={t('OOOModal.End Date')}
                        rules={[
                            {
                                required: true,
                                message: t('OOOModal.End Date required')
                            }
                        ]}
                    >
                        <DatePicker
                            disabledDate={disabledDate}
                            onChange={handleEndDateChange}
                        />
                    </Form.Item>
                </Col>
            </Row>
            {approvalFlag && userRole.canApprove && (
                <Row>
                    <Col span={24}>
                        <Form.Item
                            name="defaultDelegateUser"
                            label={t('OOOModal.Default Delegate')}>
                            <Select
                                showSearch={true}
                                placeholder={t('Messages.Select')}
                                className="approval-dropdown-select"
                                bordered={true}
                            //value={mapLevelApprovers(level.approvers, index)}
                            //onMouseEnter={() => setSelectedLevelIndex(index)}
                            //onChange={handleApproverChanges}
                            //onChange={(e) => setDefaultDelegateUser(e)}
                            >
                                {defaultDelegateOptions.map((user, index) => {
                                    return (
                                        <Select.Option value={user.value} key={index}>
                                            {user.label}
                                        </ Select.Option>
                                    )
                                }
                                )}
                            </Select>
                        </Form.Item>
                    </Col>
                </Row>)
            }
            <Row>
                <Col span={24}>
                    <Form.Item name="description"
                        label={t('OOOModal.Description')}>
                        <Input.TextArea />
                    </Form.Item>
                </Col>
            </Row>
            <Row style={{ marginTop: 10, marginBottom: 10 }}>
                <Col span={12} align="left">
                    {isEdit && (<Button
                        type="primary"
                        onClick={deleteOOO}
                    >
                        {t('Button.Delete')}
                    </Button>
                    )}

                </Col>
                <Col span={12} align="right">
                    <Space>
                        <Button
                            type="secondary"
                            onClick={handleCloseModal}
                        >
                            {isEdit ? t('Button.Close') : t('Button.Cancel')}
                        </Button>
                        <Button
                            type="primary"
                            htmlType="submit"
                        >
                            {t('Button.Save')}
                        </Button>
                    </Space>
                </Col>
            </Row>
        </Form>);

    }

    const handleDelegateeChange = (delegateId, approvalLevelApproverId) => {
        var newApprovalsToDelegate = [...approvalsToDelegate];
        var filtered = newApprovalsToDelegate.filter(x => x.approvalLevelApproverId === approvalLevelApproverId);
        if (filtered.length > 0) {
            filtered[0].delegateId = delegateId;
        }
        setApprovalsToDelegate(newApprovalsToDelegate);
    };

    const handleOnCheck = (e) => {
        var approvalLevelApproverId = e.target.value;
        var isChecked = e.target.checked;

        var newApprovalsToDelegate = [...approvalsToDelegate];
        if (isChecked) {
            // add to delegate list
            var item = assetsForApproval.filter(a => a.approvalLevelApproverId === approvalLevelApproverId)[0];
            if (newApprovalsToDelegate.filter(x => x.approvalLevelApproverId === approvalLevelApproverId).length === 0) {

                var delegateeUserName = document.getElementById('select-delegatee-' + approvalLevelApproverId).parentNode.parentNode.getElementsByTagName('span')[1].title;

                let approval = {
                    approvalLevelId: item.approvalLevelId,
                    approvalLevelApproverId: item.approvalLevelApproverId,
                    approverId: item.approverId,
                    delegateId: delegateeUserName.length > 0 ? approvers.filter(u => u.userName === delegateeUserName)[0].id : ''
                }
                newApprovalsToDelegate.push(approval);
            }
            setApprovalsToDelegate(newApprovalsToDelegate);
        } else {
            // remove from list
            let filteredList = newApprovalsToDelegate.filter(x => x.approvalLevelApproverId !== approvalLevelApproverId);

            setApprovalsToDelegate(filteredList);
        }
    };

    const handleDelegate = async () => {
        let dto = {
            delegateApprovals: approvalsToDelegate
        }
        await props.delegateApprovals(dto);

        setupData();
    }

    return (
        <Modal
            title={isEdit ? t('OOOModal.OOOHeaderEdit') : t('OOOModal.OOOHeaderCreate')}
            visible={open}
            onCancel={handleCloseModal}
            centered={true}
            width={isEdit && assetsForApproval.length > 0 ? '65%' : '342px'}
            footer={false}
            getContainer={false}
            closable={false}
            keyboard={false}
            destroyOnClose
            className="ooo-modal"
        >
            {approvalFlag && userRole.canApprove && isEdit && assetsForApproval.length > 0 ?
                <Row>
                    <Col xs={24} md={24} lg={8} xl={8} xxl={8}>
                        {renderOOOForm()}
                    </Col>
                    <Col xs={0} md={1} lg={1} xl={1} xxl={1}>
                        <Col xs={12} md={12} lg={12} className="vertical-divider"></Col>
                    </Col>
                    <Col xs={24} md={24} lg={15} xl={15} xxl={15} className="delegate-approval-section">
                        <Row>
                            <h3>{t('OOOModal.OOOWarning')}</h3>
                        </Row>
                        <br />
                        {assetsForApproval.map(approval => {
                            return (
                                <>
                                    <Row>
                                        <Col xs={24} sm={24} md={24} lg={11} xl={11} xxl={11} align="left">
                                            <Row>
                                                <Checkbox className={"approval-" + approval.approvalLevelApproverId}
                                                    value={approval.approvalLevelApproverId}
                                                    onChange={handleOnCheck}
                                                >
                                                    {approval.title}
                                                </Checkbox>
                                            </Row>
                                            <Row className="ooo-approval-label"><h5>{t('Label.Level')} {approval.levelNumber} {t('OOOModal.Due On')} {moment(approval.dueDate).format('DD/MM/YYYY HH:mm A')}</h5></Row>

                                        </Col>
                                        <Col xs={24} sm={24} md={24} lg={4} xl={4} xxl={4} align="left">
                                            <Button type="secondary" onClick={() => handleGoToAssetReview(approval.assetId)}>{t('Button.Review')}</Button>
                                        </Col>
                                        <Col xs={24} sm={24} md={24} lg={9} xl={9} xxl={9} align="right">
                                            <Space>
                                                <h5 style={{ marginTop: 5 }}>{t('OOOModal.Delegate To')}</h5>
                                                <Select
                                                    id={"select-delegatee-" + approval.approvalLevelApproverId}
                                                    showSearch={true}
                                                    placeholder={t('Messages.Select')}
                                                    className="approval-dropdown-select"
                                                    bordered={true}
                                                    onChange={(e) => handleDelegateeChange(e, approval.approvalLevelApproverId)}
                                                >
                                                    {retrieveAssetDelegationOptions(approval.companyId)}

                                                </Select>
                                            </Space>
                                        </Col>
                                    </Row>
                                    <br />
                                </>)
                        })}
                        <br />
                        <Row>
                            <Col span={12}>

                            </Col>
                            <Col span={12} align="right">
                                <Button className="delegate-btn" onClick={() => handleDelegate()} disabled={!canDelegate}>{t('Button.Delegate')}</Button>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                :
                <>
                    {renderOOOForm()}
                </>
            }
        </Modal>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        getApprovers: () => dispatch(getApprovers()),
        addUserOutOfOffice: (data) => dispatch(addUserOutOfOffice(data)),
        editUserOutOfOffice: (data) => dispatch(editUserOutOfOffice(data)),
        getAssetsForApprovalOnOOO: (id, startDate, endDate) => dispatch(getAssetsForApprovalOnOOO(id, startDate, endDate)),
        delegateApprovals: (data) => dispatch(delegateApprovals(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(OOOModal));
