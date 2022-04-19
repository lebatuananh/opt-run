import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';

@Component({
  selector: 'search-input',
  templateUrl: './search-input.component.html',
  styleUrls: ['./search-input.component.scss']
})
export class SearchInputComponent implements OnInit, AfterViewInit {
  @ViewChild('searchInput', {
    static: false
  }) searchInput: ElementRef;
  @Input() placeholder: string;
  @Output() changed = new EventEmitter()
  constructor() { }

  ngAfterViewInit(): void {
    this.searchInput && fromEvent(this.searchInput.nativeElement, 'keyup').pipe(
      map((event: any) => {
        return event.target.value;
      })
      , debounceTime(500)
      , distinctUntilChanged()
    ).subscribe((text: string) => {
      this.changed.emit(text)
    });
  }

  ngOnInit(): void {
  }

}
