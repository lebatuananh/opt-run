import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {Report, Result} from '@app/shared/types/entity.interface';
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
  constructor(private router: Router, private route: ActivatedRoute, private toast: ToastrService,
              private authenticationService: AuthenticationService,
              private cdRef: ChangeDetectorRef) { }

  ngOnInit(): void {
   this.authenticationService.report().subscribe(data => {
     this.report = data.value.data;
   }, () => {
     this.toast.error('Đã xảy ra lỗi');
   });
  }

}
