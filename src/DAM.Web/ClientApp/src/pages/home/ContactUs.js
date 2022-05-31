import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { CustomIcons } from '@damCustomIcons';
import React, { useEffect } from 'react';

function loadError(onError) {
	console.error(`Failed ${onError.target.src} didn't load correctly`);
}
export default function Home() {
	useEffect(() => {
		const externalScript = document.createElement('script');
		externalScript.onerror = loadError;
		externalScript.id = 'external';
		externalScript.async = true;
		externalScript.type = 'text/javascript';
		externalScript.setAttribute('crossorigin', 'anonymous');
		document.body.appendChild(externalScript);
		externalScript.src = `https://assets.freshservice.com/widget/freshwidget.js`;
	}, []);

	function renderCards(title, desciption, link, icon) {
		return (
			<div className="contactcard">
				<a href={link}>
					<div className="inside">
						<FontAwesomeIcon size="7x" icon={icon} />
						<h3 className="title"> {title}</h3>
						<span className="desciption">{desciption}</span>
					</div>
				</a>
			</div>
		);
	}

	return (
		<div className="home-container">
			<div className="section one">
				<div className="container">
					{renderCards('Log a Ticket', 'Log your issue now', 'https://support.simple.io/', CustomIcons.commentsalt)}
					{renderCards('Email Us', 'support@simple.io', 'mailto:support@simple.io', CustomIcons.at)}
					{renderCards('Call Us', '1300 740 679', 'tel:1300 740 679', CustomIcons.phoneAlt)}
				</div>
			</div>

			<div className="section two">
				<div className="container">
					<h3 className="title">Please add details and log your issue here</h3>
					<iframe
						className="freshwidget-embedded-form"
						id="freshwidget-embedded-form"
						src="https://support.simple.io/widgets/feedback_widget/new?&widgetType=embedded&screenshot=no"
						scrolling="no"
						height="750px"
						width="100%"
						frameborder="0"
					></iframe>
				</div>
			</div>
		</div>
	);
}
