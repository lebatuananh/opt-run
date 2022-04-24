import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {WebConfigComponent} from './web-config.component';
import {AddWebConfigModalComponent} from './add-web-config-modal/add-web-config-modal.component';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import {LaddaModule} from 'angular2-ladda';
import {SharedModule} from '@app/shared/shared.module';
import {SwitchModule} from '@app/shared/components/switch/switch.module';
import {WebConfigRoutingModule} from '@app/views/apps/web-config/web-config-routing.module';


@NgModule({
  declarations: [
    WebConfigComponent,
    AddWebConfigModalComponent
  ],
  imports: [
    CommonModule,
    NgxDatatableModule,
    FormsModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule,
    LaddaModule,
    WebConfigRoutingModule,
    SharedModule,
    SwitchModule
  ]
})
export class WebConfigModule { }
