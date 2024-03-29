import React from 'react';
import { Layout, Row, Col, Typography, Button } from 'antd';
import { Link } from 'react-router-dom';

export default function Footer() {
	return (
		<Layout.Footer className="footer">
			<Row type="flex" justify="center" align="middle">
				<Col xs={15} md={15} align="center">
					<Typography.Text>
						&#169; 2022. All Rights Reserved.
					</Typography.Text>
				</Col>
				<Col align="center">
					<Link to="">
						<Button className="contact-us">Contact us</Button>
					</Link>
				</Col>
			</Row>
		</Layout.Footer>
	);
}
