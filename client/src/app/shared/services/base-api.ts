import { HttpParams, HttpClient, HttpParameterCodec } from '@angular/common/http';
import { Injectable } from '@angular/core';

class CustomEncoder implements HttpParameterCodec {
  encodeKey(key: string): string {
    return encodeURIComponent(key);
  }

  encodeValue(value: string): string {
    return encodeURIComponent(value);
  }

  decodeKey(key: string): string {
    return decodeURIComponent(key);
  }

  decodeValue(value: string): string {
    return decodeURIComponent(value);
  }
}

@Injectable()
export class BaseApi {

  protected baseUrl: string;

  constructor(protected httpClient: HttpClient) { }

  protected setEndpoint(hostUrl: string, endpoint: string) {
    if (endpoint.startsWith('/')) {
      endpoint = endpoint.replace(/^\/+/, '');
    }
    if (endpoint.endsWith('/')) {
      endpoint = endpoint.replace(/\/+$/, '');
    }
    if (hostUrl.endsWith('/')) {
      hostUrl = hostUrl.replace(/\/+$/, '');
    }
    this.baseUrl = `${hostUrl}/${endpoint}`;
  }

  protected createParams(params: { [key: string]: any }): HttpParams {
    return Object.keys(params).reduce((m, k) => {
      if (params[k] != null) {
        return m.set(k, params[k].toString());
      }
      return m;
    }, new HttpParams({ encoder: new CustomEncoder()}));
  }

  protected createUrl(url: string) {
    if (!url.startsWith('/')) {
      url = '/' + url;
    }
    return this.baseUrl + url;
  }
}
