import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {WebConfigComponent} from '@app/views/apps/web-config/web-config.component';

const routes: Routes = [{
  path: '',
  component: WebConfigComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WebConfigRoutingModule { }
