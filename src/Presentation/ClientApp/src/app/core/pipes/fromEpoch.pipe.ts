import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fromEpoch',
  pure: false
})
export class FromEpochPipe implements PipeTransform {

  transform(utcSeconds: any): any {
    // Conditional string date
    if (utcSeconds <= 0) {
      return '-';
    }
    const resultDate = new Date(0); // The 0 there is the key, which sets the date to the epoch

    // See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/getTimezoneOffset
    // The time-zone offset is the difference, in minutes, from local time to UTC.
    // Note that this means that the offset is positive if the local timezone is behind UTC and negative if it is ahead.
    // For example, for time zone UTC+10:00 (Australian Eastern Standard Time, Vladivostok Time, Chamorro Standard Time), -600 will be returned.
    const offsetSeconds = 0; // resultDate.getTimezoneOffset() * 60;
    resultDate.setUTCSeconds(utcSeconds - offsetSeconds);
    return resultDate.toLocaleString('en-US');
  }
}
