import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserComponent } from './user.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {SharedModule} from '@app/shared/shared.module';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {UserRoutingModule} from '@app/views/apps/user/user.routing.module';
import { RechargeUserModalComponent } from './recharge-user-modal/recharge-user-modal.component';



@NgModule({
  declarations: [
    UserComponent,
    RechargeUserModalComponent
  ],
  imports: [
    CommonModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    UserRoutingModule,
    SharedModule,
    SwitchModule
  ]
})
export class UserModule { }
