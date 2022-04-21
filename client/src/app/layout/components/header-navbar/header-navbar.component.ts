import { Component, OnInit, Input } from '@angular/core';
import { NavMenu } from '@app/shared/types/nav-menu.interface';
import { navConfiguration } from '@app/configs/nav.config';
import { NavMenuColor } from '@app/shared/types/app-config.interface';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {User} from '@app/shared/types/model';

@Component({
    selector: 'header-navbar',
    templateUrl: './header-navbar.component.html',
    host: {
        '[class.header-navbar]': 'true',
        '[class.nav-menu-light]': 'color === \'light\'',
        '[class.nav-menu-dark]': 'color === \'dark\''
    }
})
export class HeaderNavbarComponent implements OnInit {

    menu: NavMenu[] = [];
    user: User;
    @Input() color: NavMenuColor = 'light';

    constructor(private authenticationServices: AuthenticationService) {
    }

    ngOnInit(): void {
        this.menu = navConfiguration;
        this.user = this.authenticationServices.getUserInfo();
    }
}
