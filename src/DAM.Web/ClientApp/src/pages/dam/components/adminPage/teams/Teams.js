import React, { useState, useEffect, memo, useContext } from 'react';
import { connect } from 'react-redux';
import { Card, Input, Table, Avatar, Button, Spin, Row, Col, Space, Checkbox, Radio, List, Divider, Modal, Form, Switch } from 'antd';
import { CopyOutlined, DeleteOutlined, UserAddOutlined, UserOutlined } from '@ant-design/icons';
import { getTeams, getUsers, saveTeam, deleteTeam } from '../../../actions';
import _ from 'lodash';
import { getUser, getUserRole } from '@damtoken';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, fas, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import moment from 'moment';

import { getUserFromUsers, getAvatarAlt } from '@damhelper';

function Teams(props) {
    const { t } = useTranslation();
    const [isTeamEditing, setIsTeamEditing] = useState(false);
    const [teamsList, setTeamsList] = useState([]);
    const [checkedTeams, setCheckedTeams] = useState([]);

    const [teamSearch, setTeamSearch] = useState('');
    const [selectedTeamMembers, setSelectedTeamMembers] = useState([]);

    const [filteredTeamList, setFilteredTeamList] = useState([]);
    const [search, setSearch] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [users, setUsers] = useState([]);
    const [checkedTeamMembers, setCheckedTeamMembers] = useState([]);

    const [isUsersModalOpen, setIsUsersModalOpen] = useState(false);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [checkedUsersForModal, setCheckedUsersForModal] = useState([]);
    const [userModalSearch, setUserModalSearch] = useState('');

    const [selectAll, setSelectAll] = useState(false);
    const [selectAllTeams, setSelectAllTeams] = useState(false);
    const [form] = Form.useForm();

    useEffect(() => {
        setupData();
    }, []);

    useEffect(() => {
        setSelectAllTeams(false);
        setCheckedTeams([]);

        if (teamSearch.trim() == '') {
            setFilteredTeamList(teamsList);
        } else {
            var filteredList = filterTeamResultsBySearchText(teamsList, teamSearch.trim());
            setFilteredTeamList(filteredList);
        }
    }, [teamSearch]);

    useEffect(() => {
        if (userModalSearch.trim() == '') {
            setFilteredUsers(users);
        } else {
            var filteredList = filterUserResultsBySearchText(users, userModalSearch.trim());
            setFilteredUsers(filteredList);
        }
    }, [userModalSearch]);

    function filterUserResultsBySearchText(userList, keyword) {
        return userList.filter(
            (user) =>
                user.userName.toLowerCase().includes(keyword.toLowerCase()) ||
                user.email.toLowerCase().includes(keyword.toLowerCase())
        );
    }
    function filterTeamResultsBySearchText(teamList, keyword) {
        return teamList.filter(
            (team) =>
                team.teamName.toLowerCase().includes(keyword.toLowerCase()) ||
                team.teamDescription.toLowerCase().includes(keyword.toLowerCase())
        );
    }

    const setDataSourceAndFilters = (data) => {
        let dataSource = [];

        data.map((team) => {
            dataSource.push({
                id: team.id,
                teamName: team.teamName,
                teamDescription: team.teamDescription,
                approvals: team.approvals,
                project: team.project,
                status: team.status,
                createdDate: team.createdDate,
                createdById: team.createdById,
                teamMembers: team.teamMembers
            });
        });

        setTeamsList(dataSource);
        setFilteredTeamList(dataSource);
    };

    async function setupData() {
        // Get current login user company
        setIsLoading(true);

        var userResult = await props.loadUsers();

        if (userResult) {
            setUsers(userResult.data.users);
        }

        let fetchTeamsResult = await props.getTeams();

        setDataSourceAndFilters(fetchTeamsResult.data.teams);

        setIsLoading(false);
    }

    const getUserName = (userId) => {
        var userName = '';

        let user = getUserFromUsers(userId, users);
        if (user) {
            userName = user.userName;

        }

        return userName;
    }

    const teamListColumns = [
        {
            width: 25,
            title: <Checkbox checked={selectAllTeams} onClick={handleSelectAllTeams}></Checkbox>,
            dataIndex: '',
            key: 'id',
            render: (team) =>
                <Checkbox.Group
                    onChange={(value) => {
                        onCheckedTeam(team);
                    }}
                    value={checkedTeams.filter((ca) => ca.id === team.id).length > 0 ? [team.id] : []}
                >
                    <Checkbox value={team.id}></Checkbox>
                </Checkbox.Group>

        },
        {
            width: 110,
            title: 'Team Name',
            dataIndex: 'teamName',
            key: 'teamName',
            sorter: (a, b) =>
                a.teamName.localeCompare(b.teamName, undefined, {
                    numeric: true,
                    sensitivity: 'base'
                }),
            sortDirections: ['ascend', 'descend']
        },
        {
            width: 120,
            title: 'Team Overview',
            dataIndex: 'teamDescription',
            key: 'teamDescription'
        },
        {
            width: 70,
            title: 'Approvals',
            dataIndex: 'approvals',
            key: 'approvals',
            render: (value) =>
                value ? <span>Yes</span> : <span>No</span>

        },
        {
            width: 70,
            title: 'Project',
            dataIndex: 'project',
            key: 'project',
            render: (value) =>
                value ? <span>Yes</span> : <span>No</span>

        },
        {
            width: 70,
            title: 'Status',
            dataIndex: 'status',
            key: 'status',
            //filters: [
            //    {
            //        text: 'Active',
            //        value: 'Active'
            //    },
            //    {
            //        text: 'Inactive',
            //        value: 'Inactive'
            //    }
            //],
            //onFilter: (value, record) => record.active.indexOf(value) === 0

            render: (value) =>
                value ? <span>Active</span> : <span>Inactive</span>

        },
        {
            width: 80,
            title: 'Created By',
            dataIndex: 'createdById',
            key: 'createdById',
            render: (value) => getUserName(value)
        },
        {
            width: 70,
            title: 'Created Date',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value) => moment(value).format('DD/MM/YYYY')
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

    function onCheckedTeam(item) {
        let tempItems = [...checkedTeams];
        const isChecked = tempItems.filter((tempItem) => tempItem.id === item.id);
        var added = true;

        if (isChecked.length === 0) {
            setCheckedTeams((ca) => [...ca, item]);

            if (tempItems.filter(t => t.id === item.id).length === 0) {
                tempItems.push(item);
            }

            added = true;
        } else {
            tempItems = tempItems.filter((fId) => fId.id !== item.id);
            setCheckedTeams(tempItems);
            added = false;
        }

        if (tempItems.length === filteredTeamList.length || (added && tempItems.length === 0 && filteredTeamList.length === 1)) {
            setSelectAllTeams(true);
        } else {
            setSelectAllTeams(false);
        }
    }


    const handleEditClick = (team) => {
        setIsTeamEditing(true);
        setCheckedTeamMembers([]);
        setSelectAll(false);

        form.setFieldsValue({
            id: team.id,
            teamName: team.teamName,
            teamDescription: team.teamDescription,
            project: team.project,
            approvals: team.approvals,
            status: team.status
        });

        if (team.teamMembers.length > 0) {
            var members = users.filter(user => {
                if (team.teamMembers.filter(t => t.userId === user.id).length > 0) {
                    return user;
                }
            })
            setSelectedTeamMembers(members);
        }
    };

    const toggleIsUsersModalOpen = () => {
        setFilteredUsers(users);
        setUserModalSearch('');
        setIsUsersModalOpen(!isUsersModalOpen);
        setCheckedUsersForModal([]);
    }

    const handleAddClick = () => {
        form.setFieldsValue({
            id: null,
            teamName: '',
            teamDescription: '',
            project: false,
            approvals: false,
            status: 1
        });

        setSelectedTeamMembers([]);
        setSelectAll(false);
        setCheckedTeamMembers([]);
        setIsTeamEditing(true);
    }

    const handleCopyClick = () => {
        if (checkedTeams[0]) {
            var team = checkedTeams[0];
            form.setFieldsValue({
                id: null,
                teamName: team.teamName + ' (copy)',
                teamDescription: team.teamDescription,
                project: team.project,
                approvals: team.approvals,
                status: team.status
            });

            if (team.teamMembers.length > 0) {
                var members = users.filter(user => {
                    if (team.teamMembers.filter(t => t.userId === user.id).length > 0) {
                        return user;
                    }
                })
                setSelectedTeamMembers(members);
            } else {
                setSelectedTeamMembers([]);
            }
        }

        setCheckedTeamMembers([]);
        setSelectAll(false);
        setIsTeamEditing(true);
    }

    const handleCheckUser = (e, user) => {
        var isChecked = e.target.checked;
        var newList = [...checkedUsersForModal];

        if (isChecked) {
            if (checkedUsersForModal.filter(cu => cu.id === user.id).length === 0) {
                newList.push(user);
            }
        } else {
            newList = checkedUsersForModal.filter(cu => cu.id !== user.id);
        }

        setCheckedUsersForModal(newList);
    }

    const handleAddUserFromModal = () => {
        var currentMembers = [...selectedTeamMembers];
        checkedUsersForModal.forEach(user => {
            if (selectedTeamMembers.filter(cu => cu.id === user.id).length === 0) {
                currentMembers.push(user);
            }
        })

        setSelectedTeamMembers(currentMembers);
        toggleIsUsersModalOpen();
        setCheckedTeamMembers([]);
        setSelectAll(false);
    }

    const handleDeleteTeamMember = () => {
        var currentMembers = [...selectedTeamMembers];

        currentMembers = [];
        selectedTeamMembers.forEach(user => {
            if (checkedTeamMembers.filter(cu => cu.id === user.id).length === 0) {
                currentMembers.push(user);
            }
        })

        setSelectedTeamMembers(currentMembers);
        setCheckedTeamMembers([]);
        setSelectAll(false);
    }

    function onCheckedTeamMember(item) {
        let tempItems = [...checkedTeamMembers];
        const isChecked = tempItems.filter((tempItem) => tempItem.id === item.id);
        var added = true;

        if (isChecked.length === 0) {
            setCheckedTeamMembers((ca) => [...ca, item]);

            if (tempItems.filter(t => t.id === item.id).length === 0) {
                tempItems.push(item);
            }

            added = true;
        } else {
            tempItems = tempItems.filter((fId) => fId.id !== item.id);
            setCheckedTeamMembers(tempItems);
            added = false;
        }

        if (tempItems.length === selectedTeamMembers.length || (added && tempItems.length === 0 && selectedTeamMembers.length === 1)) {
            setSelectAll(true);
        } else {
            setSelectAll(false);
        }
    }

    function handleSelectAll() {
        if (selectAll) {
            setCheckedTeamMembers([]);
        } else {
            let tempItems = selectedTeamMembers;

            if (tempItems && tempItems.length > 0) {
                setCheckedTeamMembers(tempItems);
            }
        }
        setSelectAll(!selectAll);
    }

    function handleSelectAllTeams() {
        if (selectAllTeams) {
            setCheckedTeams([]);
        } else {
            let tempItems = filteredTeamList;

            if (tempItems && tempItems.length > 0) {
                setCheckedTeams(tempItems);
            }
        }
        setSelectAllTeams(!selectAllTeams);
    }


    const handleSaveTeam = async (data) => {
        var teamMemberUserIds = selectedTeamMembers.map(member => {
            return member.id
        });

        var team = {
            id: data.id,
            teamName: data.teamName,
            teamDescription: data.teamDescription,
            project: data.project,
            approvals: data.approvals,
            status: data.status,
            teamMemberUserIds: teamMemberUserIds
        }

        await props.saveTeam(team);
        setIsTeamEditing(false);
        setupData();
    }

    const handleDeleteTeam = async () => {
        var teamIds = checkedTeams.map(team => {
            return team.id
        })

        var teamsToDelete = {
            teamIds: teamIds
        }

        await props.deleteTeam(teamsToDelete);
        setupData();
    }

    return (
        <>
            {isTeamEditing ? (
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
                                            setIsTeamEditing(false);
                                            form.resetFields();
                                        }}>
                                            <FontAwesomeIcon icon={faArrowLeft} />
                                        </Button>
                                        {t('Edit Teams')}
                                    </h3>
                                </Space>
                            </Col>
                            <Col span={16} align="right">
                                <Space>
                                    <Button onClick={() => {
                                        setIsTeamEditing(false);
                                        form.resetFields();
                                    }}>Cancel</Button>
                                    <Button type="primary" onClick={form.submit}>Save</Button>
                                </Space>
                            </Col>
                        </Row>
                    </Card>

                    <Row gutter={[12, 12]}>
                        <Col xs={24} lg={12}>
                            <Card
                                title={t('Details')}
                                className="card-container"
                                style={{ marginTop: 15, marginLeft: 15 }}
                            >
                                <>
                                    <Form
                                        form={form}
                                        name="Team"
                                        labelCol={{ span: 4 }}
                                        wrapperCol={{ span: 8 }}
                                        layout="horizontal"
                                        onFinish={handleSaveTeam}
                                        autoComplete="off"
                                        destroyOnClose={true}
                                    >
                                        <Form.Item name="id" style={{ height: '0px' }}></Form.Item>
                                        <Form.Item label="Team Name" name="teamName" rules={[{ required: true, message: 'Please enter a Team Name' }]}>
                                            <Input />
                                        </Form.Item>
                                        <Form.Item label="Team Description" name="teamDescription">
                                            <Input.TextArea autoSize={{ minRows: 2, maxRows: 2 }}/>
                                        </Form.Item>
                                        <Form.Item label="Project" name="project" valuePropName="checked">
                                            <Switch />
                                        </Form.Item>
                                        <Form.Item label="Approvals" name="approvals" valuePropName="checked">
                                            <Switch />
                                        </Form.Item>

                                        <Form.Item label="Status" name="status">
                                            <Radio.Group buttonStyle="solid">
                                                <Radio value={1}>{t('ModalDetail.Active')}</Radio>
                                                <Radio value={0}>{t('ModalDetail.Inactive')}</Radio>
                                            </Radio.Group>
                                        </Form.Item>
                                    </Form>
                                </>
                            </Card>
                        </Col>
                        <Col xs={24} lg={12}>
                            <Card
                                title={
                                    <Row gutter={6}>
                                        <Col span={8}>
                                            {t('Team Members')}
                                        </Col>
                                        <Col span={16} align="right">
                                            <Space>
                                                <Button className="option-button" hidden={checkedTeamMembers.length === 0} onClick={handleDeleteTeamMember}>
                                                    <DeleteOutlined />
                                                    Delete
                                                </Button>
                                                <Button className="option-button" onClick={toggleIsUsersModalOpen}>
                                                    <UserAddOutlined />
                                                    Add User
                                                </Button>
                                            </Space>
                                        </Col>

                                    </Row>
                                }
                                className="card-container"
                                style={{ marginTop: 15, marginLeft: 15, marginRight: 15, minHeight: selectedTeamMembers.length > 0 ? '42vh' : '' }}
                            >
                                {selectedTeamMembers.length > 0 && (
                                    <>
                                        <Row gutter={16}>
                                            <Col span={1}>
                                                <Checkbox checked={selectAll} onClick={handleSelectAll}>
                                                </Checkbox>
                                            </Col>
                                            <Col span={20}>
                                                Select All
                                            </Col>
                                        </Row>
                                        <Row>
                                            <Divider />
                                        </Row>

                                        <List
                                            locale={{ emptyText: <></> }}
                                            dataSource={selectedTeamMembers}
                                            renderItem={(item) => (
                                                <List.Item>
                                                    <Space>
                                                        <Checkbox.Group
                                                            onChange={(value) => {
                                                                onCheckedTeamMember(item);
                                                            }}
                                                            value={checkedTeamMembers.filter((ca) => ca.id === item.id).length > 0 ? [item.id] : []}
                                                        >
                                                            <Checkbox value={item.id}></Checkbox>
                                                        </Checkbox.Group>
                                                        <Avatar src={item.imageUrl} style={{ marginRight: 5 }}>{getAvatarAlt(item.userName)}</Avatar>
                                                        <span>{item.userName}</span>
                                                    </Space>
                                                </List.Item>
                                            )}
                                        >
                                        </List>
                                    </>
                                )}
                            </Card>
                        </Col>
                    </Row>
                </>
            ) : (
                <Spin spinning={isLoading} size="large">
                    <Card
                        title={t('Slider.Teams')}
                        className="card-container"
                        style={{ margin: '15px' }}
                        extra={
                            <Row>
                                <Space>
                                    <Button className="option-button" onClick={handleAddClick}>
                                        <UserAddOutlined />
                                        Add
                                    </Button>
                                    {checkedTeams.length === 1 && (
                                        <Button className="option-button" onClick={handleCopyClick}>
                                            <CopyOutlined />
                                            Copy
                                        </Button>
                                    )}
                                    {checkedTeams.length > 0 && (
                                        <Button className="option-button" onClick={handleDeleteTeam}>
                                            <DeleteOutlined />
                                            Delete
                                        </Button>
                                    )}
                                </Space>
                            </Row>
                        }>

                        <Input.Search
                            name="search"
                            id="search"
                            placeholder={t('ModalDetail.Search')}
                            value={teamSearch}
                            onChange={(e) => setTeamSearch(e.target.value)}
                            style={{ width: 300, marginBottom: 20 }}
                        />

                        <Table
                            columns={teamListColumns}
                            dataSource={filteredTeamList}
                            scroll={{ x: 400 }}
                        />
                    </Card>
                </Spin>
            )}

            <Modal
                title={"Add Users"}
                visible={isUsersModalOpen}
                onCancel={toggleIsUsersModalOpen}
                centered
                width={400}
                footer={false}
                getContainer={false}
                closable={false}
                keyboard={true}
                destroyOnClose={true}
            >

                <Row gutter={[16, 16]}>
                    <Col span={24} align="right">
                        <Input.Search placeholder="Search Users" value={userModalSearch} onChange={(e) => setUserModalSearch(e.target.value)}></Input.Search>
                    </Col>
                </Row>
                <List
                    locale={{ emptyText: <></> }}
                    size="small"
                    dataSource={filteredUsers}
                    className="user-list"
                    renderItem={(item) => (
                        <List.Item>
                            <Row gutter={16, 16}>
                                <Space>
                                    <Checkbox onChange={(e) => handleCheckUser(e, item)}> </Checkbox>
                                    <Avatar src={item.imageUrl} style={{ marginRight: 5 }}>{getAvatarAlt(item.userName)}</Avatar>
                                    <span>{item.userName}</span>
                                </Space>
                            </Row>
                        </List.Item>
                    )}
                >
                </List>
                <br />
                <Row gutter={[16, 16]}>
                    <Col span={24} align="right">
                        <Space>
                            <Button onClick={toggleIsUsersModalOpen}>Cancel</Button>
                            <Button onClick={handleAddUserFromModal} type="primary" disabled={checkedUsersForModal.length === 0}>Add</Button>
                        </Space>
                    </Col>
                </Row>
            </Modal>
        </>
    );
}

function mapStateToProps(state) {
    return {
        teams: state.dam.teams
    };
}

function mapDispatchToProps(dispatch) {
    return {
        getTeams: () => dispatch(getTeams()),
        saveTeam: (data) => dispatch(saveTeam(data)),
        deleteTeam: (data) => dispatch(deleteTeam(data)),
        loadUsers: () => dispatch(getUsers(false))
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(Teams));
