import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { SubscriptionPlanService } from '@app/shared/services/subscription-plan.service';
import { QueryResult, SubscriptionPlan } from '@app/shared/types/entity.interface';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-subscription-plan-selector',
  templateUrl: './subscription-plan-selector.component.html',
  styleUrls: ['./subscription-plan-selector.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SubscriptionPlanSelectorComponent),
    multi: true
  }]
})
export class SubscriptionPlanSelectorComponent implements OnInit, ControlValueAccessor {

  @Input() quantity = 10;
  @Input() isDisabled;
  @Input() hasNull = false;
  isLoading: boolean;
  selectedPlanId: string;
  plans$ = new BehaviorSubject([]);
  plansInput$ = new Subject<string>();
  private query: string;
  private onChange: Function;

  constructor(private subscriptionPlanService: SubscriptionPlanService) { }

  ngOnInit() {
    this.isLoading = true;
    this.initialize();
  }

  onOpen() {
    this.fetch(0, this.quantity).subscribe(items => {
      this.plans$.next(items);
      this.isLoading = false;
    });
  }

  writeValue(keyId) {
    this.selectedPlanId = keyId;
  }

  registerOnChange(fn) {
    this.onChange = fn;
  }

  registerOnTouched(fn) {
  }

  remove() {
    this.selectedPlanId = undefined;
    if (typeof this.onChange === 'function') {
      this.onChange(this.selectedPlanId);
    }
  }

  onKeyChange(key) {
    this.selectedPlanId = key;
    this.onChange(this.selectedPlanId);
  }

  fetchMore($event) {
    this.isLoading = true;
    const skip = this.plans$.value.length;
    const take = this.quantity;
    this.fetch(skip, take, this.query).subscribe(items => {
      const previousItems = this.plans$.value;
      this.plans$.next(previousItems.concat(items || []));
    });
  }

  private initialize() {
    this.plansInput$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(query => {
        this.isLoading = true;
        this.query = query;
      }),
      switchMap(query => this.fetch(0, this.quantity, query))
    ).subscribe(items => {
      this.plans$.next(items);
    });
    this.fetch(0, this.quantity).subscribe(items => {
      this.plans$.next(items);
      this.isLoading = false;
    });
  }

  private fetch = (skip: number, take: number, query?: string): Observable<SubscriptionPlan[]> => {
    return this.subscriptionPlanService.query({ skip, take, query}).pipe(
      catchError(() => of(null)),
      tap(() => {
        this.isLoading = false;
        this.query = query;
      }),
      map((result: QueryResult<SubscriptionPlan>) => result.items)
    );
  }

}
