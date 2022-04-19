import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import {QueryResult, ResultModel, Template} from '../types/entity.interface';
import { BaseApi } from './base-api';
import { baseUrl } from './base-url';

@Injectable({
  providedIn: 'root'
})
export class TemplateService extends BaseApi {

  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'api/v1/message/admin/template');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<QueryResult<Template>>(this.createUrl(''), { params: this.createParams(params) });
  }

  queryExclude(customerId: string, params: { skip: number, take: number, query: string }) {
    return this.httpClient.get< ResultModel<QueryResult<Template>>>(this.createUrl(`customer/${customerId}/exclude`), { params: this.createParams(params) });
  }

  get(id: string) {
    return this.httpClient.get<Template>(this.createUrl(`${id}`));
  }

  create(type: string, command) {
    return this.httpClient.post<Template>(this.createUrl(`${type}`), command);
  }

  update(id: string, type: string, command) {
    return this.httpClient.put<Template>(this.createUrl(`${id}/${type}`), command);
  }
}
