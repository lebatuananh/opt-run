import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { QueryResult, TemplateKey } from '../types/entity.interface';
import { BaseApi } from './base-api';
import { baseUrl } from './base-url';

@Injectable({
  providedIn: 'root'
})
export class TemplateKeyService extends BaseApi {

  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'api/v1/message/admin/templatekey');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<QueryResult<TemplateKey>>(this.createUrl(''), { params: this.createParams(params) });
  }

  get(id: string) {
    return this.httpClient.get<TemplateKey>(this.createUrl(`${id}`));
  }

  create(command) {
    return this.httpClient.post(this.createUrl(''), command);
  }

  update(id: string, command) {
    return this.httpClient.put(this.createUrl(`${id}`), command);
  }

  delete(id: string) {
    return this.httpClient.delete(this.createUrl(`${id}`), );
  }
}
