using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeManagementSystem.Models;

namespace TimeManagementSystem.Utils
{
    public class DayNotes
    {
        public double TotalHours => Math.Min(_totalHours, 24);
        public IList<string> Notes { get; set; }

        private double _totalHours;

        public DayNotes()
        {
            _totalHours = 0;
            Notes = new List<string>();
        }

        public void AddNote(string note, double time)
        {
            Notes.Add(note);
            _totalHours += time;
        }

        public static SortedDictionary<string, DayNotes> GetNotes(IQueryable<Event> events)
        {
            var notes = new SortedDictionary<string, DayNotes>();
            foreach (Event ev in events)
            {
                foreach (var durationData in ev.GetDuration())
                {
                    if (!notes.ContainsKey(durationData.Key))
                        notes.Add(durationData.Key, new DayNotes());
                    notes[durationData.Key].AddNote(ev.Subject, durationData.Value);
                }
            }
            return notes;
        }
    }
}