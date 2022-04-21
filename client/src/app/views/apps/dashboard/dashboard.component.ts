import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {CurrentUser, Report, Result} from '@app/shared/types/entity.interface';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {AuthenticationService} from '@app/shared/services/authentication.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  report: Report;
  currentUser: CurrentUser;
  constructor(private router: Router,
              private route: ActivatedRoute,
              private toast: ToastrService,
              private authenticationService: AuthenticationService,
              private cdRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void{
    this.route.data.subscribe(({ data, user }: { data: Result<Report>, user: Result<CurrentUser> }) => {
      this.report = data.value.data;
      this.currentUser = user.value.data;
    });
  }

}
