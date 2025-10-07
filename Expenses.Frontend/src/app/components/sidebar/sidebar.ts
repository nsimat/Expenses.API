import { Component } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faBookmark,
  faChartBar,
  faComment,
  faHome,
  faMoneyBillTransfer,
  faUser
} from '@fortawesome/free-solid-svg-icons';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [FontAwesomeModule, RouterLink],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {
  home = faHome;
  chart = faChartBar;
  message = faComment;
  bookmark = faBookmark;
  user = faUser;
  transaction = faMoneyBillTransfer;

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
