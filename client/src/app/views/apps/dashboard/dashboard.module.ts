import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard.component';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import {SharedModule} from '@app/shared/shared.module';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {DashboardRoutingModule} from '@app/views/apps/dashboard/dashboard-routing.module';



@NgModule({
  declarations: [
    DashboardComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    SharedModule,
    SwitchModule
  ]
})
export class DashboardModule { }
