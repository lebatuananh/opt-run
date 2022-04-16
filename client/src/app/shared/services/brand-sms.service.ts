import {Inject, Injectable} from '@angular/core';
import {BaseApi} from '@app/shared/services/base-api';
import {HttpClient} from '@angular/common/http';
import {baseUrl} from '@app/shared/services/base-url';
import {BrandSms, Price, QueryResult} from '@app/shared/types/entity.interface';

@Injectable({
  providedIn: 'root'
})
export class BrandSmsService extends BaseApi {

  constructor(
    httpClient: HttpClient, @Inject(baseUrl)
    protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'api/v1/message/admin/brandsms');
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<QueryResult<BrandSms>>(this.createUrl(''), {params: this.createParams(params)});
  }

  get(id: string) {
    return this.httpClient.get<BrandSms>(this.createUrl(`${id}`));
  }

  create(command) {
    return this.httpClient.post(this.createUrl(''), command);
  }

  update(id: string, command) {
    return this.httpClient.put(this.createUrl(`${id}`), command);
  }
}
