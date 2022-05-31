import React from 'react';
import { Row, Col, Popover, Button } from 'antd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronDown, faMinusCircle } from '@fortawesome/free-solid-svg-icons';

export default function ContentHeader(props) {
	return (
		<Row className="content-header" align="middle" justify="space-between">
			<Col>
				<h2>{props.title || ''}</h2>
			</Col>
			<Col>
				<Row style={{ display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center' }}>
					{props.extraButtons || ''}
				</Row>
			</Col>
		</Row>
	);
}
export function DropDownOptions(props) {
	return (
		<Popover
			placement="bottom"
			title={
				props.onClear && (
					<div
						style={{
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'space-between'
						}}
					>
						<div style={{ textAlign: 'left' }}>{props.subtitle}</div>

						<Button
							type="link"
							className="extra-button error"
							onClick={props.onClear}
							icon={<FontAwesomeIcon icon={faMinusCircle} />}
						></Button>
					</div>
				)
			}
			content={
				<div
					style={{
						display: 'flex'
					}}
				>
					{props.content}
				</div>
			}
			trigger="click"
		>
			<Button type="link" classname="filter-sort-button">
				{props.title}
				<FontAwesomeIcon style={{ fontSize: '0.7rem', marginLeft: '0.55rem' }} icon={faChevronDown} />
			</Button>
		</Popover>
	);
}
