import { Component, ChangeDetectionStrategy } from '@angular/core';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-page-not-found',
  imports: [
    RouterLink
  ],
  templateUrl: './page-not-found.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './page-not-found.css'
})
export class PageNotFound {

}
