import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve} from '@angular/router';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {Observable} from 'rxjs';
import {Report, Result} from '@app/shared/types/entity.interface';

@Injectable({
  providedIn: 'root'
})
export class DashboardResolve implements Resolve<Result<Report>> {
  constructor(private authenticationService: AuthenticationService) { }
  resolve(route: ActivatedRouteSnapshot): Observable<Result<Report>> {
    return this.authenticationService.report();
  }
}
