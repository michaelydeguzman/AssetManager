import React from 'react';

export default function ColorIcon(props) {
	return (
		<div style={{ width: `${props.width || '80px'}`, height: `${props.height || '22px'}`, marginTop: '2px' }}>
			<div
				style={{
					background: `hsl(${props.color.h}, ${props.color.s}%, ${props.color.l}%)`,
					height: '100%',
					width: '100%',
					cursor: 'pointer',
					borderRadius: '3px',
					boxShadow: 'rgba(0, 0, 0, 0.15) 0px 0px 0px 1px inset'
				}}
			></div>
		</div>
	);
}
