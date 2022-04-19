import { ChangeDetectorRef, Injectable, OnInit, ViewChild } from '@angular/core';
import { DatatableComponent } from '@swimlane/ngx-datatable';
import { Observable } from 'rxjs';
import {QueryResult, Result, ResultModel} from './entity.interface';

@Injectable()
export abstract class DataTableContainer<T> implements OnInit {
  @ViewChild(DatatableComponent) table: DatatableComponent;
  skip = 0;
  query: string;
  count: number;
  items: T[];
  isLoading = false;
  messages = {
    emptyMessage: 'Không tìm thấy bản ghi nào',
    totalMessage: 'bản ghi',

  };

  constructor(
    protected cd: ChangeDetectorRef,
    public take: number = 10,
  ) {
  }

  protected abstract fetch(): Observable<Result<QueryResult<T>>>;

  ngOnInit(): void {
    this.subscribe();
  }

  protected subscribe(): void {
    this.isLoading = true;
    const next = result => {
      this.isLoading = false;
      this.handleResult(result);
      this.cd.detectChanges();
    };

    const error = reason => {
      this.isLoading = false;
      this.handleError(reason);
      this.cd.detectChanges();
    };

    this.fetch().subscribe(next, error);
  }

  protected handleResult(result: Result<QueryResult<T>>): void {
    this.count = result.value.data.count;
    this.items = result.value.data.items;
  }


  onPage(event): void {
    this.skip = event.offset * this.take;
    this.subscribe();
  }

  refresh(): void {
    this.skip = 0;
    this.subscribe();
  }

  search(query): void {
    this.query = query;
    this.refresh();
  }

  protected abstract handleError(reason: any);

}
