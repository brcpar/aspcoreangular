import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { JobService, HangfireJob, HangfireJobs, JobSearchCriteria} from "../Job/job.service";


@Component({
  selector: 'app-job-table',
  templateUrl: './job-table.component.html',
  styleUrls: ['./job-table.component.css']
})
export class JobTableComponent implements OnInit {
  //jobService: JobService | null;
  data: HangfireJob[] = [];

  resultsLength = 0;
  isLoadingResults = true;
  isErrored = false;

  constructor(private jobService: JobService) { }

  ngOnInit() {
    this.paginator.page
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          var req = new JobSearchCriteria();
          req.term = "";
          req.page = this.paginator.pageIndex;
          req.processor = "";
          req.perPage = 100;
          return this.jobService!.getJobs(req);
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isErrored = false;
          this.resultsLength = data.total_count;

          return data.items;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          this.isErrored = true;
          return observableOf([]);
        })
      ).subscribe(data => this.data = data);
  }

  displayedColumns: string[] = ['select', 'jobId', 'name', 'status', 'duration', 'completed'];
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
