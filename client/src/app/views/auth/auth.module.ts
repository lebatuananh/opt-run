import { NgModule } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { AuthRoutingModule } from './auth-routing.module';
import { SharedModule } from '@app/shared/shared.module';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import { LogoModule } from '@app/shared/components/logo/logo.module';
import {RegisterFormComponent} from '@app/views/auth/components/register-form/register-form.component';
import {LoginFormComponent} from '@app/views/auth/components/login-form/login-form.component';
import {RegisterComponent} from '@app/views/auth/register/register.component';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterFormComponent,
    LoginFormComponent,
    RegisterComponent
  ],
  imports: [
    AuthRoutingModule,
    SharedModule,
    LogoModule,
    LaddaModule,
    NgBootstrapFormValidationModule.forRoot(),
  ]
})
export class AuthModule { }
