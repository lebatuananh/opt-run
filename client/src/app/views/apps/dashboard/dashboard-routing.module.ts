import {RouterModule, Routes} from '@angular/router';
import {DashboardComponent} from '@app/views/apps/dashboard/dashboard.component';
import {NgModule} from '@angular/core';
import {DashboardResolve} from '@app/views/apps/dashboard/dashboard.resolve';

const routes: Routes = [{
  path: '',
  component: DashboardComponent,
  resolve: {data: DashboardResolve}
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }

