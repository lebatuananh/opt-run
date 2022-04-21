import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve} from '@angular/router';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {Observable} from 'rxjs';
import {CurrentUser, Report, Result} from '@app/shared/types/entity.interface';

@Injectable({
  providedIn: 'root'
})
export class DashboardUserResolve implements Resolve<Result<CurrentUser>> {
  constructor(private authenticationService: AuthenticationService) { }
  resolve(route: ActivatedRouteSnapshot): Observable<Result<CurrentUser>> {
    return this.authenticationService.getCurrentUser();
  }
}
