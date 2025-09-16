import { Component, OnInit } from '@angular/core';
import { Itransaction } from '../../models/itransaction';
import { CommonModule } from '@angular/common';
import { Transaction } from '../../services/transaction';
import { Router } from '@angular/router';

@Component({
  selector: 'app-transaction-list',
  imports: [CommonModule],
  templateUrl: './transaction-list.html',
  styleUrl: './transaction-list.css',
})
export class TransactionList implements OnInit {
  transactions: Itransaction[] = [];

  constructor(private transactionService: Transaction, private router: Router) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    console.log('Loading transactions...');
    this.transactionService.getAllTransactions().subscribe((data) => {
      this.transactions = data;
    });
  }

  getTotalIncome(): number {
    return this.transactions
      .filter((t) => t.type === 'Income')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  getTotalExpenses(): number {
    return this.transactions
      .filter((t) => t.type === 'Expense')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  getNetBalance(): number {
    return this.getTotalIncome() - this.getTotalExpenses();
  }

  editTransaction(transaction: Itransaction): void {
    // Implement navigation to edit form with transaction details
    console.log('Editing transaction: ', transaction);

    if (transaction && transaction.id) {
      this.router.navigate(['/edit/', transaction.id]);
    }
  }

  deleteTransaction(transaction: Itransaction): void {
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
