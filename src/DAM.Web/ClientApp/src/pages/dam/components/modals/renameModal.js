import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Input, Carousel, Space } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
function RenameModal(props) {
	const { t } = useTranslation();
	const {
		modal,
		modalState,
		canEditAccess,
		canArchiveAccess,
		isUpdating,
		form,
		onSubmit,
		assetsSliderPrev,
		assetsSliderNext,
		checkedAssets,
		assetsSlider,
		onCurrentAssetsId,
		setAssetSliderControls,
		findFile,
		selectedAssetsId,
		efiles
	} = props;

	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'rename'}
			onCancel={modal().closeModal}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Spin spinning={isUpdating} size="large">
				<Form
					form={form}
					key={'rename'}
					layout="horizontal"
					name="rename"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<Row type="flex" justify="space-between" align="middle" className="toolmenu-rename-popup" gutter={10} wrap>
						<Col span={12}>
							{findFile && <img alt="simple file" loading="lazy" src={findFile.thumbnail} width={'100%'} />}
						</Col>
						<Col span={12}>
							<label style={{ textAlign: 'left' }}>Display Name:</label>
							<Form.Item
								name="name"
								onChange={modal().renameFile}
								rules={[
									{
										required: true,
										message: t('Messages.Please input file display name')
									}
								]}
							>
								<Input width="auto" autoFocus readOnly={modal().isReadOnly()} />
							</Form.Item>
						</Col>
					</Row>

					<Row type="flex" className="form-actions">
						<Col
							xs={24}
							className="form-update-actions"
							style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
						>
							<Button type="secondary" onClick={modal().closeModal}>
								{t('Button.Cancel')}
							</Button>

							{(canEditAccess && !modal().isReadOnly()) || (canArchiveAccess && !modal().isReadOnly()) ? (
								<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
									{modal().submitText()}
								</Button>
							) : (
								''
							)}
						</Col>
					</Row>
				</Form>
			</Spin>

			{checkedAssets.length !== 1 && (
				<Carousel ref={assetsSlider} dots={false} effect="fade">
					{checkedAssets.map((row, key) => {
						// .sort((acc, cur) => acc - cur, 0)
						return (
							<Row
								key={row + key}
								type="flex"
								justify="space-between"
								align="middle"
								className="assets-carousel-controls"
							>
								<Col>
									<Space size="large">
										<LeftOutlined
											onClick={(e) => {
												onCurrentAssetsId(e, row);

												assetsSlider.current.prev();
												setAssetSliderControls([true, false]);
												setTimeout(() => {
													setAssetSliderControls([false, false]);
												}, 500);
											}}
										/>
										<span>{key + 1}</span>
										<RightOutlined
											onClick={(e) => {
												onCurrentAssetsId(e, row);

												assetsSlider.current.next();
												setAssetSliderControls([false, true]);
												setTimeout(() => {
													setAssetSliderControls([false, false]);
												}, 500);
											}}
										/>
									</Space>
								</Col>
							</Row>
						);
					})}
				</Carousel>
			)}
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(RenameModal));
