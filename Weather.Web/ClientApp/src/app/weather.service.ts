import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpParams} from "@angular/common/http";
import {Observable, throwError} from "rxjs";
import {catchError} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  private currentWeatherUrl = 'api/v1/weatherforecast/current';

  constructor(private http: HttpClient) {
  }

  getCurrentWeather(cityName: string, countryCode: string): Observable<CurrentWeather> {
    const options = {params: new HttpParams().set('cityName', cityName).set('countryCode', countryCode)};

    return this.http.get<CurrentWeather>(this.currentWeatherUrl, options).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = `Sorry, we couldn't process your request at this time.`;

    if (error.status == 404) {
      errorMessage = `We couldn't find the city / country you provided.`;
    } else if (error.status === 429) {
      const retryAfter = error.headers.get('Retry-After');
      errorMessage = `The current weather won't change that often :). Please try again after ${retryAfter} seconds.`
    } else if (error.status === 503) {
      errorMessage = 'Looks like our weather guy is taking a break and is not available at the moment. Please try again later.'
    }

    return throwError(errorMessage);
  }
}

export interface CurrentWeather {
  description: string
}
