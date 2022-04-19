import {HttpClient} from '@angular/common/http';
import {Inject, Injectable} from '@angular/core';
import {QueryResult, Result, Transaction} from '../types/entity.interface';
import {BaseApi} from './base-api';
import {baseUrl} from './base-url';

@Injectable({
  providedIn: 'root'
})
export class TransactionService extends BaseApi {
  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'Transaction');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<Result<QueryResult<Transaction>>>(this.createUrl('GetPaging'),
      {params: this.createParams(params)});
  }


}
