import { useReducer, useMemo } from 'react';

const initialState = {
	isVisible: false,
	type: null,
	isLoading: false
};

function modalReducer(modalState, action) {
	switch (action.type) {
		case 'VISIBILITY':
			return {
				...modalState,
				isVisible: action.payload
			};
		case 'TYPE':
			return {
				...modalState,
				isLoading: true,
				type: action.payload
			};
		case 'BOTH':
			return {
				...modalState,
				isVisible: true,
				type: action.payload
			};
		default:
			return initialState;
	}
}

export default function useModal() {
	const [modalState, modalDispatch] = useReducer(modalReducer, initialState);
	return useMemo(() => [modalState, modalDispatch], [modalState, modalDispatch]);
}
