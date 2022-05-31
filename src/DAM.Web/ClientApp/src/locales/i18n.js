import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import en from './en-US.json';
import hi from './hi-IN.json';
import zh from './zh-CN.json';
import it from './it-IT.json';
import de from './de-DE.json';
import es from './es-ES.json';
import fr from './fr-FR.json';

export const languages = [
	{ title: 'English', shorthand: 'en', icon: '' },
	{ title: 'हिंदी', shorthand: 'hi', icon: '' },
	{ title: '中文', shorthand: 'zh', icon: '' },
	{ title: 'Italiano', shorthand: 'it', icon: '' },
	{ title: 'Deutsch', shorthand: 'de', icon: '' },
	{ title: 'Español', shorthand: 'es', icon: '' },
	{ title: 'Français', shorthand: 'fr', icon: '' }
];

const resources = {
	en: {
		translation: en
	},
	hi: {
		translation: hi
	},
	zh: {
		translation: zh
	},
	it: {
		translation: it
	},
	de: {
		translation: de
	},
	es: {
		translation: es
	},
	fr: {
		translation: fr
	}
};

export default i18n
	.use(LanguageDetector)
	.use(initReactI18next)
	.init({
		resources,
		// lng: 'en', 			// this line commented to allow preferred language default in the browser
		fallbackLng: 'en',
		interpolation: {
			escapeValue: false // not needed for react as it escapes by default
		}
	});
