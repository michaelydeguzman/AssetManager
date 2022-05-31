import Dam from '../dam/components';
import UserManagement from '../dam/components/adminPage/usermanagement/Users/UserManagement';
import UserProfile from '../dam/components/adminPage/usermanagement/Users/UserProfile';
import EditProfile from '../dam/components/adminPage/usermanagement/Users/editProfile';
import Admin from '../dam/components/adminPage/AdminContainer';
import SuperAdmin from '../dam/components/adminPage/superAdmin';
import MetaData from '../dam/components/adminPage/metadata/MetaData';
import Reports from '../dam/components/adminPage/metadata/Reports';
import PartnerManagement from '../dam/components/adminPage/company/PartnerManagement';
import InviteUser from '../dam/components/adminPage/usermanagement/Users/InviteUser';
import Archive from '../archive/components';
import AuditTrail from '../auditTrail/components/AuditTrail';
import Approvals from '../approvals/components';
import SideUpload from '../otherFeatures/upload';
import VideoIndexer from '../otherFeatures/videoindexer';
import PinContainer from '../pin/pinContainer';
import Home from '../home/home';
import Collections from '../dam/components/Collections';
import Settings from '../dam/components/adminPage/settings/general';
import Teams from '../dam/components/adminPage/teams/TeamsManagement';
import MySentApprovalContainer from '../sentApproval/mySentApprovalContainer';
import CustomThemes from '../customTheme/component/CustomThemes';
import CustomLogo from '../customTheme/component/CustomLogo';
import Projects from '../projects/Projects';
import ArchivedProjects from '../projects/ArchivedProjects';
import EmailTemplates from '../dam/components/adminPage/emailtemplates/EmailTemplatesManagement';
import { USER_ROLES } from '../constants';

const Routes = [
	{
		default: true,
		exact: true,
		slug: 'DAM Library',
		route: '/',
		component: Home,
		adminOnly: true
	},
	{
		default: false,
		exact: false,
		slug: 'Home',
		route: '/home',
		component: Home,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Pinned',
		route: '/pinned',
		component: PinContainer,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'User Management',
		route: '/admin/user-management',
		component: UserManagement,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'User Management',
		route: '/admin/user-profile',
		component: UserProfile,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Admin',
		route: '/admin/user-management',
		component: Admin,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'CustomThemes',
		route: '/admin/Theme',
		component: CustomThemes,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'CustomLogo',
		route: '/admin/Logo',
		component: CustomLogo,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Metadata',
		route: '/admin/metadata',
		component: MetaData,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Metadata',
		route: '/admin/reports',
		component: Reports,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Metadata',
		route: '/reports',
		component: Reports,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Assets',
		route: '/assets',
		component: Dam,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Assets',
		route: '/assets/:id',
		component: Dam,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Assets',
		route: '/assets/folder/:folderId',
		component: Dam,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Archive',
		route: '/archive',
		component: Archive,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Audit Trail',
		route: '/audit-trail',
		component: AuditTrail,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'D365 - Assets',
		route: '/dynamics-assets',
		component: Dam,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'D365 - Archive',
		route: '/dynamics-archive',
		component: Archive,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'D365 - Audit Trail',
		route: '/dynamics-audit',
		component: AuditTrail,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'D365 - Add From DAM',
		route: '/dynamics-addfromdam',
		component: Dam,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Approvals',
		route: '/approvals/:id',
		component: Approvals,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Approvals',
		route: '/approvals',
		component: Approvals,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'VideoIndexer',
		route: '/videoindexer',
		component: VideoIndexer,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Companies',
		route: '/admin/companies',
		component: PartnerManagement,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'SideUpload',
		route: '/sideUpload',
		component: SideUpload,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'InviteUser',
		route: '/admin/invite-user',
		component: InviteUser,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Edit Profile',
		route: '/edit-profile',
		component: EditProfile,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Super Admin',
		route: '/superAdmin',
		component: SuperAdmin,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Collections',
		route: '/collections',
		component: Collections,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Collections',
		route: '/collections/:collectionId',
		component: Collections,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Settings',
		route: '/admin/settings',
		component: Settings,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Teams',
		route: '/admin/teams',
		component: Teams,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'EmailTemplates',
		route: '/admin/emailtemplates',
		component: EmailTemplates,
		adminOnly: true
	},
	{
		default: false,
		exact: true,
		slug: 'Approvals',
		route: '/mySentApprovals',
		component: MySentApprovalContainer,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Projects',
		route: '/projects',
		component: Projects,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Projects',
		route: '/projects/:projectId',
		component: Projects,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Archived Projects',
		route: '/archivedprojects',
		component: ArchivedProjects,
		adminOnly: false
	},
	{
		default: false,
		exact: true,
		slug: 'Archived Projects',
		route: '/archivedprojects/:projectId',
		component: ArchivedProjects,
		adminOnly: false
	}
];

export default Routes;
