import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import {LocationStrategy, PathLocationStrategy, registerLocaleData} from '@angular/common';

import {AppRoutingModule} from './app-routing.module';
import {LayoutModule} from './layout/layout.module';
import {SharedModule} from './shared/shared.module';
import {NgxsModule} from '@ngxs/store';
import {NgxsReduxDevtoolsPluginModule} from '@ngxs/devtools-plugin';
import {NgxsLoggerPluginModule} from '@ngxs/logger-plugin';
import {AppConfigState} from './store/app-config/app-config.state';
import {TranslateModule} from '@ngx-translate/core';
import {AppComponent} from './app.component';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {LaddaModule} from 'angular2-ladda';
import {ToastrModule} from 'ngx-toastr';
import {NgBootstrapFormValidationModule} from 'ng-bootstrap-form-validation';
import localeVI from '@angular/common/locales/vi';
import {NgSelectModule} from '@ng-select/ng-select';
import {JwtInterceptor} from '@app/shared/interceptor/token.interceptor';

registerLocaleData(localeVI);

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    SharedModule,
    TranslateModule.forRoot(),
    LayoutModule,
    NgxsModule.forRoot([
      AppConfigState
    ]),
    LaddaModule.forRoot({
      style: 'expand-left',
      spinnerSize: 20,
    }),
    NgxsReduxDevtoolsPluginModule.forRoot(),
    NgxsLoggerPluginModule.forRoot(),
    ToastrModule.forRoot({
      timeOut: 5000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
      easing: 'flyInOut',
      closeButton: true,
    }),
    NgBootstrapFormValidationModule.forRoot(),
    NgSelectModule
  ],
  providers: [
    {
      provide: LocationStrategy,
      useClass: PathLocationStrategy
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
