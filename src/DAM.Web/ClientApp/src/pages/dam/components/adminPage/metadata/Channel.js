import React, { useState, useEffect, memo } from 'react';
import {
	Layout,
	Row,
	Col,
	Card,
	Input,
	Select,
	Button,
	List,
	Space,
	Checkbox,
	Form,
	Modal,
	message,
	Tree,
	Upload,
	Carousel,
	Image,
	Tabs,
	Spin,
	Tag,
	Table,
	Progress
} from 'antd';

export default memo(function Channel(props) {
	const columns = [
		{
			title: 'Channel',
			dataIndex: 'channel',
			key: 'channel'
		},
		{
			title: 'Description',
			dataIndex: 'description',
			key: 'description'
		},
		{
			title: 'Output',
			dataIndex: 'output',
			key: 'output'
		},
		{
			title: 'Add New',
			dataIndex: 'addNew',
			key: 'addNew'
		}
	];

	const dataSource = [
		{
			key: '1',
			channel: 'Digital',
			description: 'Digital colour code is RGB and 120DPI maximum',
			output: 'JPEG, PNG',
			addNew: 'Review'
		},
		{
			key: '2',
			channel: 'Print',
			description: 'Print colour code is CMYK and 200DPI minimum',
			output: 'Office Print, Professional Print',
			addNew: 'Review'
		}
	];

	return (
		<>
			<Table dataSource={dataSource} columns={columns} />
		</>
	);
});
