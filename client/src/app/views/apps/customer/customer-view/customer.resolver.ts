import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { CustomerService } from '@app/shared/services/customer.service';
import { Customer } from '@app/shared/types/entity.interface';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerResolver implements Resolve<Customer> {
  constructor(private customerService: CustomerService) {

  }
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Customer> {
    console.log('test', route.paramMap.get('id'));
    return this.customerService.get(route.paramMap.get('id'));
  }
}
