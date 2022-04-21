import {ChangeDetectorRef, Component} from '@angular/core';
import {DataTableContainer} from '@app/shared/types/data-table-container';
import {BsModalService} from 'ngx-bootstrap/modal';
import {ToastrService} from 'ngx-toastr';
import {TranslateService} from '@ngx-translate/core';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {QueryResult, Result, UserDto} from '@app/shared/types/entity.interface';
import {Observable} from 'rxjs';
import {RechargeUserModalComponent} from '@app/views/apps/user/recharge-user-modal/recharge-user-modal.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent extends DataTableContainer<UserDto> {

  constructor(private authenticationService: AuthenticationService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private translate: TranslateService, ) {
    super(cd);
  }

  protected fetch(): Observable<Result<QueryResult<UserDto>>> {
    return this.authenticationService.query({ skip: this.skip, take: this.take, query: this.query });
  }

  protected handleError(reason: any): void {
    this.toast.error('Đã xảy ra lỗi');
  }

  recharge(id: string): void {
    const user = this.items.find(item => item.id === id);
    const ref = this.modalService.show(RechargeUserModalComponent, {
      backdrop: true,
      ignoreBackdropClick: true,
      initialState: {
        userDto : user
      }
    });
    ref.content.submitted.subscribe((more) => {
      this.refresh();
    });
  }

}
