import {AfterContentChecked, Directive} from '@angular/core';
import {DatatableComponent} from '@swimlane/ngx-datatable';

@Directive({
  selector: '[appTableResizer]'
})
export class TableResizerDirective implements AfterContentChecked {

  constructor(private table: DatatableComponent) {}

  ngAfterContentChecked(): void {
    const timerId = setInterval(() => {
      if (this.table && this.table.element.clientWidth !== this.table._innerWidth) {
        window.dispatchEvent(new Event('resize'));
      } else {
        clearInterval(timerId);
      }
    }, 100);
  }
}
