import { Component } from '@angular/core';
import {CurrentWeather, WeatherService} from "../weather.service";
import {finalize} from "rxjs/operators";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  cityName: string;
  countryCode: string;
  currentWeather: CurrentWeather;
  errorMessage: string;

  private fetching: boolean;

  constructor(private weatherService: WeatherService) {
  }

  get isFetching(): boolean {
    return this.fetching;
  }

  get hasError(): boolean {
    return !this.isFetching && this.errorMessage != null && this.errorMessage != '';
  }

  get isCurrentWeatherAvailable(): boolean {
    return !this.isFetching && this.currentWeather != null;
  }

  getWeather(): void {
    if (!this.cityName || !this.countryCode) {
      this.fetching = false;
      this.errorMessage = "Both City Name and Country Code fields are required.";

      return;
    }

    this.fetching = true;

    this.weatherService.getCurrentWeather(this.cityName, this.countryCode)
      .pipe(
        finalize(() => this.fetching = false)
      )
      .subscribe(
        (data: CurrentWeather) => {
          this.currentWeather = {...data};
          this.errorMessage = null;
        },
        (error: string) => {
          this.currentWeather = null;
          this.errorMessage = error;

          console.log("Error is:");
          console.log(error);
        });
  }
}
