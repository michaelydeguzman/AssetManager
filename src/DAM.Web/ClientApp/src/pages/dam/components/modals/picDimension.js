import React from 'react';

import useImageSize from '@use-hooks/image-size';
import { Input } from 'antd';

export default function PicDimension(props) {
	const { url } = props;
	const [width, height] = useImageSize(url);
	let value = '';
	if (width > 0 && height > 0) {
		value = width + 'px' + ' X ' + height + 'px';
	} else {
		value = '';
	}

	return <Input disabled style={{ width: '100%' }} value={value} size={props.size ? props.size : 'default'} bordered={props.bordered ? props.bordered : true}/>;
}
