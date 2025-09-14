import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-transaction-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.css',
})
export class TransactionForm implements OnInit {
  transactionForm: FormGroup;

  incomeCategories = [
    'Salary',
    'Business',
    'Investment',
    'Gift',
    'Other'
  ];

  expenseCategories = [
    'Food',
    'Rent',
    'Transportation',
    'Utilities',
    'Entertainment',
    'Travel',
    'Health',
    'Other',
  ];

  availableCategories: string[] = [];

  constructor(private fb: FormBuilder) {
    this.transactionForm = this.fb.group({
      type: ['Expense', Validators.required],
      category: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      createdAt: [new Date(), Validators.required],
    });
  }

  ngOnInit(): void {
    const type = this.transactionForm.get('type')?.value;
    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({ category: ' ' });
  }

  cancel() {
    throw new Error('Method not implemented.');
  }

  onTypeChange() {
    ;
  }

  onSubmit() {
    throw new Error('Method not implemented.');
  }
}
