import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { QueryResult, SubscriptionPlan } from '../types/entity.interface';
import { BaseApi } from './base-api';
import { baseUrl } from './base-url';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionPlanService extends BaseApi {
  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient)
    this.setEndpoint(hostUrl, 'api/v1/message/admin/subscriptionPlan')
  }

  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<QueryResult<SubscriptionPlan>>(this.createUrl(''), { params: this.createParams(params) })
  }
}

