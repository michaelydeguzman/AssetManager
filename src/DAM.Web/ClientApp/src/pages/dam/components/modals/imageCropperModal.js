import React, { useCallback, useEffect, useRef, useState } from 'react';
import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import { Row, Col, Button, Modal, ModalBody } from 'reactstrap';
import { displayCropQualityMessage } from '../../constants/DisplayConstants';
import './styles/templates.style.css';
import { BasicModal } from '../common';
// import '../../scss/_custom.scss';

// Increase pixel density for crop preview quality on retina screens.
const pixelRatio = window.devicePixelRatio || 1;

const ImageCropperModal = (props) => {
	const [upImg, setUpImg] = useState();
	const imgRef = useRef(null);
	const previewCanvasRef = useRef(null);
	const divWidth = parseFloat(window.getComputedStyle(document.getElementById(props.dataId)).width.replace('px', ''));
	const divHeight = parseFloat(window.getComputedStyle(document.getElementById(props.dataId)).height.replace('px', ''));
	const [crop, setCrop] = useState({ unit: 'px', width: 0, aspect: divWidth / divHeight });
	const [completedCrop, setCompletedCrop] = useState(null);
	const [confirmCropModalState, setConfirmCropModalState] = useState(false);
	const dpiThreshold = 200;
	const pixelRatio = window.devicePixelRatio || 1;

	const urlToObject = async (image) => {
		// blob
		const response = await fetch(image);
		const blob = await response.blob();
		const file = new File([blob], 'image.png', { type: blob.type });

		// file
		const reader = new FileReader();
		reader.addEventListener('load', () => setUpImg(reader.result));
		reader.readAsDataURL(file);
	};

	useEffect(() => {
		urlToObject(props.value);
	}, [upImg]);

	const onLoad = useCallback((img) => {
		imgRef.current = img;
	}, []);

	// We resize the canvas down when saving on retina devices otherwise the image
	// will be double or triple the preview size.
	function getResizedCanvas(canvas, crop) {
		if (!imgRef.current) {
			return;
		}
		const scaleX = imgRef.current.naturalWidth / imgRef.current.width;
		const scaleY = imgRef.current.naturalHeight / imgRef.current.height;
		const tmpCanvas = canvas;
		tmpCanvas.width = Math.ceil(crop.width * scaleX);
		tmpCanvas.height = Math.ceil(crop.height * scaleY);

		const ctx = tmpCanvas.getContext('2d');
		const image = imgRef.current;
		ctx.drawImage(
			image,
			crop.x * scaleX,
			crop.y * scaleY,
			crop.width * scaleX,
			crop.height * scaleY,
			0,
			0,
			crop.width * scaleX,
			crop.height * scaleY
		);

		return tmpCanvas;
	}

	function geenrateUrl(previewCanvas, crop, props) {
		if (!crop || !previewCanvas) {
			return;
		}

		const canvas = getResizedCanvas(previewCanvas, crop);

		const previewUrl = canvas.toDataURL('image/png');

		// create here a dispatch function that will upload the image to blob and return as url
		console.log('preview url  ==========>', previewUrl);
		props.elementOnChangeImageSource(props.parentId, previewUrl);
		props.updateTextFromJson(props, previewUrl, 'value');
		props.toggleModalCropper();
	}

	useEffect(() => {
		if (!completedCrop || !previewCanvasRef.current || !imgRef.current) {
			return;
		}

		const image = imgRef.current;

		const canvas = previewCanvasRef.current;
		const crop = completedCrop;

		const scaleX = image.naturalWidth / image.width;
		const scaleY = image.naturalHeight / image.height;
		var originWidth = Math.ceil(crop.width * scaleX);
		var originHeight = Math.ceil(crop.height * scaleY);
		// maximum width/height
		//var maxWidth = 1200, maxHeight = 1200 / (16 / 9);
		var targetWidth = originWidth,
			targetHeight = originHeight;
		//if (originWidth > maxWidth || originHeight > maxHeight) {
		//    if (originWidth / originHeight > maxWidth / maxHeight) {
		//        targetWidth = maxWidth;
		//        targetHeight = Math.ceil(maxWidth * (originHeight / originWidth));
		//    } else {
		//        targetHeight = maxHeight;
		//        targetWidth = Math.ceil(maxHeight * (originWidth / originHeight));
		//    }
		//}
		// set canvas size
		canvas.width = targetWidth;
		canvas.height = targetHeight;
		const ctx = canvas.getContext('2d');
		ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
		ctx.imageSmoothingQuality = 'high';
		ctx.drawImage(
			image,
			crop.x * scaleX,
			crop.y * scaleY,
			crop.width * scaleX,
			crop.height * scaleY,
			0,
			0,
			targetWidth,
			targetHeight
		);

		if (targetWidth !== 0) {
			var divWidthInches = divWidth / 96;
			var divHeightInches = divHeight / 96;

			var dpi = targetWidth / divWidthInches;

			if (dpi < dpiThreshold) {
				toggleCropModal(confirmCropModalState, setConfirmCropModalState);
			}
		}
	}, [completedCrop]);

	const toggleCropModal = (modalState, setModal) => {
		setModal(!modalState);
	};

	return (
		<>
			<Modal isOpen={props.modalCropper} toggle={props.toggleModalCropper}>
				<ModalBody className="imageCropper">
					<Row>
						<div className="app">
							<ReactCrop
								src={upImg}
								onImageLoaded={onLoad}
								crop={crop}
								onChange={(c) => setCrop(c)}
								onComplete={(c) => setCompletedCrop(c)}
								imageStyle={{ width: '100%', height: '100%' }}
							/>

							<div style={{ position: 'absolute', visibility: 'hidden' }}>
								<canvas
									ref={previewCanvasRef}
									style={{
										width: Math.round(completedCrop?.width ?? 0),
										height: Math.round(completedCrop?.height ?? 0)
									}}
								/>
							</div>
						</div>
					</Row>
					<Row className="buttonsCropper">
						<Col sm="12">
							<div className="buttonDivs">
								<Button
									color="primary"
									type="button"
									onClick={() => geenrateUrl(previewCanvasRef.current, completedCrop, props)}
								>
									Crop
								</Button>
							</div>
							<div className="buttonDivs">
								<Button color="secondary" onClick={() => props.toggleModalCropper()}>
									Cancel
								</Button>
							</div>
						</Col>
					</Row>
				</ModalBody>
			</Modal>
			<BasicModal
				isOpen={confirmCropModalState}
				toggleModal={() => toggleCropModal(confirmCropModalState, setConfirmCropModalState)}
				action={() => toggleCropModal(confirmCropModalState, setConfirmCropModalState)}
				displayMessage={displayCropQualityMessage}
			/>
		</>
	);
};

export default ImageCropperModal;
