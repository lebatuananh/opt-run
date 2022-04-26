import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {Report, UserDto} from '@app/shared/types/entity.interface';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {BsModalRef} from 'ngx-bootstrap/modal';
import {TranslateService} from '@ngx-translate/core';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-report-user-info',
  templateUrl: './report-user-info.component.html',
  styleUrls: ['./report-user-info.component.scss']
})
export class ReportUserInfoComponent implements OnInit {
  @Output() submitted = new EventEmitter();
  userDto: UserDto;
  reportUser: Report;
  isLoading: boolean;
  constructor(
    private authenticationService: AuthenticationService,
    private fb: FormBuilder,
    private modalRef: BsModalRef,
    private translate: TranslateService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void{
    this.authenticationService.reportByUserId({userId: this.userDto.id}).subscribe(response => {
      this.isLoading = true;
      this.reportUser = response.value.data;
    }, () => {
      this.isLoading = false;
      this.toastr.error('Đã xảy ra lỗi');
    });
  }

  hide(): void  {
    this.modalRef.hide();
  }

}
