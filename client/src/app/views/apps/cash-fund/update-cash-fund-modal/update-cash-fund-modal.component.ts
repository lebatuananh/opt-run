import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {CashFund} from '@app/shared/types/entity.interface';
import {CashFundService} from '@app/shared/services/cash-fund.service';
import {BsModalRef} from 'ngx-bootstrap/modal';
import {TranslateService} from '@ngx-translate/core';
import {ToastrService} from 'ngx-toastr';
import {tap} from 'rxjs/operators';

@Component({
  selector: 'app-update-cash-fund-modal',
  templateUrl: './update-cash-fund-modal.component.html',
  styleUrls: ['./update-cash-fund-modal.component.scss']
})
export class UpdateCashFundModalComponent implements OnInit {

  @Output() submitted = new EventEmitter();
  createMore: boolean;
  form: FormGroup;
  isLoading: boolean;
  cashFund: CashFund;

  constructor(
    private cashFundService: CashFundService,
    private fb: FormBuilder,
    private modalRef: BsModalRef,
    private translate: TranslateService,
    private toastr: ToastrService,
  ) {
  }

  ngOnInit(): void {
    this.buildForm();
    if (this.cashFund) {
      this.form.patchValue(this.cashFund);
    }
  }

  get isCreatingNew(): boolean {
    return this.cashFund === undefined;
  }

  hide() {
    this.modalRef.hide();
    this.form = undefined;
  }

  onSubmit() {
    this.isLoading = true;
    const save$ = this.isCreatingNew ? this.create() : this.update();
    save$.subscribe(() => {
      this.submitted.emit(this.createMore);
      this.hide();
    })
      .add(() => this.isLoading = false);
  }

  private create() {
    return this.cashFundService.create()
      .pipe(
        tap(() => this.toastr.success(this.translate.instant('Tạo mới quỹ thành công')))
      );
  }

  private update() {
    return this.cashFundService.update(this.cashFund.id, this.form.value)
      .pipe(
        tap(() => this.toastr.success(this.translate.instant('Cập nhật quỹ thành công'))
        )
      );
  }

  private buildForm() {
    this.form = this.fb.group({
      name: [, Validators.required]
    });
  }
}
