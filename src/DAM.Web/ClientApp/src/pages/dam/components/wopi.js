import React, { useState, useEffect, useRef } from 'react';

export default function WopiHost(props) {
	const formRef = useRef(null);

	useEffect(() => {
		var frameholder = document.getElementById('frameholder');
		var office_frame = document.createElement('iframe');
		office_frame.name = 'office_frame';
		office_frame.id = 'office_frame';

		// The title should be set for accessibility
		office_frame.title = 'Office Frame';

		// This attribute allows true fullscreen mode in slideshow view
		// when using PowerPoint's 'view' action.
		office_frame.setAttribute('allowfullscreen', 'true');

		// The sandbox attribute is needed to allow automatic redirection to the O365 sign-in page in the business user flow
		office_frame.setAttribute(
			'sandbox',
			'allow-scripts allow-same-origin allow-forms allow-popups allow-top-navigation allow-popups-to-escape-sandbox'
		);
		frameholder.appendChild(office_frame);

		document.getElementById('office_form').submit();
		//formRef.current.submit();
	}, []);

	return (
		<React.Fragment>
			&
			<form
				id="office_form"
				ref={formRef}
				name="office_form"
				target="office_frame"
				action={props.fileInfo ? props.fileInfo.wopiUrlSrc : ''}
				method="POST"
			>
				{props.fileInfo && (
					<>
						<input name="access_token" value={props.fileInfo.accessToken} type="hidden" />
						<input name="access_token_ttl" value={props.fileInfo.accessTokenTTL} type="hidden" />
					</>
				)}
			</form>
			<span id="frameholder" style={{ width: '1000px', height: '1000px' }}></span>
		</React.Fragment>
	);
}
