import React, { useEffect, useState, memo } from 'react';
import { connect } from 'react-redux';
import { Router, Route, Redirect, Switch } from 'react-router-dom';
import { history } from '../../utilities/history';
import { getAdminMode, getToken } from '@damtoken';
import Login from '../login/component';
import CreatePassword from '../createPassword/component';
import ResetPassword from '../resetPassword/component';
import ContactUs from '../home/ContactUs';
import Routes from './routes';
import RootComponent from './rootComponent';
import { LowFrequencyContext } from '@damcontext';
import { DefaultFilterState, ApplyThemeColors, valueToHSL } from '../constants';
import '../../assets/styles/main.scss';
import { getThemeStyles } from '../customTheme/actions';

function App(props) {
	const [filterState, setFilterState] = useState(DefaultFilterState);
	const [isToggle, setToggle] = useState(false);
	const [isSiderShow, setSiderShow] = useState(false);
	const [isDynamics, setIsDynamics] = useState(false);
	const [isDynamicsAddFromDam, setIsDynamicsAddFromDam] = useState(false);
	const [isAdminMode, setAdminMode] = useState(getAdminMode());
	const [isUserUpdated, setIsUserUpdated] = useState(false);
	const [currentUser, setCurrentUser] = useState(null);
	const [isAdministrator, setIsAdministrator] = useState(false);

	//Basic information
	const [allCountries, setAllCountries] = useState();
	const [allAccounts, setAllAccounts] = useState();
	// Approval Feature Flag
	const [approvalFlag, setApprovalFlag] = useState(false);
	const [archiveFlag, setArchiveFlag] = useState(false);
	const [videoIndexerFlag, setVideoIndexerFlag] = useState(false);
	const [audiTrailFlag, setAudiTrailFlag] = useState(false);
	const [reportFlag, setReportFlag] = useState(false);
	const [assetVersionFlag, setAssetVersionFlag] = useState(true);
	const [sidePanelShow, setSidePanelShow] = useState(true);
	const [promoteMRMFlag, setPromoteMRMFlag] = useState(false);
	const [checkDuplicateFlag, setCheckDuplicateFlag] = useState(false);
	const [isOrdering, setIsOrdering] = useState(false);
	const [pinnedFolders, setPinnedFolders] = useState([]);
	const [assetContainer, setAssetContainer] = useState('');
	const [assetPreviewContainer, setAssetPreviewContainer] = useState('');

	const [themeLogo, setThemeLogo] = useState('');
	const [themeColors, setThemeColors] = useState({});

	const lowFrequencyStateAndSetter = {
		isAdminMode,
		setAdminMode,
		isToggle,
		setToggle,
		isSiderShow,
		setSiderShow,
		isDynamics,
		setIsDynamics,
		filterState,
		setFilterState,
		isDynamicsAddFromDam,
		setIsDynamicsAddFromDam,
		isUserUpdated,
		setIsUserUpdated,
		isAdministrator,
		setIsAdministrator,
		currentUser,
		setCurrentUser,
		assetVersionFlag,
		setAssetVersionFlag,
		allCountries,
		setAllCountries,
		allAccounts,
		setAllAccounts,
		approvalFlag,
		archiveFlag,
		setArchiveFlag,
		setApprovalFlag,
		videoIndexerFlag,
		setVideoIndexerFlag,
		audiTrailFlag,
		setAudiTrailFlag,
		reportFlag,
		setReportFlag,
		sidePanelShow,
		setSidePanelShow,
		promoteMRMFlag,
		setPromoteMRMFlag,
		isOrdering,
		setIsOrdering,
		checkDuplicateFlag,
		setCheckDuplicateFlag,
		pinnedFolders,
		setPinnedFolders,
		assetContainer,
		setAssetContainer,
		assetPreviewContainer,
		setAssetPreviewContainer,
		themeColors,
		setThemeColors,
		themeLogo,
		setThemeLogo
	};

	async function loadThemes() {
		var styles = await props.getThemeStyles();
		if (styles.length > 0) {
			let theme = styles.filter((row) => row.isApplied === true)[0];
			setThemeColors({
				primaryColor: valueToHSL(theme.primaryColor),
				secondaryColor: valueToHSL(theme.secondaryColor),
				tertiaryColor: valueToHSL(theme.tertiaryColor)
			});
			setThemeLogo(theme.logoUrl);
		}
	}

	// async function loadLogo() {
	// 	var logos = await props.getLogos();
	// 	if (logos.length > 0) {
	// 		let logo = logos.filter((row) => row.isApplied === true)[0];
	// 		setThemeLogo(logo.logoUrl);
	// 	}
	// }

	useEffect(() => {
		loadThemes().then(() => ApplyThemeColors(themeColors));
	}, []);

	useEffect(() => {
		ApplyThemeColors(themeColors);
	}, [themeColors]);

	return (
		<LowFrequencyContext.Provider value={lowFrequencyStateAndSetter}>
			<Router history={history}>
				<Switch>
					{Routes.map((path) => (
						<Route
							key={path.slug}
							exact={path.exact}
							path={path.route}
							render={(props) =>
								getToken() ? (
									<RootComponent />
								) : (
									<Redirect to={{ pathname: '/login', state: { from: props.location } }} />
								)
							}
						/>
					))}
					<Route path="/login" component={Login} />
					<Route path="/createpassword" component={CreatePassword} />
					<Route path="/resetpassword" component={ResetPassword} />
					<Route path="/ContactUs" component={ContactUs} />
					<Redirect from="*" to="/" />
				</Switch>
			</Router>
		</LowFrequencyContext.Provider>
	);
}
const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {
	return {
		//getLogos: () => dispatch(getLogos()),
		getThemeStyles: () => dispatch(getThemeStyles())
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(memo(App));
