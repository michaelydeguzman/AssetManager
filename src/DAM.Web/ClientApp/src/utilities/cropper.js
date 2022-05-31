import React, { useState, useEffect, useRef } from 'react';
import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import { useTranslation } from 'react-i18next';
import { Button, Modal, Card } from 'antd';

export default function Cropper({ initialImg, imageToCrop, handleCropped, cropResult, setCropResult, imageFileType, readOnly, isCropping, setCropping }) {
	//uploadUsingReplace,
	const [src, selectFile] = useState(initialImg);
	const { t } = useTranslation();
	let initImgRef = useRef(null);
	let initOrigImgRef = useRef(null);
	const [image, setImage] = useState(null);
	const [imageCrop, setImageCrop] = useState(imageToCrop);
	const [crop, setCrop] = useState({ unit: '%', aspect: 5 / 4 });
	const [result, setResult] = useState(cropResult);
	const [dimention, setDimention] = useState({});
	const { naturalWidth, naturalHeight } = dimention !== undefined && dimention;
	//const [isCropping, setCropping] = useState(false);

	const [origDimension, setOrigDimension] = useState(0);
	const [imgFileType, setImgFileType] = useState(imageFileType);

	useEffect(
		() => {
			selectFile(initialImg);
			setImageCrop(imageToCrop);
			setResult(cropResult);
			setImgFileType(imageFileType);
			//getDimensions(imageToCrop);

			//async function getDimensions(imageToCrop) {
			//	if (imageToCrop) {
			//		const { naturalWidth } = await initOrigImgRef.current;

			//		setOrigDimension(naturalWidth);
			//	}
			//}
		},
		[initialImg],
		[imageToCrop],
		[cropResult]
	);

	
	// useEffect(() => {
	//     if (uploadUsingReplace) {
	//         selectFile(URL.createObjectURL(uploadUsingReplace));
	//         setResult(null);
	//         setCropping(false);
	//         setCrop({});
	//     }
	// }, [uploadUsingReplace]);

	function getCroppedImg() {
		const canvas = document.createElement('canvas');

		const scaleX = image.naturalWidth / image.width;
		const scaleY = image.naturalHeight / image.height;

		canvas.width = crop.width;
		canvas.height = crop.height;

		const ctx = canvas.getContext('2d');
		ctx.drawImage(
			image,
			crop.x * scaleX,
			crop.y * scaleY,
			crop.width * scaleX,
			crop.height * scaleY,
			0,
			0,
			crop.width,
			crop.height
		);

		const base64 = canvas.toDataURL('image/jpeg');

		setCropResult(base64);
		handleCropped(base64);
	}

	return (
		<React.Fragment>
			<img
				style={{ display: 'none' }}
				src={imageToCrop}
				alt="hidden"
				ref={initOrigImgRef}
				onLoad={async () => {
					const { naturalWidth } = await initOrigImgRef.current;

					setOrigDimension(naturalWidth);
			}}
			/>
			{/*<Card
				title="Thumbnail"
				size="small"
				cover={
					<React.Fragment>
						<img
							//style={{ display: "none" }}
							src={!result ? src : result}
							alt="hidden"
							ref={initImgRef}
							onLoad={async () => {
								const { naturalWidth, naturalHeight } = await initImgRef.current;

								setDimention({
									naturalWidth,
									naturalHeight
								});
							}}
						/>
						<img
							style={{ display: 'none' }}
							src={imageToCrop}
							alt="hidden"
							ref={initOrigImgRef}
							onLoad={async () => {
								const { naturalWidth } = await initOrigImgRef.current;

								setOrigDimension(naturalWidth);
							}}
						/>
					</React.Fragment>
				}
				actions={
					imgFileType && imgFileType.includes('image') && !readOnly
						? ((
								<Button
									onClick={() => {
										setCropping((prev) => !prev);
									}}
									className="ant-btn-primary"
								>
									Crop
								</Button>
						  ),
						  `${naturalWidth}px ${naturalHeight}px`)
						: `${naturalWidth}px ${naturalHeight}px`
				}
				style={{ marginTop: '10px' }}
				className="cropper-container"
			/>*/}

			<Modal
				visible={isCropping}
				width={origDimension > 250 ? origDimension : 250}
				onOk={() => {
					getCroppedImg();
					setCropping(false);
					setCrop({});
				}}
				closable={false}
				centered
				okText={t('Button.Crop')}
				cancelText={t('Button.Cancel')}
				onCancel={() => {
					setCropping(false);
					setCrop({});
				}}
				className="cropper-modal"
			>
				{src && (
					<ReactCrop
						src={imageCrop}
						crop={crop}
						onImageLoaded={(target) => {
							target.crossOrigin = 'anonymous';
							setImage(target);
						}}
						onChange={setCrop}
					/>
				)}
			</Modal>
		</React.Fragment>
	);
}
