import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Input, Divider } from 'antd';
import {
    FullscreenOutlined,
    ExclamationCircleOutlined,
    DownOutlined,
    DeleteOutlined,
    EditOutlined
} from '@ant-design/icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
    faPen,
    faShareAlt,
    faDownload,
    faArrowRight,
    faFile,
    faFileArchive,
    faCheckCircle,
    faFolder,
    faPlusCircle,
    faMinus
} from '@fortawesome/free-solid-svg-icons';
import { getUserRole } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { useTranslation } from 'react-i18next';
import { FaAngleDown } from 'react-icons/fa';
import moment from 'moment';
import { PROJECT_STATUS } from '../constants';

function ProjectTeams(props) {
    const { t } = useTranslation();
    const { confirm } = Modal;
    const {
        projectTeams
    } = props;
    const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
    const userRole = getUserRole();

    const [isDeleteProjectModalVisible, setIsDeleteProjectModalVisible] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [selectedProjectName, setSelectedProjectName] = useState('');

    useEffect(() => {
        setIsEditing(false);
    }, [projectTeams]);


    const showConfirm = (e, basketId) => {
        confirm({
            title: t('Confirm Delete'),
            content: t('Are you sure you want to delete this basket?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                //deleteBasket(basketId);
            },
            onCancel() { }
        });
    };

    return (
        <>
            {projectTeams && (
                <Row>
                    {!isEditing ? (
                        <>
                            <Row className="project-list-row">
                                
                            </Row>
                         
                        </>
                    ) : (
                        <>
                          
                        </>
                    )}
                </Row>
            )}
        </>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectTeams));
