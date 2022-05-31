import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Modal, Button, Spin, Space, List, Input, message, Tag } from 'antd';
import {
    ExclamationCircleOutlined,
    PlusOutlined,
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { PROJECT_STATUS } from '../constants';
import { getProjectStatusTagName, getProjectStatusTagColor } from '@damhelper';

function ProjectList(props) {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = useState(false);
    const { confirm } = Modal;

    const [createNew, setCreateNew] = useState(false);
    const [newProjectName, setNewProjectName] = useState('');
    const { setSelectedProject, userProjects, loadProjects, efiles, createMode, toggleCreateMode, setCreateMode, isArchiveMode } = props;

    const selectProject = (item) => {
        setCreateMode(false);
        setSelectedProject(item);

        userProjects.forEach((project) => {
            if (item.id !== project.id) {
                var li = document.getElementById(`${project.id}-li`);
                li.style.backgroundColor = 'white';
            }
        });

        var li = document.getElementById(`${item.id}-li`);
        li.style.backgroundColor = '#c6c8c540';
    };

    return (
        <Spin spinning={isLoading}>
            {!isArchiveMode && (
                <>
                    {!createMode && (
                        <Row style={{ marginTop: 10 }}>
                            <Button className="option-button" onClick={toggleCreateMode}>
                                <Space>
                                    <PlusOutlined />
                                    {t('Projects.Create')}
                                </Space>
                            </Button>
                        </Row>
                    )}
                </>
            )}
          
            <List
                size="small"
                style={{ marginTop: 20 }}
                bordered
                dataSource={userProjects}
                renderItem={(item) => (
                    <List.Item id={item.id + '-li'} onClick={(e) => selectProject(item)}>
                        <Row className="project-list-row" align="middle" gutter={6}>
                            <Col xxs={16} xs={16} sm={18} md={18} lg={24} xl={24} xxl={16}>
                                <span id={item.id + '-span'}>
                                    {item.projectName}
                                </span>
                            </Col>
                            <Col xxs={8} xs={8} sm={6} md={6} lg={24} xl={24} xxl={8} align="right" >
                                <Tag className="project-status-tag" color={getProjectStatusTagColor(item.projectStatus)}>
                                    {getProjectStatusTagName(item.projectStatus)}
                                </Tag>
                            </Col>
                        </Row>
                    </List.Item>
                )}
            />
        </Spin>
    );
}

function mapStateToProps(state) {
    return {
        folderRefresh: state.dam.folderRefresh
    };
}

function mapDispatchToProps(dispatch) {
    return {
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectList));
