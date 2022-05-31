import React, { useContext, useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';

import { SketchPicker } from 'react-color';
import {
	Space,
	Button,
	Popover,
	Table,
	Card,
	Form,
	Switch,
	Input,
	Radio,
	Col,
	Row,
	Upload,
	Divider,
	Tooltip
} from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { hslToNumbers, HSLtoPercentage, ApplyThemeColors, valueToHSL } from '../../constants';
import { getThemeStyles, saveThemeStyle, addThemeStyle, deleteThemeStyle, copyThemeStyle } from '../actions';
import ColorIcon from './ColorIcon';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faEdit } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import { getBase64, Uint8ToBase64 } from '../../../utilities';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';
import {
	LoadingOutlined,
	PlusOutlined,
	DeleteOutlined,
	CopyOutlined,
	CloudUploadOutlined,
	EditOutlined,
	QuestionCircleOutlined
} from '@ant-design/icons';
import moment from 'moment';

function CustomThemes(props) {
	const { t } = useTranslation();

	const { themeColors, setThemeColors, setThemeLogo } = useContext(LowFrequencyContext);
	const [isEditingMode, setIsEditingMode] = useState(false);
	const [themeList, setThemeList] = useState([]);
	const [selectedTheme, setSelectedTheme] = useState({});
	const [selectedRowKeys, setSelectedRowKeys] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const [imageUrl, setImageUrl] = useState('');
	const [selectedFile, setSelectedFile] = useState({});
	const [logoBytes, setLogoBytes] = useState(null);
	const [themeNameEdit, setThemeNameEdit] = useState(false);

	useEffect(() => {
		loadThemes();
	}, []);

	async function loadThemes() {
		var results = await props.getThemeStyles();
		if (results.length > 0) {
			let alltheme = results.filter((row) => row.deleted === false);
			alltheme.map((row) => {
				row.key = row.id;
				if (row.isApplied) {
					setThemeColors({
						primaryColor: valueToHSL(row.primaryColor),
						secondaryColor: valueToHSL(row.secondaryColor),
						tertiaryColor: valueToHSL(row.tertiaryColor)
					});
					setThemeLogo(row.logoUrl);
				}
			});
			setThemeList(alltheme);
			setSelectedRowKeys([]);
			setImageUrl('');
			setSelectedFile({});
			setLogoBytes(null);
		}
	}

	async function modifyTheme(data) {
		await props.saveThemeStyle(data).then(() => {
			loadThemes();
		});
	}
	async function addTheme(data) {
		await props.addThemeStyle(data).then(() => {
			loadThemes();
		});
	}
	async function copyTheme() {
		await props.copyThemeStyle({ id: selectedRowKeys[0] }).then(() => {
			loadThemes();
		});
	}

	function onColorChange(nextColor) {
		const mergedNextColor = {
			...themeColors,
			...nextColor
		};
		setThemeColors(mergedNextColor);
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
			tertiaryColor: selectedTheme.tertiaryColor,
			logo: logoBytes,
			name: data.name ? data.name : selectedTheme.name
		};
		if (thisTheme.id) {
			console.log(data);
			console.log(thisTheme);
			modifyTheme(thisTheme);
		} else {
			addTheme(thisTheme);
		}
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
		setImageUrl(thisTheme.logoUrl);
	}

	async function onDeleteTheme() {
		let deleteList = [];
		selectedRowKeys.map((row) => {
			deleteList.push({ id: row });
		});
		await props.deleteThemeStyle(deleteList).then(() => {
			loadThemes();
		});
		setIsEditingMode(false);
	}
	async function onDeleteThisTheme(theme) {
		await props.deleteThemeStyle([theme]).then(() => {
			loadThemes();
		});
		setIsEditingMode(false);
	}
	function onChangeApplied(thisTheme) {
		ApplyThemeColors({
			primaryColor: valueToHSL(thisTheme.primaryColor),
			secondaryColor: valueToHSL(thisTheme.secondaryColor),
			tertiaryColor: valueToHSL(thisTheme.tertiaryColor)
		});
		setThemeLogo(thisTheme.logoUrl);
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

	function beforeUpload(file) {
		const isAcceptable = file.type === 'image/png';
		if (!isAcceptable) {
			FeedbackMessage(TYPE._REJECTED, 'You can only upload PNG files!');
		}
		setSelectedFile(file);
		return isAcceptable;
	}

	function handleChange(info) {
		console.log(info);
		if (info.file.status === 'uploading') {
			setIsLoading(true);
			getBase64(info.file.originFileObj, async (imageUrl) => {
				setImageUrl(imageUrl);
				setIsLoading(false);
			});

			if (selectedFile.name) {
				const file = selectedFile;
				let base;

				let fileReader = new FileReader();

				fileReader.onload = async (e) => {
					base = e.target.result;
					var byteString = new Uint8Array(base);
					var fileBytes = Uint8ToBase64(byteString);

					setLogoBytes(fileBytes);
				};

				fileReader.readAsArrayBuffer(file);
			}
			return;
		}
		if (info.file.status === 'done') {
			getBase64(info.file.originFileObj, (imageUrl) => {
				setImageUrl(imageUrl);
				setIsLoading(false);
			});
		}
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
				labelCol={{ span: 6 }}
				wrapperCol={{ span: 10 }}
				layout="horizontal"
				initialValues={thisTheme}
				onFinish={onSubmit}
				autoComplete="off"
			>
				<Card
					title={
						<Row style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
							<Col span={16}>
								<Row style={{ display: 'flex', alignItems: 'center' }}>
									<Col>
										<Button type="link" onClick={() => setIsEditingMode(false)}>
											<FontAwesomeIcon icon={faArrowLeft} />
										</Button>
									</Col>
									{themeNameEdit ? (
										<Col span={20}>
											<Form.Item
												label="Theme Name"
												name="name"
												rules={[{ required: true, message: 'Please enter a Theme Name!' }]}
												labelCol={{ span: 6 }}
												wrapperCol={{ span: 18 }}
												labelAlign="left"
												style={{ marginBottom: 0 }}
											>
												<Input />
											</Form.Item>
										</Col>
									) : (
										<span>Themes Name - {thisTheme.name} </span>
									)}
									<Button
										type="link"
										onClick={() => {
											setThemeNameEdit((prev) => !prev);
										}}
									>
										<EditOutlined />
									</Button>
								</Row>
							</Col>
							<Col>
								<Row style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
									<Col>
										{selectedTheme.id !== undefined && selectedTheme.isApplied !== true && (
											<Button type="link" onClick={() => onDeleteThisTheme(selectedTheme)}>
												<DeleteOutlined />
												Delete
											</Button>
										)}
									</Col>
									<Col>
										<Form.Item style={{ margin: '0' }}>
											<Button type="primary" htmlType="submit">
												{thisTheme.id !== undefined ? 'Save' : 'Add'}
											</Button>
										</Form.Item>
									</Col>
								</Row>
							</Col>
						</Row>
					}
					type="inner"
					className="card-container"
					style={{ margin: '15px', backgroundColor: '#f0f4f7' }}
				>
					{thisTheme.id !== undefined && <Form.Item name="id" hidden></Form.Item>}
					<Row style={{ display: 'flex', alignItems: 'start', justifyContent: 'space-between' }}>
						<Col span={11} style={{ backgroundColor: 'white' }}>
							<Row style={{ margin: '10px' }}>
								<Col>
									<h3>Color</h3>
								</Col>
							</Row>
							<Divider style={{ marginTop: '0' }} />
							<Form.Item
								label={
									<Tooltip title="Navigation bar background, section headings, action items icons">
										Primary
										<QuestionCircleOutlined style={{ marginLeft: '3px' }} />
									</Tooltip>
								}
								name="primaryColor"
							>
								{EditColor('primaryColor', thisColor.primaryColor)}
							</Form.Item>
							<Form.Item
								label={
									<Tooltip title="Feature and action item highlighter, buttons, feature subheads">
										Secondary
										<QuestionCircleOutlined style={{ marginLeft: '3px' }} />
									</Tooltip>
								}
								name="secondaryColor"
							>
								{EditColor('secondaryColor', thisColor.secondaryColor)}
							</Form.Item>
							{/* <Form.Item
								label={
									<Tooltip title="some Texts">
										Tertiary
										<QuestionCircleOutlined style={{ marginLeft: '3px' }} />
									</Tooltip>
								}
								name="tertiaryColor"
							>
								{EditColor('tertiaryColor', thisColor.tertiaryColor)}
							</Form.Item> */}
							{thisTheme.id === undefined && (
								<Form.Item label="Is Applied" name="isApplied" valuePropName="checked">
									<Switch />
								</Form.Item>
							)}
						</Col>
						<Col span={11} style={{ backgroundColor: 'white' }}>
							<Row style={{ margin: '10px', alignItems: 'center' }}>
								<Col>
									<h3>Logo</h3>
								</Col>
							</Row>
							<Divider style={{ marginTop: '0' }} />
							<Row>
								<Col span={12}>
									<Form.Item
										label="File Name"
										name="logoFileName"
										labelCol={{ span: 6, marginLeft: '10px' }}
										wrapperCol={{ span: 18 }}
									>
										<Space>{thisTheme.logoFileName}</Space>
									</Form.Item>
								</Col>
								<Col span={12}>
									<Upload
										name="logo"
										listType="picture-card"
										showUploadList={false}
										beforeUpload={beforeUpload}
										onChange={handleChange}
										className="upload_logo"
										accept="image/png"
									>
										{imageUrl && <img src={imageUrl} alt="avatar" style={{ width: '100%' }} />}
										<div className="hover-box">
											{isLoading ? <LoadingOutlined /> : <CloudUploadOutlined style={{ fontSize: '40px' }} />}
											<div>{t('Button.Upload')}</div>
										</div>
									</Upload>
								</Col>
							</Row>
						</Col>
					</Row>
				</Card>
			</Form>
		);
	}
	const color_columns = [
		{
			title: 'Theme Name',
			dataIndex: 'name',
			key: 'name'
		},
		{
			title: 'Enabled',
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
							tertiaryColor: record.tertiaryColor,
							logoUrl: record.logoUrl
						});
					}}
				></Radio>
			)
		},
		{
			title: 'Created By',
			dataIndex: 'createdByName',
			key: 'createdByName'
		},
		{
			title: 'Created On',
			dataIndex: 'createdDate',
			key: 'createdDate',
			render: (value, record) => moment(record.createdDate).format('ddd MMM DD YYYY hh:mm:ss A')
		},
		{
			title: 'Edit',
			dataIndex: 'id',
			key: 'id',
			render: (text, record) => (
				<Button
					disabled={record.id > 2 ? false : true}
					className="option-button"
					onClick={() => {
						onEditTheme(record);
					}}
				>
					<FontAwesomeIcon icon={faEdit} />
					{t('Button.Edit')}
				</Button>
			)
		}
	];

	const onSelectChange = (selectedRowkeys) => {
		setSelectedRowKeys([...selectedRowkeys]);
	};

	const rowSelection = {
		selectedRowKeys,
		onChange: onSelectChange
	};

	function ColorListView() {
		return (
			<Card
				title={
					<Row style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
						<Col>
							<span>Themes</span>
						</Col>
						<Col>
							<Button type="link" onClick={onAddNewTheme}>
								<PlusOutlined />
								New
							</Button>
							{selectedRowKeys.length === 1 && (
								<Button type="link" onClick={copyTheme}>
									<CopyOutlined />
									Copy
								</Button>
							)}
							{selectedRowKeys.length === 1 && (
								<Button type="link" className="error" onClick={onDeleteTheme}>
									<DeleteOutlined />
									Delete
								</Button>
							)}
						</Col>
					</Row>
				}
				type="inner"
				className="card-container"
				style={{ margin: '15px', backgroundColor: '#f0f4f7' }}
			>
				<Table rowSelection={rowSelection} bordered dataSource={themeList} columns={color_columns} />
			</Card>
		);
	}

	return <>{isEditingMode ? ThemeModifier(selectedTheme) : ColorListView()}</>;
}

const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {
	return {
		getThemeStyles: () => dispatch(getThemeStyles()),
		saveThemeStyle: (data) => dispatch(saveThemeStyle(data)),
		addThemeStyle: (data) => dispatch(addThemeStyle(data)),
		deleteThemeStyle: (data) => dispatch(deleteThemeStyle(data)),
		copyThemeStyle: (data) => dispatch(copyThemeStyle(data))
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(memo(CustomThemes));
