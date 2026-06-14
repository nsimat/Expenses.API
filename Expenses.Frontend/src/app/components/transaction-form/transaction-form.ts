import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TransactionService } from '../../services/transaction-service';

@Component({
  selector: 'app-transaction-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './transaction-form.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './transaction-form.css',
})
export class TransactionForm implements OnInit {

  // Reactive form for transaction with validation rules
  transactionForm: FormGroup;

  // Income sources
  incomeCategories = [
    'Salary',
    'Business Profits',
    'Wages',
    'Commissions',
    'Investment',
    'Freelance',
    'Dividends',
    'Rental Income',
    'Interest',
    'Taxes Refunds',
    'Gift',
    'Other',
  ];

  // List of possible expenses
  expenseCategories = [
    'Housing',
    'Food/Grocery',
    'Car insurance',
    'Car payment',
    'Rent',
    'Transportation',
    'Utilities Bills',
    'Entertainment & Recreation',
    'Insurance',
    'Travel',
    'Personal Care',
    'Healthcare',
    'Pet Care',
    'Savings & Investments',
    'Gifts & Donations',
    'Debt payments',
    'Taxes',
    'Other',
  ];

  // Categories available based on the selected type (Income or Expense)
  availableCategories: string[] = [];

  // Flag to determine if the form is in edit mode
  isEditMode: boolean = false;

  // Store the transaction ID when in edit mode
  transactionId?: string;

  /**
   * Initializes a new instance of the class.
   *
   * @param {FormBuilder} fb - An instance of FormBuilder used to create and manage reactive forms.
   * @param {Router} router - An instance of Router used for navigation and route handling.
   * @param {ActivatedRoute} activatedRouter - An instance of ActivatedRoute used to access route-specific information.
   * @param {TransactionService} transactionService - A service for handling operations related to transactions.
   * @return {void}
   */
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private activatedRouter: ActivatedRoute,
    private transactionService: TransactionService
  ) {
    this.transactionForm = this.fb.group({
      type: ['Expense', Validators.required],
      category: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      createdAt: [new Date(), Validators.required]
    });
  }

  /**
   * A lifecycle hook that is called after Angular has initialized the component.
   * This method initializes the component by:
   * - Setting up the available categories based on the selected transaction type.
   * - Determining if the component is in edit mode by checking for an 'id' route parameter.
   * - Loading the transaction details if in edit mode.
   *
   * @return {void} Returns nothing.
   */
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
      this.transactionId = id; //+id
      this.loadTransaction(this.transactionId);
    }
  }

  /**
   * Loads a transaction based on the provided ID, updates the form with transaction details,
   * and ensures the createdAt field is disabled to prevent editing.
   *
   * @param {string} id - The unique identifier of the transaction to be loaded.
   * @return {void} No value is returned by this method.
   */
  private loadTransaction(id: string): void {

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
          createdAt: transaction.createdAt,
        });
        console.log('Patching form with: ', this.transactionForm.value);
        // Disable the createdAt field to prevent editing
        this.transactionForm.get('createdAt')?.disable();
        console.log('Loaded transaction for editing: ', transaction);
      },
      error: (error) => {
        console.error('Error fetching transaction: - ', error);
      },
    });
  }

  /**
   * Cancels the current operation and navigates the user to the transactions page.
   *
   * @return {void} Does not return a value.
   */
  cancel() {
    this.router.navigate(['/transactions']);
  }

  /**
   * Handles the change event for the transaction type field in the form.
   * Logs the new type value and updates the form based on the selected type.
   *
   * @return {void} This method does not return a value.
   */
  onTypeChange() {
    const type = this.transactionForm.get('type')?.value;
    console.log('Type changed to: ', type);
    this.updateFormType(type);
  }

  /**
   * Handles the submission of the transaction form.
   * This method determines whether a new transaction is being created or an existing transaction is being updated based on the mode and processes the operation accordingly.
   * If the form is invalid, the submission will not proceed.
   *
   * @return {void} Does not return any value.
   */
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

  /**
   * Updates the form type by changing the available categories and resetting the category field in the form.
   *
   * @param {string} type - The type of transaction, either 'Expense' or 'Income'.
   * @return {void} This method does not return a value.
   */
  private updateFormType(type: string): void {
    console.log('Changing type...');

    this.availableCategories = type === 'Expense' ? this.expenseCategories : this.incomeCategories;
    this.transactionForm.patchValue({ category: '' });
  }
}
