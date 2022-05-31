import React, { useEffect, memo, useContext } from 'react';
import UserManagement from './usermanagement/Users/UserManagement';

import { LowFrequencyContext } from '@damcontext';

export default memo(function Admin() {
	const { setAdminMode } = useContext(LowFrequencyContext);

	useEffect(() => {
		setAdminMode(true);
	}, []);

	return (
		<>
			{' '}
			<UserManagement />
		</>
	);
});
