import { Component, inject, Inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { SaleSummary } from '../../domain/generated/apimodel';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-summary',
  imports: [FormsModule, DecimalPipe],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
  standalone: true
})
export class SummaryComponent implements OnInit {

  apiService = inject(ApiService);

  dateFrom: string | null = new Date().toISOString().split('T')[0];
  dateTo: string | null = new Date().toISOString().split('T')[0];

  saleSummary: SaleSummary | null = null;

  dateRanges = ['Dzisiaj', 'Wczoraj', 'Bieżący tydzień', 'Bieżący miesiąc', 'Niestandardowy'];
  selectedRange: string = 'Dzisiaj';

  loading: boolean = false;

  ngOnInit(): void {
    this.setDateRange();
    this.loadSummary();
  }

  loadSummary() {
    if (this.dateFrom && this.dateTo) {
      this.loading = true;
      this.apiService.getSaleSummary(this.dateFrom, this.dateTo).subscribe({
        next: summary => {
          this.loading = false;
          console.log(summary);
          if (summary && summary.data) {
            this.saleSummary = summary.data;
          }
        },
        error: err => {
          console.error(err);
          this.loading = false;
        }
      });
    }
  }

  onDateRangeChange(range: string) {
    this.selectedRange = range;
    this.setDateRange();
    this.loadSummary();
  }

  private setDateRange() {
    const today = new Date();
    switch (this.selectedRange) {
      case 'Dzisiaj':
        this.dateFrom = today.toISOString().split('T')[0];
        this.dateTo = today.toISOString().split('T')[0];
        break;
      case 'Wczoraj':
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        this.dateFrom = yesterday.toISOString().split('T')[0];
        this.dateTo = yesterday.toISOString().split('T')[0];
        break;
      case 'Bieżący tydzień':
        const firstDayOfWeek = new Date(today.setDate(today.getDate() - today.getDay() + (today.getDay() === 0 ? -6 : 1)));
        this.dateFrom = firstDayOfWeek.toISOString().split('T')[0];
        const lastDayOfWeek = new Date(firstDayOfWeek);
        lastDayOfWeek.setDate(lastDayOfWeek.getDate() + 6);
        this.dateTo = lastDayOfWeek.toISOString().split('T')[0];
        break;
      case 'Bieżący miesiąc':
        const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1, 12);
        this.dateFrom = firstDayOfMonth.toISOString().split('T')[0];
        const lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0, 12);
        this.dateTo = lastDayOfMonth.toISOString().split('T')[0];
        break;
    }
  }
}
