import {Injectable} from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanActivateChild,
  CanLoad,
  Route, Router,
  RouterStateSnapshot,
  UrlSegment,
  UrlTree
} from '@angular/router';
import {Observable, of} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {AuthenticationService} from './authentication.service';

@Injectable({providedIn: 'root'})
export class AuthenticationGuard implements CanActivate, CanActivateChild {
  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
  ) {
  }

  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (localStorage.getItem('tokenInfo')){
      return true;
    }
    else {
      this.router.navigate(['/auth/login'], {
      });
      return false;
    }
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (localStorage.getItem('tokenInfo')){
        return true;
      }
      else {
        this.router.navigate(['/auth/login'], {
          queryParams: {
            returnUrl: state.url
          }
        });
        return false;
      }
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (localStorage.getItem('tokenInfo')){
      return true;
    }
    else {
      this.router.navigate(['/auth/login'], {
        queryParams: {
          returnUrl: state.url
        }
      });
      return false;
    }
  }
}


