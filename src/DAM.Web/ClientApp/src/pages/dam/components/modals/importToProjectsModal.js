import React, { memo, useEffect, useState } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Space, List, Checkbox, Tag } from 'antd';
import { AiOutlineFolder } from 'react-icons/ai';
import useTreeMove from '@damhookuseTreeMove';
import { useTranslation } from 'react-i18next';
import { getProjects, importAssetsToProject } from '../../actions';
import { getProjectStatusTagName, getProjectStatusTagColor } from '@damhelper';

function ImportToProjectsModal(props) {
    const { t } = useTranslation();

    const {
        checkedAssetsItem,
        isImportToProjectsModalOpen,
        setIsImportToProjectsModalOpen
    } = props;

    const [userProjects, setUserProjects] = useState([]);
    const [checkedProjects, setCheckedProjects] = useState([]);
    const [selectAll, setSelectAll] = useState(false);

    useEffect(() => {
        if (isImportToProjectsModalOpen) {
            loadProjects();
        }
    }, [isImportToProjectsModalOpen]);

    const loadProjects = async () => {
        var result = await props.getProjects();

        if (result) {
            var projects = result.data.projects;
            setUserProjects(projects);
        }
    }

    const handleCloseModal = () => {
        setIsImportToProjectsModalOpen(false);
    }

    function onCheckedProject(project) {
        let tempItems = checkedProjects;
        const isChecked = tempItems.filter((tempItem) => tempItem.id === project.id);
        var added = true;

        if (isChecked.length === 0) {
            setCheckedProjects((ca) => [...ca, project]);
            added = true;
        } else {
            tempItems = tempItems.filter((fId) => fId.id !== project.id);
            setCheckedProjects(tempItems);
            added = false
        }

        if (tempItems.length === userProjects.length || (added && tempItems.length === 0 && userProjects.length === 1)) {
            setSelectAll(true);
        } else {
            setSelectAll(false);
        }
    }

    function handleSelectAll() {
        if (selectAll) {
            setCheckedProjects([]);
        } else {
            let tempItems = userProjects;

            if (tempItems && tempItems.length > 0) {
                setCheckedProjects(tempItems);
            }
        }
        setSelectAll(!selectAll);
    }

    const importAssets = async () => {
        let assetIdsToImport = checkedAssetsItem.map((item) => {
            return item.id;
        })

        let projectIds = checkedProjects.map((project) => {
            return project.id;
        })

        let importData = {
            assetIds: assetIdsToImport,
            projectIds: projectIds
        }

        await props.importAssetsToProject(importData);

        handleCloseModal();
    }

    return (
        <Modal
            className="alert-modals"
            title={t('Select Projects to Import To...')}
            visible={isImportToProjectsModalOpen}
            onCancel={handleCloseModal}
            closable={false}
            onOk={importAssets}
        >
            <Row className="margin-bottom-8" justify="end">
                <Checkbox checked={selectAll} onClick={handleSelectAll}>
                    Select All
                </Checkbox>
            </Row>
            <Row className="import-to-project-ls">
                <List
                    size="small"
                    style={{ width: '100%' }}
                    bordered
                    dataSource={userProjects}
                    renderItem={(item) => (
                        <List.Item id={item.id + '-li'} className="project-list-item">
                            <Col span={2}>
                                <Checkbox.Group
                                    onChange={(value) => {
                                        onCheckedProject(item);
                                    }}
                                    value={checkedProjects.filter((ca) => ca.id === item.id).length > 0 ? [item.id] : []}
                                >
                                    <Checkbox value={item.id}></Checkbox>
                                </Checkbox.Group>
                            </Col>
                            <Col span={16}>
                                <span id={item.id + '-span'} style={{ marginRight: 12 }}>
                                    {item.projectName}
                                </span>
                            </Col>
                            <Col span={1}>
                            </Col>
                            <Col span={5} align="right" style={{ width: '100%', textAlign: 'center' }}>
                                <Tag className="project-status-tag" color={getProjectStatusTagColor(item.projectStatus)}>
                                    {getProjectStatusTagName(item.projectStatus)}
                                </Tag>
                            </Col>
                        </List.Item>
                    )}
                />
            </Row>
        </Modal>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        getProjects: () => dispatch(getProjects()),
        importAssetsToProject: (data) => dispatch(importAssetsToProject(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ImportToProjectsModal));
