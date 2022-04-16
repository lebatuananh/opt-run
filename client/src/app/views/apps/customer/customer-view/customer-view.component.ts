import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '@app/shared/services/customer.service';
import { SubscriptionService } from '@app/shared/services/subscription.service';
import { Customer } from '@app/shared/types/entity.interface';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { CustomerCoordinatorService } from '../customer-coordinator.service';
import { ExtendSubscriptionModalComponent } from '../extend-subscription-modal/extend-subscription-modal.component';

@Component({
  selector: 'app-customer-view',
  templateUrl: './customer-view.component.html',
  styleUrls: ['./customer-view.component.scss']
})
export class CustomerViewComponent implements OnInit {
  customer: Customer;
  status = false;
  subscriptionExpiredDate: Date;
  constructor(private router: Router, private route: ActivatedRoute, private toast: ToastrService,
              private customerService: CustomerService,
              private customerCoordinatorService: CustomerCoordinatorService,
              private modalService: BsModalService,
              private subscriptionService: SubscriptionService,
              private cdRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.customer = data.customer.data;
      this.status = data.customer?.data.status;
      this.fetchSubscriptionExpiredDate();
    });
  }

  fetchSubscriptionExpiredDate() {
    this.subscriptionService.getExpiredDate(this.customer.id).subscribe(data => {
      this.subscriptionExpiredDate = data.expiredDate;
      this.cdRef.detectChanges();
    });
  }

  onChangeStatus() {
    const next = () => {
      this.status ? this.toast.success('Kích hoạt tài khoản thành công') : this.toast.success('Khóa tài khoản thành công');
      this.customerCoordinatorService.next();
    };
    this.status ? this.customerService.disable(this.customer.id).subscribe(next) : this.customerService.enable(this.customer.id).subscribe(next);
    this.status = !this.status;
  }

  showExtendModal() {
    const ref = this.modalService.show(ExtendSubscriptionModalComponent, {
      backdrop: true,
      ignoreBackdropClick: true,
      initialState: {
        customerId: this.customer.id
      }
    });
    ref.content.submitted.subscribe(() => {
      this.fetchSubscriptionExpiredDate();
    });
  }

  cancelSubscrition() {
    this.subscriptionService.cancel(this.customer.id).subscribe(() => {
      this.toast.success('Đã hủy gói cước');
      this.fetchSubscriptionExpiredDate();
    }, () => this.toast.error('Đã xảy ra lỗi'));
  }


}
