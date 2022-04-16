import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerCreateComponent } from './customer-create/customer-create.component';
import { CustomerDepositComponent } from './customer-deposit/customer-deposit.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerMessageComponent } from './customer-message/customer-message.component';
import { CustomerViewComponent } from './customer-view/customer-view.component';
import { CustomerResolver } from './customer-view/customer.resolver';
import { CustomerComponent } from './customer.component';

const routes: Routes = [
  {
    path: '',
    component: CustomerComponent,
    children: [
      {
        path: 'create',
        component: CustomerCreateComponent,
        data: {
          title: 'Thêm mới khách hàng',
          hidePageHeader: false
        }
      },
      {
        path: ':id',
        component: CustomerViewComponent,
        data: {
          title: 'Khách hàng',
          hidePageHeader: false
        },
        resolve: {
          customer: CustomerResolver
        }
      },
      {
        path: ':id/edit',
        component: CustomerEditComponent,
        data: {
          title: 'Sửa thông tin',
          hidePageHeader: false
        },
        resolve: {
          customer: CustomerResolver
        }
      },
      {
        path: ':id/deposit',
        component: CustomerDepositComponent,
        data: {
          title: 'Nạp tiền',
          hidePageHeader: false
        },
        resolve: {
          customer: CustomerResolver
        }
      },
      {
        path: ':id/messages',
        component: CustomerMessageComponent,
        data: {
          title: 'Lịch sử gửi tin',
          hidePageHeader: false
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerRoutingModule { }
