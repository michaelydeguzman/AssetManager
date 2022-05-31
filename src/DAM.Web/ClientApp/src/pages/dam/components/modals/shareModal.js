import React, { useState, memo, useRef, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Button, Modal, Alert, Input, Select, TreeSelect, Form, Checkbox } from 'antd';
import { CloseOutlined } from '@ant-design/icons';
import { shareBulkAssets } from '../../actions';
import { getUserRole } from '@damtoken';
import { NestedChild } from '@damhelper';
import { useTranslation } from 'react-i18next';

function ShareModal(props) {
	const { t } = useTranslation();
	const {
		shareModalState,
		setShareModalState,
		efolders,
		selectedShareAssets,
		setSelectedShareAssets,
		onLoad,
		hasWatermark
	} = props;

	const copylinkInput = useRef();
	const downLoadlinkInput = useRef();
	const [form] = Form.useForm();
	const [showCopySuccess, setShowCopySuccess] = useState(false);
	const [shareAccess, setShareAccess] = useState(1);
	const [publicLink, setPublicLink] = useState('');
	const [internalLink, setInternalLink] = useState('');
	const [userRole, setUserRole] = useState(getUserRole());
	const [shareFolderSelect, setShareFolderSelect] = useState(false);
	const [selectedOption, setSelectedOption] = useState('');

	useEffect(() => {
		if (selectedShareAssets.length === 1) {
			setPublicLink(`${window.location.origin}/api/Assets/Download/${selectedShareAssets[0].id}/public/false`);
			setInternalLink(`${window.location.origin}/Assets/${selectedShareAssets[0].id}`);
		}
		if (selectedShareAssets.length > 1) {
			var ids = [];
			selectedShareAssets.map((f) => {
				ids.push(f.id);
			});
			setPublicLink(`${window.location.origin}/api/Assets/DownloadBulk/${ids.toString()}/public/false`);
		}
	}, []);

	const copyLinkToClipboard = () => {
		var link = document.getElementById('copyLink');
		if (shareAccess == 2) {
			link = document.getElementById('downLoadLink');
		}
		link.type = 'text';
		link.select();
		document.execCommand('copy', false, null);
		link.type = 'hidden';
		setShowCopySuccess(true);
	};

	function handleShareChange(value) {
		setShareAccess(value);
		if (value == 1 && selectedShareAssets.length > 1) {
			setShareFolderSelect(true);
		} else {
			setShareFolderSelect(false);
		}
		setSelectedOption(value);
	}

	async function submitInternalShare(values) {
		var ids = [];
		selectedShareAssets.map((f) => {
			ids.push(f.id);
		});
		await props.onShareBulkAssets(ids.toString(), values.shareIds ? values.shareIds.toString() : '');
		setSelectedShareAssets([]);
		onLoad();
	}

	function handleApplywmcheck(e) {
		var newLink = publicLink;
		if (e.target.checked) {
			newLink = newLink.replace('/false', '');
			newLink = newLink + '/true';
		} else {
			newLink = newLink.replace('/true', '');
			newLink = newLink + '/false';
		}
		setPublicLink(newLink);
	}

	return (
		<Modal
			title={<Row>{t('Share Message.Asset')}</Row>}
			visible={shareModalState}
			onCancel={() => {
				setSelectedShareAssets([]);
				setShareModalState(false);
			}}
			centered={true}
			footer={false}
			getContainer={false}
			closable={true}
			closeIcon={<CloseOutlined />}
			className={`share-modal`}
		>
			<Row>
				<div>{t('Share Message.Body')}</div>
			</Row>
			<Row>
				<Select
					style={{ width: '350px', marginTop: '12px' }}
					onChange={handleShareChange}
					placeholder={t('Messages.Select')}
				>
					<Select.Option key={1}>{t('Share Message.With Access')}</Select.Option>
					<Select.Option key={2}>{t('Share Message.With Anyone')}</Select.Option>
				</Select>
			</Row>
			{userRole.canShareFolders && shareFolderSelect && (
				<Form form={form} onFinish={submitInternalShare} layout={'vertical'}>
					<Form.Item name="shareIds" label={t('Share Message.Asset to')} style={{ marginTop: '12px' }}>
						<TreeSelect
							mode="multiple"
							placeholder={t('Messages.Select folders')}
							treeData={NestedChild(efolders, 1)}
							allowClear
							treeCheckable
							style={{ width: '350px', marginTop: '8px' }}
						></TreeSelect>
					</Form.Item>
					<Row>
						<Button htmlType="submit" className="share-modal-copy-button">
							{t('Share Message.Asset')}
						</Button>
					</Row>
				</Form>
			)}
			{hasWatermark &&
				selectedOption === '2' &&
				(selectedShareAssets[0].fileType.includes('image') || shareFolderSelect) && (
					<Row style={{ marginTop: 16 }}>
						<Checkbox onChange={handleApplywmcheck}>{t('Messages.ApplyWM')}</Checkbox>
					</Row>
				)}

			{!shareFolderSelect && (
				<Row>
					<Button onClick={() => copyLinkToClipboard()} className="share-modal-copy-button">
						{t('Share Message.Copy Link')}
					</Button>
				</Row>
			)}
			<>
				<Input id={'copyLink'} type="hidden" ref={copylinkInput} value={internalLink} title="copyUrl" />
				<Input id={'downLoadLink'} type="hidden" ref={downLoadlinkInput} value={publicLink} title="download" />
				{showCopySuccess && (
					<Row>
						<Alert
							message={t('Share Message.Copied Message')}
							type="success"
							showIcon
							closable
							onClose={() => setShowCopySuccess(false)}
							style={{ marginTop: '12px', width: '100%' }}
						/>
					</Row>
				)}
			</>
		</Modal>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		onShareBulkAssets: (data, type) => dispatch(shareBulkAssets(data, type))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ShareModal));
