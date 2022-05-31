import React from 'react';

import { Row, Result } from 'antd';

export default function NotFound() {
	return (
		<Row type="flex" justify="center" align="middle" className="not-found">
			<Result
				status="500"
				title="Page Not Found"
				subTitle="Sorry, this page is not available or under construction at the moment."
			/>
		</Row>
	);
}
