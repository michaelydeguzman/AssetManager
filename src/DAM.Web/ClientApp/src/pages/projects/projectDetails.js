import React, { useState, memo, useEffect, useContext } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Space, Modal, Checkbox, Tag, Dropdown, Menu, Input, Divider, DatePicker, Select, Avatar, Tooltip, message, List } from 'antd';
import {
    FullscreenOutlined,
    ExclamationCircleOutlined,
    DownOutlined,
    DeleteOutlined,
    EditOutlined,
    ContainerOutlined,
    FileSyncOutlined
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
import { getUserRole, getUser } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { getUserFromUsers, getProjectStatusTagName, getProjectStatusTagColor, getAvatarAlt } from '@damhelper';
import { useTranslation } from 'react-i18next';
import { FaAngleDown } from 'react-icons/fa';
import moment from 'moment';
import { PROJECT_STATUS } from '../constants';
import { getProjectOwners, getProjectFollowers, getProjectComments, saveProject, archiveProject, deleteProject, unarchiveProject, getTeans } from '../dam/actions';
import ProjectUsers from './projectUsers';
import ProjectTeams from './projectTeams';
import ProjectComments from './projectComments';

function ProjectDetails(props) {
    let user = getUser();

    const { t } = useTranslation();
    const { Option } = Select;
    const { confirm } = Modal;
    const {
        createMode,
        setCreateMode,
        toggleCreateMode,
        selectedProject,
        setSelectedProject,
        users,
        teams,
        editMode,
        setEditMode,
        loadProjectsAndUsers,
        isArchiveMode
    } = props;
    const { approvalFlag, isOrdering, setIsOrdering } = useContext(LowFrequencyContext);
    const userRole = getUserRole();

    const [isDeleteProjectModalVisible, setIsDeleteProjectModalVisible] = useState(false);
    const [canEdit, setCanEdit] = useState(false);
    const [selectedProjectName, setSelectedProjectName] = useState('');
    const [selectedProjectOverview, setSelectedProjectOverview] = useState('');
    const [selectedProjectOwners, setSelectedProjectOwners] = useState([]);
    const [selectedProjectUserFollowers, setSelectedProjectUserFollowers] = useState([]);
    const [selectedProjectTeams, setSelectedProjectTeams] = useState([]);
    const [selectedProjectComments, setSelectedProjectComments] = useState([]);
    const [selectedProjectStatus, setSelectedProjectStatus] = useState(0);
    const [selectedProjectDueDate, setSelectedProjectDueDate] = useState(null);
    const [selectedProjectFollowerUserIds, setSelectedProjectFollowerUserIds] = useState([]);
    const [selectedProjectOwnerUserIds, setSelectedProjectOwnerUserIds] = useState([]);
    const [selectedProjectTeamIds, setSelectedProjectTeamIds] = useState([]);

    useEffect(() => {
        setEditMode(false);

        if (selectedProject) {
            if (selectedProject.createdById === user.id) { // if creator, user can edit
                setCanEdit(true);
            } else {
                setCanEdit(false);
            }

            if (userRole.id === 1) { // if admin, user can edit
                setCanEdit(true);
            }

            setSelectedProjectName(selectedProject.projectName);
            setSelectedProjectOverview(selectedProject.projectOverview);
            setSelectedProjectStatus(selectedProject.projectStatus);
            setSelectedProjectDueDate(selectedProject.projectDueDate);

            loadProjectOwners(selectedProject.Id);
            loadProjectFollowers(selectedProject.id);
            loadProjectComments(selectedProject.id);
        } else {
            clearSelectedState();
        }

    }, [selectedProject]);

    useEffect(() => {
        var selectedUsers = [];

        selectedProjectUserFollowers.forEach(user => {
            selectedUsers.push(user.userId);
        })

        setSelectedProjectFollowerUserIds(selectedUsers);

    }, [selectedProjectUserFollowers]);

    useEffect(() => {
        var selectedUsers = [];

        selectedProjectOwners.forEach(user => {
            selectedUsers.push(user.userId);
        })

        setSelectedProjectOwnerUserIds(selectedUsers);

    }, [selectedProjectOwners]);

    useEffect(() => {
        var selectedTeams = [];

        selectedProjectTeams.forEach(team => {
            selectedTeams.push(team.teamId);
        })

        setSelectedProjectTeamIds(selectedTeams);

    }, [selectedProjectTeams]);

    useEffect(() => {
        if (createMode) {
            var defaultOwner = [];

            defaultOwner.push({
                userId: user.id
            });

            setSelectedProjectOwners(defaultOwner);
        }
    }, [createMode])

    const clearSelectedState = () => {
        setSelectedProjectName('');
        setSelectedProjectOverview('');
        setSelectedProjectComments([]);
        setSelectedProjectOwners([]);
        setSelectedProjectUserFollowers([]);
        setSelectedProjectTeams([]);
        setSelectedProjectStatus(0);
        setSelectedProjectDueDate(null);
        setSelectedProjectFollowerUserIds([]);
        setSelectedProjectOwnerUserIds([]);
    }

    async function loadProjectFollowers() {
        var result = await props.getProjectFollowers(selectedProject.id);

        if (result) {
            setSelectedProjectUserFollowers(result.data.followers.users);
            setSelectedProjectTeams(result.data.followers.teams);
        }
    }

    async function loadProjectOwners() {
        var result = await props.getProjectOwners(selectedProject.id);

        if (result) {
            setSelectedProjectOwners(result.data.owners);

            if (result.data.owners.filter(x => x.userId === user.id).length > 0) { // If owner, user can edit
                setCanEdit(true);
            }
        }
    }

    async function loadProjectComments() {
        var result = await props.getProjectComments(selectedProject.id);

        if (result) {
            setSelectedProjectComments(result.data.comments);
        }
    }

    const handleEditClick = () => {
        setEditMode(true);
    };

    const getUserName = (userId) => {
        var userName = '';

        let user = getUserFromUsers(userId, users);
        if (user) {
            userName = user.userName;
        }

        return userName;
    }

    const projectStatusMenu = () => {
        return (<Menu>
            <Menu.Item>
                <Tag className="project-status-tag" color={getProjectStatusTagColor(0)} onClick={() => setSelectedProjectStatus(0)}>
                    {PROJECT_STATUS[0]}
                </Tag>
            </Menu.Item>
            <Menu.Item>
                <Tag className="project-status-tag" color={getProjectStatusTagColor(1)} onClick={() => setSelectedProjectStatus(1)}>
                    {PROJECT_STATUS[1]}
                </Tag>
            </Menu.Item>
            <Menu.Item>
                <Tag className="project-status-tag" color={getProjectStatusTagColor(2)} onClick={() => setSelectedProjectStatus(2)}>
                    {PROJECT_STATUS[2]}
                </Tag>
            </Menu.Item>
            <Menu.Item>
                <Tag className="project-status-tag" color={getProjectStatusTagColor(3)} onClick={() => setSelectedProjectStatus(3)}>
                    {PROJECT_STATUS[3]}
                </Tag>
            </Menu.Item>
        </Menu>
        )
    }

    const renderUserOptions = (includeUser = false) => {
        var userOptions = includeUser ? users : users.filter(u => u.id !== user.id);

        var result = userOptions.map(user => {
            return (
                <Option
                    value={user.id} key={user.id}
                >
                    <Space>
                        <Avatar size="small" src={getUserProfilePic(user.id)}>{getAvatarAlt(getUserName(user.id))}</Avatar>
                        {user.userName}
                    </Space>
                </Option>
            );
        })

        return (
            <React.Fragment>
                {result}
            </React.Fragment>
        )
    }

    const renderTeamOptions = () => {
        var teamOptions = teams;

        var result = teamOptions.map(team => {
            return (
                <Option
                    value={team.id} key={team.id}
                >
                    <Space>
                        {team.teamName}
                    </Space>
                </Option>
            );
        })

        return (
            <React.Fragment>
                {result}
            </React.Fragment>
        )
    }

    const getUserProfilePic = (userId) => {
        var url = '';

        let user = getUserFromUsers(userId, users);
        if (user) {
            url = user.imageUrl;
        }

        return url;
    }

    function handleDueDateChange(dueDate) {
        setSelectedProjectDueDate(dueDate);
    }

    function disabledDate(date) {
        if (selectedProject) {
            return date && date <= moment(selectedProject.createdDate);
        } else {
            return date && date <= moment();
        }
    }

    const handleSelectOwner = (userId) => {
        let projectSelectedUsers = [...selectedProjectOwners];

        if (projectSelectedUsers.filter(user => user.userId === userId).length === 0) {
            let newRecord = {
                projectId: selectedProject ? selectedProject.id : 0,
                userId: userId
            }
            projectSelectedUsers.push(newRecord);
        }

        if (selectedProjectUserFollowers.filter(user => user.userId === userId).length > 0) {
            message.error("User is already selected as a follower");
        } else {
            setSelectedProjectOwners(projectSelectedUsers);
        }
    }

    const handleDeselectOwner = (userId) => {
        let projectSelectedUsers = [...selectedProjectOwners];
        setSelectedProjectOwners(projectSelectedUsers.filter(user => user.userId != userId));
    }

    const handleSelectTeam = (teamId) => {
        let projectSelectedTeams = [...selectedProjectTeams];

        if (projectSelectedTeams.filter(team => team.teamId === teamId).length === 0) {
            let newRecord = {
                projectId: selectedProject ? selectedProject.id : 0,
                teamId: teamId
            }
            projectSelectedTeams.push(newRecord);
            setSelectedProjectTeams(projectSelectedTeams);
        }
    }

    const handleDeselectTeam = (teamId) => {
        let projectSelectedTeams = [...selectedProjectTeams];
        setSelectedProjectTeams(projectSelectedTeams.filter(team => team.teamId != teamId));

    }


    const handleSelectFollower = (userId) => {
        let projectSelectedUsers = [...selectedProjectUserFollowers];

        if (projectSelectedUsers.filter(user => user.userId === userId).length === 0) {
            let newRecord = {
                projectId: selectedProject ? selectedProject.id : 0,
                userId: userId
            }
            projectSelectedUsers.push(newRecord);
        }

        if (selectedProjectOwners.filter(user => user.userId === userId).length > 0) {
            message.error("User is already selected as an owner");
        } else {
            setSelectedProjectUserFollowers(projectSelectedUsers);
        }

    }

    const handleDeselectFollower = (userId) => {
        let projectSelectedUsers = [...selectedProjectUserFollowers];
        setSelectedProjectUserFollowers(projectSelectedUsers.filter(user => user.userId != userId));
    }

    const saveProjectDetails = async () => {

        if (selectedProjectName.length === 0) {
            message.error("Please enter a title for your project");
            return;
        }

        if (selectedProjectOwners.length === 0) {
            message.error("Please select project owners");
            return;
        }

        var updateProjectDetails = {
            id: selectedProject ? selectedProject.id : null,
            projectName: selectedProjectName,
            projectOverview: selectedProjectOverview,
            projectDueDate: selectedProjectDueDate ? moment(selectedProjectDueDate).toISOString() : null,
            projectStatus: selectedProjectStatus,
            projectUserFollowers: selectedProjectUserFollowers,
            projectOwners: selectedProjectOwners,
            projectTeamFollowers: selectedProjectTeams
        }

        await props.saveProject(updateProjectDetails);

        loadProjectsAndUsers();

        if (selectedProject) {
            setEditMode(false);

        } else {
            setCreateMode(false);
            clearSelectedState();
        }
    }

    const deleteProject = async (projectId) => {
        var deleteProject = {
            projectId: projectId
        }

        await props.deleteProject(deleteProject);
        loadProjectsAndUsers();
        setSelectedProject(null);
    }

    const archiveProject = async (projectId) => {
        var archiveProject = {
            projectId: projectId
        }

        await props.archiveProject(archiveProject);
        loadProjectsAndUsers();
        setSelectedProject(null);
    }

    const unArchiveProject = async (projectId) => {
        var unarchiveProject = {
            projectId: projectId
        }

        await props.unarchiveProject(unarchiveProject);
        loadProjectsAndUsers();
        setSelectedProject(null);
    }

    const showDeleteConfirm = (e, projectId) => {
        confirm({
            title: t('Confirm Delete'),
            content: t('Are you sure you want to delete this project?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                deleteProject(projectId);
            },
            onCancel() { }
        });
    };

    const showArchiveConfirm = (e, projectId) => {
        confirm({
            title: t('Confirm Archive'),
            content: t('Are you sure you want to archive this project?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                archiveProject(projectId);
            },
            onCancel() { }
        });
    };


    const showUnarchiveConfirm = (e, projectId) => {
        confirm({
            title: t('Confirm Restore'),
            content: t('Are you sure you want to restore this project from archive?'),
            icon: <ExclamationCircleOutlined />,
            onOk() {
                unArchiveProject(projectId);
            },
            onCancel() { }
        });
    };


    return (
        <>
            <Row className="project-list-row" gutter={[16, 16]}>
                {editMode || createMode ?
                    <>
                        <Col xs={23} lg={8}>
                            <Input value={selectedProjectName} placeholder="Type Project Name here" onChange={(e) => setSelectedProjectName(e.target.value)} style={{ fontSize: 18 }} />
                        </Col>
                        <Col lg={1}>
                        </Col>
                    </>
                    :
                    <>
                        <Col xs={18} lg={6}>
                            <h2>{selectedProject.projectName}</h2>
                        </Col>
                        <Col xs={6} lg={3} align="center">
                            {isArchiveMode ?
                                <Tooltip title="Unarchive">
                                    <Space className="project-action-btn-span">
                                        <FileSyncOutlined onClick={(e) => showUnarchiveConfirm(e, selectedProject.id)} />
                                    </Space>
                                </Tooltip>
                                :
                                <>
                                    {canEdit && !createMode ?
                                        <Space className="project-action-btn-span">
                                            <Tooltip title="Edit">
                                                <EditOutlined onClick={handleEditClick} />
                                            </Tooltip>
                                            <Tooltip title="Delete">
                                                <DeleteOutlined onClick={(e) => showDeleteConfirm(e, selectedProject.id)} />
                                            </Tooltip>
                                            <Tooltip title="Archive">
                                                <ContainerOutlined onClick={(e) => showArchiveConfirm(e, selectedProject.id)} />
                                            </Tooltip>
                                        </Space>
                                        : ''
                                    }
                                </>
                            }
                        </Col>
                    </>
                }
                <Col xs={6} lg={4}>
                    <Row>
                        <h4>Created By</h4>
                    </Row>
                    {createMode ?
                        <Row>
                            {getUserName(user.id)}
                        </Row>
                        :
                        <Row>
                            {getUserName(selectedProject.createdById)}
                        </Row>
                    }
                </Col>
                <Col xs={6} lg={4}>
                    <Row>
                        <h4>Created On</h4>
                    </Row>
                    {createMode ?
                        <Row>
                            -
                        </Row>
                        :
                        <Row>
                            {moment(selectedProject.createdDate).format('ddd MMM DD YYYY')}
                        </Row>
                    }
                </Col>
                <Col xs={6} lg={4}>
                    <Row>
                        <h4>Due Date</h4>
                    </Row>
                    {editMode || createMode ?
                        <Row>
                            <DatePicker style={{ width: 120, marginTop: -5 }}
                                value={selectedProjectDueDate ? moment(selectedProjectDueDate) : null}
                                onChange={handleDueDateChange}
                                disabledDate={disabledDate}
                            />
                        </Row>
                        :
                        <Row>
                            {selectedProject.projectDueDate ? moment(selectedProject.projectDueDate).format('ddd MMM DD YYYY') : '-'}
                        </Row>
                    }
                </Col>

                <Col xs={6} lg={3}>
                    <Row>
                        <h4>Status</h4>
                    </Row>
                    {editMode || createMode ?
                        <Row>
                            <Dropdown overlay={projectStatusMenu} placement="bottomCenter">

                                <Tag className="project-status-tag" color={getProjectStatusTagColor(selectedProjectStatus)}>
                                    <Space>
                                        {getProjectStatusTagName(selectedProjectStatus)}
                                        <FaAngleDown className="icons" onClick={(e) => e.preventDefault()} style={{ marginTop: 6 }} />
                                    </Space>
                                </Tag>


                            </Dropdown>
                        </Row>
                        :
                        <Row>
                            <Tag className="project-status-tag" color={getProjectStatusTagColor(selectedProject.projectStatus)}>
                                {getProjectStatusTagName(selectedProject.projectStatus)}
                            </Tag>
                        </Row>
                    }
                </Col>
            </Row>
            <Row className="project-list-row bottom" gutter={[16, 16]}>
                <Col lg={8}>
                    <Row>
                        <Col>
                            <h4>Project ID</h4>
                        </Col>
                    </Row>
                    {createMode ?
                        <Row>
                            -
                        </Row>
                        :
                        <Row>
                            {selectedProject.id}
                        </Row>
                    }

                    <Row className="project-list-row bottom">
                        <h4>Project Overview</h4>
                    </Row>

                    <Row>
                        {editMode || createMode ?
                            <Input.TextArea value={selectedProjectOverview} placeholder="Type Project Overview here" onChange={(e) => setSelectedProjectOverview(e.target.value)} autoSize={{ minRows: 4, maxRows: 10 }} bordered={true} />
                            :
                            <Input.TextArea value={selectedProject.projectOverview} autoSize={{ minRows: 4, maxRows: 8 }} bordered={false} />
                        }
                    </Row>

                </Col>
                <Col lg={1}>
                </Col>
                <Col lg={6}>
                    <Row>
                        <h4>Owners</h4>
                    </Row>
                    {(editMode || createMode) ?
                        <Row>
                            <Select mode="multiple" placeholder="Select owners" style={{ width: '90%' }}
                                value={selectedProjectOwnerUserIds}
                                onSelect={handleSelectOwner}
                                onDeselect={handleDeselectOwner}
                            >
                                {renderUserOptions(true)}
                            </Select>
                        </Row>
                        :
                        <Row>
                            <ProjectUsers
                                users={users}
                                projectUsers={selectedProjectOwners}
                            />
                        </Row>
                    }
                    <Row className="project-list-row bottom">
                        <h4>Collaborators</h4>
                    </Row>
                    {(editMode || createMode) ?
                        <Row>
                            <Select mode="multiple" placeholder="Select contributors" style={{ width: '90%' }}
                                value={selectedProjectFollowerUserIds}
                                onSelect={handleSelectFollower}
                                onDeselect={handleDeselectFollower}
                            >
                                {renderUserOptions()}
                            </Select>
                        </Row>
                        :
                        <Row>
                            <ProjectUsers
                                users={users}
                                projectUsers={selectedProjectUserFollowers}
                            />
                        </Row>
                    }


                    {(!editMode && !createMode) && (
                        <>
                            <Row className="project-list-row bottom">
                                <h4>Teams</h4>
                            </Row>
                            {teams.length > 0 && (
                                <Row>
                                    {selectedProjectTeams.map((item) => (
                                        <Col key={item.id} className="cognitive-tags-list">
                                            <Tag
                                                style={{ borderRadius: 40, marginTop: 5 }}
                                                visible
                                                className="cognitive-tag"
                                            >
                                                {teams.filter(team => team.id == item.teamId)[0].teamName}
                                            </Tag>
                                        </Col>
                                    ))}
                                </Row>)
                            }
                        </>)
                    }


                </Col>

                {(!editMode && !createMode) ?
                    <Col xs={24} lg={9}>
                        <Row>
                            <h4>Comments</h4>
                        </Row>
                        <Row>
                            <ProjectComments
                                selectedProjectId={selectedProject.id}
                                users={users}
                                projectComments={selectedProjectComments}
                                loadProjectComments={loadProjectComments}
                                isArchiveMode={isArchiveMode}
                                user={user}
                            />
                        </Row>
                    </Col>
                    :
                    <Col xs={24} lg={9}>
                        <Row>
                            <h4>Teams</h4>
                        </Row>
                        <Select mode="multiple" placeholder="Select teams" style={{ width: '90%' }}
                            value={selectedProjectTeamIds}
                            onSelect={handleSelectTeam}
                            onDeselect={handleDeselectTeam}

                        >
                            {renderTeamOptions()}
                        </Select>
                    </Col>
                }
            </Row>

            {createMode && (
                <Row className="project-list-row" justify="end">
                    <Space>
                        <Button type="secondary" onClick={toggleCreateMode}>Cancel </Button>
                        <Button type="primary" onClick={saveProjectDetails}>Save </Button>
                    </Space>
                </Row>
            )}


            {editMode && (
                <Row className="project-list-row" justify="end">
                    <Space>
                        <Button type="secondary" onClick={(e) => { setEditMode(false); loadProjectFollowers(); loadProjectOwners(); }}>Cancel </Button>
                        <Button type="primary" onClick={saveProjectDetails}>Save </Button>
                    </Space>
                </Row>
            )}
        </>
    );
}


function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {
        getProjectOwners: (id) => dispatch(getProjectOwners(id)),
        getProjectFollowers: (id) => dispatch(getProjectFollowers(id)),
        getProjectComments: (id) => dispatch(getProjectComments(id)),
        saveProject: (data) => dispatch(saveProject(data)),
        archiveProject: (data) => dispatch(archiveProject(data)),
        unarchiveProject: (data) => dispatch(unarchiveProject(data)),
        deleteProject: (data) => dispatch(deleteProject(data))
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ProjectDetails));
