import React, { useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { history } from '../../utilities/history';
import { LowFrequencyContext } from '@damcontext';
import HomeCard from './homeCard';
import { Avatar, Col } from 'antd';
import { faPencilRuler, faPhotoVideo, faShoppingCart, faFillDrip } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import { USER_ROLES } from '../constants';

function Home(props) {
	const { t } = useTranslation();

	//collections , Pinned assets , Library , Brand hub and Guidelines
	const details = [
		{
			color: 'primary',
			icon: faShoppingCart,
			route: '/collections',
			title: t('Home.Collections')
		},
		{
			color: 'primary',
			icon: faFillDrip,
			route: 'https://lam.simplemrm.com/',
			title: 'Brand Automate' //t('Home.Pinned Assets')
		},
		{
			color: 'primary',
			icon: faPhotoVideo,
			route: '/assets',
			title: t('Home.Library')
		},
		{
			color: 'primary',
			icon: faPencilRuler,
			route: 'https://simple-brandhub.powerappsportals.com/',
			title: 'Brand Hub and Guidelines' //t('Home.Brand Hub')
		},
		{
			color: 'primary',
			icon: {
				prefix: 'fab',
				iconName: 'arcadecompany',
				icon: [
					384,
					512,
					[],
					'e002',
					'M377 105L279.1 7c-4.5-4.5-10.6-7-17-7H256v128h128v-6.1c0-6.3-2.5-12.4-7-16.9zm-153 31V0H24C10.7 0 0 10.7 0 24v464c0 13.3 10.7 24 24 24h336c13.3 0 24-10.7 24-24V160H248c-13.2 0-24-10.8-24-24zm-96 299.2c0 6.4-6.4 12.8-12.8 12.8H76.8c-6.4 0-12.8-6.4-12.8-12.8v-70.4c0-6.4 6.4-12.8 12.8-12.8h38.4c6.4 0 12.8 6.4 12.8 12.8v70.4zm96 0c0 6.4-6.4 12.8-12.8 12.8h-38.4c-6.4 0-12.8-6.4-12.8-12.8V236.8c0-6.4 6.4-12.8 12.8-12.8h38.4c6.4 0 12.8 6.4 12.8 12.8v198.4zm32-134.4c0-6.4 6.4-12.8 12.8-12.8h38.4c6.4 0 12.8 6.4 12.8 12.8v134.4c0 6.4-6.4 12.8-12.8 12.8h-38.4c-6.4 0-12.8-6.4-12.8-12.8V300.8z'
				]
			},
			route: '/reports',
			title: t('Home.Reporting')
		}
	];
	const { currentUser } = useContext(LowFrequencyContext);

	//useEffect(() => {
	//	if (currentUser) {
	//		// check if current user is admin

	//		if (currentUser.userRole.Name === USER_ROLES.DAM_ADMIN) {
	//			history.push('/home');
	//		} else {
	//			history.push('/assets');
 //           }
 //       }
	//}, [currentUser]);


	function getUserNameIcon(name) {
		var spitUserName = name.split(' ');
		return spitUserName[0].split('')[0] + (spitUserName[1] != undefined ? spitUserName[1].split('')[0] : '');
	}
	return (
		<div className="home-container">
			<div className="home-pageheader">
				<Col style={{ marginRight: '40px' }}>
					<Avatar style={{ fontSize: '36px' }} size={120} src={currentUser ? currentUser.imageUrl : ''}>
						{currentUser && getUserNameIcon(currentUser.userName)}
					</Avatar>
				</Col>
				<Col>
					<h2 className="title">
						{t('Home.Title', { Name: currentUser && currentUser.userName && currentUser.userName })}
					</h2>
					<p className="tagline">{t('Home.Sub-Title')}</p>
				</Col>
			</div>
			<div className="home-card-area">
				{details.map((item) => (
					<HomeCard to={item.route} icon={item.icon} title={item.title} className={item.color} />
				))}
			</div>
		</div>
	);
}

const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {};
export default connect(mapStateToProps, mapDispatchToProps)(memo(Home));
