import {ChangeDetectorRef, Component} from '@angular/core';
import {DataTableContainer} from '@app/shared/types/data-table-container';
import {OrderHistory, QueryResult, Result, Transaction} from '@app/shared/types/entity.interface';
import {BsModalService} from 'ngx-bootstrap/modal';
import {ToastrService} from 'ngx-toastr';
import {TranslateService} from '@ngx-translate/core';
import {TransactionService} from '@app/shared/services/transaction.service';
import {Observable} from 'rxjs';

@Component({
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent extends DataTableContainer<Transaction> {

  constructor(private transactionService: TransactionService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService,
              private translate: TranslateService,
  ) {
    super(cd);
  }

  protected fetch(): Observable<Result<QueryResult<Transaction>>> {
    return this.transactionService.query({ skip: this.skip, take: this.take, query: this.query });
  }

  protected handleError(reason: any): void {
    this.toast.error('Đã xảy ra lỗi');
  }


}
