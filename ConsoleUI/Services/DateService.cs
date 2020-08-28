using System;

namespace ConsoleUI.Services
{
    public static class DateService
    {
        public static DateTime GetLastSaturday()
        {
            /*****************************************************
             * GETS LAST SATURDAYS DATE DEPENDIN ON TODAY'S DATE
             *****************************************************/

            DateTime value = DateTime.Now;

            switch (value.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    value = value.AddDays(-2);
                    break;
                case DayOfWeek.Tuesday:
                    value = value.AddDays(-3);
                    break;
                case DayOfWeek.Wednesday:
                    value = value.AddDays(-4);
                    break;
                case DayOfWeek.Thursday:
                    value = value.AddDays(-5);
                    break;
                case DayOfWeek.Friday:
                    value = value.AddDays(-6);
                    break;
                case DayOfWeek.Saturday:
                    value = value.AddDays(0);
                    break;
                case DayOfWeek.Sunday:
                    value = value.AddDays(-1);
                    break;
            }

            value = value.Date.Add(new TimeSpan(0, 0, 0));

            return value;
        }
    }
}