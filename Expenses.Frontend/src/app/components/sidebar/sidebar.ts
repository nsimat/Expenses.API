import {Component, ChangeDetectionStrategy} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faChartLine,
  faComment, faEye, faGear,
  faHome, faMagnifyingGlassChart, faMoneyBill1Wave,
  faUser
} from '@fortawesome/free-solid-svg-icons';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [FontAwesomeModule, RouterLink],
  templateUrl: './sidebar.html',
  changeDetection: ChangeDetectionStrategy.Eager,
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

  // Optional title for the sidebar, initialized to 'Dashboard'
  title?: string = 'Dashboard';

  // Method to get the current title of the sidebar, returns 'Dashboard' if title is not set
  getTitle(): string {
    return this.title || 'Dashboard';
  }

  // Method to set a new title for the sidebar
  setTitle(newTitle: string): void {
    this.title = newTitle;
  }

  /**
   * Updates the title with the specified item.
   *
   * @param {string} item - The new title to set.
   * @return {void} This method does not return a value.
   */
  updateTitle(item: string): void {
    this.setTitle(item);
  }

}
