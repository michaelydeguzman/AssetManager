This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.<br>
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.<br>
You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.<br>
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.<br>
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.<br>
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (Webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: https://facebook.github.io/create-react-app/docs/code-splitting

### Analyzing the Bundle Size

This section has moved here: https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size

### Making a Progressive Web App

This section has moved here: https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app

### Advanced Configuration

This section has moved here: https://facebook.github.io/create-react-app/docs/advanced-configuration

### Deployment

This section has moved here: https://facebook.github.io/create-react-app/docs/deployment

### `npm run build` fails to minify

This section has moved here: https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify




VSCode：
## Autoformatting - Prettier
1. Download the [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) VS Code plugin.
2. `npm install --save-dev prettier`
3. Add a `.prettierrc` file to the root folder of your project. current example setup for prettier:
    ```
    {
            "printWidth":120,
            "tabWidth":2,
            "useTabs":true,
            "singleQuote":true,
            "trailingComma":"none",
            "bracketSpacing":true,
            "semi":true
    }
    ```
4. If you would like prettier to Autoformat on save, add `"editor.formatOnSave":true` to your workspace settings.
   Currently, if a ternery is a return value of an arrow function, parentheses are added.
   [See Here](https://github.com/prettier/prettier/issues/1379)

# ESLint for project
## Setup for VScode
1.  Install dependencies:
    -ESLint:
    `npm install -g eslint`
    -ESLint extensions:
    `npm install --save-dev eslint-config-airbnb`
    `npm install --save-dev eslint-plugin-import`
    `npm install --save-dev eslint-plugin-jsx-a11y`
    `npm install --save-dev eslint-plugin-react-hooks`
    `npm install eslint-plugin-babel@latest --save-dev`
2.  Run the init script in Terminal:
    `./node_modules/.bin/eslint --init` or `eslint --init`
            
    If you don't have the permission to excute the script, please follow this:
     open the powershell with admin permission:
     `get-ExecutionPolicy`
     if it shows Restricted
     `set-ExecutionPolicy RemoteSigned`
     `Y`
    
    init script will create the config file for ESLint(recommand the js file type)
# Integreted prettier into ESLint
1. run scripts in Terminal to install prettier:
    `npm install --save-dev prettier eslint-plugin-prettier`
    `npm install --save-dev prettier eslint-config-prettier`
2. add
   'prettier',
   'plugin:prettier/recommended',
    into .eslintrc.js extends block
    ```
    extends: [
        'eslint:recommended',
        'plugin:react/recommended',
        'airbnb',
    //add the block
        'plugin:import/errors',
        'plugin:import/warnings'
    ],
    ```
# How to run ESLint check:
1. Run the script in Terminal:
   `eslint --ext .js --ext .jsx --ext .tsx src --fix`
   --ext define file type
   src define the checking path
   --fix fix the errors and warnings automatically if ESLint could
If you don't have the permission to excute the script, please follow this:
            open the powershell with admin permission:
            `get-ExecutionPolicy`
            if it shows Restricted
            `set-ExecutionPolicy RemoteSigned`
            `Y`