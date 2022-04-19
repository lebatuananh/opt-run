import {NgModule} from '@angular/core';
import {RouterModule, Routes, PreloadAllModules} from '@angular/router';
import {AppLayoutComponent} from './layout/app-layout/app-layout-component';

import {APP_LAYOUT_ROUTES} from './routes/app-layout.routes';
import {AuthenticationGuard} from './shared/services/authentication.guard';
import {AuthLayoutComponent} from '@app/layout/auth-layout/auth-layout.component';
import {AUTH_LAYOUT_ROUTES} from '@app/routes/auth-layout.routes';

const appRoutes: Routes = [
  {
    path: '',
    component: AppLayoutComponent,
    children: APP_LAYOUT_ROUTES,
    canActivate: [
      AuthenticationGuard
    ]
  },
  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: AUTH_LAYOUT_ROUTES
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes, {
      preloadingStrategy: PreloadAllModules,
      anchorScrolling: 'enabled',
      scrollPositionRestoration: 'enabled',
      relativeLinkResolution: 'legacy'
    })
  ],
  exports: [
    RouterModule
  ]
})

export class AppRoutingModule {
}
