import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Form, Modal, Tree, Spin, Space } from 'antd';
import { AiOutlineFolder } from 'react-icons/ai';
import useTreeMove from '@damhookuseTreeMove';
import { useTranslation } from 'react-i18next';

function MoveAssetModal(props) {
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
        combinedData,
        nestedChild
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
    }

    return (
        <Modal
            title={modal().header()}
            visible={modalState.isVisible && modalState.type === 'move'}
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
                    key={'move'}
                    layout="horizontal"
                    name="move"
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
                        onExpand={expandTreeMoveKeys}
                        expandedKeys={treeMoveState.expandedKeys}
                        autoExpandParent={treeMoveState.autoExpandParent}
                        onCheck={treeMoveCheckKey}
                        checkedKeys={treeMoveState.checkedKeys}
                        onSelect={selectTreeMoveKey}
                        selectedKeys={treeMoveState.selectedKeys}
                        treeData={nestedChild(combinedData, 0)}
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
                                    <Button htmlType="submit" type="primary" disabled={modal().submitDisabled()}>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(MoveAssetModal));
