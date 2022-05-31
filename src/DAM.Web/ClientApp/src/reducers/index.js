import { combineReducers } from 'redux';
import { reducer as form } from 'redux-form';

import login from '../pages/login/reducers';
import dam from '../pages/dam/reducers';
import archive from '../pages/archive/reducers';
import audits from '../pages/auditTrail/reducers';
import approvals from '../pages/approvals/reducers';
import createPassword from '../pages/createPassword/reducers';
import resetPassword from '../pages/resetPassword/reducers';

const rootReducer = combineReducers({
	form,
	login,
	dam,
	archive,
	audits,
	approvals,
	createPassword,
	resetPassword
});

export default rootReducer;
