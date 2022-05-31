import React, { useEffect, memo, useContext } from 'react';
import { Layout, Row, Col, Button, Tabs } from 'antd';
import Channel from './Channel';
import CountryRegion from './CountryRegion';
import Collateral from './Collateral';

import { LowFrequencyContext } from '@damcontext';

export default memo(function MetaData() {
	const { setAdminMode } = useContext(LowFrequencyContext);

	useEffect(() => {
		setAdminMode(true);
	}, []);

	const userPaneTypes = ['channel', 'country/region', 'collateral'];

	function TabpaneContent(pane) {
		if (pane === 'channel') {
			return (
				<Row className="tabpane-layout">
					<Col span={24}>
						<Channel />
					</Col>
				</Row>
			);
		} else if (pane === 'country/region') {
			return (
				<Row className="tabpane-layout">
					<Col span={24}>
						<CountryRegion />
					</Col>
				</Row>
			);
		} else if (pane === 'collateral') {
			return (
				<Row className="tabpane-layout">
					<Col span={24}>
						<Collateral />
					</Col>
				</Row>
			);
		} else {
			//return <>{pane} content</>
		}
	}

	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				<Tabs className="control-card" type="card" defaultActiveKey="dashboard">
					{userPaneTypes.map((pane, key) => (
						<Tabs.TabPane forceRender={true} tab={<Button type="secondary">{pane}</Button>} key={pane + key}>
							{TabpaneContent(pane)}
						</Tabs.TabPane>
					))}
				</Tabs>
			</Layout.Content>
		</Layout>
	);
});
