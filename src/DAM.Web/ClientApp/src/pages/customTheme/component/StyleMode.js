import React, { useContext, useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';

import { SketchPicker } from 'react-color';
import { Space, Button, Popover, Table, Card, Form, Switch, Input, Radio } from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { hslToNumbers, HSLtoPercentage, ApplyThemeColors, valueToHSL } from '../../constants';
import { getThemeStyles, saveThemeStyle } from '../actions';
import ColorIcon from './ColorIcon';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faEdit, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';

function StyleMode(props) {
	const { t } = useTranslation();

	const { themeColors, setThemeColors } = useContext(LowFrequencyContext);
	const [isEditingMode, setIsEditingMode] = useState(false);
	const [themeList, setThemeList] = useState([]);
	const [selectedTheme, setSelectedTheme] = useState({});

	async function loadThemes() {
		var results = await props.getThemeStyles();
		if (results.length > 0) {
			let alltheme = results.filter((row) => row.deleted === false);
			setThemeList(alltheme);
		}
	}

	async function modifyTheme(data) {
		await props.saveThemeStyle(data).then(() => {
			loadThemes();
		});
	}

	function applyUpdatedTheme(thisTheme = undefined) {
		let currenttheme = thisTheme || themeList.filter((row) => row.isApplied === true)[0];

		setThemeColors({
			primaryColor: valueToHSL(currenttheme.primaryColor),
			secondaryColor: valueToHSL(currenttheme.secondaryColor),
			tertiaryColor: valueToHSL(currenttheme.tertiaryColor)
		});
		ApplyThemeColors(themeColors);
		setIsEditingMode(false);
	}

	useEffect(() => {
		loadThemes();
	}, [themeColors]);

	function onColorChange(nextColor) {
		const mergedNextColor = {
			...themeColors,
			...nextColor
		};
		setThemeColors(mergedNextColor);
		ApplyThemeColors(mergedNextColor);
		setSelectedTheme({
			...selectedTheme,
			primaryColor: hslToNumbers(mergedNextColor.primaryColor),
			secondaryColor: hslToNumbers(mergedNextColor.secondaryColor),
			tertiaryColor: hslToNumbers(mergedNextColor.tertiaryColor)
		});
	}

	function onSubmit(data) {
		var thisTheme = {
			...data,
			primaryColor: selectedTheme.primaryColor,
			secondaryColor: selectedTheme.secondaryColor,
			tertiaryColor: selectedTheme.tertiaryColor
		};
		modifyTheme(thisTheme);
		setIsEditingMode(false);
	}

	function onAddNewTheme() {
		setIsEditingMode(true);
		setSelectedTheme({
			name: `Custom-${themeList.length + 1}`,
			isApplied: false,
			deleted: false,
			primaryColor: '246,57,21',
			secondaryColor: '150,100,30',
			tertiaryColor: '246,20,41'
		});
	}

	function onEditTheme(thisTheme) {
		setIsEditingMode(true);
		setSelectedTheme(thisTheme);
	}

	function onDeleteTheme(thisTheme) {
		modifyTheme({ id: thisTheme.id, deleted: true });
		setIsEditingMode(false);
	}

	function onChangeApplied(thisTheme) {
		ApplyThemeColors({
			primaryColor: valueToHSL(thisTheme.primaryColor),
			secondaryColor: valueToHSL(thisTheme.secondaryColor),
			tertiaryColor: valueToHSL(thisTheme.tertiaryColor)
		});
		modifyTheme({ id: thisTheme.id, isApplied: true });
	}

	function EditColor(name, thisColor) {
		return (
			<Popover
				content={
					<SketchPicker
						color={{
							h: thisColor.h,
							s: thisColor.s / 100,
							l: thisColor.l / 100
						}}
						onChange={({ hsl }) => {
							onColorChange({
								[name]: HSLtoPercentage(hsl)
							});
						}}
					/>
				}
			>
				<Button type="text">
					<ColorIcon color={thisColor} />
				</Button>
			</Popover>
		);
	}

	function ThemeModifier(thisTheme) {
		var thisColor = {
			primaryColor: valueToHSL(thisTheme.primaryColor),
			secondaryColor: valueToHSL(thisTheme.secondaryColor),
			tertiaryColor: valueToHSL(thisTheme.tertiaryColor)
		};
		return (
			<Form
				name="Custom Theme"
				labelCol={{ span: 4 }}
				wrapperCol={{ span: 8 }}
				layout="horizontal"
				initialValues={thisTheme}
				onFinish={onSubmit}
				autoComplete="off"
			>
				{thisTheme.id !== undefined && <Form.Item name="id" style={{ height: '0px' }}></Form.Item>}

				<Form.Item label="Theme Name" name="name" rules={[{ required: true, message: 'Please enter a Theme Name!' }]}>
					<Input />
				</Form.Item>
				<Form.Item label="Primary" name="primaryColor">
					{EditColor('primaryColor', thisColor.primaryColor)}
				</Form.Item>
				<Form.Item label="Secondary" name="secondaryColor">
					{EditColor('secondaryColor', thisColor.secondaryColor)}
				</Form.Item>
				<Form.Item label="Tertiary" name="tertiaryColor">
					{EditColor('tertiaryColor', thisColor.tertiaryColor)}
				</Form.Item>
				{thisTheme.id === undefined && (
					<Form.Item label="Is Applied" name="isApplied" valuePropName="checked">
						<Switch />
					</Form.Item>
				)}
				<Form.Item wrapperCol={{ offset: 4 }}>
					<Space>
						{thisTheme.id !== undefined && thisTheme.isApplied !== true && (
							<Button
								type="link"
								danger
								onClick={() => onDeleteTheme(thisTheme)}
								icon={<FontAwesomeIcon icon={faTrashAlt} />}
							/>
						)}
						<Button type="primary" htmlType="submit">
							{thisTheme.id !== undefined ? 'Update' : 'Add'}
						</Button>
						<Button
							onClick={() => {
								applyUpdatedTheme();
							}}
						>
							Cancel
						</Button>
					</Space>
				</Form.Item>
			</Form>
		);
	}
	const columns = [
		{
			title: 'Name',
			dataIndex: 'name',
			key: 'name'
		},
		{
			title: 'Primary',
			dataIndex: 'primaryColor',
			key: 'primaryColor',
			render: (color) => {
				return <ColorIcon color={valueToHSL(color)} />;
			}
		},
		{
			title: 'Secondary',
			dataIndex: 'secondaryColor',
			key: 'secondaryColor',
			render: (color) => {
				return <ColorIcon color={valueToHSL(color)} />;
			}
		},
		{
			title: 'Tertiary',
			dataIndex: 'tertiaryColor',
			key: 'tertiaryColor',
			render: (color) => {
				return <ColorIcon color={valueToHSL(color)} />;
			}
		},
		{
			title: 'Applied',
			dataIndex: 'isApplied',
			key: 'isApplied',
			render: (value, record) => (
				<Radio
					checked={value}
					onChange={() => {
						onChangeApplied({
							id: record.id,
							isApplied: true,
							primaryColor: record.primaryColor,
							secondaryColor: record.secondaryColor,
							tertiaryColor: record.tertiaryColor
						});
					}}
				></Radio>
			)
		},
		{
			title: 'Edit',
			dataIndex: 'id',
			key: 'id',
			render: (text, record) => (
				<Button
					className="option-button"
					onClick={() => {
						onEditTheme(record);
					}}
					icon={<FontAwesomeIcon icon={faEdit} />}
				>
					{t('Button.Edit')}
				</Button>
			)
		}
	];

	function ListView() {
		return <Table bordered dataSource={themeList} columns={columns} />;
	}

	return (
		<Card
			title={
				<div style={{ display: !isEditingMode && 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
					{isEditingMode && (
						<Button type="link" onClick={() => setIsEditingMode(false)} icon={<FontAwesomeIcon icon={faArrowLeft} />} />
					)}
					<span>Themes</span>
					{!isEditingMode && <Button onClick={onAddNewTheme}>Add</Button>}
				</div>
			}
			type="inner"
			className="card-container"
			style={{ margin: '15px' }}
		>
			{isEditingMode ? ThemeModifier(selectedTheme) : ListView()}
		</Card>
	);
}

const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {
	return {
		getThemeStyles: () => dispatch(getThemeStyles()),
		saveThemeStyle: (data) => dispatch(saveThemeStyle(data))
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(memo(StyleMode));
