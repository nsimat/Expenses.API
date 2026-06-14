import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Transaction } from '../../models/transaction';
import { CommonModule } from '@angular/common';
import { TransactionService } from '../../services/transaction-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-transaction-list',
  imports: [CommonModule],
  templateUrl: './transaction-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './transaction-list.css',
})
export class TransactionList implements OnInit {

  // Array to hold the list of transactions
  transactions: Transaction[] = [];

  // Injecting TransactionService and Router into the component
  constructor(private transactionService: TransactionService, private router: Router) {}

  /**
   * Lifecycle hook that is called after the component's data-bound properties are initialized.
   * Typically used for component initialization tasks.
   *
   * @return {void} No return value.
   */
  ngOnInit(): void {
    this.loadTransactions();
  }

  /**
   * Fetches and loads all transactions by invoking the transaction service.
   * Updates the internal transactions storage with the fetched data.
   *
   * @return {void} Does not return a value.
   */
  loadTransactions(): void {
    console.log('Loading transactions...');
    this.transactionService.getAllTransactions().subscribe((data) => {
      this.transactions = data;
    });
  }

  /**
   * Calculates and returns the total income by summing up all transaction amounts
   * that are categorized as 'Income'.
   *
   * @return {number} The total income amount.
   */
  getTotalIncome(): number {
    return this.transactions
      .filter((t) => t.type === 'Income')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  /**
   * Calculates the total amount of expenses from the list of transactions.
   * Filters the transactions to include only those with a type of 'Expense'
   * and then sums up their amounts.
   *
   * @return {number} The total amount of expenses.
   */
  getTotalExpenses(): number {
    return this.transactions
      .filter((t) => t.type === 'Expense')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  /**
   * Calculates and returns the net balance by subtracting total expenses from total income.
   *
   * @return {number} The net balance, which is the result of total income minus total expenses.
   */
  getNetBalance(): number {
    return this.getTotalIncome() - this.getTotalExpenses();
  }

  /**
   * Navigates to the edit form pre-filled with the details of the provided transaction.
   *
   * @param {Transaction} transaction - The transaction object containing the data to be edited. It must include a valid `id` property.
   * @return {void} This method does not return any value.
   */
  editTransaction(transaction: Transaction): void {
    // Implement navigation to edit form with transaction details
    console.log('Editing transaction: ', transaction);

    if (transaction && transaction.id) {
      this.router.navigate(['/edit/', transaction.id]);
    }
  }

  /**
   * Deletes a given transaction after user confirmation.
   *
   * @param {Transaction} transaction - The transaction object to be deleted.
   * @return {void} Does not return a value.
   */
  deleteTransaction(transaction: Transaction): void {
    if (confirm('Are you sure you want to delete this transaction?')) {
      this.transactionService.deleteTransaction(transaction.id).subscribe({
        next: () => {
          console.log('Transaction deleted with id: ', transaction.id);

          // Refresh the list after deletion
          this.loadTransactions();

          // Navigate back to transactions list
          //this.router.navigate(['/transactions']);
        },
        error: (error) => {
          console.error('Error deleting transaction: ', error);
        },
      });
    }
  }
}
