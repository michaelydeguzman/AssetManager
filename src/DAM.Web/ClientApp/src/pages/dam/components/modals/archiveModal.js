import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Spin } from 'antd';
import { getUserRole } from '@damtoken';
import { useTranslation } from 'react-i18next';

function ArchiveModal(props) {
	const { t } = useTranslation();
	const { modal, modalState, isUpdating, form, onSubmit, assetsSliderPrev, assetsSliderNext } = props;
	const userRole = getUserRole();
	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'archive'}
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
					key={'archive'}
					layout="horizontal"
					name="archive"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<p>
						<span style={{ color: '#5F41D2' }}>{t('Archive Message.Question')}</span>
						<br />
						<br />
						<span style={{ color: '#000' }}>{t('Archive Message.preBody')}</span>
						<br />
						<br />
						<span style={{ color: '#000' }}>{t('Archive Message.Body')}</span>
					</p>

					<Row type="flex" className="form-actions">
						<Col
							xs={24}
							className="form-update-actions"
							style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
						>
							<Button type="secondary" onClick={modal().closeModal}>
								{t('Button.Cancel')}
							</Button>
							{(userRole.canEdit && !modal().isReadOnly()) || (userRole.canArchive && !modal().isReadOnly()) ? (
								<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
									{modal().submitText()}
								</Button>
							) : (
								<></>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(ArchiveModal));
