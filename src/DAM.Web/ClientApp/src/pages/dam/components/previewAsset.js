import React from 'react';
import { Image } from 'antd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
	faBook,
	faFile,
	faFileAlt,
	faFileArchive,
	faFileAudio,
	faFileCode,
	faFileExcel,
	faFileImage,
	faFilePdf,
	faFilePowerpoint,
	faFileVideo,
	faFileWord,
	faFont,
	faEyeSlash
} from '@fortawesome/free-solid-svg-icons';
import NoPriviewPic from '../../../assets/images/NoPreview.png';

const FileExtentions = {
	otf: 'otf',
	ttf: 'ttf',
	eps: 'eps',
	zip: 'zip',
	svg: 'svg',
	pdf: 'pdf',
	xls: 'xls',
	doc: 'doc',
	ppt: 'ppt'
};
const FileType = {
	html: 'html',
	audio: 'audio',
	text: 'text',
	image: 'image',
	video: 'video'
};

function PreviewAsset(props) {
	const { item, onClickCard, isOriginal } = props;

	function iconBuilder(data) {
		// switch (String(data)) {
		// 	case FileExtentions.otf:
		// 	case FileExtentions.ttf:
		// 		return <FontAwesomeIcon icon={faFont} />;
		// 	case FileExtentions.eps:
		// 		return <FontAwesomeIcon icon={faBook} />;
		// 	case FileExtentions.zip:
		// 		return <FontAwesomeIcon icon={faFileArchive} />;
		// 	case FileExtentions.pdf:
		// 		return <FontAwesomeIcon icon={faFilePdf} />;
		// 	case FileExtentions.xls:
		// 		return <FontAwesomeIcon icon={faFileExcel} />;
		// 	case FileExtentions.doc:
		// 		return <FontAwesomeIcon icon={faFileWord} />;
		// 	case FileExtentions.ppt:
		// 		return <FontAwesomeIcon icon={faFilePowerpoint} />;
		// 	case FileExtentions.svg:
		// 	// return getThumbnailPic();
		// 	case FileType.image:
		// 		return <FontAwesomeIcon icon={faFileImage} />;
		// 	case FileType.html:
		// 		return <FontAwesomeIcon icon={faFileCode} />;
		// 	case FileType.text:
		// 		return <FontAwesomeIcon icon={faFileAlt} />;
		// 	case FileType.audio:
		// 		return <FontAwesomeIcon icon={faFileAudio} />;
		// 	case FileType.video:
		// 		return <FontAwesomeIcon icon={faFileVideo} />;
		// 	default:
		// 		return <FontAwesomeIcon icon={faFile} />;
		// }
		return (
			// <>
			// 	<i class="ms-Icon ms-Icon--View no-preview-slash" aria-hidden="true"></i>
			// 	<h1 style={{ fontSize: '2em', color: '#bababa' }}>No preview</h1>
			// </>
			isOriginal ? (
				<Image title={item.name} src={NoPriviewPic} alt={item.name} preview={false} height="50vh" />
			) : (
				<Image title={item.name} src={NoPriviewPic} alt={item.name} preview={false} width="100%" />
			)
		);
	}
	function findType(itemData, checkType) {
		for (let type in checkType) {
			if (itemData.includes(checkType[type])) {
				return type;
			}
		}
		return null;
	}

	function renderThumbnail() {
		console.log(item);
		if (item.thumbnail && item.thumbnail.includes('https')) {
			return isOriginal ? getOriginalPic() : getThumbnailPic();
		} else if (item.preview && item.preview.includes('https')) {
			return <Image title={item.name} src={item.preview} alt={item.name} preview={false} width="100%" />;
		} else {
			const thisExt = findType(item.extension, FileExtentions);
			if (thisExt != null) {
				return iconBuilder(thisExt);
			} else {
				const thisType = findType(item.fileType, FileType);
				return iconBuilder(thisType);
			}
		}
	}

	function getOriginalPic() {
		return (
			<Image
				title={item.name}
				src={item.originalUrl}
				alt={item.name}
				style={{ cursor: 'pointer' }}
				id="preview-fullscreen"
				className="img-preview"
			/>
		);
	}
	function getThumbnailPic() {
		return <Image title={item.name} src={item.thumbnail} alt={item.name} preview={false} width="100%" />;
	}
	return (
		<div className="card-thumbnail" onClick={onClickCard && onClickCard}>
			{renderThumbnail()}
		</div>
	);
}

export default PreviewAsset;
