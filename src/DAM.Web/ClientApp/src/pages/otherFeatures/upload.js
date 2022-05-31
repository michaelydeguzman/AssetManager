import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Button, Upload } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { authenticateVideoForIndexing, uploadVideoForIndexing } from '../dam/actions';

function SideUpload(props) {
	const accountName = 'damblob1';
	const sasToken =
		'sv=2019-12-12&ss=b&srt=sco&sp=rw&se=2022-02-17T01:39:43Z&st=2021-02-16T17:39:43Z&spr=https&sig=6waOp24HwyTUQG%2FJq4GDVAx783G7A6xesUM2vFGu6ZI%3D';
	const destContainerName = 'videoindexercontainer';
	const sourceContainerName = 'assetcontainer';

	const { BlobServiceClient } = require('@azure/storage-blob');

	return (
		<>
			<div style={{ height: '100%', width: '100%', textAlign: 'center', marginTop: '25%' }}>
				<Upload accept={'video/*'} customRequest={handleUpload} maxCount={1}>
					<Button icon={<UploadOutlined />}>Upload video</Button>
				</Upload>
			</div>
		</>
	);

	async function handleUpload(option) {
		await props.authenticate().then(async (response) => {
			console.log(response);
			const blobName = '6b52961b67164b3b940ff0b8cb7db9b9.mp4';
			const blobService = new BlobServiceClient(`https://${accountName}.blob.core.windows.net?${sasToken}`);

			const sourceContainer = blobService.getContainerClient(sourceContainerName);
			const destContainer = blobService.getContainerClient(destContainerName);
			const sourceBlob = sourceContainer.getBlobClient(blobName);

			if (await sourceBlob.exists()) {
				let sourceProperties = await sourceBlob.getProperties();

				const lease = sourceBlob.getBlobLeaseClient();

				if (sourceProperties.leaseState == 'leased') {
					// Break the lease on the source blob.
					await lease.breakLease();

					// Update the source blob's properties to check the lease state.
					sourceProperties = await sourceBlob.getProperties();
					console.log(`Lease state: ${sourceProperties.LeaseState}`);
				}

				await lease.acquireLease(-1);

				const destBlob = destContainer.getBlobClient(sourceBlob.name);

				await destBlob.deleteIfExists();

				await destBlob.syncCopyFromURL(sourceBlob.url);

				const destProperties = await destBlob.getProperties();

				console.log(`Copy status: ${destProperties.copyStatus}`);
				console.log(`Copy progress: ${destProperties.copyProgress}`);
				console.log(`Completion time: ${destProperties.copyCompletedOn}`);
				console.log(`Total bytes: ${destProperties.contentLength}`);

				sourceProperties = await sourceBlob.getProperties();

				if (sourceProperties.leaseState == 'leased') {
					// Break the lease on the source blob.
					await lease.breakLease();

					// Update the source blob's properties to check the lease state.
					sourceProperties = await sourceBlob.getProperties();
					console.log(`Lease state: ${sourceProperties.LeaseState}`);
				}
			}

			// const { file } = option;
			// let base;

			// let fileReader = new FileReader();
			// fileReader.readAsArrayBuffer(file);

			// fileReader.onload = async (e) => {
			//     console.log(e.target.result);
			//     base = e.target.result;

			//     var byteString = new Uint8Array(base);

			//     const formData = new FormData();
			//     formData.append('file', Uint8ToBase64(byteString));

			//     const config = {
			//         headers: {
			//             'Ocp-Apim-Subscription-Key': '058b08005c2c44c39a062a4cb1d621b2',
			//             'x-ms-client-request-id': '',
			//             'Content-Type': 'multipart/form-data'
			//         }
			//     };

			//     var querString = `accessToken=${response.data}&location=trial&name=${file.name}&privacy=Public&partition=parition&description=${file.name}`;
			//     var url = `trial/Accounts/06f672dd-a001-45fb-919c-29771f87f85f/Videos?${querString}`;

			//     await props.uploadVideo(url, formData, config)
			//         .then((response) => {
			//             console.log('Upload', response);
			//         });
			// };
		});
	}

	function Uint8ToBase64(u8Arr) {
		var CHUNK_SIZE = 0x8000; //arbitrary number
		var index = 0;
		var { length } = u8Arr;
		var result = '';
		var slice;
		while (index < length) {
			slice = u8Arr.subarray(index, Math.min(index + CHUNK_SIZE, length));
			result += String.fromCharCode.apply(null, slice);
			index += CHUNK_SIZE;
		}
		return btoa(result);
	}
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		authenticate: () => dispatch(authenticateVideoForIndexing()),
		uploadVideo: (fileName, data) => dispatch(uploadVideoForIndexing(fileName, data))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(SideUpload));
