import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Input, Select, Button, Form, Modal, Spin, Space } from 'antd';
import { useTranslation } from 'react-i18next';

function AddFolderModal(props) {
	const { t } = useTranslation();
	const {
		modal,
		modalState,
		canAddAccess,
		canEditAccess,
		canArchiveAccess,
		isUpdating,
		form,
		onSubmit,
		assetsSliderPrev,
		assetsSliderNext,
		selectOptionsType,
		accounts,
		countries,
		isFromCollection
	} = props;
	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'add'}
			onCancel={modal().closeModal}
			centered
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard={true}
		>
			<Spin spinning={isUpdating} size="large">
				<Form
					form={form}
					key={'add'}
					layout="horizontal"
					name="add"
					onFinish={onSubmit}
					// hideRequiredMark
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					// onFinishFailed={onFinishFailed}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{
						xs: 24,
						sm: 16,
						md: 16,
						lg: 16,
						xl: 16,
						xxl: 16,
						span: 16,
						style: { ...{ lineHeight: 2.2 } }
					}}
				>
					<Col>
						<Form.Item
							name="folder_name"
							label={t('ModalDetail.Folder Name')}
							rules={[
								{
									required: true,
									message: t('Messages.Please input folder name')
								}
							]}
						>
							<Input width="auto" autoFocus onChange={modal().setTitle} />
						</Form.Item>

						<Form.Item name="accounts" label={t('ModalDetail.Account')}>
							<Select
								mode="multiple"
								placeholder={t('Messages.Select')}
								onChange={modal().selectAccount}
								dropdownStyle={{ position: 'fixed' }}
							>
								{selectOptionsType(accounts)}
							</Select>
						</Form.Item>

						<Form.Item name="country" label={t('ModalDetail.Country')}>
							<Select
								mode="multiple"
								placeholder={t('Messages.Select')}
								onChange={modal().selectCountry}
								dropdownStyle={{ position: 'fixed' }}
							>
								{selectOptionsType(countries)}
							</Select>
						</Form.Item>

						{/* Temporarily remove it */}
						{/* <Form.Item name="region" label={t('ModalDetail.Region')}>
							<Select
								mode="multiple"
								placeholder={t('Messages.Select')}
								onChange={modal().selectRegion}
								dropdownStyle={{ position: 'fixed' }}
							>
								{selectGroupOptionsType(regionOptions.sort(compare))}
							</Select>
						</Form.Item> */}

						<Form.Item label={t('ModalDetail.Comments')} name="comments">
							<Input.TextArea placeholder={t('Messages.Comments here')} onChange={modal().setComments} />
						</Form.Item>

						<Row type="flex" className="form-actions">
							<Col
								xs={24}
								className="form-update-actions"
								style={{ textAlign: modal().submitText() !== 'Update' ? 'center' : 'right' }}
							>
								<Space>
									<Button type="secondary" onClick={modal().closeModal}>
										{t('Button.Cancel')}
									</Button>
									{((canAddAccess && canEditAccess && (!modal().isReadOnly() || isFromCollection)) ||
										(canArchiveAccess && !modal().isReadOnly())) && (
										<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
											{modal().submitText()}
										</Button>
									)}
								</Space>
							</Col>
						</Row>
					</Col>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(AddFolderModal));
