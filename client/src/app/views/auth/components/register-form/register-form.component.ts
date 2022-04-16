import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import {TranslateService} from '@ngx-translate/core';
import {Router} from '@angular/router';
import {AuthenticationService} from '@app/shared/services/authentication.service';

@Component({
    selector: 'app-register-form',
    templateUrl: './register-form.component.html'
})
export class RegisterFormComponent implements OnInit {
  @Output() succeed = new EventEmitter();
  showResult = false;
  showPassword = false;
  backendError = null;
  isSubmit = false;
  formGroup: FormGroup;

    constructor(private formBuilder: FormBuilder,
                private authenticationService: AuthenticationService,
                private translate: TranslateService,
                private router: Router) {}

    ngOnInit() {
        this.formGroup = this.formBuilder.group({
            fullName: [null, [Validators.required]],
            userName: [null, [Validators.required]],
            email: [null, [Validators.required, Validators.email]],
            password: [null, [Validators.required, Validators.minLength(6)]],
            confirmPassword: [null, [Validators.required, Validators.minLength(6), this.confirmationValidator]]
        });
    }

    register() {
        this.isSubmit = true;
        const next = () => {
        this.succeed.emit();
      };
        const error = err => {
        this.showResult = true;
        const data = err.error;
        this.backendError = this.translate.instant(data?.error_description || data?.error || 'Đã có lỗi xảy ra');
      };
        this.authenticationService.register(this.formGroup.value.userName,
          this.formGroup.value.fullName,
          this.formGroup.value.email, this.formGroup.value.password, this.formGroup.value.confirmPassword)
          .subscribe(next, error).add(() => this.isSubmit = false);

    }

    confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
        if (!control.value) {
            return { required: true };
        } else if (control.value !== this.formGroup.controls.password.value) {
            return { confirm: true, error: true };
        }
        return {};
    }

  onShowPasswordClick(): void {
    this.showPassword = !this.showPassword;
  }
}
