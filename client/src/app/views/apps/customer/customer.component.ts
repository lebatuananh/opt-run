import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '@app/shared/services/customer.service';
import { DataTableContainer } from '@app/shared/types/data-table-container';
import { Customer } from '@app/shared/types/entity.interface';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { tap } from 'rxjs/operators';
import { CustomerCoordinatorService } from './customer-coordinator.service';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss']
})
export class CustomerComponent extends DataTableContainer<Customer> implements OnInit {
  mobilePanelOpen: boolean;
  constructor(private customerService: CustomerService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private customerCoordinatorService: CustomerCoordinatorService,
              private router: Router,
              private route: ActivatedRoute) {
    super(cd, 1000);
  }

  ngOnInit() {
    super.ngOnInit();
    this.customerCoordinatorService.changed.subscribe((val) => val && this.refresh());
  }

  protected fetch() {
    return this.customerService.query({ skip: this.skip, take: this.take, query: this.query }).pipe(tap((result) => {
      const id = this.route.snapshot.params.id;
      if (!id && result?.data.count) {
        this.router.navigate([result.data.items?.[0]?.id], { relativeTo: this.route });
      }
    }));
  }

  protected handleError(reason: any) {
    this.toast.error('Đã xảy ra lỗi');
  }

}
