import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Transaction } from '../models/transaction';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  // Base URL for the transactions API, sourced from environment configuration
  private apiUrl = environment.apiTransactionsUrl;

  // Constructor with HttpClient injection for making HTTP requests to the backend API
  constructor(private http: HttpClient) {}

  // Retrieve all transactions
  /**
   * Retrieves all transactions for the user.
   *
   * @return {Observable<Transaction[]>} An observable emitting an array of transactions.
   */
  getAllTransactions(): Observable<Transaction[]>{
    return this.http.get<Transaction[]>(`${this.apiUrl}/all-user-transactions`);
  }

  // Retrieve a transaction by id
  /**
   * Retrieves the transaction details for a given transaction ID.
   *
   * @param {string} transactionId - The unique identifier of the transaction to be retrieved.
   * @return {Observable<Transaction>} An observable containing the transaction details.
   */
  getTransactionById(transactionId: string): Observable<Transaction>{
    return this.http.get<Transaction>(`${this.apiUrl}/`+ transactionId +`/details/`);
  }

  // Create a new transaction
  /**
   * Creates a new transaction by sending the provided transaction data to the server.
   *
   * @param {Transaction} transaction - The transaction data to be created.
   * @return {Observable<Transaction>} An observable that emits the created transaction object.
   */
  createTransaction(transaction: Transaction): Observable<Transaction>{
    return this.http.post<Transaction>(this.apiUrl + "/create", transaction);
  }

  // Update an existing transaction
  /**
   * Updates an existing transaction with new data.
   *
   * @param {string} transactionId - The unique identifier of the transaction to be updated.
   * @param {Transaction} transaction - The transaction object containing the updated data.
   * @return {Observable<Transaction>} An observable emitting the updated transaction.
   */
  updateTransaction(transactionId: string, transaction: Transaction): Observable<Transaction>{
    return this.http.put<Transaction>(this.apiUrl+ "/" + transactionId + "/update/", transaction);
  }

  // Delete a transaction by id
  /**
   * Deletes a transaction based on the provided transaction ID.
   *
   * @param {string} transactionId - The unique identifier of the transaction to be deleted.
   * @return {Observable<void>} An observable that emits when the delete operation is complete.
   */
  deleteTransaction(transactionId: string): Observable<void>{
    return this.http.delete<void>(this.apiUrl + "/" + transactionId + "/delete");
  }
}
