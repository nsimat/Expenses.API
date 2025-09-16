import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Itransaction } from '../../models/itransaction';
import { ActivatedRoute, Router } from '@angular/router';
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
    'Other',
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

  isEditMode: boolean = false;

  transactionId?: number;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private activatedRouter: ActivatedRoute,
    private transactionService: Transaction
  ) {
    this.transactionForm = this.fb.group({
      type: ['Expense', Validators.required],
      category: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      createdAt: [new Date(), Validators.required],
    });
  }

  ngOnInit(): void {
    // Initialize available categories based on default type
    const type = this.transactionForm.get('type')?.value;
    console.log('Initial type: ', type);
    this.updateFormType(type);

    // Check if we're in edit mode by looking for an 'id' parameter
    const id = this.activatedRouter.snapshot.paramMap.get('id');

    if (id) {
      console.log('Edit mode for transaction with id: ', id);
      this.isEditMode = true;
      this.transactionId = +id;
      this.loadTransaction(this.transactionId);
    }
  }

  private loadTransaction(id: number): void {

    this.transactionService.getTransactionById(id).subscribe({
      next: (transaction) => {
        // Ensure categories are updated based on type
        console.log('Transaction to edit: ', transaction);
        this.updateFormType(transaction.type);

        // Patch form values
        this.transactionForm.patchValue({
          type: transaction.type,
          category: transaction.category,
          amount: transaction.amount,
          createdAt: new Date(transaction.createdAt),
        });
        console.log('Patching form with: ', this.transactionForm.value);
        // Disable the createdAt field to prevent editing
        this.transactionForm.get('createdAt')?.disable();
        console.log('Loaded transaction for editing: ', transaction);
      },
      error: (error) => {
        console.error('Error fetching transaction: ', error);
      },
    });
  }

  cancel() {
    this.router.navigate(['/transactions']);
  }

  onTypeChange() {
    const type = this.transactionForm.get('type')?.value;
    console.log('Type changed to: ', type);
    this.updateFormType(type);
  }

  onSubmit() {
    console.log('Submitting form...', this.transactionForm.value);

    // Handle form submission logic here
    if (this.transactionForm.valid) {
      const newTransaction = this.transactionForm.value;

      if (this.isEditMode && this.transactionId) {
        console.log('Form Submitted in Update Mode!', newTransaction);
        this.transactionService.updateTransaction(this.transactionId, newTransaction).subscribe({
          next: (transaction) => {
            console.log('Transaction updated successfully: ', transaction);
            // Redirect to transactions list
            this.router.navigate(['/transactions']);
          },
          error: (error) => {
            console.error('Error updating transaction: ', error);
          },
        });
      } else {
        console.log('Form Submitted in Creation Mode!', newTransaction);
        this.transactionService.createTransaction(newTransaction).subscribe({
          next: (transaction) => {
            console.log('Transaction created successfully: ', transaction);
            // Redirect to transactions list
            this.router.navigate(['/transactions']);
          },
          error: (error) => {
            console.error('Error creating transaction: ', error);
          },
        });
      }
    }
  }

  private updateFormType(type: string): void {
    console.log('Changing type...');

    //const type = this.transactionForm.get('type')?.value;
    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({ category: '' });
  }
}
