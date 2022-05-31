import React, { memo, useState } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Modal, Upload, Input, Space } from 'antd';
import { useTranslation } from 'react-i18next';
import { LoadingOutlined, PlusOutlined, CloudUploadOutlined } from '@ant-design/icons';
import { FeedbackMessage, TYPE } from '../../../messageTextConstants';
import { getBase64, Uint8ToBase64 } from '../../../../utilities';
import { addThumbnail, convertPDFPreview } from '../../actions';

function UploadThumbnailModal(props) {
	const { t } = useTranslation();
	const { modal, modalState, findFileState, refresh } = props;
	const [isLoading, setIsLoading] = useState(false);
	const [imageUrl, setImageUrl] = useState('');
	const [imageBytes, setImageBytes] = useState('');
	const [selectedFile, setSelectedFile] = useState([]);

	const beforeUpload = (file) => {
		console.log('beforeUpload');
		const acceptAbleFileTypes = ['image/png', 'image/jpeg', 'image/jpg', 'image/gif', 'application/pdf'];
		const isAcceptable = acceptAbleFileTypes.includes(file.type);
		if (!isAcceptable) {
			FeedbackMessage(TYPE._REJECTED, 'You can only upload JPG/JPEG/PNG/GIF/PDF file!');
		}
		if (file.type === 'application/pdf') {
			//convert pdf to image
			let base;
			let fileReader = new FileReader();
			fileReader.onload = async (e) => {
				base = e.target.result;
				var byteString = new Uint8Array(base);
				var fileBytes = Uint8ToBase64(byteString);
				var pdf = await props.convertPDFPreview({ Thumbnail: fileBytes, Extension: file.name.split('.').pop() });
				setImageUrl('data:image/jpeg;base64,' + pdf.data);
			};
			fileReader.readAsArrayBuffer(file);
		} else {
			setSelectedFile(file);
		}
		return isAcceptable;
	};

	function handleChange(info) {
		console.log('handleChange');

		if (info.file.status === 'uploading') {
			setIsLoading(true);
			if (info.file.type === 'application/pdf') {
				setIsLoading(false);
			} else {
				getBase64(info.file.originFileObj, async (imageUrl) => {
					setImageUrl(imageUrl);
					setIsLoading(false);
				});
			}

			if (selectedFile.name) {
				const file = selectedFile;
				let base;
				let fileReader = new FileReader();

				fileReader.onload = async (e) => {
					base = e.target.result;
					var byteString = new Uint8Array(base);
					var fileBytes = Uint8ToBase64(byteString);

					setImageBytes(fileBytes);
				};

				fileReader.readAsArrayBuffer(file);
			}
			return;
		}
		if (info.file.status === 'done') {
			if (info.file.type === 'application/pdf') {
				setIsLoading(false);
			}
			getBase64(info.file.originFileObj, (imageUrl) => {
				setImageUrl(imageUrl);
				setIsLoading(false);
			});
		}
	}

	const saveThumbnail = () => {
		var data = {
			AssetId: findFileState.id,
			Thumbnail: imageBytes,
			Extension: selectedFile.name.split('.').pop(),
			FileType: selectedFile.type
		};
		props.addThumbnail(data).then((res) => {
			modal().closeModal();
			refresh();
		});
	};

	return (
		<Modal
			title="New Thumbnail"
			visible={modalState.isVisible && modalState.type === 'newThumbnail'}
			onCancel={modal().closeModal}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			{/* <Form
				form={form}
				key={'newThumbnail'}
				layout="horizontal"
				name="newThumbnail"
				//onFinish={}
				className="dam-form"
				labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
				wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
			> */}
			<Row>
				<Col span={6}>
					<Space>Asset Thumbnail</Space>
				</Col>
				<Col span={12}>
					<Input value={selectedFile.name}></Input>
				</Col>
				<Col span={4} style={{ marginLeft: '10px' }}>
					<Upload
						name="thumbnail"
						showUploadList={false}
						beforeUpload={beforeUpload}
						onChange={handleChange}
						accept="image/png,image/jpg,image/jpeg,image/gif,application/pdf"
					>
						<Button icon={<CloudUploadOutlined />}></Button>
					</Upload>
				</Col>
			</Row>
			<Row style={{ marginTop: '10px' }}>
				<Col span={8}>
					<Upload
						name="thumbnail"
						listType="picture-card"
						showUploadList={false}
						beforeUpload={beforeUpload}
						onChange={handleChange}
						className="upload_thumbnail"
						accept="image/png,image/jpg,image/jpeg,image/gif,application/pdf"
					>
						{imageUrl && <img src={imageUrl} alt="avatar" style={{ width: '100%' }} />}
						<div className="hover-box">
							{isLoading ? <LoadingOutlined /> : <PlusOutlined />}
							<div>{t('Button.Upload')}</div>
						</div>
					</Upload>
				</Col>
			</Row>
			<Row type="flex" className="form-actions">
				<Col xs={24} className="form-update-actions" style={{ textAlign: 'right' }}>
					<Button type="secondary" onClick={modal().closeModal}>
						{t('Button.Cancel')}
					</Button>
					<Button type="primary" onClick={saveThumbnail}>
						Save
					</Button>
				</Col>
			</Row>
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		addThumbnail: (data) => dispatch(addThumbnail(data)),
		convertPDFPreview: (data) => dispatch(convertPDFPreview(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(UploadThumbnailModal));
