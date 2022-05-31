﻿import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Upload } from 'antd';
import Wopi from '../wopi';

import { useTranslation } from 'react-i18next';
function WopiModal(props) {
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
		fileInfo,
		setUploadUsingReplace
	} = props;

	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'wopi'}
			onCancel={modal().closeModal}
			centered={false}
			width={580}
			footer={false}
			getContainer={false}
			closable={true}
			style={{ top: 10, bottom: 0 }}
			bodyStyle={{ height: 900 }}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Spin spinning={isUpdating} size="large">
				<Form
					form={form}
					key={'wopi'}
					layout="horizontal"
					name="wopi"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<Wopi fileInfo={fileInfo} />
					<Row type="flex" className="form-actions">
						<Col
							xs={24}
							className="form-update-actions"
							style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
						>
							{modalState.type !== 'wopi' &&
								!(
									modalState.type === 'edit-meta-data' ||
									modalState.type === 'edit-details' ||
									modalState.type === 'approval'
								) && (
									<Button type="secondary" onClick={modal().closeModal}>
										{t('Button.Cancel')}
									</Button>
								)}
							{((canEditAccess && !modal().isReadOnly()) || (canArchiveAccess && !modal().isReadOnly())) &&
							modalState.type !== 'approval' ? (
								<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
									{modal().submitText()}
								</Button>
							) : (
								''
							)}

							{(modalState.type === 'edit-meta-data' || modalState.type === 'edit-details') && (
								<Upload
									beforeUpload={() => false}
									onChange={(info) => {
										if (info) {
											setUploadUsingReplace(info.file);
										}
									}}
									showUploadList={false}
								></Upload>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(WopiModal));
