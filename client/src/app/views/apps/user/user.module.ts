import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {UserComponent} from './user.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {SharedModule} from '@app/shared/shared.module';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {UserRoutingModule} from '@app/views/apps/user/user.routing.module';
import {RechargeUserModalComponent} from './recharge-user-modal/recharge-user-modal.component';
import {UpdateDiscountModalComponent} from '@app/views/apps/user/update-discount-modal/update-discount-modal.component';
import {ReportUserInfoComponent} from '@app/views/apps/user/report-user-info/report-user-info.component';


@NgModule({
  declarations: [
    UserComponent,
    RechargeUserModalComponent,
    UpdateDiscountModalComponent,
    ReportUserInfoComponent
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
export class UserModule {
}
