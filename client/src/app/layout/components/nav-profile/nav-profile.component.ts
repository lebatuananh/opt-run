import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '@app/shared/services/authentication.service';
import {User} from '@app/shared/types/model';

@Component({
  selector: 'nav-profile',
  templateUrl: './nav-profile.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.header-nav-item]': 'true'
  }
})
export class NavProfileComponent implements OnInit {
  constructor(private authenticationService: AuthenticationService, private router: Router) { }
  user: User;
  profileMenuList = [
    {
      path: '',
      icon: 'feather icon-user',
      item: 'Profile'
    },
    {
      path: '',
      icon: 'feather icon-power',
      item: 'Sign Out',
      onclick: () => this.logout()
    }
  ];

  logout(): void {
    this.router.navigate(['/auth/login']);
    this.authenticationService.logout();
  }


  ngOnInit(): void {
    this.user = this.authenticationService.getUserInfo();
    console.log(this.authenticationService.getUserInfo());
  }
}
