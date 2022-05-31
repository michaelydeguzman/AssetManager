import { useReducer, useMemo } from 'react';

const treeInitialState = {
	expandedKeys: [],
	checkedKeys: [],
	selectedKeys: [],
	autoExpandParent: true
};

function treeReducer(treeMoveState, action) {
	switch (action.type) {
		case 'EXPANDED_KEYS':
			return {
				...treeMoveState,
				expandedKeys: action.payload
			};
		case 'CHECKED_KEYS':
			return {
				...treeMoveState,
				checkedKeys: action.payload
			};
		case 'SELECTED_KEYS':
			return {
				...treeMoveState,
				selectedKeys: action.payload
			};
		case 'AUTO_EXPAND_PARENT':
			return {
				...treeMoveState,
				autoExpandParent: action.payload
			};
		default:
			return treeMoveState;
	}
}

export default function useTreeMove() {
	const [treeMoveState, treeMoveDispatch] = useReducer(treeReducer, treeInitialState);
	return useMemo(() => [treeMoveState, treeMoveDispatch], [treeMoveState, treeMoveDispatch]);
}
