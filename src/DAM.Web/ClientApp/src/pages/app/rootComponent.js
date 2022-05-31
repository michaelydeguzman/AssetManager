import React, { useState, useEffect, useMemo, useContext, memo } from 'react';
import { Route, Switch, Redirect, withRouter } from 'react-router-dom';
import { connect } from 'react-redux';

import moment from 'moment';
// import 'antd/dist/antd.css';
import { Layout } from 'antd';

import NotFound from '../notFound';
import DamHeader from '../shared/header/components/index';
import Sider from '../shared/sider/components/index';
import Footer from '../shared/footer/index';

import Routes from './routes';

import { useWindowWidth } from '@damhookuseWindowWidth';
import { getFeatureFlag, getAccounts, getCountries, checkLogin } from '../dam/actions';
import { LowFrequencyContext } from '@damcontext';
import { getUser, getUserRole } from '@damtoken';
import { getAllowedRoutes } from '@damhelper';

moment.updateLocale(moment.locale(), { invalidDate: 'N/A' });



function RootComponent(props) {
	const {
		isAdminMode,
		isSiderShow,
		isDynamics,
		setIsDynamics,
		setIsDynamicsAddFromDam,
		setAssetVersionFlag,
		setIsAdministrator,
		setAllCountries,
		setAllAccounts,
		setArchiveFlag,
		setApprovalFlag,
		setVideoIndexerFlag,
		setAudiTrailFlag,
		setReportFlag,
		sidePanelShow,
		setSidePanelShow,
		setPromoteMRMFlag,
		setCheckDuplicateFlag
	} = useContext(LowFrequencyContext);

	const width = useWindowWidth();
	const isMobileView = width < 768;
	const [isLogin, setIsLogin] = useState(false);
	const [paths, setPaths] = useState([]);

	useEffect(() => {
		async function getFeatureFlags() {
			let featureObj = await props.getFeatureFlag();
			if (featureObj) {
				featureObj.featureFlags.map((flag) => {
					if (flag.featureFlagName === 'Approvals') {
						setApprovalFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'VideoIndexer') {
						setVideoIndexerFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'AuditTrail') {
						setAudiTrailFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'AssetVersioning') {
						setAssetVersionFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'Report') {
						setReportFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'Archive') {
						setArchiveFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'PromoteMRM') {
						setPromoteMRMFlag(flag.isActivated);
					}
					if (flag.featureFlagName === 'CheckDuplicate') {
						setCheckDuplicateFlag(flag.isActivated);
					}
				});
			}
		}
		async function check() {
			if (await props.checkLogin()) {
				setIsLogin(true);
			}
		}
		check();
		getFeatureFlags();
		props.loadCountries();
		props.loadAccounts();
		var roleObj = getUser();
		if (roleObj != null && roleObj.roleId == '1') {
			setSidePanelShow(true);
		} else if (roleObj != null && roleObj.roleId == '2' && isAdminMode) {
			setSidePanelShow(true);
		} else if (roleObj != null && roleObj.roleId == '2') {
			setSidePanelShow(true);
		} else {
			setSidePanelShow(false);
		}
		var isAdmin = getUserRole().canAccessAdmin;
		setIsAdministrator(isAdmin);

		setPaths(getAllowedRoutes(Routes, isAdmin));

	}, []);

	useEffect(() => {
		setAllCountries(props.countries);
		setAllAccounts(props.accounts);
	}, [props.countries, props.accounts]);

	useEffect(() => {
		if (
			props.location.pathname === '/dynamics-assets' ||
			props.location.pathname === '/dynamics-archive' ||
			props.location.pathname === '/dynamics-audit' ||
			props.location.pathname === '/dynamics-addfromdam'
		) {
			setIsDynamics(true);
		}

		if (props.location.pathname === '/dynamics-addfromdam') {
			setIsDynamicsAddFromDam(true);
		}
	}, [props.location.pathname]);

	const activeURL = useMemo(() => {
		return props.location.pathname;
	}, [props.location.pathname]);

	return (
		<Layout className={`layout ${isDynamics ? 'dynamics' : ''}`}>
			{sidePanelShow && <Sider {...props} getWidth={width} />}

			<Layout.Content className="content">
				{!isDynamics && (
					// Header that contains search bar and logo
					<DamHeader {...props} />
				)}

				{isLogin && (
					<Layout
						className="sub-layout"
						style={{
							marginLeft: sidePanelShow ? (!isSiderShow && width >= 768 ? '13%' : isMobileView ? '0' : '80px') : ''
						}}
					>
						<Layout.Content className="sub-layout-content fade-in">
							<Switch>
								{window.scrollTo(0, 0)}

								{paths.map((path) => (
									<Route key={path.slug} exact={path.exact} path={path.route} component={path.component} />
								))}
								<Route component={NotFound} />
								<Redirect to="/404" />
							</Switch>
						</Layout.Content>

						{!isDynamics && <Footer />}
					</Layout>
				)}
			</Layout.Content>
		</Layout>
	);
}

const mapStateToProps = (state) => ({
	loggedIn: state.login.loggedIn,
	accounts: state.dam.accounts,
	countries: state.dam.countries
});
const mapDispatchToProps = (dispatch) => {
	return {
		getFeatureFlag: () => dispatch(getFeatureFlag()),
		loadAccounts: () => dispatch(getAccounts()),
		loadCountries: () => dispatch(getCountries()),
		checkLogin: () => dispatch(checkLogin())
	};
};
export default withRouter(connect(mapStateToProps, mapDispatchToProps)(memo(RootComponent)));
