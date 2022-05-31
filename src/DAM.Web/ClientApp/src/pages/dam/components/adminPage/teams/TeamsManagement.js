import React, { memo } from 'react';
import { Layout, Row, Col } from 'antd';
import Teams from './Teams';

export default memo(function TeamsManagement(props) {
	//SET a prop where defaultActiveKey of Tabs can be manipulated (for back button)
	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				<Row className="tabpane-layout">
					<Col span={24}>
						<Teams />
					</Col>
				</Row>
			</Layout.Content>
		</Layout>
	);
});
