import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Transaction } from '../models/transaction';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  //private apiUrl = 'https://localhost:7063/api/Transactions';
  // Alternatively, use environment variables if set up
  private apiUrl = environment.apiTransactionsUrl;

  constructor(private http: HttpClient) {}

  // Retrieve all transactions
  getAllTransactions(): Observable<Transaction[]>{
    return this.http.get<Transaction[]>(`${this.apiUrl}/all`);
  }

  // Retrieve a transactions by id
  getTransactionById(id: number): Observable<Transaction>{
    return this.http.get<Transaction>(`${this.apiUrl}/details/` + id);
  }

  // Create a new transaction
  createTransaction(transaction: Transaction): Observable<Transaction>{
    return this.http.post<Transaction>(this.apiUrl + "/create", transaction);
  }

  // Update an existing transaction
  updateTransaction(id: number, transaction: Transaction): Observable<Transaction>{
    return this.http.put<Transaction>(this.apiUrl + "/update/" + id, transaction);
  }

  // Delete a transaction by id
  deleteTransaction(id: number): Observable<void>{
    return this.http.delete<void>(this.apiUrl + "/delete/" + id);
  }
}
