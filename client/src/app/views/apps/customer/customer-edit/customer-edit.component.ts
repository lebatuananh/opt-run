import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '@app/shared/services/customer.service';
import { ToastrService } from 'ngx-toastr';
import { CustomerCoordinatorService } from '../customer-coordinator.service';

@Component({
  selector: 'app-customer-edit',
  templateUrl: './customer-edit.component.html',
  styleUrls: ['./customer-edit.component.scss']
})
export class CustomerEditComponent implements OnInit {
  form: FormGroup;
  isSubmit = false;
  constructor(private router: Router, private route: ActivatedRoute, private toast: ToastrService,
              private customerService: CustomerService,
              private customerCoordinatorService: CustomerCoordinatorService,
              private fb: FormBuilder,
              private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.buildForm();
    this.route.data.subscribe(data => {
      this.form.patchValue(data.customer);
    });
  }

  private buildForm() {
    this.form = this.fb.group({
      name: [, Validators.required],
    });
  }

  onSubmit() {
    const value = this.form.value;
    this.isSubmit = true;
    const next = () => {
      this.isSubmit = false;
      this.router.navigate(['..'], { relativeTo: this.route });
      this.toast.success('Cập nhật thông tin khách hàng thành công');
      this.customerCoordinatorService.next();
    };

    const error = err => {
      this.isSubmit = false;
    };

    this.customerService.edit(this.route.snapshot.params.id, value).subscribe(next, error);
  }
}
