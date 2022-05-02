using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MGCap.Domain.Tests
{
    public class CalendarFrequencyDateCalculator
    {
        private readonly ITestOutputHelper output;

        public CalendarFrequencyDateCalculator(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenerateDates()
        {
            List<DateTime> newDates = new List<DateTime>();

            // Parameters
            DateTime startDate = DateTime.Today;

            int quantity = 4;
            List<int> months = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12 };
            months.Sort();

            int year = startDate.Year;
            int day = startDate.Day;

            this.output.WriteLine($@"year: {year}, day:{day}, month: {startDate.Month}");

            for (int i = 0; i < months.Count(); i++)
            {
                if (newDates.Count() == quantity)
                {
                    break;
                }

                DateTime newDate = new DateTime(year, months[i], day);
                string result = $"-- generated: -- {newDate.ToString("MM/dd/yyyy")} [{months[i]}] [{i}] [{months.Count()}] --";

                if (DateTime.Compare(newDate, startDate) >= 0)
                {
                    newDates.Add(newDate);
                    result += " [ADDED] ";

                }

                // renew the counter
                if (i == (months.Count() - 1))
                {
                    i = -1;
                    year++;
                }

                this.output.WriteLine(result);
            }
        }
    }
}
