import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Input, Divider, Tooltip, Avatar, Comment } from 'antd';
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
import { getUserFromUsers, getAvatarAlt } from '@damhelper';

function ProjectUsers(props) {
    const { t } = useTranslation();
    const { confirm } = Modal;
    const {
        projectUsers,
        users
    } = props;
    const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
    const userRole = getUserRole();

    const [isDeleteProjectModalVisible, setIsDeleteProjectModalVisible] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [selectedProjectName, setSelectedProjectName] = useState('');

    useEffect(() => {
        setIsEditing(false);
    }, [projectUsers]);

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

    const getUserName = (userId) => {
        var userName = '';

        let user = getUserFromUsers(userId, users);
        if (user) {
            userName = user.userName;
        }

        return userName;
    }

    const getUserProfilePic = (userId) => {
        var url = '';

        let user = getUserFromUsers(userId, users);
        if (user) {
            url = user.imageUrl;
        }

        return url;
    }

    function renderUserFollowers() {
        if (projectUsers) {
            var result = projectUsers.map(user => {
                return (
                    <Tooltip
                        title={getUserName(user.userId)}
                        placement="top"
                    >
                        <Avatar src={getUserProfilePic(user.userId)} style={{marginRight: 10}}>{getAvatarAlt(getUserName(user.userId))}</Avatar>
                    </Tooltip>
                );
            })

            return (
                <React.Fragment>
                    <Avatar.Group maxCount={4}>
                        {result}
                   </Avatar.Group>
                </React.Fragment>
            )
        }
    }

    return (
        <>
            {projectUsers && (
                <>
                    {renderUserFollowers()}
                </>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectUsers));
