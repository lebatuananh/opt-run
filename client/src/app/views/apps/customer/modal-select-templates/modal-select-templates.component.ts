import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { CustomerService } from '@app/shared/services/customer.service';
import { TemplateService } from '@app/shared/services/template.service';
import { DataTableContainer } from '@app/shared/types/data-table-container';
import { Template } from '@app/shared/types/entity.interface';
import { TemplateStatus, TemplateType } from '@app/shared/types/enum';
import { ColumnMode, SelectionType } from '@swimlane/ngx-datatable';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-modal-select-templates',
  templateUrl: './modal-select-templates.component.html',
  styleUrls: ['./modal-select-templates.component.scss']
})
export class ModalSelectTemplatesComponent extends DataTableContainer<Template> {
  @ViewChild('modalTemplate', { static: true }) modalTemplate;
  @Input() customerId: string;
  @Output() submitted = new EventEmitter();
  modal: BsModalRef;
  isSubmitting: boolean;
  selected = [];
  ColumnMode = ColumnMode;
  SelectionType = SelectionType;
  TemplateType = TemplateType;
  TemplateStatus = TemplateStatus;
  constructor(private templateService: TemplateService,
              private customerService: CustomerService,
              protected cd: ChangeDetectorRef,
              private modalService: BsModalService,
              private toast: ToastrService) {
    super(cd);
  }

  protected fetch() {
    return this.templateService.queryExclude(this.customerId, { skip: this.skip, take: this.take, query: this.query });
  }

  protected handleError(reason: any) {
    this.toast.error('Đã xảy ra lỗi');
  }

  show() {
    this.modal = this.modalService.show(this.modalTemplate, { backdrop: 'static', class: 'modal-lg' });
    this.selected = [];
    this.refresh();
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
  }

  submit() {
    this.isSubmitting = true;
    this.customerService.addTemplates(this.customerId, { templateIds: this.selected.map(template => template.id) }).subscribe(() => {
      this.toast.success('Thêm mẫu thành công');
      this.modal.hide();
      this.submitted.emit();
      this.cd.detectChanges();
    }, () => this.toast.error('Đã xảy ra lỗi')).add(() => this.isSubmitting = false);
  }

}
