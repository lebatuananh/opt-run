import {Inject, Injectable} from '@angular/core';
import {BaseApi} from '@app/shared/services/base-api';
import {HttpClient} from '@angular/common/http';
import {baseUrl} from '@app/shared/services/base-url';
import {BrandSms, CashFund, OrderHistory, QueryResult, Result, ResultModel} from '@app/shared/types/entity.interface';

@Injectable({
  providedIn: 'root'
})
export class CashFundService extends BaseApi {

  constructor(
    httpClient: HttpClient, @Inject(baseUrl)
    protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'OrderHistory');
  }
  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<Result<QueryResult<OrderHistory>>>(this.createUrl('GetPaging'),
      {params: this.createParams(params)});
  }

  get(id: string) {
    return this.httpClient.get<CashFund>(this.createUrl(`${id}`));
  }

  create() {
    return this.httpClient.post(this.createUrl('Create'), {
      webType: 1
    } );
  }

  update(id: string, command) {
    return this.httpClient.put(this.createUrl(`${id}`), command);
  }

}
