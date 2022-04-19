import { InjectionToken } from '@angular/core';
import { API_ENDPOINT, IDENTITY_ENDPOINT } from '@app/configs/app.config';

export const baseUrl = new InjectionToken<string>('baseUrl', {
  providedIn: 'root',
  factory: () => API_ENDPOINT
});

export const identityUrl = new InjectionToken<string>('identityUrl', {
  providedIn: 'root',
  factory: () => IDENTITY_ENDPOINT
});
