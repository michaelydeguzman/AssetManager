import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { ReactComponent as UploadIcon } from '../../../assets/images/upload_icon.svg';
import { Row, Col, Button, Upload, Empty } from 'antd';

import { getUserRole } from '@damtoken';

function NoFiles(props) {
	const userRole = getUserRole();
	const { dragUploaderProps, clickUploaderProps, keySearchUsed } = props;

	const dragContainerStyle = {
		height: '20vh',
		border: '2px dashed'
	};

	return (
		<>
			<Row className="empty-folder-container" type="flex" justify="center" align="middle">
				<Col span={24} align="center">
					{userRole.canUpload ? (
						keySearchUsed.length === 0 && (
							<Upload.Dragger {...dragUploaderProps} style={dragContainerStyle}>
								<Row align="middle" justify="center" type="flex">
									<Col>
										<p className="upload-drag-icon">
											<UploadIcon />
										</p>
									</Col>
									<Col style={{ paddingRight: 10, borderRight: '2px solid' }}>
										<p className="ant-upload-text" id="drag-upload-text" style={{ fontWeight: 500, color: '#003169' }}>
											Drag files
											<br />
											onto here
											<br /> to upload
										</p>
									</Col>
									<Col id="second-row">
										<p className="choose-file-text">or you can also</p>
										<Upload {...clickUploaderProps}>
											<Button type="primary">Choose Files</Button>
										</Upload>
									</Col>
								</Row>
							</Upload.Dragger>
						)
					) : (
						<Empty image="https://gw.alipayobjects.com/zos/antfincdn/ZHrcdLPrvN/empty.svg" />
					)}
				</Col>
			</Row>
		</>
	);
}
function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(NoFiles));
