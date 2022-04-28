import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {map} from 'rxjs/operators';
import {BaseApi} from './base-api';
import {identityUrl} from './base-url';
import * as jwt_decode from 'jwt-decode';
import {User} from '@app/shared/types/model';
import {CurrentUser, QueryResult, Report, Result, Transaction, UserDto} from '@app/shared/types/entity.interface';
import {Observable} from 'rxjs';

interface TokenInfo {
  expires_in: number;
  access_token: string;
  token_type: string;
  refresh_token: string;
}


@Injectable(
  {
    providedIn: 'root',
  }
)
export class AuthenticationService extends BaseApi {
  constructor(
    httpClient: HttpClient,
    @Inject(identityUrl) protected hostUrl: string
  ) {
    super(httpClient);
    this.setEndpoint(hostUrl, 'Account');
  }

  public get currentTokenValue(): string {
    return this.getUserInfo() && this.getUserInfo().accessToken;
  }

  login(userName: string, password: string): any {
    return this.httpClient.post<any>(this.createUrl(`Login`), {
      userName,
      password,
    })
      .pipe(map(data => {
        if (data.statusCode === 200){
          localStorage.setItem('tokenInfo', data.value.token);
          return this.getUserInfo();
        }
      }));
  }

  getUserInfo(): User {
    let user: User;
    if (this.isUserAuthenticated()) {
      const userData = jwt_decode(localStorage.getItem('tokenInfo'));
      user = new User(localStorage.getItem('tokenInfo'),
        userData.username,
        userData.fullName,
        userData.Email,
        userData.avatar,
        userData.role
        );
    } else {
      user = null;
    }
    return user;
  }

  isUserAuthenticated(): boolean {
    const user = localStorage.getItem('tokenInfo');
    if (user != null) {
      return true;
    } else {
      return false;
    }
  }

  logout(): void {
    localStorage.removeItem('tokenInfo');
  }

  register(userName: string, fullName: string, email: string, password: string, confirmPassword: string): Observable<any>{
    return this.httpClient.post<any>(this.createUrl(`Register`), {
      userName,
      fullName,
      email,
      password,
      confirmPassword
    })
      .pipe(map(data => {
        if (data.statusCode === 200){
          console.log(data);
        }
      }));
  }

  getCurrentUser(){
    return this.httpClient.get<Result<CurrentUser>>(this.createUrl('GetCurrentUser'));
  }

  report(){
    return this.httpClient.get<Result<Report>>(this.createUrl('Report'));
  }


  reportByUserId(params: { userId: string}){
    return this.httpClient.get<Result<Report>>(this.createUrl('GetReportByUserId'), {params: this.createParams(params)});
  }

  recharge(command){
    return this.httpClient.post(this.createUrl('Recharge'), command);

  }

  deduction(command){
    return this.httpClient.post(this.createUrl('Deduction'), command);
  }


  query(params: { skip: number, take: number, query: string }) {
    return this.httpClient.get<Result<QueryResult<UserDto>>>(this.createUrl('GetPaging'),
      {params: this.createParams(params)});
  }

  active(command){
    return this.httpClient.post(this.createUrl('Active'), command);
  }

  inActive(command){
    return this.httpClient.post(this.createUrl('InActive'), command);
  }

  updateDiscount(command){
    return this.httpClient.post(this.createUrl('UpdateDiscount'), command);
  }
}
