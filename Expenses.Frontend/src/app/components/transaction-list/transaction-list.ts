import { Component } from '@angular/core';
import { Itransaction } from '../../models/itransaction';
import { CommonModule} from '@angular/common';

@Component({
  selector: 'app-transaction-list',
  imports: [CommonModule],
  templateUrl: './transaction-list.html',
  styleUrl: './transaction-list.css'
})
export class TransactionList {

  transactions: Itransaction[] = [
    { id: 1, type: 'Expense', amount: 50, category: 'Food', createdAt: new Date(), updateAt: new Date() },
    { id: 2, type: 'Income', amount: 1000, category: 'Salary', createdAt: new Date(), updateAt: new Date() },
    { id: 3, type: 'Expense', amount: 20, category: 'Transport', createdAt: new Date(), updateAt: new Date() }
  ];

}
