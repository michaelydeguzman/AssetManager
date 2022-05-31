import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin } from 'antd';
import { useTranslation } from 'react-i18next';

function VideoModal(props) {
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
		setTouchMoveFolder,
		mFolders,
		nestedChild
	} = props;

	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'video'}
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
					key={'video'}
					layout="horizontal"
					name="video"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<video width="800" height="600" controls controlsList="nodownload">
						<source src={videoPreview} type="video/mp4"></source>
						<source src={videoPreview} type="video/webm"></source>
						<source src={videoPreview} type="video/ogg"></source>
					</video>

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
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(VideoModal));
