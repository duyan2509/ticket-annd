using System;

namespace TicketAnnd.Domain.Services;

public static class BusinessHourCalculator
{
    // Default working hours: 09:00 - 17:00 local time, working days Mon-Fri
    public static double GetWorkingMinutesBetween(DateTime startUtc, DateTime endUtc)
    {
        if (endUtc <= startUtc)
            return 0;

        // convert to local time to apply working hours
        var startLocal = startUtc.ToLocalTime();
        var endLocal = endUtc.ToLocalTime();

        var total = 0.0;

        var currentDay = startLocal.Date;
        var lastDay = endLocal.Date;

        while (currentDay <= lastDay)
        {
            var dayOfWeek = currentDay.DayOfWeek;
            if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
            {
                var workStartLocal = currentDay.AddHours(9);
                var workEndLocal = currentDay.AddHours(17);

                var windowStart = workStartLocal < startLocal ? startLocal : workStartLocal;
                var windowEnd = workEndLocal > endLocal ? endLocal : workEndLocal;

                if (windowEnd > windowStart)
                {
                    total += (windowEnd - windowStart).TotalMinutes;
                }
            }

            currentDay = currentDay.AddDays(1);
        }

        return total;
    }
}
