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
export const IDENTITY_ENDPOINT = 'https://api.runotp.xyz/api';
// export const API_ENDPOINT = environment.production ? 'https://api.runotp.xyz' : 'https://api.runotp.xyz/api';
export const API_ENDPOINT =  'https://api.runotp.xyz/api';
