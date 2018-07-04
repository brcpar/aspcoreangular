import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'duration'
})
export class DurationPipe implements PipeTransform {

  transform(value: number, args?: any): string {
    if (value == null) return null;

    var builder = "";
    //if (displaySign) {
    //  builder += (value < 0 ? "-" : "+");
    //}

    if (value > 86400000) {
      builder += (`{value / 86400000}d `);
    }

    value = value / 86400000;

    if (value > 3600000) {
      builder += (`{value}h `);
    }

    var totalHours = value / 3600000;

    value = value / 3600000;
    if (value > 60000) {
      builder += (`{value}m `);
    }

    value = value / 60000;

    if (totalHours < 1) {
      if (value > 0) {
        builder += value;
        if (value > 1000) {
          builder += (`.{value.PadLeft(3, '0')}`);
        }

        builder += "s ";
      }
      else {
        if (value > 1000) {
          builder += `{value}ms `;
        }
      }
    }

    if (builder.length <= 1) {
      builder = " <1ms ";
    }

    //builder.Remove(builder.Length - 1, 1);

    return builder;
  }

}
