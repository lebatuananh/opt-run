import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html'
})
export class LoginFormComponent implements OnInit {
  @Output() succeed = new EventEmitter();
  formGroup: FormGroup;
  showResult = false;
  showPassword = false;
  backendError = null;
  isSubmit = false;

  constructor(private formBuilder: FormBuilder, private authenticationService: AuthenticationService, private translate: TranslateService, private router: Router) {
  }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      username: ['', [
        Validators.required
      ]],
      password: ['', [
        Validators.required
      ]]
    });
  }

  login() {
    this.isSubmit = true;
    const next = () => {
      this.succeed.emit();
    };
    const error = err => {
      this.showResult = true;
      const data = err.error;
      this.backendError = this.translate.instant(data?.error_description || data?.error || 'Đã có lỗi xảy ra');
    };
    this.authenticationService.login(this.formGroup.value.username, this.formGroup.value.password).subscribe(next, error)
      .add(() => this.isSubmit = false);
  }

  onShowPasswordClick() {
    this.showPassword = !this.showPassword;
  }

  onReset() {
    this.formGroup.reset();
  }
}
