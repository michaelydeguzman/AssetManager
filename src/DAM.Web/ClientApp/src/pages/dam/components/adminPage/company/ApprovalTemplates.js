import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { getApprovalTemplates, updateApprovalTemplate, deleteApprovalTemplate, createApprovalTemplate, getApprovers } from '../../../actions';
import { ArrowLeftOutlined, PlusOutlined } from '@ant-design/icons';

import { Row, Col, Card, Input, Select, Button, Form, TreeSelect, Radio, Divider } from 'antd';
import { useTranslation } from 'react-i18next';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMinus, faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';

function ApprovalTemplates(props) {
    const { Option } = Select;
    const [approvalTemplates, setApprovalTemplates] = useState([]);
    const [newApprovalTemplateName, setNewApprovalTemplateName] = useState('');
    const [usersList, setUsersList] = useState([]);
    const [hasChanges, setHasChanges] = useState(false);

    const [selectedApprovalTemplate, setSelectedApprovalTemplate] = useState(null);
    const [selectedApprovalTemplateName, setSelectedApprovalTemplateName] = useState('');
    const [selectedApprovalTemplateLevels, setSelectedApprovalTemplateLevels] = useState([]);
    const [selectedLevelIndex, setSelectedLevelIndex] = useState(0);
    const [currentTemplateId, setCurrentTemplateId] = useState(null);
    const { t } = useTranslation();
    const formItemLayout = {
        labelCol: {
            span: 4
        },
        wrapperCol: {
            span: 20
        },
        layout: 'horizontal'
    };

    useEffect(() => {
        setupData();
    }, [props]);

    const setupData = async () => {
        // fetch approval templates
        let approvalTemplates = await props.getApprovalTemplates(props.company.id);
        if (approvalTemplates && approvalTemplates.data.approvalTemplates) {
            setApprovalTemplates(approvalTemplates.data.approvalTemplates);
        }

         // fetch approvers
        let approverResponse = await props.getApprovers();
        let approversList = []
        let filterApproversByCompany = approverResponse.data.users.filter(u => {
            if (u.companyId === props.company.id || (u.companyId == null && u.userRole.canApprove)) {
                return u;
            }
        })
        approversList = filterApproversByCompany.map(approver => ({
            value: approver.id,
            label: approver.userName
        }));

        setUsersList(approversList);
    };

    const onSubmit = async () => {
        let saveTemplate = {
            id: selectedApprovalTemplate.id,
            templateName: selectedApprovalTemplateName,
            companyId: props.company.id,
            isDeleted: false,
            approvalTemplateLevels: selectedApprovalTemplateLevels
        };

        var result = await props.updateApprovalTemplate(saveTemplate);
        if (result && result.data.approvalTemplates) {
            setApprovalTemplates(result.data.approvalTemplates);
            let filterTemplates = result.data.approvalTemplates.filter(x => x.id === selectedApprovalTemplate.id);
            setCurrentTemplateId(filterTemplates[0].id);
            setSelectedApprovalTemplate(filterTemplates[0]);
            setSelectedApprovalTemplateName(filterTemplates[0].templateName);
            setSelectedApprovalTemplateLevels(filterTemplates[0].approvalTemplateLevels);
            setHasChanges(false);
        }
    };

    const handleEditNewApprovalTemplate = (e) => {
        setNewApprovalTemplateName(e.target.value);
    }

    const handleTemplateNameEdit = (e) => {
        setSelectedApprovalTemplateName(e.target.value);
        setHasChanges(true);
    }

    const handleTemplateSelect = (e) => {
        var id = parseInt(e);
        setCurrentTemplateId(id);
        let filterTemplates = approvalTemplates.filter(x => x.id === id);
        setSelectedApprovalTemplate(filterTemplates[0]);
        setSelectedApprovalTemplateName(filterTemplates[0].templateName);
        setSelectedApprovalTemplateLevels(filterTemplates[0].approvalTemplateLevels);
        setHasChanges(false);
    }

    const handleDropdownVisibleChange = (e) => {
        setNewApprovalTemplateName('');
    }

    function selectApproverOptions(level) {
        return usersList.map((user, index) => {
            return (
                <Select.Option value={user.value} key={index} level={level}>
                    {user.label}
                </Select.Option>
            );
        });
    }

    const handleAddNewLevelClick = () => {
        let levelCount = selectedApprovalTemplateLevels.length;
        let currApprovalLevels = [...selectedApprovalTemplateLevels];

        var newLevel = {
            id: null,
            levelOrderNumber: levelCount + 1,
            approvers: []
        }
        currApprovalLevels.push(newLevel);
        setSelectedApprovalTemplateLevels(currApprovalLevels);
        setHasChanges(true);
    }

    const handleRemoveLevelClick = () => {
        let lastLevel = [...selectedApprovalTemplateLevels];
        lastLevel.pop();
        setSelectedApprovalTemplateLevels(lastLevel);
        setHasChanges(true);
    }

    const mapLevelApprovers = (approvers, index) => {
        let selectedApproversInLevel = [];
        if (approvers) {
            selectedApproversInLevel = approvers.map(user => { return user.approverId });
        }
        return selectedApproversInLevel;
    }

    const handleApproverChanges = (item, options) => {
        var index = selectedLevelIndex;
        var updateApprovalLevel = [...selectedApprovalTemplateLevels];

        var approversToModify = [];

        options.forEach((option) => {
            var newApprover = {
                approverId: option.value,
                id: option.id,
            };
            approversToModify.push(newApprover);
        });

        var approvalLevelToModify = selectedApprovalTemplateLevels[index];
        approvalLevelToModify.approvers = approversToModify;

        setSelectedApprovalTemplateLevels(updateApprovalLevel);
        setHasChanges(true);
    }

    const handleCreateNewTemplate = async () => {
        if (newApprovalTemplateName.length > 0 && approvalTemplates.filter(x => x.templateName === newApprovalTemplateName).length === 0) {
            let newTemplate = {
                id: null,
                templateName: newApprovalTemplateName,
                companyId: props.company.id,
                isDeleted: false,
                approvalTemplateLevels: []
            };

            var result = await props.createApprovalTemplate(newTemplate);
            if (result && result.data.approvalTemplates) {
                setApprovalTemplates(result.data.approvalTemplates);
                let filterTemplates = result.data.approvalTemplates.filter(x => x.templateName === newApprovalTemplateName);
                setCurrentTemplateId(filterTemplates[0].id);
                setSelectedApprovalTemplate(filterTemplates[0]);
                setSelectedApprovalTemplateName(filterTemplates[0].templateName);
                setSelectedApprovalTemplateLevels(filterTemplates[0].approvalTemplateLevels);
                setNewApprovalTemplateName('');

                setHasChanges(false);
            }
        }
    }

    const handleDeleteTemplate = async () => {
        let saveTemplate = {
            id: selectedApprovalTemplate.id,
            templateName: selectedApprovalTemplateName,
            companyId: props.company.id,
            isDeleted: true,
            approvalTemplateLevels: selectedApprovalTemplateLevels
        };

        var result = await props.deleteApprovalTemplate(saveTemplate);
        if (result && result.data.approvalTemplates) {
            setCurrentTemplateId(null);
            setApprovalTemplates(result.data.approvalTemplates);
            setSelectedApprovalTemplate(null);
            setSelectedApprovalTemplateName('');
            setSelectedApprovalTemplateLevels([]);
            setHasChanges(false);
        }
    }

    return (
        <Card title={t('Label.Edit Approval Templates')} type="inner" className="card-container" style={{ margin: '15px' }}>
            <Row>
                <ArrowLeftOutlined
                    onClick={() => {
                        props.handleBackButton(false);
                    }}
                />
            </Row>
            <br />
            <Row gutter={8, 8}>
                <Col xs={24} sm={24} md={24} lg={24} xl={3} xxl={3} align="left" style={{paddingTop: 5}}>
                    <span>{t('Label.Select Approval Template')}</span>
                </Col>
                <Col xs={24} sm={24} md={24} lg={24} xl={9} xxl={9} align="left">
                    <Select
                        label={t('Label.Approval Template')}
                        className="approval-template-select"
                        placeholder={t('Messages.Select') + "..."}
                        dropdownRender={menu => (
                            <div>
                                {menu}
                                <Divider />
                                <div className="create-new-approval">
                                    <Input type="text" value={newApprovalTemplateName} onChange={handleEditNewApprovalTemplate} placeholder={t('Label.Create New')} />
                                    <a className="plus-symbol">
                                        <PlusOutlined onClick={handleCreateNewTemplate} />
                                    </a>
                                </div>
                            </div>
                        )}
                        value={currentTemplateId}
                        onSelect={handleTemplateSelect}
                        onDropdownVisibleChange={handleDropdownVisibleChange}
                        className="approval-dropdown-select"
                    >
                        {approvalTemplates.map(item => (
                            <Option key={item.id} value={item.id}>{item.templateName}</Option>
                        ))}
                    </Select>
                </Col>
            </Row>
            <br />

            {currentTemplateId ?
                <>
                    <Divider />
                    <Row gutter={16, 16}>
                        <Col xs={24} sm={24} md={24} lg={24} xl={3} xxl={3} align="left" style={{marginTop: 10}}>
                            <span> {t('Label.Template Name')} </span>
                        </Col>
                        <Col xs={24} sm={24} md={24} lg={24} xl={9} xxl={9} align="left" style={{ marginTop: 10 }}>
                            <Input value={selectedApprovalTemplateName} onChange={(e) => handleTemplateNameEdit(e)}></Input>
                        </Col>

                        <Col style={{ marginTop: 10 }}>
                            <Button className="action-buttons" color="secondary" onClick={() => handleDeleteTemplate()} >
                                <FontAwesomeIcon icon={faTrash} />
                                {t('Button.Delete')}
                            </Button>
                        </Col>
                    </Row>
                </> : ''
            }
            <br />

            {currentTemplateId && selectedApprovalTemplate && selectedApprovalTemplateLevels ?
                <Row gutter={16, 16}>
                    <Col xs={24} sm={24} md={24} lg={24} xl={3} xxl={3} align="left">
                        <span>   {t('Label.Workflow')} </span>
                    </Col>
                    <Col xs={24} sm={24} md={24} lg={24} xl={9} xxl={9} align="left">
                        {selectedApprovalTemplateLevels.map((level, index) => (
                            <>
                                <Row>
                                    <Col span={24}>
                                        <Row>
                                            <Col span={24}>
                                                <div className="approver-level">
                                                    <label className="approver-level-header">{t('Label.Approval Level') + ' ' + (index + 1)}</label>

                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="select-approvers-row">
                                            <Col span={24}>
                                                <Select
                                                    showSearch={true}
                                                    mode="multiple"
                                                    placeholder={t('Messages.Select')}
                                                    className="approval-dropdown-select"
                                                    bordered={true}
                                                    value={mapLevelApprovers(level.approvers, index)}
                                                    onMouseEnter={() => setSelectedLevelIndex(index)}
                                                    onChange={handleApproverChanges}
                                                >
                                                    {selectApproverOptions(index)}
                                                </Select>
                                            </Col>
                                        </Row>
                                    </Col>
                                </Row>
                                <br />
                            </>
                        ))}
                        <Row gutter={8}>
                            <Col>
                                <Button className="action-buttons" color="secondary" onClick={() => handleAddNewLevelClick()} disabled={selectedApprovalTemplate.approvalTemplateLevels.length === 9}>
                                    <FontAwesomeIcon icon={faPlus} />
                                    {t('Button.Add Level')}
                                </Button>
                            </Col>
                            {selectedApprovalTemplateLevels && selectedApprovalTemplateLevels.length > 0 ?
                                <Col className="buttonDivs">
                                    <Button className="action-buttons" color="secondary" onClick={() => handleRemoveLevelClick()}>
                                        <FontAwesomeIcon icon={faMinus} />
                                        {t('Button.Remove Level')}
                                    </Button>
                                </Col>
                                : ""
                            }
                        </Row>
                    </Col>
                </Row> : ''
            }
            <br />
            {currentTemplateId ?
                <Row gutter={16, 16}>
                    <Col>
                        <Button type="primary" htmlType="submit" disabled={!hasChanges} onClick={onSubmit}>
                            {t('Button.Save')}
                        </Button>
                    </Col>
                </Row> : ''
            }


        </Card>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        getApprovalTemplates: (id) => dispatch(getApprovalTemplates(id)),
        getApprovers: () => dispatch(getApprovers()),
        createApprovalTemplate: (data) => dispatch(createApprovalTemplate(data)),
        updateApprovalTemplate: (data) => dispatch(updateApprovalTemplate(data)),
        deleteApprovalTemplate: (data) => dispatch(deleteApprovalTemplate(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ApprovalTemplates));
