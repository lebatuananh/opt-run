import {Inject, Injectable} from '@angular/core';
import {BaseApi} from '@app/shared/services/base-api';
import {HttpClient} from '@angular/common/http';
import {baseUrl} from '@app/shared/services/base-url';
import {CashFund, OrderHistory, QueryResult, Result, WebConfiguration} from '@app/shared/types/entity.interface';

@Injectable({
  providedIn: 'root'
})
export class WebConfigurationService extends BaseApi {

  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'WebConfiguration');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<Result<QueryResult<WebConfiguration>>>(this.createUrl('GetPaging'),
      {params: this.createParams(params)});
  }

  create() {
    return this.httpClient.post(this.createUrl('Create'), {
      webType: 1
    } );
  }

  update( command) {
    return this.httpClient.put(this.createUrl(`Update`), command);
  }

  changeSelected(command){
    return this.httpClient.post(this.createUrl(`ChangeSelected`), command);
  }

}
