import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'cash-fund'
  },
  {
    path: 'cash-fund',
    loadChildren: () => import('./cash-fund/cash-fund.module').then(m => m.CashFundModule),
    data: {
      title: 'Quỹ',
      hidePageHeader: false
    }
  },
  // {
  //   path: 'customer',
  //   loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule),
  //   data: {
  //     title: 'Khách hàng',
  //     hidePageHeader: false
  //   }
  // },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AppsRoutingModule { }
