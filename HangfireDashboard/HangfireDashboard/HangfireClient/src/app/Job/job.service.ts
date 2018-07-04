import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  constructor(private http: HttpClient) { }

  public getJobs(req: JobSearchCriteria): Observable<HangfireJobs> {
    const requestUrl =
      `api/job`;
    return this.http.post<HangfireJobs>(requestUrl, req);
  }
}

export class JobSearchCriteria {
  public term: string;
  public page: number;
  public status: JobStatus;
  public processor: string;
  public perPage: number;
}

export interface HangfireJobs {
  items: HangfireJob[];
  total_count: number;
}

export interface HangfireJob {
  display: string;
  jobId: number;
  state: JobStatus;
  completedAt: Date;
  duration: number;
  type: string;
}

export enum JobStatus {
  Success = <any>"Success",
  Failed = <any>"Failed",
  Enqueued = <any>"Enqueued",
  Processing = <any>"Processing"
}
