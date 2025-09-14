import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Itransaction } from '../../models/itransaction';
import { Router } from '@angular/router';
import { Transaction } from '../../services/transaction';

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
    'Freelance',
    'Rental Income',
    'Interest',
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

  constructor(private fb: FormBuilder, private router: Router, private transactionService: Transaction) {
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
    // Handle form submission logic here
    if (this.transactionForm.valid) {
       const newTransaction = this.transactionForm.value;

       console.log('Form Submitted!', newTransaction);
        this.transactionService.createTransaction(newTransaction).subscribe({ next: (transaction) => {
          console.log('Transaction created successfully: ', transaction);
        }, error: (error) => {
          console.error('Error creating transaction: ', error);
        }});
      // You can also reset the form here if needed
      this.transactionForm.reset({ type: 'Expense', createdAt: new Date() });
      this.changingType();
      this.router.navigate(['/transactions']);
    }
  }

  private changingType(): void {
    console.log('Changing type...');

    const type = this.transactionForm.get('type')?.value;
    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({ category: '' });
  }
}
