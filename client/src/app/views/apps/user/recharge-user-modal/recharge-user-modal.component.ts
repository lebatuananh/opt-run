import {Component, OnInit, Output, EventEmitter} from '@angular/core';
import {UserDto} from '@app/shared/types/entity.interface';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {BsModalRef} from 'ngx-bootstrap/modal';
import {ToastrService} from 'ngx-toastr';
import {TranslateService} from '@ngx-translate/core';
import {tap} from 'rxjs/operators';
import {Observable} from 'rxjs';

@Component({
  selector: 'app-recharge-user-modal',
  templateUrl: './recharge-user-modal.component.html',
  styleUrls: ['./recharge-user-modal.component.scss']
})
export class RechargeUserModalComponent implements OnInit {
  @Output() submitted = new EventEmitter();
  createMore: boolean;
  form: FormGroup;
  userDto: UserDto;
  isLoading: boolean;

  constructor(
    private authenticationService: AuthenticationService,
    private fb: FormBuilder,
    private modalRef: BsModalRef,
    private translate: TranslateService,
    private toastr: ToastrService
  ) {
  }

  ngOnInit(): void {
    this.buildForm();
  }


  onSubmit(): void {
    this.isLoading = true;
    const save$ = this.recharge();
    save$.subscribe(() => {
      this.submitted.emit(this.createMore);
      this.hide();
    })
      .add(() => this.isLoading = false);
  }

  hide(): void  {
    this.modalRef.hide();
    this.form = undefined;
  }


  private buildForm(): void {
    this.form = this.fb.group({
      note: [, Validators.required],
      bankAccount: [, Validators.required],
      totalAmount: [, [Validators.required, Validators.pattern('^[0-9]*$')]]
    });
  }


  private recharge(): Observable<any> {
    const command = {
      ...this.form.value,
      userId: this.userDto.id
    };
    return this.authenticationService.recharge(command)
      .pipe(
        tap(() => this.toastr.success(this.translate.instant('Cập nhật template key thành công'))
        )
      );
  }


}
