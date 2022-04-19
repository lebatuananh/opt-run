import { EventEmitter, Injectable } from '@angular/core';

@Injectable()
export class CustomerCoordinatorService {
  changed = new EventEmitter(false);

  constructor() {
  }

  next() {
    this.changed.emit(true);
  }
}
