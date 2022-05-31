import React, { memo, useState } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Space, message } from 'antd';
import { AiOutlineFolder } from 'react-icons/ai';
import useTreeMove from '@damhookuseTreeMove';
import { useTranslation } from 'react-i18next';
import { PlusOutlined } from '@ant-design/icons';
function ExportCollectionModal(props) {
	const { t } = useTranslation();
	const [selectedKeys, setSelectedKeys] = useState([]);
	const {
		modal,
		modalState,
		isUpdating,
		form,
		onSubmit,
		assetsSliderPrev,
		assetsSliderNext,
		setTouchMoveFolder,
		combinedData,
		nestedChild,
		touchMoveFolder
	} = props;
	const [treeMoveState, treeMoveDispatch] = useTreeMove();

	function expandTreeMoveKeys(expandedKeys) {
		treeMoveDispatch({ type: 'EXPANDED_KEYS', payload: expandedKeys });
		treeMoveDispatch({ type: 'AUTO_EXPAND_PARENT', payload: false });
	}

	function treeMoveCheckKey(checkedKeys) {
		treeMoveDispatch({ type: 'CHECKED_KEYS', payload: checkedKeys });
	}

	function selectTreeMoveKey(selectedKeys, info) {
		treeMoveDispatch({ type: 'SELECTED_KEYS', payload: selectedKeys });
		setTouchMoveFolder(info.node.id);

		setSelectedKeys(selectedKeys);
	}

	function renderLoadingText(isLoading) {
		if (isLoading) return t('Messages.Exporting');
	}

	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'export-collection'}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			className={`${modalState.type}-modal`}
			closable={isUpdating ? false : true}
			keyboard
			onCancel={modal().closeModal}
			maskClosable={false}
		>
			<Spin spinning={isUpdating} size="large" tip={renderLoadingText(isUpdating)}>
				<Form
					form={form}
					key={'export'}
					layout="horizontal"
					name="export"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<Row>
						<Col xs={24} sm={24} md={14} lg={14} xl={14} xxl={14}>
							<h4>{t('Messages.ExportToFolderSub')}</h4>
						</Col>
						{touchMoveFolder && (
							<Col xs={24} sm={24} md={10} lg={10} xl={10} xxl={10} align="right">
								<Button onClick={modal().addFolder}>
									{' '}
									<PlusOutlined /> {t('Button.Create Folder')}{' '}
								</Button>
							</Col>
						)}
					</Row>

					<Row style={{ marginTop: 16 }}>
						<Tree.DirectoryTree
							blockNode
							showIcon
							icon={() => <AiOutlineFolder />}
							onExpand={expandTreeMoveKeys}
							//expandedKeys={treeMoveState.expandedKeys}
							autoExpandParent={false}
							onCheck={treeMoveCheckKey}
							checkedKeys={treeMoveState.checkedKeys}
							onSelect={selectTreeMoveKey}
							selectedKeys={treeMoveState.selectedKeys}
							treeData={nestedChild(combinedData, 0)}
							defaultExpandAll={true}
							defaultSelectedKeys={selectedKeys}
						/>
					</Row>

					<Row type="flex" className="form-actions" style={{ marginTop: 16 }}>
						<Col
							xs={24}
							className="form-update-actions"
							style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
						>
							<Space>
								<Button type="secondary" onClick={modal().closeModal}>
									{t('Button.Cancel')}
								</Button>

								<Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
									{t('Button.Save')}
								</Button>
							</Space>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(ExportCollectionModal));
