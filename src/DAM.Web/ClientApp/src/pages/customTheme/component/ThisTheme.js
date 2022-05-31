import React, { useContext, useState, memo, useEffect } from 'react';
import { connect } from 'react-redux';

import { SketchPicker } from 'react-color';
import { Space, Button, Row, Popover } from 'antd';
import { LowFrequencyContext } from '@damcontext';
import { HSLtoPercentage, setThemeColors } from '../../constants';
import { getLogos, getThemeStyles, saveLogo, saveThemeStyle } from '../actions';
import ColorIcon from './ColorIcon';

function ThisTheme(props) {
	const { setColors } = useContext(LowFrequencyContext);

	useEffect(() => {
		console.log('colors', props.color);
	}, [props.color]);

	function onColorChange(nextColor) {
		const mergedNextColor = {
			...props.color,
			...nextColor
		};
		setColors(mergedNextColor);
		setThemeColors(mergedNextColor);
	}

	return (
		props.color.primaryhsl !== undefined && (
			<div>
				<Space>
					<Row align="middle">
						Primary:
						<Popover
							content={
								<SketchPicker
									color={{
										h: props.color.primaryhsl.h,
										s: props.color.primaryhsl.s / 100,
										l: props.color.primaryhsl.l / 100
									}}
									onChange={({ hsl }) => {
										onColorChange({
											primaryhsl: HSLtoPercentage(hsl)
										});
									}}
								/>
							}
						>
							<Button type="text">
								<ColorIcon
									color={`hsl(${props.color.primaryhsl.h}, ${props.color.primaryhsl.s}%, ${props.color.primaryhsl.l}%)`}
								/>
							</Button>
						</Popover>
					</Row>
					<Row align="middle">
						Secondary:
						<Popover
							content={
								<SketchPicker
									color={{
										h: props.color.secondaryhsl.h,
										s: props.color.secondaryhsl.s / 100,
										l: props.color.secondaryhsl.l / 100
									}}
									onChange={({ hsl }) => {
										onColorChange({
											secondaryhsl: HSLtoPercentage(hsl)
										});
									}}
								/>
							}
						>
							<Button type="text">
								<ColorIcon
									color={`hsl(${props.color.secondaryhsl.h}, ${props.color.secondaryhsl.s}%, ${props.color.secondaryhsl.l}%)`}
								/>
							</Button>
						</Popover>
					</Row>
					<Row align="middle">
						Tertiary:
						<Popover
							content={
								<SketchPicker
									color={{
										h: props.color.tertiaryhsl.h,
										s: props.color.tertiaryhsl.s / 100,
										l: props.color.tertiaryhsl.l / 100
									}}
									onChange={({ hsl }) => {
										onColorChange({
											tertiaryhsl: HSLtoPercentage(hsl)
										});
									}}
								/>
							}
						>
							<Button type="text">
								<ColorIcon
									color={`hsl(${props.color.tertiaryhsl.h}, ${props.color.tertiaryhsl.s}%, ${props.color.tertiaryhsl.l}%)`}
								/>
							</Button>
						</Popover>
					</Row>
				</Space>
			</div>
		)
	);
}

const mapStateToProps = (state) => ({});

const mapDispatchToProps = (dispatch) => {
	return {
		getLogos: () => dispatch(getLogos()),
		getThemeStyles: () => dispatch(getThemeStyles()),
		saveLogo: () => dispatch(saveLogo()),
		saveThemeStyle: () => dispatch(saveThemeStyle())
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(memo(ThisTheme));
