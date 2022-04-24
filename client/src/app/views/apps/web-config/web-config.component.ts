import {ChangeDetectorRef, Component} from '@angular/core';
import {DataTableContainer} from '@app/shared/types/data-table-container';
import {QueryResult, Result, UserDto, WebConfiguration} from '@app/shared/types/entity.interface';
import {AuthenticationService} from '@app/shared/services/authentication.service';
import {BsModalService} from 'ngx-bootstrap/modal';
import {ToastrService} from 'ngx-toastr';
import {TranslateService} from '@ngx-translate/core';
import {WebConfigurationService} from '@app/shared/services/web-configuration.service';
import {Observable} from 'rxjs';

@Component({
  selector: 'app-web-config',
  templateUrl: './web-config.component.html',
  styleUrls: ['./web-config.component.scss']
})
export class WebConfigComponent extends DataTableContainer<WebConfiguration> {

  constructor(private webConfigurationService: WebConfigurationService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private translate: TranslateService, ) {
    super(cd);
  }

  protected fetch(): Observable<Result<QueryResult<WebConfiguration>>> {
    return this.webConfigurationService.query({skip: this.skip, take: this.take, query: this.query});
  }

  protected handleError(reason: any): void {
    this.toast.error('Đã xảy ra lỗi');
  }

  public selected(value): void {
    this.webConfigurationService.changeSelected({webId: value}).subscribe(response => {
          this.refresh();
    }, () => this.toast.error('Đã xảy ra lỗi'));
  }
}
