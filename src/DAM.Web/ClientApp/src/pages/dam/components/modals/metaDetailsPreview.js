import React, {useEffect,useState} from 'react';
import PreviewAsset from '../previewAsset.js';
import { Row, Col } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import { Document, Page } from 'react-pdf/dist/esm/entry.webpack';

function MetaDetailsPreview (props) {
const file = props.assetFile;
const [pageNumber, setPageNumber] = useState(1);

  function handlePageChange(action) {
		if (action === 'left') {
			if (pageNumber === 1) {
				setPageNumber(props.numPages);
			} else {
				setPageNumber(pageNumber - 1);
			}
		} else if (action === 'right') {
			if (pageNumber === props.numPages) {
				setPageNumber(1);
			} else {
				setPageNumber(pageNumber + 1);
			}
		}
	}

	function onDocumentLoadSuccess({ numPages }) {
		props.setNumPages(numPages);
	}

  function renderPreviewComponent () {
		if (file.fileType.includes('audio'))
		{
			return (
				<div className="edit-assets-preview-container">
					<audio style={{ width: '100%' }} controls controlsList="nodownload" key={file.fileName}>
						<source src={file.originalUrl} type="audio/mpeg"></source>
						<source src={file.originalUrl} type="audio/ogg"></source>
						<embed src={file.originalUrl}></embed>
						browser does not support this audio format.
					</audio>
				</div>
			)
		}
		if (file.fileType.includes('video'))
		{
			return (
				<div className="edit-assets-preview-container">
					<video style={{ width: '100%' }} controls controlsList="nodownload" key={file.fileName}>
						<source src={file.originalUrl} type="video/mp4"></source>
						<source src={file.originalUrl} type="video/webm"></source>
						<source src={file.originalUrl} type="video/ogg"></source>
						browser does not support this video format.
					</video>
				</div>
			)
		}
		if (file.extension === 'pdf')
		{
			return (
				<div className="edit-assets-preview-container">
					<div style={{ width: '100%' }}>
						<Row type="flex" align="middle" justify="center">
							<Col span={2} align="center">
								{props.numPages && (
									<LeftOutlined
										style={{ color: 'white', width: '2vw' }}
										onClick={(e) => {
											handlePageChange('left');
										}}
									/>
								)}
							</Col>
							<Col span={20} align="center">
								<Document
									file={file.originalUrl}
									onLoadSuccess={onDocumentLoadSuccess}
									loading={'Loading PDF...'}
								>
									<Page pageNumber={pageNumber} />
								</Document>
							</Col>
							<Col span={2} align="center">
								{props.numPages && (
									<RightOutlined
										style={{ color: 'white', width: '2vw' }}
										onClick={(e) => {
											handlePageChange('right');
										}}
									/>
								)}
							</Col>
						</Row>
						<Row type="flex" align="middle" justify="center">
							<p style={{ color: 'white' }}>{props.numPages && 'Page ' + pageNumber + ' of ' + props.numPages}</p>
						</Row>
					</div>
				</div>
			)
		}
		if (file.extension.includes('doc') ||
				file.extension.includes('xls') ||
				file.extension.includes ('ppt'))
		{
			return (
				<div className="edit-assets-preview-container" style={{ height: '75vh', width: '100%' }}>
					<iframe
							src={'https://view.officeapps.live.com/op/embed.aspx?src='.concat(
								encodeURIComponent(file.originalUrl)
							)}
							width="100%"
							height="100%"
							style={{ border: 'none' }}
					></iframe>
				</div>
			)
		}
		if (file.extension === 'txt' ||
				file.extension === 'css' ||
				file.extension === 'js' ||
				file.extension === 'json' ||
				file.extension === 'html' ||
				file.extension === 'scss' ||
				file.extension === 'xml' ||
				file.extension === 'yml' ||
				file.extension === 'xhtml') {
				return (
					<div className="edit-assets-preview-container" style={{ height: '75vh', width: '100%' }}>
					<iframe
							src={file.originalUrl}
							width="100%"
							height="100%"
							style={{ border: 'none' }}
					></iframe>
				</div>
				)
		}
		if(file.extension === 'csv' ||
				file.extension === 'pages' ||
				file.extension === 'ai' ||
				file.extension === 'psd' ||
				file.extension === 'dxf' ||
				file.extension === 'eps' ||
				file.extension === 'ps' ||
				file.extension === 'ttf' ||
				file.extension === 'xps')
		{
			return (
				<div className="edit-assets-preview-container" style={{ height: '75vh', width: '100%' }}>
					<iframe
							id='google-doc-viewer'
							src={'https://docs.google.com/viewer?embedded=true&url='.concat(
								encodeURIComponent(file.downloadUrl)
							)}
							width="100%"
							height="100%"
							style={{ border: 'none' }}
					></iframe>
				</div>
			)
		}
		return (
			<div className="edit-assets-preview-container" style={{ height: '75vh', width: '100%' }}>
				<PreviewAsset item={file} isOriginal />
			</div>
		)
	}

  return (
    <>
      {renderPreviewComponent()}
    </>
  )
}

export default MetaDetailsPreview;