import React, { memo } from 'react';
import { Layout, Row, Col } from 'antd';
import Users from './Users';

export default memo(function UserManagement(props) {
	//SET a prop where defaultActiveKey of Tabs can be manipulated (for back button)
	return (
		<Layout className="dam-layout page-layout">
			<Layout.Content>
				<Row className="tabpane-layout">
					<Col span={24}>
						<Users />
					</Col>
				</Row>
			</Layout.Content>
		</Layout>
	);
});
