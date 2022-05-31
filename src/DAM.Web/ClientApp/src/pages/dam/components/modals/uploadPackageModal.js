import React, { memo, useState } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Modal, Upload, Input, Space } from 'antd';
import { useTranslation } from 'react-i18next';
import { LoadingOutlined, PlusOutlined, CloudUploadOutlined } from '@ant-design/icons';
import { FeedbackMessage, TYPE } from '../../../messageTextConstants';
import { getBase64, Uint8ToBase64 } from '../../../../utilities';
import { addPackage } from '../../actions';
import fileArchive from '../../../../assets/images/file-archive-solid.svg';

function UploadPackageModal(props) {
	const { t } = useTranslation();
	const { modal, modalState, findFileState, refresh } = props;
	const [isLoading, setIsLoading] = useState(false);
	const [isUploaded, setIsUploaded] = useState(false);
	const [packageFileBytes, setPackageFileBytes] = useState('');
	const [selectedFile, setSelectedFile] = useState([]);

	const beforeUpload = (file) => {
		console.log(file);
		const acceptAbleFileTypes = ['application/x-zip-compressed'];
		const isAcceptable = acceptAbleFileTypes.includes(file.type);
		if (!isAcceptable) {
			FeedbackMessage(TYPE._REJECTED, 'You can only upload ZIP file!');
		}
		setSelectedFile(file);
		return isAcceptable;
	};

	function handleChange(info) {
		if (info.file.status === 'uploading') {
			setIsLoading(true);
			getBase64(info.file.originFileObj, async () => {
				setIsUploaded(true);
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

					setPackageFileBytes(fileBytes);
				};

				fileReader.readAsArrayBuffer(file);
			}
			return;
		}
		if (info.file.status === 'done') {
			getBase64(info.file.originFileObj, () => {
				setIsUploaded(true);
				setIsLoading(false);
			});
		}
	}

	const savePackage = () => {
		var data = {
			AssetId: findFileState.id,
			Package: packageFileBytes,
			Extension: selectedFile.name.split('.').pop(),
			FileType: selectedFile.type,
			PackageName: selectedFile.name.split('.').shift()
		};
		props.addPackage(data).then((res) => {
			modal().closeModal();
			//refresh();
		});
	};

	return (
		<Modal
			title="New Package"
			visible={modalState.isVisible && modalState.type === 'newPackage'}
			onCancel={modal().closeModal}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Row>
				<Col span={6}>
					<Space>Asset Package</Space>
				</Col>
				<Col span={12}>
					<Input value={selectedFile.name}></Input>
				</Col>
				<Col span={4} style={{ marginLeft: '10px' }}>
					<Upload
						name="package"
						showUploadList={false}
						beforeUpload={beforeUpload}
						onChange={handleChange}
						accept="application/x-zip-compressed"
					>
						<Button icon={<CloudUploadOutlined />}></Button>
					</Upload>
				</Col>
			</Row>
			<Row style={{ marginTop: '10px' }}>
				<Col span={8}>
					<Upload
						name="package"
						listType="picture-card"
						showUploadList={false}
						beforeUpload={beforeUpload}
						onChange={handleChange}
						className="upload_thumbnail"
						accept="application/x-zip-compressed"
					>
						{isUploaded && <img src={fileArchive} alt="avatar" style={{ width: '100%' }} />}
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
					<Button type="primary" onClick={savePackage}>
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
		addPackage: (data) => dispatch(addPackage(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(UploadPackageModal));
