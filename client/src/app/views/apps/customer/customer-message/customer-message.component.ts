import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from '@app/shared/services/message.service';
import { DataTableContainer } from '@app/shared/types/data-table-container';
import { Message } from '@app/shared/types/entity.interface';
import { MessageStatus, MessageType } from '@app/shared/types/enum';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-message',
  templateUrl: './customer-message.component.html',
  styleUrls: ['./customer-message.component.scss']
})
export class CustomerMessageComponent extends DataTableContainer<Message> {
  MessageType = MessageType
  MessageStatus = MessageStatus
  constructor(private messageService: MessageService,
    private toastr: ToastrService,
    protected cd: ChangeDetectorRef,
    private route: ActivatedRoute) {
    super(cd);
  }

  protected fetch() {
    const customerId = this.route.snapshot.params.id
    return this.messageService.query(customerId, { skip: this.skip, take: this.take, query: this.query })
  }

  protected handleError(reason: any) {
    this.toastr.error('Đã xảy ra lỗi')
  }
}
