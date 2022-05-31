import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { Card, Input, Table, Button, Spin, Row, Col, Space, Form } from 'antd';
import { getEmailTemplates, saveEmailTemplate } from '../../../actions';
import _, { forEach } from 'lodash';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';

import { getEmailTemplateClassification, getEmailTemplateCategory } from '@damhelper';

function EmailTemplates(props) {
    const { t } = useTranslation();
    const [isTemplateEditing, setIsTemplateEditing] = useState(false);
    const [templatesList, setTeamsList] = useState([]);
    const [templatesSearch, setTemplatesSearch] = useState('');

    const [filteredTemplateList, setFilteredTemplateList] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const [form] = Form.useForm();
    const [subject, setSubject] = useState('');
    const [body, setBody] = useState('');
    const [prevMsg, setPrevMsg] = useState('');

    useEffect(() => {
        setupData();
    }, []);

    useEffect(() => {
        const previewHtml = document.getElementById('previewHtml');

        if (previewHtml) {
            let styleEl = previewHtml.getElementsByTagName('style');

            for (let index = 0; index < styleEl.length; index++) {
                styleEl[index].parentNode.removeChild(styleEl[index]);
            }
        }
    }, [body]);

    useEffect(() => {
        if (templatesSearch.trim() == '') {
            setFilteredTemplateList(templatesList);
        } else {
            var filteredList = filterTemplateResultsBySearchText(templatesList, templatesSearch.trim());
            setFilteredTemplateList(filteredList);
        }
    }, [templatesSearch]);

    function filterTemplateResultsBySearchText(teamList, keyword) {
        return teamList.filter(
            (team) =>
                team.teamName.toLowerCase().includes(keyword.toLowerCase()) ||
                team.teamDescription.toLowerCase().includes(keyword.toLowerCase())
        );
    }

    const setDataSourceAndFilters = (data) => {
        let dataSource = [];

        data.map((template) => {
            dataSource.push({
                id: template.id,
                emailTemplateName: template.emailTemplateName,
                emailTemplateKey: template.emailTemplateKey,
                subject: template.subject,
                message: template.message,
                classification: template.classification,
                recipientType: template.recipientType,
                category: template.category,
                contents: template.contents
            });
        });

        setTeamsList(dataSource);
        setFilteredTemplateList(dataSource);
    };

    async function setupData() {
        // Get current login user company
        setIsLoading(true);

        let fetchTemplatesResult = await props.getEmailTemplates();

        setDataSourceAndFilters(fetchTemplatesResult.data.emailTemplates);

        setIsLoading(false);
    }

    const templateListColumns = [
        {
            width: 120,
            title: 'Event Trigger',
            dataIndex: 'emailTemplateName',
            key: 'emailTemplateName',
            sorter: (a, b) =>
                a.emailTemplateName.localeCompare(b.emailTemplateName, undefined, {
                    numeric: true,
                    sensitivity: 'base'
                }),
            sortDirections: ['ascend', 'descend']
        },
        {
            width: 70,
            title: 'Category',
            dataIndex: 'category',
            key: 'category',
            render: (value) => getEmailTemplateCategory(value)
        },
        {
            width: 120,
            title: 'Template Key',
            dataIndex: 'emailTemplateKey',
            key: 'emailTemplateKey',
            sorter: (a, b) =>
                a.emailTemplateKey.localeCompare(b.emailTemplateKey, undefined, {
                    numeric: true,
                    sensitivity: 'base'
                }),
            sortDirections: ['ascend', 'descend']
        },
        {
            width: 110,
            title: 'Recipient Type',
            dataIndex: 'recipientType',
            key: 'recipientType',
            sorter: (a, b) =>
                a.recipientType.localeCompare(b.recipientType, undefined, {
                    numeric: true,
                    sensitivity: 'base'
                }),
            sortDirections: ['ascend', 'descend']
        },
        {
            width: 70,
            title: 'Classification',
            dataIndex: 'classification',
            key: 'classification',
            render: (value) => getEmailTemplateClassification(value)
        },
        {
            width: 50,
            fixed: 'right',
            render: (team) => (
                <div>
                    <Button className="option-button" onClick={() => handleEditClick(team)}>
                        <FontAwesomeIcon icon={faEdit} />
                        {t('Button.Edit')}
                    </Button>
                </div>
            )
        }
    ];

    const handleEditClick = (template) => {
        setIsTemplateEditing(true);

        form.setFieldsValue({
            id: template.id,
            emailTemplateName: template.emailTemplateName,
            emailTemplateKey: template.emailTemplateKey,
            subject: template.subject,
            message: template.message,
            classification: getEmailTemplateClassification(template.classification),
            recipientType: template.recipientType,
            category: getEmailTemplateCategory(template.category),
            contents: template.contents
        });

        setSubject(template .subject);
        var content = template.contents.replace('%%Message%%', template.message).replace('%%To%%','[' + template.recipientType + ']');
        setBody(content);

        setPrevMsg(template.message);
    };

    const changeSubject = () => {
        setSubject(form.getFieldValue('subject'));
    }

    const changeMessage = () => {
        var newMsg = form.getFieldValue('message');
        var content = body.replace(prevMsg, newMsg);
        setBody(content);
        setPrevMsg(newMsg);
    }

    const handleSaveTemplate = async (data) => {
        var template = {
            id: data.id,
            subject: data.subject,
            message: data.message,
        }

        await props.saveEmailTemplate(template);
        setIsTemplateEditing(false);
        setupData();
    }

    return (
        <>
            {isTemplateEditing ? (
                <>
                    <Card
                        className="card-container"
                        style={{ marginTop: 15, marginLeft: 15, marginRight: 15 }}
                    >
                        <Row>
                            <Col span={8}>
                                <Space>
                                    <h3>
                                        <Button type="link" onClick={() => {
                                            setIsTemplateEditing(false);
                                            form.resetFields();
                                        }}>
                                            <FontAwesomeIcon icon={faArrowLeft} />
                                        </Button>
                                        {t('Edit Template')}
                                    </h3>
                                </Space>
                            </Col>
                            <Col span={16} align="right">
                                <Space>
                                    <Button onClick={() => {
                                        setIsTemplateEditing(false);
                                        form.resetFields();
                                    }}>Cancel</Button>
                                    <Button type="primary" onClick={form.submit}>Save</Button>
                                </Space>
                            </Col>
                        </Row>
                    </Card>

                    <Row gutter={[12, 12]}>
                        <Col xs={24} lg={10}>
                            <Card
                                title={t('Details')}
                                className="card-container"
                                style={{ marginTop: 15, marginLeft: 15 }}
                            >
                                <>
                                    <Form
                                        form={form}
                                        name="Team"
                                        labelCol={{ span: 6 }}
                                        wrapperCol={{ span: 14 }}
                                        layout="horizontal"
                                        onFinish={handleSaveTemplate}
                                        autoComplete="off"
                                        destroyOnClose={true}
                                    >
                                        <Form.Item name="id" style={{ height: '0px' }}></Form.Item>
                                        <Form.Item label="Event Trigger" name="emailTemplateName">
                                            <Input disabled/>
                                        </Form.Item>
                                        <Form.Item label="Category" name="category">
                                            <Input disabled/>
                                        </Form.Item>

                                        <Form.Item label="Recipient Type" name="recipientType">
                                            <Input disabled />
                                        </Form.Item>
                                        <Form.Item label="Subject" name="subject">
                                            <Input.TextArea autoSize={{ minRows: 2, maxRows: 2 }} onChange={changeSubject} />
                                        </Form.Item>
                                        <Form.Item label="Message" name="message">
                                            <Input.TextArea autoSize={{ minRows: 3, maxRows: 3 }} onChange={changeMessage} />
                                        </Form.Item>
                                         <Form.Item label="Classification" name="classification">
                                            <Input disabled/>
                                        </Form.Item>
                                    </Form>
                                </>
                            </Card>
                        </Col>
                        <Col xs={24} lg={14}>
                            <Card
                                title={
                                    'Preview'
                                }
                                className="card-container"
                                style={{ marginTop: 15, marginLeft: 15, marginRight: 15 }}
                            >
                                <span>Subject: {subject}</span>
                                <div id="previewHtml" dangerouslySetInnerHTML={{ __html: body }}/>
                            </Card>
                        </Col>
                    </Row>
                </>
            ) : (
                <Spin spinning={isLoading} size="large">
                    <Card
                        title={t('Slider.EmailTemplates')}
                        className="card-container"
                        style={{ margin: '15px' }}
                       >
                        <Table
                            columns={templateListColumns}
                            dataSource={filteredTemplateList}
                            scroll={{ x: 400 }}
                        />
                    </Card>
                </Spin>
            )}

        </>
    );
}

function mapStateToProps(state) {
    return {
       
    };
}

function mapDispatchToProps(dispatch) {
    return {
        getEmailTemplates: () => dispatch(getEmailTemplates()),
        saveEmailTemplate: (data) => dispatch(saveEmailTemplate(data))
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(EmailTemplates));
