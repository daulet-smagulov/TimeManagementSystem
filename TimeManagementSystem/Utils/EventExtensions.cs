using System;
using System.Collections.Generic;
using TimeManagementSystem.Models;

namespace TimeManagementSystem.Utils
{
    public static class EventExtensions
    {
        public static Dictionary<string, double> GetDuration(this Event ev)
        {
            var dateKeyFormat = "yyyy.MM.dd";
            var duration = new Dictionary<string, double>();
            var startDateKey = ev.Start.ToString(dateKeyFormat);

            if (!ev.End.HasValue)
            {
                duration[startDateKey] = DurationToStartOrEndOfDay(ev.Start, true);
                return duration;
            }

            if (ev.Start.Date == ev.End.Value.Date)
            {
                duration[startDateKey] = Duration(ev.Start, ev.End.Value);
                return duration;
            }

            duration[startDateKey] = DurationToStartOrEndOfDay(ev.Start, true);
            if (ev.End.Value != ev.End.Value.Date)
                duration[ev.End.Value.ToString(dateKeyFormat)] = DurationToStartOrEndOfDay(ev.End.Value, false);

            int daysBetween = (ev.End.Value.Date - ev.Start.Date).Days;
            for (int i = 1; i < daysBetween; i++)
            {
                var currentDate = ev.Start.AddDays(i);
                duration[currentDate.ToString(dateKeyFormat)] = 24.0;
            }

            return duration;
        }

        static double Duration(DateTime start, DateTime end)
        {
            return (end - start).TotalHours;
        }

        static double DurationToStartOrEndOfDay(DateTime d, bool start)
        {
            var hoursDiff = Duration(d.Date, d);
            return start ? (24 - hoursDiff) : hoursDiff;
        }
    }
}