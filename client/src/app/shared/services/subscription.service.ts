import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseApi } from './base-api';
import { baseUrl } from './base-url';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService extends BaseApi {
  constructor(
    httpClient: HttpClient,
    @Inject(baseUrl) protected hostUrl: string
  ) {
    super(httpClient)
    this.setEndpoint(hostUrl, 'api/v1/message/admin/subscription')
  }

  extend(command) {
    return this.httpClient.post(this.createUrl("extend"), command)
  }

  getExpiredDate(customerId: string) {
    return this.httpClient.get<{ expiredDate: Date }>(this.createUrl(`customer/${customerId}/expired-date`))
  }

  cancel(customerId: string) {
    return this.httpClient.post(this.createUrl(`customer/${customerId}/cancel`), {})
  }
}
