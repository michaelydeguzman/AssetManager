import { useReducer, useMemo } from 'react';

const treeInitialState = {
	expandedFolderKeys: [],
	checkedFolderKeys: [],
	selectedFolderKeys: [],
	autoExpandParentFolder: true
};

function treeReducer(treeMoveFolderState, action) {
	switch (action.type) {
		case 'EXPANDED_KEYS':
			return {
				...treeMoveFolderState,
				expandedFolderKeys: action.payload
			};
		case 'CHECKED_KEYS':
			return {
				...treeMoveFolderState,
				checkedFolderKeys: action.payload
			};
		case 'SELECTED_KEYS':
			return {
				...treeMoveFolderState,
				selectedFolderKeys: action.payload
			};
		case 'AUTO_EXPAND_PARENT':
			return {
				...treeMoveFolderState,
				autoExpandParentFolder: action.payload
			};
		default:
			return treeMoveFolderState;
	}
}

export default function useTreeMoveFolder() {
	const [treeMoveFolderState, treeMoveFolderDispatch] = useReducer(treeReducer, treeInitialState);
	return useMemo(() => [treeMoveFolderState, treeMoveFolderDispatch], [treeMoveFolderState, treeMoveFolderDispatch]);
}
