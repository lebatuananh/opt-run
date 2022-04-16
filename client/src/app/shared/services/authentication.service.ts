import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {BaseApi} from './base-api';
import {identityUrl} from './base-url';
import * as jwt_decode from 'jwt-decode';
import {User} from '@app/shared/types/model';
import {resolveSrv} from 'dns';

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
      console.log('userData', userData);
      user = new User(localStorage.getItem('tokenInfo'),
        userData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        userData.fullName,
        userData.Email,
        userData.avatar);
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

  register(userName: string, fullName: string, email: string, password: string, confirmPassword: string){
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
}
