import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Card, Input, Button, Form, Upload, Tabs, Radio } from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { getBase64, Uint8ToBase64 } from '../../../../../utilities/index';
import { saveDefaultWatermark, getDefaultWatermark } from '../../../actions';
import { LoadingOutlined, PlusOutlined } from '@ant-design/icons';
import { FeedbackMessage, TYPE } from '../../../../messageTextConstants';
import { useTranslation } from 'react-i18next';
import { SAMPLE_IMG } from '../../../../constants';

function GeneralSettings(props) {
	const { t } = useTranslation();
	const { currentUser, setCurrentUser } = useContext(LowFrequencyContext);

	const [imageUrl, setImgUrl] = useState('');
	const [selectedFile, setSelectedFile] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const { TabPane } = Tabs;

	const [activeTab, setActiveTab] = useState('1');

	const [wmWidth, setwmWidth] = useState(50);
	const [wmOpacity, setwmOpacity] = useState(0.5);
	const [wmPosition, setwmPosition] = useState(3);
	const [wmFileBytes, setwmFileBytes] = useState(null);
	const [wmId, setwmId] = useState(null);

	const getPreviewPaddingTop = () => {
		if (wmPosition === 3) {
			return '50%';
		} else if (wmPosition === 1 || wmPosition === 2) {
			return '10px';
		}
	};

	const getWMPosition = () => {
		if (wmPosition === 3 || wmPosition === 4 || wmPosition === 5) {
			return 'absolute';
		}
	};

	const getWMTop = () => {
		if (wmPosition === 3) {
			return '50%';
		}
	};

	const getWMLeft = () => {
		if (wmPosition === 3) {
			return '50%';
		}
	};

	const getWMBottom = () => {
		if (wmPosition === 4 || wmPosition === 5) {
			return '0';
		}
	};

	const getWMRight = () => {
		if (wmPosition === 5) {
			return '0';
		}
	};

	const getWMTransform = () => {
		if (wmPosition === 3) {
			return 'translate(-50%, -50%)';
		}
	};

	const getPreviewTextAlign = () => {
		if (wmPosition === 3) {
			return 'center';
		} else if (wmPosition === 1 || wmPosition === 4) {
			return 'left';
		} else if (wmPosition === 2 || wmPosition === 5) {
			return 'right';
		}
	};

	function beforeUpload(file) {
		const isPng = file.type === 'image/png';
		if (!isPng) {
			FeedbackMessage(TYPE._REJECTED, 'You can only upload PNG files!');
		}
		setSelectedFile(file);
		return isPng;
	}

	function handleChange(info) {
		if (info.file.status === 'uploading') {
			setIsLoading(true);
			getBase64(info.file.originFileObj, async (imageUrl) => {
				setImgUrl(imageUrl);
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

					setwmFileBytes(fileBytes);
				};

				fileReader.readAsArrayBuffer(file);
			}
			return;
		}
		if (info.file.status === 'done') {
			getBase64(info.file.originFileObj, (imageUrl) => {
				setImgUrl(imageUrl);
				setIsLoading(false);
			});
		}
	}

	function onError(e) {
		e.target.src = imageUrl;
	}

	const saveWatermark = async () => {
		var watermark = {
			id: wmId,
			watermark: wmFileBytes,
			opacity: wmOpacity,
			size: wmWidth,
			watermarkPosition: wmPosition
		};
		await props.saveDefaultWatermark(watermark);
	};

	const getWatermark = async () => {
		var result = await props.getDefaultWatermark();

		if (result && result.data.watermark) {
			var wm = result.data.watermark;

			if (wm.id) {
				setwmId(wm.id);
				setImgUrl(wm.watermarkUrl);
				setwmOpacity(wm.opacity);
				setwmPosition(wm.watermarkPosition);
				setwmWidth(wm.size);
			}
		}
	};

	useEffect(() => {
		getWatermark();
	}, []);

	return (
		<Card title={t('ModalDetail.General Settings')} type="inner" className="card-container" style={{ margin: '15px' }}>
			<Tabs activeKey={activeTab} onChange={(e) => setActiveTab(e)} className="general-settings">
				<TabPane tab={t('Settings.Watermark')} key="1">
					{activeTab === '1' && (
						<>
							<Row>
								<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12}>
									<Row align="left" style={{ marginTop: '5px' }}>
										<h4>{t('Messages.WMHeader')}</h4>
									</Row>
								</Col>
								<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="right">
									<Button type="primary" htmlType="submit" onClick={saveWatermark} disabled={!imageUrl}>
										{t('Button.Save Watermark')}
									</Button>
								</Col>
							</Row>
							<br />
							<Row>
								<Col xs={24} sm={6} md={6} lg={6} xl={6} xxl={6} align="left">
									<Row>
										<h3>{t('Settings.DefaultWM')}</h3>
									</Row>
									<Row style={{ marginTop: 16 }}>
										<Upload
											name="avatar"
											listType="picture-card"
											showUploadList={false}
											beforeUpload={beforeUpload}
											onChange={handleChange}
											className="watermark"
											accept="image/png"
										>
											{imageUrl && <img src={imageUrl} onError={onError} alt="avatar" />}
											<div className="hover-box">
												{isLoading ? <LoadingOutlined /> : <PlusOutlined />}
												<div>{t('Button.Upload')}</div>
											</div>
										</Upload>
									</Row>
								</Col>
								{imageUrl && (
									<>
										<Col xs={24} sm={1} md={1} lg={1} xl={1} xxl={1} align="left"></Col>
										<Col xs={24} sm={5} md={5} lg={5} xl={5} xxl={5} align="left">
											<Row>
												<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
													<h3>{t('Settings.WMProps')}</h3>
												</Col>
											</Row>
											<br />
											<Row style={{ marginTop: 40 }}>
												<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12}>
													<h4>{t('Settings.WMSize')}</h4>
												</Col>
												<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="right">
													<Input
														type="number"
														value={wmWidth}
														onChange={(e) => setwmWidth(e.target.value)}
														max={100}
														min={0}
													/>
												</Col>
											</Row>
											<br />

											<Row>
												<h4>{t('Settings.WMPosition')}</h4>
											</Row>
											<br />

											<Row>
												<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
													<Radio.Group
														onChange={(e) => setwmPosition(e.target.value)}
														value={wmPosition}
														style={{ width: '100%' }}
													>
														<Row>
															<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="left">
																<Radio value={1}>{t('Settings.Top Left')}</Radio>
															</Col>
															<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="right">
																<Radio value={2}>{t('Settings.Top Right')}</Radio>
															</Col>
														</Row>
														<br />
														<Row>
															<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24} align="center">
																<Radio value={3}>{t('Settings.Center')}</Radio>
															</Col>
														</Row>
														<br />
														<Row>
															<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="left">
																<Radio value={4}>{t('Settings.Bottom Left')}</Radio>
															</Col>
															<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="right">
																<Radio value={5}>{t('Settings.Bottom Right')}</Radio>
															</Col>
														</Row>
													</Radio.Group>
												</Col>
											</Row>
											<br />
											<br />
											<Row>
												<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12}>
													<h4>{t('Settings.WMOpacity')}</h4>
												</Col>

												<Col xs={12} sm={12} md={12} lg={12} xl={12} xxl={12} align="right">
													<Input
														type="number"
														value={wmOpacity}
														onChange={(e) => setwmOpacity(e.target.value)}
														max={1}
														min={0}
														step={0.1}
													/>
												</Col>
											</Row>
											<br />
										</Col>
										<Col xs={24} sm={1} md={1} lg={1} xl={1} xxl={1} align="left"></Col>
										<Col xs={24} sm={11} md={11} lg={11} xl={11} xxl={11} align="left">
											<Row>
												<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24} align="center">
													<h3>{t('Settings.WMPreview')}</h3>
												</Col>
											</Row>
											<Row style={{ marginTop: 16 }}>
												<Col xs={24} sm={24} md={24} lg={24} xl={24} xxl={24}>
													<div
														style={{
															backgroundImage: `url(${SAMPLE_IMG})`,
															backgroundRepeat: 'no-repeat',
															backgroundSize: 'cover',
															width: '100%',
															height: '40vh',
															paddingTop: getPreviewPaddingTop(),
															textAlign: getPreviewTextAlign(),
															paddingLeft: '10px',
															paddingRight: '10px',
															paddingBottom: '10px',
															position: 'relative',
															overflow: 'hidden'
														}}
													>
														<img
															src={imageUrl}
															style={{
																opacity: wmOpacity,
																width: wmWidth + '%',
																position: getWMPosition(),
																top: getWMTop(),
																left: getWMLeft(),
																bottom: getWMBottom(),
																right: getWMRight(),
																transform: getWMTransform()
															}}
														/>
													</div>
												</Col>
											</Row>
										</Col>
									</>
								)}
							</Row>
						</>
					)}
				</TabPane>
			</Tabs>
		</Card>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getDefaultWatermark: () => dispatch(getDefaultWatermark()),
		saveDefaultWatermark: (data) => dispatch(saveDefaultWatermark(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(GeneralSettings));
