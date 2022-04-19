import {HttpClient} from '@angular/common/http';
import {Inject, Injectable} from '@angular/core';
import {Customer, QueryResult, ResultModel, Template, Transaction} from '../types/entity.interface';
import {BaseApi} from './base-api';
import {baseUrl} from './base-url';

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends BaseApi {

  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'api/v1/admin/customer');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<ResultModel<QueryResult<Customer>>>(this.createUrl(''), {params: this.createParams(params)});
  }

  queryTransactions(id: string, params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<ResultModel<QueryResult<Transaction>>>(this.createUrl(`${id}/transactions`), {params: this.createParams(params)});
  }

  get(id: string) {
    return this.httpClient.get<Customer>(this.createUrl(`${id}`));
  }

  create(command) {
    return this.httpClient.post(this.createUrl(''), command);
  }

  enable(id: string) {
    return this.httpClient.put(this.createUrl(`${id}/enable`), {});
  }

  disable(id: string) {
    return this.httpClient.put(this.createUrl(`${id}/disable`), {});
  }

  edit(id: string, command) {
    return this.httpClient.put(this.createUrl(`${id}`), command);
  }

  addTemplates(id: string, command) {
    return this.httpClient.post(this.createUrl(`${id}/templates`), command);
  }

  removeTemplates(id: string, command) {
    return this.httpClient.request('delete', this.createUrl(`${id}/templates`), {
      body: command
    });
  }

  deposit(id: string, command) {
    return this.httpClient.post(this.createUrl(`${id}/deposit`), command);
  }
}
