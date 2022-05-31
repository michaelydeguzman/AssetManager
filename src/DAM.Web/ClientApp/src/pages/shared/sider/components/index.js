import React, { memo, useEffect, useState, useContext } from 'react';
import { connect } from 'react-redux';
import { NavLink, useHistory } from 'react-router-dom';
import { Menu, Layout, Drawer, Modal } from 'antd';
import { CgMenu, CgClose } from 'react-icons/cg';
import { getUserRole, getUser } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';

import {
	LineChartOutlined,
	HomeOutlined,
	FolderOpenOutlined,
	BookOutlined,
	PictureOutlined,
	AuditOutlined,
	LikeOutlined,
	VideoCameraOutlined,
	FolderOutlined,
	UserAddOutlined,
	IdcardOutlined,
	FileSearchOutlined,
	PushpinOutlined,
	ShoppingCartOutlined,
	SettingOutlined,
	AimOutlined,
	ExclamationCircleOutlined,
	SendOutlined,
	ProjectOutlined,
	CopyrightOutlined,
	BgColorsOutlined,
	TeamOutlined,
	CodeOutlined
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { getPinFolders } from '../../../dam/actions';

function Sider(props) {
	const { t } = useTranslation();
	const userRole = getUserRole();
	const [selectedMenu, setSelectedMenu] = useState('1');
	const {
		isAdminMode,
		setAdminMode,
		isSiderShow,
		setSiderShow,
		isToggle,
		setToggle,
		currentUser,
		approvalFlag,
		videoIndexerFlag,
		audiTrailFlag,
		reportFlag,
		archiveFlag,
		isOrdering,
		setIsOrdering,
		pinnedFolders,
		setPinnedFolders
	} = useContext(LowFrequencyContext);
	const { confirm } = Modal;
	const { getWidth } = props;
	let history = useHistory();

	// const [carts, setCarts] = useState([]);
	// const [selectedCart, setSelectedCart] = useState();
	// const [currentUserId, setCurrentUserId] = useState('');
	// const [isLoading, setIsLoading] = useState(true);

	useEffect(() => {
		//loadCarts();
		loadPinnedFolders();
	}, []);

	// async function loadCarts() {
	// 	setIsLoading(true);

	// 	let user = await getUser();
	// 	let cartData = await props.getCarts(user.id);
	// 	let savedCarts = cartData.filter((cart) => cart.isCurrentCart === false);

	// 	setCarts([...savedCarts]);
	// 	setCurrentUserId(user.id);

	// 	setCarts([...savedCarts]);
	// 	setCurrentUserId(user.id);
	// 	setSelectedCart({});

	// 	setIsLoading(false);
	// }
	async function loadPinnedFolders() {
		let result = await props.getPinFolders();
		if (result?.length > 0) {
			setPinnedFolders(
				result.map((p) => {
					return { id: p.folderId, folderName: p.folderName };
				})
			);
		}
	}

	useEffect(() => {
		let menuKey = '1';
		switch (props.location.pathname) {
			case '/':
			// menuKey = '1';
			// break;
			case '/home':
				menuKey = '1';
				break;
			case '/recent':
				menuKey = '2';
				break;
			case '/pinned':
				menuKey = '3';
				break;
			case '/assets':
			case (props.location.pathname.match('/assets/') || {}).input:
				menuKey = '4';
				break;
			case '/admin/user-management':
				menuKey = '5';
				break;
			case '/admin/metadata':
				menuKey = '6';
				break;
			case '/archive':
				menuKey = '7';
				break;
			case '/audit-trail':
				menuKey = '8';
				break;
			case '/dynamics-assets':
				menuKey = '9';
				break;
			case '/dynamics-archive':
				menuKey = '10';
				break;
			case '/dynamics-audit':
				menuKey = '11';
				break;
			case '/approvals':
			case (props.location.pathname.match('/approvals/') || {}).input:
				menuKey = '12';
				break;
			case '/upload':
				menuKey = '13';
				break;
			case '/videoindexer':
				menuKey = '14';
				break;
			case '/admin/regions':
				menuKey = '15';
				break;
			case '/admin/invite-user':
				menuKey = '16';
				break;
			case '/reports':
				menuKey = '17';
				break;
			case '/collections':
			case (props.location.pathname.match('/collections/') || {}).input:
				menuKey = '18';
				break;
			case '/admin/settings':
				menuKey = '19';
				break;
			case '/mySentApprovals':
				menuKey = '20';
				break;
			case '/admin/teams':
				menuKey = '21';
				break;
			case '/projects':
			case (props.location.pathname.match('/projects/') || {}).input:
				menuKey = '22';
				break;
			case '/archivedprojects':
			case (props.location.pathname.match('/archivedprojects/') || {}).input:
				menuKey = '23';
				break;
			case '/admin/Theme':
				menuKey = '24';
				break;
			case '/admin/emailtemplates':
				menuKey = '25';
				break;
			default:
				menuKey = '1';
				break;
		}
		setSelectedMenu(menuKey);
		if (props.location.pathname.includes('/admin') && currentUser && (currentUser.userRoleId === 1 || currentUser.userRoleId === 2) ) {
			setAdminMode(true);
		} else {
			setAdminMode(false);
		}
	}, [currentUser, props.location.pathname]);

	function sitemapMenus(props) {
		if (isAdminMode) {
			return (
				<Menu theme="light" mode="inline" defaultOpenKeys={['admin', 'system','templates']} selectedKeys={[selectedMenu]}>
					<Menu.SubMenu
						key="system"
						icon={isSiderShow && <LineChartOutlined />}
						title={'System Set Up'}
						style={{ marginTop: 20 }}
					>
						{currentUser && currentUser.userRoleId === 1 && (
							<Menu.Item key="24" icon={<BgColorsOutlined className="menu-icon" />}>
								<NavLink to={'/admin/Theme'}>{'Theme'}</NavLink>
							</Menu.Item>
						)}
						<Menu.Item key="19" icon={<SettingOutlined className="menu-icon" />}>
							<NavLink to={'/admin/settings'}>{'Watermark'}</NavLink>
						</Menu.Item>
					</Menu.SubMenu>
					<Menu.SubMenu
						key="admin"
						icon={isSiderShow && <LineChartOutlined />}
						title={t('Slider.Admin')}
						style={{ marginTop: 20 }}
					>
						<Menu.Item key="5" icon={<IdcardOutlined className="menu-icon" />}>
							<NavLink to={'/admin/user-management'}>{t('Slider.Users')}</NavLink>
						</Menu.Item>
						{/*<Menu.Item key="6" icon={<ProfileOutlined className="menu-icon" />}>
              <NavLink to={"/admin/metadata"}>{t('Slider.Metadata')}</NavLink>
            </Menu.Item>*/}
						<Menu.Item key="16" icon={<UserAddOutlined className="menu-icon" />}>
							<NavLink to={'/admin/invite-user'}>{t('Slider.Invite User')}</NavLink>
						</Menu.Item>
						{currentUser && currentUser.userRoleId === 1 && (
							<Menu.Item key="15" icon={<AimOutlined className="menu-icon" />}>
								<NavLink to={'/admin/companies'}>{t('Slider.Companies')}</NavLink>
							</Menu.Item>
						)}
						<Menu.Item key="21" icon={<TeamOutlined className="menu-icon" />}>
							<NavLink to={'/admin/teams'}>{t('Slider.Teams')}</NavLink>
						</Menu.Item>
					</Menu.SubMenu>
					<Menu.SubMenu
						key="templates"
						icon={isSiderShow && <CodeOutlined/>}
						title={t('Slider.Templates')}
						style={{ marginTop: 20 }}
					>
						<Menu.Item key="25" icon={<IdcardOutlined className="menu-icon" />}>
							<NavLink to={'/admin/emailtemplates'}>{t('Slider.EmailTemplates')}</NavLink>
						</Menu.Item>
					</Menu.SubMenu>
				</Menu>
			);
		} else {
			return (
				<Menu
					theme="light"
					mode="inline"
					selectedKeys={[selectedMenu]}
					defaultOpenKeys={[
						'dashboard',
						'my-pinned',
						'my-pinned-folder',
						'dam-library',
						'other-features',
						'audit-trail'
					]}
					inlineIndent={15}
				>
					<Menu.SubMenu key="dashboard" icon={isSiderShow && <LineChartOutlined />} title={t('Slider.Dashboard')}>
						<Menu.Item key="1" icon={<HomeOutlined className="menu-icon" />} onClick={() => menuItemJump('/home')}>
							<span>{t('Slider.Home')}</span>
						</Menu.Item>
					</Menu.SubMenu>

					{userRole.canPinAsset && (
						<Menu.SubMenu key="my-pinned" title={'My Work'}>
							<Menu.Item
								key="22"
								icon={<ProjectOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/projects')}
							>
								<span>{'Projects'}</span>
							</Menu.Item>
							<Menu.Item
								key="3"
								icon={<PushpinOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/pinned')}
							>
								<span>{'Pinned Assets'}</span>
							</Menu.Item>
							{isSiderShow ? (
								<></>
							) : (
								<Menu.SubMenu
									key="my-pinned-folder"
									icon={<PushpinOutlined className="menu-icon" />}
									title={'Pinned Folders'}
									className="pinned_folder_title"
								>
									{pinnedFolders.map((p) => {
										return (
											<Menu.Item key={`/assets/folder/${p.id}`} icon={<FolderOutlined className="menu-icon" />}>
												<NavLink to={`/assets/folder/${p.id}`}>{p.folderName}</NavLink>
											</Menu.Item>
										);
									})}
								</Menu.SubMenu>
							)}
							<Menu.Item
								key="23"
								icon={<BookOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/archivedprojects')}
							>
								<span>{'Archived Projects'}</span>
							</Menu.Item>
						</Menu.SubMenu>
					)}

					<Menu.SubMenu key="dam-library" icon={isSiderShow && <FolderOpenOutlined />} title={t('Slider.DAM Library')}>
						<Menu.Item key="4" icon={<PictureOutlined className="menu-icon" />} onClick={() => menuItemJump('/assets')}>
							<span>{t('Slider.Assets')}</span>
						</Menu.Item>
						{approvalFlag && userRole.canApprove && (
							<Menu.Item
								key="12"
								icon={<LikeOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/approvals')}
							>
								<span>{t('Slider.Approvals')}</span>
							</Menu.Item>
						)}
						{approvalFlag && userRole.canApprove && (
							<Menu.Item
								key="20"
								icon={<SendOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/mySentApprovals')}
							>
								<span>{t('Slider.Sent Approvals')}</span>
							</Menu.Item>
						)}
						{archiveFlag && userRole.canArchive && (
							<Menu.Item key="7" icon={<BookOutlined className="menu-icon" />} onClick={() => menuItemJump('/archive')}>
								<span>{t('Slider.Archive')}</span>
							</Menu.Item>
						)}
						<Menu.Item
							key="18"
							icon={<ShoppingCartOutlined className="menu-icon" />}
							onClick={() => menuItemJump('/collections')}
						>
							<span>{t('Collections')}</span>
						</Menu.Item>
						{currentUser && currentUser.userRoleId === 1 && reportFlag && (
							<Menu.Item
								key="17"
								icon={<FileSearchOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/reports')}
							>
								<span>{t('Slider.Reports')}</span>
							</Menu.Item>
						)}
					</Menu.SubMenu>

					{audiTrailFlag && (
						<Menu.SubMenu
							key="audit-trail"
							icon={isSiderShow && <AuditOutlined className="menu-icon" />}
							title={t('Slider.Audit Trail')}
						>
							<Menu.Item
								key="8"
								icon={<AuditOutlined className="menu-icon" />}
								onClick={() => menuItemJump('/audit-trail')}
							>
								<span>{t('Slider.Audit Trail')}</span>
							</Menu.Item>
						</Menu.SubMenu>
					)}
					{videoIndexerFlag && (
						<>
							<Menu.SubMenu
								key="other-features"
								icon={isSiderShow && <AuditOutlined className="menu-icon" />}
								title={t('Slider.Other Features')}
							>
								<Menu.Item
									key="13"
									icon={<VideoCameraOutlined className="menu-icon" />}
									onClick={() => menuItemJump('/videoindexer')}
								>
									<span>{t('Slider.Video Indexer')}</span>
								</Menu.Item>
							</Menu.SubMenu>
						</>
					)}
				</Menu>
			);
		}
	}

	function renderTrigger() {
		console.log(isSiderShow);
		if (!isSiderShow) {
			return <CgClose className="drawer-close-icon" onClick={() => setSiderShow((prev) => !prev)} />;
		} else {
			return <CgMenu className="drawer-open-icon" onClick={() => setSiderShow((prev) => !prev)} />;
		}
	}

	const menuItemJump = (page) => {
		if (isOrdering) {
			confirm({
				title: t('Leave Pinned Assets.Title'),
				content: t('Leave Pinned Assets.Body'),
				icon: <ExclamationCircleOutlined />,
				onOk() {
					setIsOrdering(!isOrdering);
					history.push(page);
				},
				onCancel() {}
			});
		} else {
			history.push(page);
		}
	};

	return (
		<>
			{getWidth >= 768 ? (
				<Layout.Sider
					theme="light"
					className="sider"
					width="13%"
					collapsible
					collapsed={isSiderShow}
					trigger={renderTrigger()}
				>
					{sitemapMenus(props)}
				</Layout.Sider>
			) : (
				<Drawer className="drawer" placement="left" visible={isToggle} closable={true} onClose={() => setToggle(false)}>
					{sitemapMenus(props)}
				</Drawer>
			)}
		</>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getPinFolders: (data) => dispatch(getPinFolders(data))
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(Sider));
