import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Signup } from './components/signup/signup';
import { TransactionList } from './components/transaction-list/transaction-list';
import { TransactionForm } from './components/transaction-form/transaction-form';
import {PageNotFound} from './components/page-not-found/page-not-found';
import {authGuard} from './guards/auth-guard';

export const routes: Routes = [
  // Route mapping the URL '/login' to Login component
  {
    path: 'login',
    component: Login,
  },
  // Route mapping the URL '/signup' to Signup component
  {
    path: 'signup',
    component: Signup,
  },
  // Route mapping the URL '/transactions' to TransactionList component
  {
    path: 'transactions',
    component: TransactionList,
    canActivate: [authGuard]
  },
  // Route mapping the URL '/add' to TransactionForm component
  {
    path: 'add',
    component: TransactionForm,
    canActivate: [authGuard]
  },
  // Route mapping the URL '/edit/:id' to TransactionForm component
  {
    path: 'edit/:id',
    component: TransactionForm,
    canActivate: [authGuard]
  },
  // Route redirecting the root URL '' to the '/transactions' route
  {
    path: '',
    redirectTo: '/transactions',
    pathMatch: 'full',
    //canActivate: [authGuard]
  },
  // Redirecting all unmatched URLs to the '404 Page'; Catching all to prevent errors
  {
    path: '**',
    component: PageNotFound
  },
];
