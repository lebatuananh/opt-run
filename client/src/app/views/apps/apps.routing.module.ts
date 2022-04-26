import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard'
  },
  {
    path: 'request',
    loadChildren: () => import('./cash-fund/cash-fund.module').then(m => m.CashFundModule),
    data: {
      title: 'Yêu cầu',
      hidePageHeader: false
    }
  },
  {
    path: 'transaction',
    loadChildren: () => import('./transaction/transaction.module').then(m => m.TransactionModule),
    data: {
      title: 'Giao dịch',
      hidePageHeader: false
    }
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule),
    data: {
      title: 'Trang chủ',
      hidePageHeader: true
    }
  },
  {
    path: 'customer',
    loadChildren: () => import('./user/user.module').then(m => m.UserModule),
    data: {
      title: 'Khách hàng',
      hidePageHeader: false
    }
  },
  {
    path: 'web-config',
    loadChildren: () => import('./web-config/web-config.module').then(m => m.WebConfigModule),
    data: {
      title: 'Cấu hình',
      hidePageHeader: false
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AppsRoutingModule { }
