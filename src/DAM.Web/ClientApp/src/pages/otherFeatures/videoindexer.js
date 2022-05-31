import React, { useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Select, Spin } from 'antd';
import { getVideoIndexList, authenticateVideoForIndexing } from '../dam/actions';
import { useTranslation } from 'react-i18next';

function VideoIndexer(props) {
	const { t } = useTranslation();
	const { Option } = Select;
	const [selectedVideo, setSelectedVideo] = useState(null);
	const [videoList, setVideoList] = useState([]);
	const [isLoading, setIsLoading] = useState(false);

	const LinkButton = {
		color: '#5f41d2',
		textDecoration: 'none',
		background: '#fff',
		padding: '6px 7px',
		borderRadius: '7px',
		border: '0px',
		fontSize: '12px'
	};

	const config = {
		headers: {
			'Ocp-Apim-Subscription-Key': '058b08005c2c44c39a062a4cb1d621b2'
		}
	};

	const url = `trial/Accounts/06f672dd-a001-45fb-919c-29771f87f85f/Videos?accessToken=`;

	useEffect(() => {
		setupData();
	}, []);

	async function setupData() {
		setIsLoading(true);

		await props.authenticateVideoForIndexing().then(async (response) => {
			let items = [];
			let videos = await props.getVideoIndexList(url.concat(response.data), config);

			videos.data.results.map((video) => {
				items.push(
					<Option key={video.id} value={video.id}>
						{video.name}
					</Option>
				);
			});

			setVideoList(items);
		});

		setIsLoading(false);
	}

	return (
		<Spin spinning={isLoading} size="large">
			<Row>
				<Select
					placeholder={t('Messages.Select Video')}
					style={{ LinkButton, ...{ width: '20vw', padding: '10px' } }}
					onChange={(value) => {
						setSelectedVideo(value);
					}}
				>
					{videoList}
				</Select>
			</Row>
			<Row style={{ display: selectedVideo ? 'flex' : 'none' }}>
				<div style={{ display: 'flex' }} align={'middle'}>
					<iframe
						width="1055"
						height="620"
						src={`https://www.videoindexer.ai/embed/player/06f672dd-a001-45fb-919c-29771f87f85f/${selectedVideo}/?&locale=en&location=Trial`}
						frameBorder="0"
						allowFullScreen
					></iframe>
					<iframe
						width="580"
						height="825"
						src={`https://www.videoindexer.ai/embed/insights/06f672dd-a001-45fb-919c-29771f87f85f/${selectedVideo}/?&locale=en&location=Trial`}
						frameBorder="0"
						allowFullScreen
					></iframe>
				</div>
			</Row>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getVideoIndexList: (url, config) => dispatch(getVideoIndexList(url, config)),
		authenticateVideoForIndexing: () => dispatch(authenticateVideoForIndexing())
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(VideoIndexer));
