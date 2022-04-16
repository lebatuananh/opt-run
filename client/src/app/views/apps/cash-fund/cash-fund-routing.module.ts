import {NgModule} from '@angular/core';
import {CashFundComponent} from '@app/views/apps/cash-fund/cash-fund.component';
import {RouterModule, Routes} from '@angular/router';

const routes: Routes = [{
  path: '',
  component: CashFundComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CashFundRoutingModule {
}
