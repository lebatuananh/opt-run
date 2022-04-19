import { AppConfig } from '@app/shared/types/app-config.interface';
import { environment } from 'src/environments/environment';
import { defaultLanguge } from './i18n.config';

export const AppConfiguration: AppConfig = {
  layoutType: 'vertical',
  sideNavCollapse: false,
  mobileNavCollapse: false,
  lang: defaultLanguge,
  navMenuColor: 'light',
  headerNavColor: '#ffffff'
};

// Change your API endpoint here
export const IDENTITY_ENDPOINT = 'https://localhost:7058/api';
export const API_ENDPOINT = environment.production ? 'https://mobi-api.mobihome.vn' : 'https://localhost:7058/api';
