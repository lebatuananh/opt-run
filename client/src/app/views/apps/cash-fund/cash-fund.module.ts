import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CashFundComponent} from './cash-fund.component';
import {UpdateCashFundModalComponent} from './update-cash-fund-modal/update-cash-fund-modal.component';
import {CashFundRoutingModule} from '@app/views/apps/cash-fund/cash-fund-routing.module';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {SharedModule} from '@app/shared/shared.module';
import {LaddaModule} from 'angular2-ladda';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';


@NgModule({
  declarations: [
    CashFundComponent,
    UpdateCashFundModalComponent
  ],
  imports: [
    CommonModule,
    CashFundRoutingModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    SharedModule,
    SwitchModule
  ]
})
export class CashFundModule {
}
