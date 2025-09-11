import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Itransaction } from '../models/itransaction';

@Injectable({
  providedIn: 'root'
})
export class Transaction {

  private apiUrl = 'https://localhost:7063/api/Transactions';

  constructor(private http: HttpClient) {}

  getAllTransactions(): Observable<Itransaction[]>{
    return this.http.get<Itransaction[]>(`${this.apiUrl}/All`);
  }

  getTransactionById(id: number): Observable<Itransaction>{
    return this.http.get<Itransaction>(`${this.apiUrl}/Details/` + id);
  }

  createTransaction(transaction: Itransaction): Observable<Itransaction>{
    return this.http.post<Itransaction>(this.apiUrl + "/Create", transaction);
  }

  updateTransaction(id: number, transaction: Itransaction): Observable<Itransaction>{
    return this.http.put<Itransaction>(this.apiUrl + "/Update/" + id, transaction);
  }

  deleteTransaction(id: number): Observable<void>{
    return this.http.delete<void>(this.apiUrl + "/Delete/" + id);
  }

}
