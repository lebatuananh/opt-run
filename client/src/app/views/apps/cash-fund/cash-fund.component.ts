import {ChangeDetectorRef, Component} from '@angular/core';
import {DataTableContainer} from '@app/shared/types/data-table-container';
import {OrderHistory, QueryResult, Result} from '@app/shared/types/entity.interface';
import {CashFundService} from '@app/shared/services/cash-fund.service';
import {BsModalService} from 'ngx-bootstrap/modal';
import {ToastrService} from 'ngx-toastr';
import {Observable} from 'rxjs';
import {
  UpdateCashFundModalComponent
} from '@app/views/apps/cash-fund/update-cash-fund-modal/update-cash-fund-modal.component';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-cash-fund',
  templateUrl: './cash-fund.component.html',
  styleUrls: ['./cash-fund.component.scss']
})
export class CashFundComponent extends DataTableContainer<OrderHistory> {

  isLoading: boolean;

  constructor(private cashFundService: CashFundService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private translate: TranslateService,
  ) {
    super(cd);
  }

  protected fetch(): Observable<Result<QueryResult<OrderHistory>>> {
    return this.cashFundService.query({ skip: this.skip, take: this.take, query: this.query });
  }

  protected handleError(reason: any): void {
    this.toast.error('Đã xảy ra lỗi');
  }


  onAdd(): void{
    this.isLoading = true;
    const next = result => {
      console.log(result);
      this.toast.success(this.translate.instant('Tạo mới yêu cầu thành công'));
      this.refresh();
    };

    const error = err => {
      const data = err.error;
      this.toast.error('Đã xảy ra lỗi, không tạo được yêu cầu');
    };

    this.cashFundService.create().subscribe(next, error).add(() => this.isLoading = false);
  }

  edit(id: string): void{
    const cashFund = this.items.find(item => item.id === id);
    const ref = this.modalService.show(UpdateCashFundModalComponent, {
      backdrop: true,
      ignoreBackdropClick: true,
      initialState: {
        cashFund
      }
    });
    ref.content.submitted.subscribe(() => {
      this.refresh();
    });
  }
}
