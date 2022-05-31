import React, { useState, memo, useContext, useEffect } from 'react';
import { getUser, logout, setAppMode } from '@damtoken';
import { useSelector, connect } from 'react-redux';
import { NavLink, Link, useLocation } from 'react-router-dom';

import { ROLES, BRAND_LOGO } from '../../../constants';
import { Layout, Row, Col, Menu, Dropdown, Avatar, Typography, Space, Input, Select } from 'antd';

import { useWindowWidth } from '@damhookuseWindowWidth';

import { FaAngleDown } from 'react-icons/fa';

import { CgMenu, CgClose } from 'react-icons/cg';
import { BsThreeDotsVertical } from 'react-icons/bs';
import { LowFrequencyContext } from '@damcontext';
import { getUsers, getAllTags } from '../../../dam/actions';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBell, faHistory, faSearch, faTag, faTimes } from '@fortawesome/free-solid-svg-icons';
import { CustomIcons } from '@damCustomIcons';
import { getAppMode, setSearchHistory } from '@damtoken';

import { useTranslation } from 'react-i18next';
import { languages } from '../../../../locales/i18n';
// import NotificationIcon from '../../../notification/component/notificationIcon';

function DamHeader(props) {
	const { t, i18n } = useTranslation();
	const location = useLocation();
	const { themeLogo } = useContext(LowFrequencyContext);

	const {
		filterState,
		setFilterState,
		isAdminMode,
		setAdminMode,
		isToggle,
		setToggle,
		isUserUpdated,
		setIsUserUpdated,
		currentUser,
		setCurrentUser,
		isAdministrator
	} = useContext(LowFrequencyContext);

	const [userId, setUserId] = useState();
	const [userName, setUserName] = useState('');
	const [isDropdownMenuShow, setDropdownMenu] = useState(false);
	const [isSearchOpen, setSearchOpen] = useState(false);
	const [isShowMore, setShowMore] = useState(false);
	const fileData = useSelector((state) => state.dam.fileData);
	const archiveData = useSelector((state) => state.archive.fileData);
	const approvalsData = useSelector((state) => state.approvals.fileData);
	const [allTags, setAllTags] = useState([]);
	const [allSearchHistorys, setAllSearchHistorys] = useState([]);
	const [dropdownList, setDropdownList] = useState([]);
	const width = useWindowWidth();
	const isDesktopView = width >= 768;
	function getFileData() {
		if (window.location.href.indexOf('/approvals') > -1) {
			return approvalsData;
		} else if (window.location.href.indexOf('/archive') > -1) {
			return archiveData;
		} else {
			return fileData;
		}
	}
	useEffect(() => {
		onChangeSearch([], [], true);
	}, [location]);

	useEffect(() => {
		renderSearchValues();
	}, [filterState]);

	useEffect(() => {
		var loggedInUser = getUser();
		if (loggedInUser) {
			setUserId(loggedInUser.id);
			setUserName(loggedInUser.userName);
			setupData(loggedInUser.id);
			setupTags();
			setupHistory();
		}
	}, []);

	useEffect(() => {
		if (userId && isUserUpdated) {
			setIsUserUpdated(false);
			setupData(userId);
		}
	}, [props]);

	const changeLanguage = (lng) => {
		i18n.changeLanguage(lng);
	};

	const clickToggleMode = () => {
		setAdminMode((prev) => !prev);
	};

	const clickHomePage = () => {
		setAdminMode(false);
	};

	async function setupData(id) {
		let users = await props.getUsers(id, window.location.href.includes('/admin/'));
		if (users) {
			setCurrentUser(users.data.users);
		}
	}

	async function setupTags() {
		let tags = await props.getAllTags();
		if (tags.data.tags.length > 0) {
			var newTags = [];
			tags.data.tags.map((tag) => {
				if (newTags.filter((e) => e.value.toLowerCase() === tag.name.toLowerCase()).length === 0) {
					let newTag = {
						type: 'tags',
						title: tag.name.toLowerCase(),
						value: tag.name.toLowerCase()
					};
					newTags.push(newTag);
				}
			});
			setAllTags(newTags);
		}
	}

	function setupHistory() {
		let historys = getAppMode().History;
		if (historys.length > 0) {
			var tempHistory = [];
			historys.map((history) => {
				tempHistory.push(history);
			});
			setAllSearchHistorys(tempHistory);
		}
	}

	function addHistory(searchText) {
		var history = allSearchHistorys;
		if (history.length > 9) {
			// for saving 10 search histories
			history.shift();
		}
		history.push({
			type: 'history',
			title: searchText.toLowerCase(),
			value: searchText.toLowerCase()
		});
		setAllSearchHistorys(history);
		setSearchHistory(history);
		console.log('Searchhistory', history);
	}

	function renderSearchValues() {
		var dataValues = [];
		dataValues = allSearchHistorys.concat(allTags);
		setDropdownList(
			dataValues.map((data) => (
				<Select.Option value={data.value} title={data.title} key={data.value}>
					<FontAwesomeIcon icon={data.type === 'tags' ? faTag : faHistory} /> {data.title}
				</Select.Option>
			))
		);
	}

	function onChangeSearch(value, options, didPageChange = false) {
		var searchValues = didPageChange ? filterState.Search[0] : value;
		var searchFileData = getFileData();

		if (options.length > 0 && !options[options.length - 1].title) {
			addHistory(searchValues[options.length - 1]);
		}

		if (searchValues.length === 0) {
			searchFileData && setFilterState({ ...filterState, Search: [[], searchFileData] });
			return;
		}

		setFilterState({ ...filterState, Search: [searchValues, []] });
		searchFileData &&
			setFilterState((prev) => {
				let ids = [];
				searchFileData.map((data) => {
					searchValues.map((key) => {
						let flag = true;
						if (data.tags) {
							data.tags.map((tag) => {
								if (tag.name.toLowerCase().includes(key.toLowerCase())) {
									ids.push(data.id);
									flag = false;
								}
							});
						}

						if (flag && data.name && data.name.toLowerCase().includes(key.toLowerCase())) {
							ids.push(data.id);
							flag = false;
						}
						if (flag && data.extension && data.extension.toLowerCase().includes(key.toLowerCase())) {
							ids.push(data.id);
							flag = false;
						}
						if (flag && data.fileName && data.fileName.toLowerCase().includes(key.toLowerCase())) {
							ids.push(data.id);
							flag = false;
						}
						if (flag && data.comments && data.comments.toLowerCase().includes(key.toLowerCase())) {
							ids.push(data.id);
							flag = false;
						}
					});
				});

				return {
					...filterState,
					Search: [
						searchValues,
						searchFileData
							.filter((row) => ids.includes(row.id))
							.sort((a, b) =>
								a.name.localeCompare(b.name, undefined, {
									numeric: true,
									sensitivity: 'base'
								})
							)
					]
				};
			});
	}

	function SearchComponent(width = '100%') {
		return (
			<Select
				style={{ width }}
				className="input-search"
				onChange={onChangeSearch}
				showSearch
				placeholder={t('ModalDetail.Search')}
				allowClear
				value={filterState.Search ? filterState.Search[0] : []}
				tokenSeparators={[',']}
				mode="tags"
				bordered={false}
				onDropdownVisibleChange={() => renderSearchValues()}
			>
				{dropdownList}
			</Select>
		);
	}

	function getUserNameIcon(name) {
		var spitUserName = name.split(' ');
		return spitUserName[0].split('')[0] + (spitUserName[1] != undefined ? spitUserName[1].split('')[0] : '');
	}

	function showMore() {
		return setShowMore((prev) => !prev);
	}

	const handleLogOut = () => {
		logout();
	};
	function DropdownProfileComponent(outerLooks) {
		return (
			<Dropdown
				overlay={
					<Menu onClick={() => setDropdownMenu(false)}>
						{isAdministrator && (
							<>
								{isAdminMode ? (
									<Menu.Item key="adminMode" onClick={clickToggleMode}>
										<NavLink to={'/assets'}>{t('Slider.Dashboard')}</NavLink>
									</Menu.Item>
								) : (
									<>
										<Menu.Item key="adminMode" onClick={clickToggleMode}>
											<NavLink to={'/admin/user-management'}>{t('Slider.Admin Settings')}</NavLink>
										</Menu.Item>
										{currentUser && currentUser.email === 'simple.user@simple.io' && (
											<Menu.Item key="superAdmin" onClick={clickToggleMode}>
												<NavLink to={'/superAdmin'}>Super Admin</NavLink>
											</Menu.Item>
										)}
									</>
								)}
							</>
						)}
						<Menu.Item key="editProfile">
							<NavLink to={'/edit-profile'}>{t('Slider.Edit Profile')}</NavLink>
						</Menu.Item>
						<Menu.Item key="logout" onClick={handleLogOut}>
							{t('Login Page.Logout')}
						</Menu.Item>
					</Menu>
				}
				trigger={['click']}
				onVisibleChange={(flag) => setDropdownMenu(flag)}
				visible={isDropdownMenuShow}
				overlayClassName={isDesktopView ? 'dropdown-overlay' : 'xs-dropdown-overlay'}
				onClick={() => setDropdownMenu((prev) => !prev)}
			>
				{/* {outerLooks} */}
				<Col className={`right-corner ${isDesktopView ? 'border-gray' : ''} `} align="center">
					<Space className="outer-space">
						<Avatar size={44} src={currentUser ? currentUser.imageUrl : ''}>
							{currentUser && getUserNameIcon(currentUser.userName)}
						</Avatar>
						{isDesktopView && (
							<Col className="user">
								<Typography.Text className="name">{userName}</Typography.Text>
								<Typography.Text type="primary" className="profession">
									{currentUser ? ROLES[currentUser.userRoleId - 1] : ''}
								</Typography.Text>
							</Col>
						)}
						<Row type="flex" justify="space-between" align="middle">
							<Col>
								<FaAngleDown className="icons" onClick={(e) => e.preventDefault()} />
							</Col>
						</Row>
					</Space>
				</Col>
			</Dropdown>
		);
	}

	function languageDropdown(color = 'color-white') {
		return (
			<Dropdown
				overlay={
					<Menu>
						{languages.map((item, index) => (
							<Menu.Item key={`Language:${index}`} onClick={() => changeLanguage(item.shorthand)}>
								{item.title}
							</Menu.Item>
						))}
					</Menu>
				}
			>
				<Col className="right-corner" align="center" style={{ marginRight: '1rem' }}>
					<Avatar
						className={`avatar-icon ${color}`}
						size={30}
						icon={<FontAwesomeIcon size="2x" icon={CustomIcons.globe} />}
					></Avatar>
					{isDesktopView && <span style={{ marginLeft: '0.5rem' }}>{t('Selected Language')}</span>}
				</Col>
			</Dropdown>
		);
	}
	return (
		<Layout.Header className="header">
			<Row className="header-content" type="flex" justify="space-between" align="middle">
				{!isDesktopView && (
					<Col>
						{' '}
						{isToggle ? (
							<CgClose className="drawer-close-icon" onClick={() => setToggle((prev) => !prev)} />
						) : (
							<CgMenu className="drawer-open-icon" onClick={() => setToggle((prev) => !prev)} />
						)}{' '}
					</Col>
				)}

				<Col className="left-corner" align="center">
					<Link to="/" className="logo" onClick={clickHomePage}>
						<img src={themeLogo} loading="lazy" alt="logo" />
					</Link>
				</Col>
				{!isDesktopView && (
					<Col>
						<BsThreeDotsVertical onClick={showMore} className="dots-icon" />
					</Col>
				)}

				{!isDesktopView && isShowMore && (
					// this component will show when viewport is less than 768px
					<Row className="show-more">
						<Col span={24}>
							<Row type="flex" justify="space-between" align="middle">
								<Col xs={4} sm={2}>
									<Avatar
										size={40}
										icon={!isSearchOpen ? <FontAwesomeIcon icon={faSearch} /> : <FontAwesomeIcon icon={faTimes} />}
										onClick={() => setSearchOpen((prev) => !prev)}
									></Avatar>
								</Col>
								<Col xs={20} sm={22}>
									{isSearchOpen ? (
										SearchComponent('100%')
									) : (
										<div style={{ display: 'flex', alignItems: 'center', justifyContent: 'flex-end' }}>
											{languageDropdown('color-black')}
											{DropdownProfileComponent()}
										</div>
									)}
								</Col>
							</Row>
						</Col>
					</Row>
				)}
				{isDesktopView && (
					<>
						{window.location.pathname !== '/home' && <Col className="global-search-bar">{SearchComponent()}</Col>}
						<div style={{ display: 'flex', alignItems: 'center' }}>
							<FontAwesomeIcon size="2x" style={{ color: 'white', marginRight: '17px' }} icon={faBell} />
							{/* {<NotificationIcon />} */}
							{languageDropdown()}
							{DropdownProfileComponent()}
						</div>
					</>
				)}
			</Row>
		</Layout.Header>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getUsers: (id, isAdmin) => dispatch(getUsers(id, isAdmin)),
		getAllTags: () => dispatch(getAllTags())
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(DamHeader));
