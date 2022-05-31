import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Space } from 'antd';
import { AiOutlineFolder } from 'react-icons/ai';
import useTreeMoveFolder from '@damhookuseTreeMoveFolder';
import { useTranslation } from 'react-i18next';

function CopyFolderModal(props) {
	const { t } = useTranslation();
	const {
		modal,
		modalState,
		canEditAccess,
		canArchiveAccess,
		isUpdating,
		onSubmit,
		assetsSliderPrev,
		assetsSliderNext,
		setTouchMoveFolder,
		mFolders,
		nestedChild
	} = props;

	const [treeMoveFolderState, treeMoveFolderDispatch] = useTreeMoveFolder();

	function expandTreeMoveFolderKeys(expandedKeys) {
		treeMoveFolderDispatch({ type: 'EXPANDED_KEYS', payload: expandedKeys });
		treeMoveFolderDispatch({ type: 'AUTO_EXPAND_PARENT', payload: false });
	}

	function treeMoveCheckFolderKey(checkedKeys) {
		treeMoveFolderDispatch({ type: 'CHECKED_KEYS', payload: checkedKeys });
	}

	function selectTreeMoveFolderKey(selectedKeys, info) {
		treeMoveFolderDispatch({ type: 'SELECTED_KEYS', payload: selectedKeys });
		setTouchMoveFolder(info.node.id);
	}

	function renderLoadingText(isLoading) {
		if (isLoading) return t('ModalDetail.Copying');
	}

	return (
		<Modal
			title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'copy-folder'}
			onCancel={modal().closeModal}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Spin spinning={isUpdating} size="large" tip={renderLoadingText(isUpdating)}>
				<Form
					key={'copy-folder'}
					layout="horizontal"
					name="copy-folder"
					onFinish={onSubmit}
					scrollToFirstError
					className={`dam-form ${assetsSliderPrev ? 'fade-left' : ''} ${assetsSliderNext ? 'fade-right' : ''}`}
					labelCol={{ xs: 24, sm: 8, md: 8, lg: 8, xl: 8, xxl: 8, style: { ...{ lineHeight: 2.2 } } }}
					wrapperCol={{ xs: 24, sm: 16, md: 16, lg: 16, xl: 16, xxl: 16, span: 16, style: { ...{ lineHeight: 2.2 } } }}
				>
					<Tree.DirectoryTree
						blockNode
						showIcon
						icon={() => <AiOutlineFolder />}
						checkable={modalState.type === 'edit-only'}
						onExpand={expandTreeMoveFolderKeys}
						expandedKeys={treeMoveFolderState.expandedFolderKeys}
						autoExpandParent={treeMoveFolderState.autoExpandParentFolder}
						onCheck={treeMoveCheckFolderKey}
						checkedKeys={treeMoveFolderState.checkedFolderKeys}
						onSelect={selectTreeMoveFolderKey}
						selectedKeys={treeMoveFolderState.selectedFolderKeys}
						treeData={nestedChild(mFolders, 0)}
					/>

					<Row type="flex" className="form-actions">
						<Col
							xs={24}
							className="form-update-actions"
							style={{ textAlign: modal().submitText() != 'Update' ? 'center' : 'right' }}
						>
							<Space>
								<Button type="secondary" onClick={modal().closeModal}>
									{t('Button.Cancel')}
								</Button>
								{(canEditAccess && !modal().isReadOnly()) || (canArchiveAccess && !modal().isReadOnly()) ? (
									<Button htmlType="submit" type="primary" disabled={modal().submitDisabled() || isUpdating}>
										{modal().submitText()}
									</Button>
								) : (
									''
										)}
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(CopyFolderModal));
