import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './footer.css',
})
export class Footer {
  // Property to hold the current year, initialized to the current year using JavaScript's Date object
  currentYear: number = new Date().getFullYear();
}
