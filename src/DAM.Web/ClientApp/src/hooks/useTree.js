import { useReducer, useMemo } from 'react';

const treeInitialState = {
	expandedKeys: [],
	checkedKeys: [],
	selectedKeys: [],
	autoExpandParent: true
};

function treeReducer(treeState, action) {
	switch (action.type) {
		case 'EXPANDED_KEYS':
			return {
				...treeState,
				expandedKeys: action.payload
			};
		case 'CHECKED_KEYS':
			return {
				...treeState,
				checkedKeys: action.payload
			};
		case 'SELECTED_KEYS':
			return {
				...treeState,
				selectedKeys: action.payload
			};
		case 'AUTO_EXPAND_PARENT':
			return {
				...treeState,
				autoExpandParent: action.payload
			};
		default:
			return treeState;
	}
}

export default function useTreeConfig() {
	const [treeState, treeDispatch] = useReducer(treeReducer, treeInitialState);
	return useMemo(() => [treeState, treeDispatch], [treeState, treeDispatch]);
}
