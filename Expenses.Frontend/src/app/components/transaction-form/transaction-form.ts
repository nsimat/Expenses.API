import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Itransaction } from '../../models/itransaction';
import { Router } from '@angular/router';

@Component({
  selector: 'app-transaction-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.css',
})
export class TransactionForm implements OnInit {
  transactionForm: FormGroup;
  newTransaction: Itransaction = {
    id: 0,
    type: 'Expense',
    category: '',
    amount: 0,
    createdAt: new Date(),
    updateAt: new Date()
  };


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

  constructor(private fb: FormBuilder, private router: Router) {
    this.transactionForm = this.fb.group({
      type: ['Expense', Validators.required],
      category: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      createdAt: [new Date(), Validators.required],
    });
  }

  ngOnInit(): void {
    this.changingType();
  }

  cancel() {
    this.router.navigate(['/transactions']);
  }

  onTypeChange() {
    this.changingType();
  }

  onSubmit() {
    throw new Error('Method not implemented.');
  }

  private changingType(): void {
    console.log('Changing type...');

    const type = this.transactionForm.get('type')?.value;
    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({ category: '' });
  }
}
