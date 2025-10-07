import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Itransaction } from '../models/itransaction';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class Transaction {

  //private apiUrl = 'https://localhost:7063/api/Transactions';
  // Alternatively, use environment variables if set up
  private apiUrl = environment.apiTransactionsUrl;

  constructor(private http: HttpClient) {}

  // Retrieve all transactions
  getAllTransactions(): Observable<Itransaction[]>{
    return this.http.get<Itransaction[]>(`${this.apiUrl}/All`);
  }

  // Retrieve a transactions by id
  getTransactionById(id: number): Observable<Itransaction>{
    return this.http.get<Itransaction>(`${this.apiUrl}/Details/` + id);
  }

  // Create a new transaction
  createTransaction(transaction: Itransaction): Observable<Itransaction>{
    return this.http.post<Itransaction>(this.apiUrl + "/Create", transaction);
  }

  // Update an existing transaction
  updateTransaction(id: number, transaction: Itransaction): Observable<Itransaction>{
    return this.http.put<Itransaction>(this.apiUrl + "/Update/" + id, transaction);
  }

  // Delete a transaction by id
  deleteTransaction(id: number): Observable<void>{
    return this.http.delete<void>(this.apiUrl + "/Delete/" + id);
  }
}
