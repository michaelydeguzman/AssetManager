const {
	override,
	fixBabelImports,
	addLessLoader,
	getBabelLoader,
	addWebpackPlugin,
	addWebpackAlias
} = require('customize-cra');
const path = require('path');

module.exports = override(
	fixBabelImports('import', {
		libraryName: 'antd',
		libraryDirectory: 'es',
		style: true // change importing css to less
	}),
	addLessLoader({
		javascriptEnabled: true
		// modifyVars: { "@primary-color": "red" }
	}),
	addWebpackAlias({
		'@damhelper': path.resolve('./src/utilities/helper.js'),
		'@damcontext': path.resolve('./src/utilities/damcontext.js'),
		'@damtoken': path.resolve('./src/utilities/token.js'),
		'@damhistory': path.resolve('./src/utilities/history.js'),
		'@damhookuseTree': path.resolve('./src/hooks/useTree.js'),
		'@damhookuseTreeMove': path.resolve('./src/hooks/useTreeMove.js'),
		'@damhookuseTreeMoveFolder': path.resolve('./src/hooks/useTreeMoveFolder.js'),
		'@damhookuseWindowWidth': path.resolve('./src/hooks/useWindowWidth.js'),
		'@damhookuseWindowHeight': path.resolve('./src/hooks/useWindowHeight.js'),
		'@damCustomIcons': path.resolve('./src/assets/images/CustomIcons.js')
	})
);
