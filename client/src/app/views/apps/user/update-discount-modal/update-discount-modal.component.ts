import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {UserDto} from '@app/shared/types/entity.interface';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {BsModalRef} from 'ngx-bootstrap/modal';
import {TranslateService} from '@ngx-translate/core';
import {ToastrService} from 'ngx-toastr';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';

@Component({
  selector: 'app-update-discount-modal',
  templateUrl: './update-discount-modal.component.html',
  styleUrls: ['./update-discount-modal.component.scss']
})
export class UpdateDiscountModalComponent implements OnInit {
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
    const save$ = this.updateDiscount();
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
      discount: [, [Validators.required, Validators.pattern('^[0-9]*$')]]
    });
  }


  private updateDiscount(): Observable<any> {
    const command = {
      ...this.form.value,
      id: this.userDto.id
    };
    return this.authenticationService.updateDiscount(command)
      .pipe(
        tap(() => this.toastr.success(this.translate.instant('Cập nhật chiết khấu thành công'))
        )
      );
  }

}
