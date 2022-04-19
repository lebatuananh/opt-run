import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => EditorComponent),
    multi: true
  }]
})
export class EditorComponent implements OnInit, ControlValueAccessor {
  editor: any
  content: string
  @Input() height: number = 200
  private onChange: Function

  constructor() { }

  writeValue(content: string): void {
    this.content = content;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void { }

  setDisabledState?(isDisabled: boolean): void { }

  ngOnInit(): void {
  }

  onCreate(quill) {
    this.editor = quill.editor;
  }

  onContentChange(quill) {
    this.content = quill.html
    if (typeof this.onChange === 'function') {
      this.onChange(this.content)
    }
  }

}
