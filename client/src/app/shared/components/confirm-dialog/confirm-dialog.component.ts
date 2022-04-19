import { Component, EventEmitter, HostListener, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: '[confirm-dialog]',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent {

  @Input('confirm-dialog') confirmation: string
  @Input() size: 'small' | 'medium' | 'large' = 'medium'

  @Output() confirmed = new EventEmitter()
  @ViewChild('template') template: TemplateRef<any>

  modalRef: BsModalRef

  constructor(
    private modalService: BsModalService,
  ) { }

  hide() {
    this.modalRef.hide()
  }

  confirm() {
    this.confirmed.emit()
    this.hide()
  }

  private getSizeClassName() {
    switch (this.size) {
      case 'small':
        return 'modal-sm'
      case 'large':
        return 'modal-lg'
      case 'medium':
      default:
        return 'modal-md'
    }
  }

  @HostListener('click')
  onclick() {
    const initialState = {
      title: 'Xác nhận',
      text: this.confirmation,
    }
    this.modalRef = this.modalService.show(this.template, {
      initialState,
      class: this.getSizeClassName(),
    })
  }

}
