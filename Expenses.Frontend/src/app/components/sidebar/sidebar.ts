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
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './sidebar.css'
})
export class Sidebar {
  readonly home = faHome;
  readonly chart = faChartLine;
  readonly message = faComment;
  readonly overview = faMagnifyingGlassChart;
  readonly settings = faGear;
  readonly user = faUser;
  readonly transactionsLog = faEye;
  readonly transaction = faMoneyBill1Wave;

  // Optional title for the sidebar, initialized to 'Dashboard'
  title = 'Dashboard';

  /**
   * Updates the title with the specified item.
   *
   * @param {string} title - The new title to set.
   * @return {void} This method does not return a value.
   */
  updateTitle(title: string): void {
    this.title = title;
  }

}
