import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerComponent } from './customer.component';
import { SharedModule } from '@app/shared/shared.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgBootstrapFormValidationModule } from 'ng-bootstrap-form-validation';
import { LaddaModule } from 'angular2-ladda';
import { SwitchModule } from '@app/shared/components/switch/switch.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { ColumnPanelModule } from '@app/shared/components/column-panel/column-panel.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { CustomerCreateComponent } from './customer-create/customer-create.component';
import { AvatarModule } from '@app/shared/components/avatar/avatar.module';
import { CustomerViewComponent } from './customer-view/customer-view.component';
import { CustomerCoordinatorService } from './customer-coordinator.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { ModalSelectTemplatesComponent } from './modal-select-templates/modal-select-templates.component';
import { CustomerDepositComponent } from './customer-deposit/customer-deposit.component';
import { CustomerDepositModalComponent } from './customer-deposit/customer-deposit-modal/customer-deposit-modal.component';
import { ExtendSubscriptionModalComponent } from './extend-subscription-modal/extend-subscription-modal.component';
import { SubscriptionPlanSelectorComponent } from './subscription-plan-selector/subscription-plan-selector.component';
import { CustomerMessageComponent } from './customer-message/customer-message.component';
import {TableResizerDirective} from '@app/shared/directive/table-resizer.directive';

@NgModule({
  declarations: [
    CustomerComponent,
    CustomerCreateComponent,
    CustomerViewComponent,
    CustomerEditComponent,
    ModalSelectTemplatesComponent,
    CustomerDepositComponent,
    CustomerDepositModalComponent,
    ExtendSubscriptionModalComponent,
    SubscriptionPlanSelectorComponent,
    CustomerMessageComponent
  ],
  imports: [
    CommonModule,
    CustomerRoutingModule,
    SharedModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    SwitchModule,
    ColumnPanelModule,
    PerfectScrollbarModule,
    AvatarModule,
    TabsModule,
    NgSelectModule
  ],
  providers: [
    CustomerCoordinatorService
  ]
})
export class CustomerModule { }
