import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TransactionComponent} from './transaction.component';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import {SharedModule} from '@app/shared/shared.module';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {TransactionRoutingModule} from '@app/views/apps/transaction/transaction-routing.module';


@NgModule({
  declarations: [
    TransactionComponent
  ],
  imports: [
    CommonModule,
    TransactionRoutingModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    SharedModule,
    SwitchModule
  ]
})
export class TransactionModule { }
