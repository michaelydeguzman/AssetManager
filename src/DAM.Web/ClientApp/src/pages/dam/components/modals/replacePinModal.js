import React, { useState, useEffect, memo } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Modal, Tree } from 'antd';
import { replacePinAssets } from '../../actions';
import { ExclamationCircleOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

function ReplacePinModal(props) {
	const { t } = useTranslation();
	const { showReplacePinModal, setShowReplacePinModal, newAssetId, replacePin, replaceList } = props;

	const [selectPinnedAssetID, setSelectPinnedAssetID] = useState();
	const { confirm } = Modal;

	function selectAssetID(selectedKey) {
		setSelectPinnedAssetID(selectedKey);
	}

	function showConfirm() {
		confirm({
			title: 'Do you Want to replace this asset?',
			icon: <ExclamationCircleOutlined />,
			onOk() {
				var data = {
					NewAssetId: newAssetId,
					PreAssetId: selectPinnedAssetID[0]
				};
				replacePin(data);
			},
			onCancel() {
				setShowReplacePinModal(false);
			}
		});
	}

	return (
		<Modal
			title="Replace"
			visible={showReplacePinModal}
			onCancel={() => setShowReplacePinModal(false)}
			centered={true}
			width={580}
			footer={false}
			getContainer={false}
			closable
			destroyOnClose={true}
		>
			<Tree
				blockNode
				onSelect={selectAssetID}
				selectedKeys={selectPinnedAssetID}
				treeData={replaceList}
				multiple={false}
			/>
			<Row type="flex" style={{ marginTop: '10px' }}>
				<Col xs={24} className="form-update-actions" style={{ textAlign: 'center' }}>
					<Button xs={8} type="secondary" onClick={() => setShowReplacePinModal(false)} style={{ marginRight: '70px' }}>
						{t('Button.Cancel')}
					</Button>
					<Button xs={8} type="primary" onClick={showConfirm}>
						Replace
					</Button>
				</Col>
			</Row>
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		replacePinAssets: (data) => dispatch(replacePinAssets(data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ReplacePinModal));
