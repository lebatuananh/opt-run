import {ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '@app/shared/services/customer.service';
import { PaymentGateway } from '@app/shared/types/enum';
import { TranslateService } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-deposit-modal',
  templateUrl: './customer-deposit-modal.component.html',
  styleUrls: ['./customer-deposit-modal.component.scss']
})
export class CustomerDepositModalComponent implements OnInit {
  @Input() customerId: string;
  @Output() submitted = new EventEmitter();
  form: FormGroup;
  isLoading: boolean;

  constructor(private customerService: CustomerService,
              private fb: FormBuilder,
              private modalRef: BsModalRef,
              private cd: ChangeDetectorRef,
              private translate: TranslateService,
              private toastr: ToastrService) { }

  ngOnInit(): void {
    this.buildForm();
  }

  hide() {
    this.modalRef.hide();
    this.form = undefined;
  }

  onSubmit() {
    this.isLoading = true;
    this.customerService.deposit(this.customerId, this.form.value).subscribe(() => {
      this.cd.detectChanges();
      this.submitted.emit();
      this.hide();
    })
      .add(() => this.isLoading = false);
  }

  private buildForm() {
    this.form = this.fb.group({
      totalAmount: [, [Validators.required, Validators.min(0)]],
      note: [],
      bankAccount: [, [Validators.required]],
      paymentGateway: [PaymentGateway.BankTransfer, Validators.required]
    });
  }
}
