import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Input, Divider, Tooltip, Avatar, Comment, List, Mentions } from 'antd';
import {
    FullscreenOutlined,
    ExclamationCircleOutlined,
    DownOutlined,
    DeleteOutlined,
    EditOutlined
} from '@ant-design/icons';
import { getUserRole } from '@damtoken';
import { useTranslation } from 'react-i18next';
import moment from 'moment';
import { getUserFromUsers, getAvatarAlt } from '@damhelper';
import { saveProjectComment, deleteProjectComment } from '../dam/actions';

function ProjectComments(props) {
    const { t } = useTranslation();
    const { confirm } = Modal;
    const { Option, getMentions } = Mentions;
    const {
        selectedProjectId,
        projectComments,
        users,
        loadProjectComments,
        isArchiveMode,
        user
    } = props;
    const userRole = getUserRole();
    const [projectCommentsWithAction, setProjectCommentsWithAction] = useState([]);

    const [newCommentValue, setNewCommentValue] = useState('');
    const [editCommentValue, setEditCommentValue] = useState('');
    const [selectedMentions, setSelectedMentions] = useState([]);

    useEffect(() => {
        var projectCommentsWithFlag = projectComments.map(comment => {
            var newComment = {
                id: comment.id,
                comment: comment.comment,
                createdById: comment.createdById,
                createdDate: comment.createdDate,
                projectId: comment.projectId,
                isEditing: false
            }

            return newComment;
        })

        setProjectCommentsWithAction(projectCommentsWithFlag);
    }, [projectComments]);

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
            url = user.imageUrl ? user.imageUrl : '';
        }
        return url;
    }

    const getUserOptions = () => {

        var options = users.map(user => {
            return (<Option value={user.userName}>{user.userName}</Option>)
        })

        return options;
    }

    const handleCreateComment = async () => {
        var saveCommentData = {
            id: null,
            projectId: selectedProjectId,
            comment: newCommentValue,
            mentions: selectedMentions
        }

        saveComment(saveCommentData);
    }

    const saveComment = async (comment) => {
        await props.saveProjectComment(comment);
        setNewCommentValue('');
        setEditCommentValue('');
        setSelectedMentions([]);
        loadProjectComments();
    }

    const deleteComment = async (commentId) => {
        var commentToDelete = {
            projectCommentId: commentId
        }

        await props.deleteProjectComment(commentToDelete);
        loadProjectComments();
    }

    const handleEditCommentClick = (e, commentToEdit) => {
        var projectCommentsWithFlag = [...projectCommentsWithAction];
        projectCommentsWithFlag = projectCommentsWithAction.map(comment => {

            if (comment.id === commentToEdit.id) {
                comment.isEditing = true;
                setEditCommentValue(comment.comment);
            } else {
                comment.isEditing = false;
            }

            return comment;
        })

        setProjectCommentsWithAction(projectCommentsWithFlag);
    }

    const handleCancelEditCommentClick = (e, commentToEdit) => {
        var projectCommentsWithFlag = [...projectCommentsWithAction];
        projectCommentsWithFlag = projectCommentsWithAction.map(comment => {
            comment.isEditing = false;
            return comment;
        })

        setProjectCommentsWithAction(projectCommentsWithFlag);
    }

    const showSaveCommentConfirm = (commentToEdit) => {
        confirm({
            title: t('Confirm Edit'),
            content: t('Are you sure you want to edit this comment?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                handleSaveCommentClick(commentToEdit);
            },
            onCancel() { setSelectedMentions([]); }
        });
    };

    const handleSaveCommentClick = (commentToEdit) => {
        commentToEdit.comment = editCommentValue;
        commentToEdit.mentions = selectedMentions;

        saveComment(commentToEdit);
    }

    const showDeleteCommentConfirm = (id) => {
        confirm({
            title: t('Confirm Delete'),
            content: t('Are you sure you want to delete this comment?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                deleteComment(id);
            },
            onCancel() { }
        });
    };

    const handleMentionSelect = (option) => {
        var mentions = [...selectedMentions];
        mentions.push(option.value);
        setSelectedMentions(mentions);
    }

    return (
        <>
            {projectComments && (
                <div className={isArchiveMode ? 'project-comment-list-long' : 'project-comment-list'}>
                    {projectComments.length > 0 ? (
                        <Row className="scrollable-list">
                            <List
                                locale={{ emptyText: <></> }}
                                itemLayout="horizontal"
                                dataSource={projectCommentsWithAction}
                                size="small"
                                renderItem={comment => (
                                    <li id={'comment#' + comment.id}>
                                        <Comment
                                            actions={user.id === comment.createdById && !isArchiveMode ?
                                                (comment.isEditing ?
                                                    [
                                                        <span onClick={(e) => handleCancelEditCommentClick(e, comment)}>Cancel</span>,
                                                        <span onClick={(e) => showSaveCommentConfirm(comment)}>Save</span>
                                                    ]
                                                    :
                                                    [
                                                        <Tooltip title="Edit Comment">
                                                            <EditOutlined onClick={(e) => handleEditCommentClick(e, comment)} />
                                                        </Tooltip>
                                                        ,
                                                        <Tooltip title="Delete Comment" >
                                                            <DeleteOutlined onClick={(e) => showDeleteCommentConfirm(comment.id)} />
                                                        </Tooltip>
                                                    ]
                                                )
                                                : []
                                            }
                                            author={getUserName(comment.createdById)}
                                            avatar={
                                                <Tooltip
                                                    title={getUserName(comment.createdById)}
                                                    placement="top"
                                                >
                                                    <Avatar src={getUserProfilePic(comment.createdById)}>
                                                        {getAvatarAlt(getUserName(comment.createdById))}
                                                    </Avatar>
                                                </Tooltip>
                                            }
                                            content={comment.isEditing ?
                                                <Mentions value={editCommentValue} onSelect={(e) => handleMentionSelect(e)} onChange={(value) => setEditCommentValue(value)} autoSize={{ minRows: 2, maxRows: 2 }}>
                                                    {getUserOptions()}
                                                </Mentions>
                                                : comment.comment
                                            }
                                            datetime={<span>{moment(comment.createdDate).format('ddd MMM DD YYYY hh:mm:ss A')}</span>}
                                        />
                                    </li>
                                )}
                            />
                        </Row>)
                        : ''
                    }

                    {!isArchiveMode && (
                        <>
                            <Row className="margin-top-8">
                                <Col span={24}>
                                    <Mentions value={newCommentValue} placeholder="Write a comment. Use @ to mention someone." onSelect={(e) => handleMentionSelect(e)} onChange={(value) => setNewCommentValue(value)} autoSize={{ minRows: 2 }}>
                                        {getUserOptions()}
                                    </Mentions>
                                </Col>
                            </Row>
                            <Row className="margin-top-8">
                                <Col span={24} align="right">
                                    <Space>
                                        <Button type="secondary" size="small" onClick={(e) => { setNewCommentValue(''); setSelectedMentions([]); }}> Clear </Button>
                                        <Button type="primary" size="small" onClick={handleCreateComment} disabled={newCommentValue.trim().length === 0}> Add </Button>
                                    </Space>
                                </Col>
                            </Row>
                        </>
                    )}
                </div>
            )}
        </>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        saveProjectComment: (data) => dispatch(saveProjectComment(data)),
        deleteProjectComment: (data) => dispatch(deleteProjectComment(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectComments));
