import {ChangeDetectorRef, Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TransactionService } from '@app/shared/services/transaction.service';
import { DataTableContainer } from '@app/shared/types/data-table-container';
import { Transaction } from '@app/shared/types/entity.interface';
import { PaymentGateway, TransactionStatus } from '@app/shared/types/enum';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { CustomerDepositModalComponent } from './customer-deposit-modal/customer-deposit-modal.component';
import {CustomerService} from '@app/shared/services/customer.service';

@Component({
  selector: 'app-customer-deposit',
  templateUrl: './customer-deposit.component.html',
  styleUrls: ['./customer-deposit.component.scss']
})
export class CustomerDepositComponent extends DataTableContainer<Transaction> implements OnChanges {
  @Input() customerId: string;
  TransactionStatus = TransactionStatus;
  PaymentGateway = PaymentGateway;
  constructor(private customerService: CustomerService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private route: ActivatedRoute) {
    super(cd);
  }

  protected fetch() {
    const customerId = this.route.snapshot.params.id;
    return this.customerService.queryTransactions(customerId, { skip: this.skip, take: this.take, query: this.query });
  }

  protected handleError(reason: any) {
    this.toast.error('Đã xảy ra lỗi');
  }

  showDepositModal() {
    const ref = this.modalService.show(CustomerDepositModalComponent, {
      backdrop: true,
      ignoreBackdropClick: true,
      initialState: {
        customerId: this.route.snapshot.params.id
      }
    });
    ref.content.submitted.subscribe((more) => {
      this.refresh();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ('customerId' in changes) {
      this.refresh();
    }
  }

  ngAfterViewChecked(): void{
    window.dispatchEvent(new Event('resize'));
  }
}
