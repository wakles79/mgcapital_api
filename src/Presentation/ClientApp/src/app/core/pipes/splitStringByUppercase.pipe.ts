import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'splitStringByUppercase',
  pure: false
})
export class SplitStringByUppercasePipe implements PipeTransform {

  transform(stringToSplit: any): any {
    // insert a space before all caps
    return stringToSplit
      .replace(/([A-Z])/g, ' $1')
      // uppercase the first character
      .replace(/^./, function (str) { return str.toUpperCase(); });
  }
}
