import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SubscriptionService } from '@app/shared/services/subscription.service';
import { TranslateService } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-extend-subscription-modal',
  templateUrl: './extend-subscription-modal.component.html',
  styleUrls: ['./extend-subscription-modal.component.scss']
})
export class ExtendSubscriptionModalComponent implements OnInit {
  customerId: string
  @Output() submitted = new EventEmitter();
  form: FormGroup;
  isLoading: boolean;

  constructor(
    private subscriptionService: SubscriptionService,
    private fb: FormBuilder,
    private modalRef: BsModalRef,
    private translate: TranslateService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.buildForm();
  }

  hide() {
    this.modalRef.hide();
    this.form = undefined;
  }

  onSubmit() {
    this.isLoading = true;
    const save$ = this.create();
    save$.subscribe(() => {
      this.submitted.emit();
      this.hide();
    })
      .add(() => this.isLoading = false);
  }

  private buildForm() {
    this.form = this.fb.group({
      subscriptionPlanId: [, Validators.required]
    });
  }

  private create() {
    return this.subscriptionService.extend(Object.assign({}, this.form.value, { customerId: this.customerId }))
      .pipe(
        tap(() => this.toastr.success(this.translate.instant('Gia hạn gói cước thành công')))
      );
  }

}
