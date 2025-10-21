import {Component, inject, OnInit, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './components/header/header';
import { Footer } from './components/footer/footer';
import { TransactionList } from './components/transaction-list/transaction-list';
import {Sidebar} from './components/sidebar/sidebar';
import {AuthService} from './services/auth-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer, Sidebar],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit{
  protected readonly title = signal('Expenses.Frontend');
  private readonly authService = inject(AuthService);

  ngOnInit() {
    // Initialize authentication status on app startup
    this.authService.initializeAuthStatus();
  }
}
