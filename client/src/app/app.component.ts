import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService, LangChangeEvent } from '@ngx-translate/core';
import { Select } from '@ngxs/store';
import { AppConfig } from '@app/shared/types/app-config.interface';
import { Observable, Subscription } from 'rxjs';
import { en_US } from './i18n/en/index';
import { vi_VN } from './i18n/vi/index';

const storageKey = 'lang';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {
    @Select((state: { app: AppConfig; }) => state.app) app$: Observable<AppConfig>;
    private langChangeSubscription!: Subscription;
    currentLang: string;

    constructor(private translateService: TranslateService) {
        translateService.setTranslation('en_US', en_US);
        translateService.setTranslation('vi_VN', vi_VN);
    }

    ngOnInit() {
        this.app$.subscribe(app => {
            this.currentLang = localStorage.getItem(storageKey) || app.lang || this.translateService.getBrowserCultureLang();
            this.translateService.use(this.currentLang);
        });
        this.langChangeSubscription = this.translateService.onLangChange.subscribe((event: LangChangeEvent) => {
            localStorage.setItem(storageKey, event.lang);
        });
    }

    ngOnDestroy() {
        if (this.langChangeSubscription) {
            this.langChangeSubscription.unsubscribe();
        }
    }

}
