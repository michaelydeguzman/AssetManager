import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Avatar, Card } from 'antd';
import React from 'react';
import { NavLink } from 'react-router-dom';

export default function HomeCard(props) {
	const { icon, title, className, to } = props;
	return (
		<>
			<a href={to}>
				<div className={`home-card ${className}`}>
					<Avatar size={150} className="home-icons" icon={<FontAwesomeIcon icon={icon} />} />
					<div className="box-text">{title}</div>
				</div>
			</a>
		</>
	);
}
