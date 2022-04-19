import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import {Message, QueryResult, ResultModel} from '../types/entity.interface';
import { BaseApi } from './base-api';
import { baseUrl } from './base-url';

@Injectable({
  providedIn: 'root'
})
export class MessageService extends BaseApi {
  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'api/v1/message/admin/message');
  }

  query(customerId: string, params: { skip: number, take: number, query: string }) {
    return this.httpClient.get< ResultModel<QueryResult<Message>>>(this.createUrl(`customer/${customerId}`), { params: this.createParams(params) });
  }
}
