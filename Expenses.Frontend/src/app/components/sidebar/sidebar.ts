import {Component, inject} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faChartLine,
  faComment, faEye, faGear,
  faHome, faMagnifyingGlassChart, faMoneyBill1Wave,
  faUser
} from '@fortawesome/free-solid-svg-icons';
import {RouterLink} from '@angular/router';
import {AuthService} from '../../services/auth-service';

@Component({
  selector: 'app-sidebar',
  imports: [FontAwesomeModule, RouterLink],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {
  home = faHome;
  chart = faChartLine;
  message = faComment;
  overview = faMagnifyingGlassChart;
 settings = faGear;
  user = faUser;
  transactionsLog = faEye;
  transaction = faMoneyBill1Wave;


  title?: string = 'Dashboard';

  getTitle(): string {
    return this.title || 'Dashboard';
  }

  setTitle(newTitle: string): void {
    this.title = newTitle;
  }

  updateTitle(item: string): void {
    this.setTitle(item);
  }

}
