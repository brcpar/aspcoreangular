import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-job-table',
  templateUrl: './job-table.component.html',
  styleUrls: ['./job-table.component.css']
})
export class JobTableComponent implements OnInit {
  exampleDatabase: ExampleHttpDao | null;
  data: HangfireJob[] = [];

  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.exampleDatabase = new ExampleHttpDao(this.http);
    
    this.paginator.page
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.exampleDatabase!.getRepoIssues(
            "", this.paginator.pageIndex);
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          debugger;
          this.resultsLength = data.total_count;

          return data.items;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          // Catch if the GitHub API has reached its rate limit. Return empty data.
          this.isRateLimitReached = true;
          return observableOf([]);
        })
      ).subscribe(data => this.data = data);
  }

  displayedColumns: string[] = ['select', 'jobId', 'name', 'status', 'symbol'];
  //dataSource = new MatTableDataSource(ELEMENT_DATA);
  selection = new SelectionModel<HangfireJob>(true, []);

  @ViewChild(MatPaginator) paginator: MatPaginator;

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.data.forEach(row => this.selection.select(row));
  }
}

export interface HangfireJobs {
  items: HangfireJob[];
  total_count: number;
}

export interface HangfireJob {
  name: string;
  jobId: number;
  status: JobStatus;
  symbol: string;
}

export enum JobStatus {
  Success = <any>"Success",
  Failed = <any>"Failed",
  Enqueued = <any>"Enqueued",
  Processing = <any>"Processing"
}

const ELEMENT_DATA: HangfireJob[] = [
  { jobId: 1, name: 'Hydrogen', status: JobStatus.Success, symbol: 'H' },
  { jobId: 2, name: 'Helium', status: JobStatus.Failed, symbol: 'He' },
  { jobId: 3, name: 'Lithium', status: JobStatus.Success, symbol: 'Li' },
  { jobId: 4, name: 'Beryllium', status: JobStatus.Enqueued, symbol: 'Be' },
  { jobId: 5, name: 'Boron', status: JobStatus.Success, symbol: 'B' },
  { jobId: 6, name: 'Carbon', status: JobStatus.Success, symbol: 'C' },
  { jobId: 7, name: 'Nitrogen', status: JobStatus.Processing, symbol: 'N' },
  { jobId: 8, name: 'Oxygen', status: JobStatus.Success, symbol: 'O' },
  { jobId: 9, name: 'Fluorine', status: JobStatus.Success, symbol: 'F' },
  { jobId: 10, name: 'Neon', status: JobStatus.Success, symbol: 'Ne' },
];

export class ExampleHttpDao {
  constructor(private http: HttpClient) { }

  getRepoIssues(term: string, page: number): Observable<HangfireJobs> {
    const requestUrl =
      `api/job?query=${term}&pageNumber=${page}`;

    return this.http.get<HangfireJobs>(requestUrl);
  }
}
