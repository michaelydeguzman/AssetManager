import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { Layout, Row, Col, Button, Tabs } from 'antd';
import CreatePartner from './CreatePartner';
import Companies from './Companies';
import { getUsers, addCompany, getCompanies, getFolders, getUserRoles } from '../../../actions';
import { useTranslation } from 'react-i18next';
const { TabPane } = Tabs;

function PartnerManagement(props) {
	const { t } = useTranslation();
	const partnerPaneTypes = ['View', 'Create'];
	const [userList, setUserList] = useState([]);
	const [companyList, setCompanyList] = useState([]);
	const [folderList, setFolderList] = useState([]);
	const [userRoles, setUserRoles] = useState([]);
	useEffect(() => {
		setupData();
	}, []);

	const setupData = async () => {
		let users = await props.getUsers(0);
		let companies = await props.getCompanies();
		let folders = await props.getFolders();
		let userRoles = await props.getUserRoles();
		if (users && users.data.users) {
			setUserList(users.data.users);
		}
		if (companies && companies.data.companies) {
			setCompanyList(companies.data.companies);
		}
		if (folders && folders.data.folders) {
			setFolderList(folders.data.folders);
		}
		if (userRoles && userRoles.data.userRoles) {
			setUserRoles(userRoles.data.userRoles);
		}
	};

	function TabpaneContent(pane) {
		if (pane === 'View') {
			return (
				<Row className="tabpane-layout">
					<Col span={24}>
						<Companies userList={userList} companyList={companyList} folderList={folderList} setupData={setupData} />
					</Col>
				</Row>
			);
		} else if (pane === 'Create') {
			return (
				<Row className="tabpane-layout">
					<Col span={24}>
						<CreatePartner userList={userList} partnerList={companyList} setupData={setupData} />
					</Col>
				</Row>
			);
		} else {
			return <></>;
		}
	}

	//SET a prop where defaultActiveKey of Tabs can be manipulated (for back button)
	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				<Tabs className="control-card" type="card" defaultActiveKey="dashboard">
					{partnerPaneTypes.map((pane, key) => (
						<Tabs.TabPane forceRender={true} tab={t(`Button.${pane}`)} key={pane + key}>
							{TabpaneContent(pane)}
						</Tabs.TabPane>
					))}
				</Tabs>
			</Layout.Content>
		</Layout>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getUsers: (id) => dispatch(getUsers(id, true)),
		addPartner: (data) => dispatch(addCompany(data)),
		getCompanies: () => dispatch(getCompanies()),
		getFolders: () => dispatch(getFolders()),
		getUserRoles: () => dispatch(getUserRoles())
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(PartnerManagement));
