import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { SearchInputComponent } from './components/search-input/search-input.component';
import { EditorComponent } from './components/editor/editor.component';
import { QuillModule } from 'ngx-quill';
import { TableResizerDirective } from './directive/table-resizer.directive';

@NgModule({
  imports: [
    QuillModule.forRoot(),
    ReactiveFormsModule,
    FormsModule
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    TranslateModule,
    ConfirmDialogComponent,
    SearchInputComponent,
    EditorComponent,
    TableResizerDirective
  ],
  declarations: [
    ConfirmDialogComponent,
    SearchInputComponent,
    EditorComponent,
    TableResizerDirective
  ]
})

export class SharedModule { }
