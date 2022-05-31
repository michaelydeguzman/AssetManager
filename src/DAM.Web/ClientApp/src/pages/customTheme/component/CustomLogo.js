import React, { useContext, useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';

import { Space, Button, Table, Card, Form, Switch, Input, Radio, Upload, Popconfirm } from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { getLogos, saveLogo } from '../actions';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import { LoadingOutlined, PlusOutlined } from '@ant-design/icons';
import { getBase64, Uint8ToBase64 } from '../../../utilities';
import { FeedbackMessage, TYPE } from '../../messageTextConstants';

function CustomLogo(props) {
	const { t } = useTranslation();

	const { setThemeLogo } = useContext(LowFrequencyContext);
	const [isEditingMode, setIsEditingMode] = useState(false);
	const [logoList, setLogoList] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const [logoBytes, setLogoBytes] = useState(null);
	const [imageUrl, setImageUrl] = useState('');
	const [selectedLogo, setSelectedLogo] = useState({});
	const [selectedFile, setSelectedFile] = useState({});

	async function loadLogos() {
		var results = await props.getLogos().then((res) => {
			if (res.length > 0) {
				let alltheme = res.filter((row) => row.isDeleted === false);
				setLogoList(alltheme);
				applyUpdatedLogo(alltheme.find((row) => row.isApplied === true));
			}
		});
	}

	async function modifyLogo(data) {
		await props.saveLogo(data).then(() => {
			loadLogos();
		});
	}

	useEffect(() => {
		loadLogos();
	}, []);

	function applyUpdatedLogo(thisLogo = undefined) {
		let currentLogo = thisLogo || logoList.filter((row) => row.isApplied === true)[0];

		setThemeLogo(currentLogo.logoUrl);
	}

	function onSubmit(data) {
		var thisLogo = { ...data, logo: logoBytes };
		console.log('onSubmit', thisLogo);

		modifyLogo(thisLogo);
		clearLogo();
	}

	function onAddNewLogo() {
		setIsEditingMode(true);
		setSelectedLogo({
			fileName: `Logo-${logoList.length + 1}`,
			isApplied: false,
			logo: null
		});
	}

	function clearLogo() {
		setLogoBytes(null);
		setImageUrl('');
		setIsEditingMode(false);
	}

	function onDeleteTheme(id) {
		modifyLogo({ id: id, isDeleted: true });
	}

	function onChangeApplied(record) {
		modifyLogo({ id: record.id, isApplied: true });
		applyUpdatedLogo(record);
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
		console.log('handleChange', info);
		if (info.file.status === 'uploading') {
			setIsLoading(true);
			getBase64(info.file.originFileObj, async (imageUrl) => {
				console.log('handleChange', 'async');
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
		// if (info.file.status === 'done') {
		// 	getBase64(info.file.originFileObj, (imageUrl) => {
		// 		console.log('handleChange', 'status done');

		// 		setImageUrl(imageUrl);
		// 		setIsLoading(false);
		// 	});
		// }
	}

	function LogoModifier(thisTheme) {
		return (
			<Form
				name="Custom Logo"
				labelCol={{ span: 4 }}
				wrapperCol={{ span: 8 }}
				layout="horizontal"
				initialValues={thisTheme}
				onFinish={onSubmit}
				autoComplete="off"
			>
				<Form.Item
					label="Theme Name"
					name="fileName"
					rules={[{ required: true, message: 'Please enter a Theme Name!' }]}
				>
					<Input />
				</Form.Item>

				<Form.Item name="logo" valuePropName="fileList">
					<Upload
						name="logo"
						listType="picture-card"
						showUploadList={false}
						beforeUpload={beforeUpload}
						onChange={handleChange}
						className="watermark"
						accept="image/png"
					>
						{imageUrl && <img src={imageUrl} alt="avatar" />}
						<div className="hover-box">
							{isLoading ? <LoadingOutlined /> : <PlusOutlined />}
							<div>{t('Button.Upload')}</div>
						</div>
					</Upload>
				</Form.Item>

				<Form.Item label="Is Applied" name="isApplied" valuePropName="checked">
					<Switch />
				</Form.Item>

				<Form.Item wrapperCol={{ offset: 4 }}>
					<Space>
						<Button type="primary" htmlType="submit">
							{'Add'}
						</Button>
						<Button
							onClick={() => {
								clearLogo();
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
			title: 'File Name',
			dataIndex: 'fileName',
			key: 'fileName'
		},
		{
			title: 'Logo',
			dataIndex: 'logoUrl',
			key: 'logoUrl',
			render: (value) => <img src={value} style={{ width: '10rem' }} loading="lazy" alt="logo" />
		},
		{
			title: 'Applied',
			dataIndex: 'isApplied',
			key: 'isApplied',
			render: (value, record) => (
				<Radio
					checked={value}
					onChange={() => {
						onChangeApplied(record);
					}}
				></Radio>
			)
		},
		{
			title: 'Delete',
			dataIndex: 'id',
			key: 'id',
			render: (text, record) =>
				!record.isApplied && (
					<Popconfirm
						title="Are you sure to delete this logo?"
						onConfirm={() => {
							onDeleteTheme(record.id);
						}}
						okButtonProps={{ type: 'danger' }}
						okText="Yes"
						cancelText="No"
					>
						<Button type="link" danger>
							<FontAwesomeIcon icon={faTrashAlt} />
						</Button>
					</Popconfirm>
				)
		}
	];

	function ListView() {
		return <Table bordered dataSource={logoList} columns={columns} />;
	}

	return (
		<Card
			title={
				<div style={{ display: !isEditingMode && 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
					{isEditingMode && (
						<Button type="link" onClick={() => clearLogo()} icon={<FontAwesomeIcon icon={faArrowLeft} />} />
					)}
					<span></span>
					{!isEditingMode && <Button onClick={onAddNewLogo}>Add</Button>}
				</div>
			}
			type="inner"
			className="card-container"
			style={{ margin: '15px' }}
		>
			{isEditingMode ? LogoModifier(selectedLogo) : ListView()}
		</Card>
	);
}

const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {
	return {
		getLogos: () => dispatch(getLogos()),
		saveLogo: (data) => dispatch(saveLogo(data))
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(memo(CustomLogo));
