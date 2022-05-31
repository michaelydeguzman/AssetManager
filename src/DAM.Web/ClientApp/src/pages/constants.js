export const ROLES = ['Admin', 'Company Admin', 'Subscriber', 'User'];

export const USER_ROLES = {
	DAM_ADMIN: ROLES[0],
	COMPANY_ADMIN: ROLES[1],
	SUBSCRIBER: ROLES[2],
	USER: ROLES[3]
};

export const APPROVAL_LEVEL_STATUS = ['Pending', 'Approved', 'Rejected'];

export const ASSET_STATUS = {
	DRAFT: 0,
	ARCHIVED: 1,
	DELETED: 2,
	SUBMITTED: 3,
	APPROVED: 4,
	REJECTED: 5
};

export const PROJECT_STATUS = ['Draft', 'To Do', 'In Progress', 'Complete', 'Archived'];

export const EMAIL_TEMPLATE_CLASSIFICATION = ['Announcement', 'Action', 'Digest', 'Reminder'];

export const EMAIL_TEMPLATE_CATEGORIES = ['System', 'Approvals', 'Projects', 'Assets'];

export const BRAND_LOGO = require('../assets/logo/logo-simple.png');

export const SAMPLE_IMG = require('../assets/images/bg.jpg');

export const OPTIONS = {
	SORTBY: {
		DisplayName: { label: 'Display Name', value: 'Display Name' },
		DateCreated: { label: 'Date Created', value: 'Date Created' },
		DateApproved: { label: 'Date Approved', value: 'Date Approved' },
		DateRejected: { label: 'Date Rejected', value: 'Date Rejected' },
		DateSubmitted: { label: 'Date Submitted', value: 'Date Submitted' },
		Default: { label: 'Default', value: 'Default' }
	},
	FILTERBY: {
		AssetType: {
			Photos: { label: 'Photos', value: 'Photos' },
			Videos: { label: 'Videos', value: 'Videos' },
			Audios: { label: 'Audios', value: 'Audios' },
			Document: { label: 'Document', value: 'Document' },
			Others: { label: 'Others', value: 'Others' }
		},
		Status: {
			Draft: { label: 'Draft', value: 'Draft' },
			SubmittedForReview: { label: 'Submitted For Review', value: 'Submitted For Review' },
			Approved: { label: 'Approved', value: 'Approved' },
			Rejected: { label: 'Rejected', value: 'Rejected' }
		},
		Tag: {
			AI_Tag: { label: 'AI Tag', value: 'AI Tag' },
			User_Tag: { label: 'User Tag', value: 'User Tag' }
		}
	}
};
export function getFilterType(delta) {
	switch (String(delta)) {
		case 'image':
			return 'Photos';
		case 'video':
			return 'Videos';
		case 'audio':
			return 'Audios';
		case 'application':
			return 'Document';
		case '':
			return 'Others';
		case 'true':
			return 'AI Tag';
		case 'false':
			return 'User Tag';
		default:
			return delta;
	}
}

export const DefaultFilterState = {
	Status: [],
	Type: [],
	Tag: [],
	Account: [],
	Country: [],
	Search: [[], []],
	SortBy: '',
	SortOrder: true
};

export const FilterStateKeys = () => {
	var keys = {};
	Object.keys(DefaultFilterState).map((item) => {
		keys[item] = item;
	});
	return keys;
};

export function HSLtoPercentage(color) {
	return {
		h: `${color.h}`,
		s: `${parseInt(color.s * 100)}`,
		l: `${parseInt(color.l * 100)}`
	};
}

export function valueToHSL(color) {
	var hsl = color.split(',');
	return {
		h: hsl[0],
		s: hsl[1],
		l: hsl[2]
	};
}

export function hslToNumbers(color) {
	return `${color.h},${color.s},${color.l}`;
}

export function ApplyThemeColors(colors) {
	let rootStyle = document.body;
	if (colors.primaryColor !== undefined) {
		rootStyle.style.setProperty('--primary-h', colors.primaryColor.h);
		rootStyle.style.setProperty('--primary-s', `${colors.primaryColor.s}%`);
		rootStyle.style.setProperty('--primary-l', `${colors.primaryColor.l}%`);
		rootStyle.style.setProperty('--secondary-h', colors.secondaryColor.h);
		rootStyle.style.setProperty('--secondary-s', `${colors.secondaryColor.s}%`);
		rootStyle.style.setProperty('--secondary-l', `${colors.secondaryColor.l}%`);
		rootStyle.style.setProperty('--tertiary-h', colors.tertiaryColor.h);
		rootStyle.style.setProperty('--tertiary-s', `${colors.tertiaryColor.s}%`);
		rootStyle.style.setProperty('--tertiary-l', `${colors.tertiaryColor.l}%`);
	}
}

export function shadeColor(color, percent) {
	var R = parseInt(color.substring(1, 3), 16);
	var G = parseInt(color.substring(3, 5), 16);
	var B = parseInt(color.substring(5, 7), 16);

	R = parseInt((R * (100 + percent)) / 100);
	G = parseInt((G * (100 + percent)) / 100);
	B = parseInt((B * (100 + percent)) / 100);

	R = R < 255 ? R : 255;
	G = G < 255 ? G : 255;
	B = B < 255 ? B : 255;

	var RR = R.toString(16).length == 1 ? '0' + R.toString(16) : R.toString(16);
	var GG = G.toString(16).length == 1 ? '0' + G.toString(16) : G.toString(16);
	var BB = B.toString(16).length == 1 ? '0' + B.toString(16) : B.toString(16);

	return '#' + RR + GG + BB;
}
