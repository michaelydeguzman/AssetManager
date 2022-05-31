import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Modal, Spin } from 'antd';
import Approval from '../approval';

function ApprovalModal(props) {
	const {
		modal,
		modalState,
		modalDispatch,
		isUpdating,
		isApprovalFromModal,
		folders,
		checkedAssetsItem,
		findFileState
	} = props;

	return (
		<Modal
			//title={modal().header()}
			visible={modalState.isVisible && modalState.type === 'approval'}
			onCancel={modal().closeModal}
			centered={true}
			width={'85%'}
			footer={false}
			getContainer={false}
			closable={false}
			className={`${modalState.type}-modal`}
			keyboard
		>
			<Spin spinning={isUpdating} size="large">
				<Approval
					modalDispatch={modalDispatch}
					isApprovalFromModal={isApprovalFromModal}
					close={modal().closeModal}
					folders={folders}
					checkedAssetsItem={checkedAssetsItem}
					findFileState={findFileState}
				/>
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

export default connect(mapStateToProps, mapDispatchToProps)(memo(ApprovalModal));
