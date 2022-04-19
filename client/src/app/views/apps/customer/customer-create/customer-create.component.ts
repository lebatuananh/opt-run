import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {CustomerService} from '@app/shared/services/customer.service';
import {ToastrService} from 'ngx-toastr';
import {CustomerCoordinatorService} from "@app/views/apps/customer/customer-coordinator.service";

@Component({
  selector: 'app-customer-create',
  templateUrl: './customer-create.component.html',
  styleUrls: ['./customer-create.component.scss']
})
export class CustomerCreateComponent implements OnInit {
  form: FormGroup;
  isSubmit = false;
  constructor(private customerService: CustomerService,
              private fb: FormBuilder,
              private toastr: ToastrService,
              private router: Router,
              private customerCoordinatorService: CustomerCoordinatorService,
              private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.buildForm();
  }

  private buildForm() {
    this.form = this.fb.group({
      name: [, Validators.required],
      userName: [, Validators.required],
      status: [true, ],
      deviceId: ['', ]
    });
  }

  onSubmit() {
    const value = this.form.value;
    this.isSubmit = true;
    const next = () => {
      this.isSubmit = false;
      // this.router.navigate(['..']);
      this.customerCoordinatorService.next();
      // this.cdr.detectChanges();
    };

    const error = err => {
      this.isSubmit = false;
      this.customerCoordinatorService.next();
      // this.cdr.detectChanges();
    };

    this.customerService.create(Object.assign({}, value, { status: +value.status })).subscribe(next, error);
  }
}
