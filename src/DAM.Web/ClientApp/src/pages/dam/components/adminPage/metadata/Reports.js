import React, { memo, useEffect, useState } from 'react';
import { Layout, Card, Tabs } from 'antd';
import { connect } from 'react-redux';
import { GetPowerBiURL } from '../../../actions';
import { useTranslation } from 'react-i18next';
import ContentHeader from '../../contentHeader';

function Reports(props) {
	const { t } = useTranslation();
	const [powerBIUrl, setPowerBIUrl] = useState('');
	const [isAdminMode, setisAdminMode] = useState(false);
	props.GetPowerBiURL().then((result) => {
		setPowerBIUrl(result);
	});
	// useEffect(() => {
	// 	if (window.location.pathname.includes('admin')) {
	// 		setisAdminMode(true);
	// 	} else {
	// 		setisAdminMode(false);
	// 	}
	// }, [isAdminMode]);

	return isAdminMode ? (
		<Layout className="dam-layout page-layout">
			{/* <Card
				title={<div style={{ marginLeft: 12, fontSize: '1.3em' }}>{t('Slider.Reports')}</div>}
				className="card-container"
			> */}
			<ContentHeader title={t('Slider.Reports')} />
			<iframe id="customPowerBi" width="100%" height="800vh" src={powerBIUrl} frameBorder="0" />
			{/* </Card> */}
		</Layout>
	) : (
		<Layout className="dam-layout page-layout">
			<Card
				title={<div style={{ marginLeft: 12, fontSize: '1.3em' }}>{t('Slider.Reports')}</div>}
				className="card-container"
			>
				<Tabs>
					<Tabs.TabPane tab="Approval Enhanced Dashboard" key="1">
						<iframe
							id="customPowerBi"
							width="100%"
							height="800vh"
							src="https://app.powerbi.com/view?r=eyJrIjoiYzdiZTBkMGUtODlhNy00OWY3LTkwMTItN2Q3ZWMzMDQ1YThjIiwidCI6IjA0OTI2MGEwLTg1NzgtNDlkZi1hMTdhLWVkNGFlM2IyODQ3YyIsImMiOjEwfQ%3D%3D"
							frameBorder="0"
						/>
					</Tabs.TabPane>
					<Tabs.TabPane tab="Asset Metadata Report" key="2">
						<iframe
							id="customPowerBi"
							width="100%"
							height="800vh"
							src="https://app.powerbi.com/view?r=eyJrIjoiMTQyMGY2ZTgtZTAzNi00NzA0LThkYzgtODY3YzE1MzgwYTMyIiwidCI6IjA0OTI2MGEwLTg1NzgtNDlkZi1hMTdhLWVkNGFlM2IyODQ3YyIsImMiOjEwfQ%3D%3D"
							frameBorder="0"
						/>
					</Tabs.TabPane>
					<Tabs.TabPane tab="Assets by Region" key="3">
						<iframe
							id="customPowerBi"
							width="100%"
							height="800vh"
							src="https://app.powerbi.com/view?r=eyJrIjoiYWM2NjQyYjYtNjk5MS00MWZjLTgwODYtYjczZTMwNzY2NGI3IiwidCI6IjA0OTI2MGEwLTg1NzgtNDlkZi1hMTdhLWVkNGFlM2IyODQ3YyIsImMiOjEwfQ%3D%3D"
							frameBorder="0"
						/>
					</Tabs.TabPane>
					<Tabs.TabPane tab="System Overview Dashboard" key="4">
						<iframe
							id="customPowerBi"
							width="100%"
							height="800vh"
							src="https://app.powerbi.com/view?r=eyJrIjoiYzQ0MWY4NWEtZDZjYi00M2E0LWJmMTQtNGVhMDFlMjQ5YWJlIiwidCI6IjA0OTI2MGEwLTg1NzgtNDlkZi1hMTdhLWVkNGFlM2IyODQ3YyIsImMiOjEwfQ%3D%3D"
							frameBorder="0"
						/>
					</Tabs.TabPane>
					<Tabs.TabPane tab="Approval Report" key="5">
						<iframe
							id="customPowerBi"
							width="100%"
							height="800vh"
							src="https://app.powerbi.com/view?r=eyJrIjoiOWVkNzhmOTItZTNhMC00MDY4LTg5MzQtYjMzYzFjN2QyN2IzIiwidCI6IjA0OTI2MGEwLTg1NzgtNDlkZi1hMTdhLWVkNGFlM2IyODQ3YyIsImMiOjEwfQ%3D%3D"
							frameBorder="0"
						/>
					</Tabs.TabPane>
				</Tabs>
			</Card>
		</Layout>
		// <Layout className="dam-layout page-layout">
		// 	<Card title={t('Slider.Reports')} type="inner" className="card-container" style={{ margin: '15px' }}>
		// 		<iframe id="customPowerBi" width="100%" height="800vh" src={powerBIUrl} frameBorder="0" />
		// 	</Card>
		// </Layout>
	);
}
function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		GetPowerBiURL: () => dispatch(GetPowerBiURL())
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(Reports));
