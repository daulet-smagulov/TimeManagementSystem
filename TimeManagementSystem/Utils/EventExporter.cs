using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using TimeManagementSystem.Models;

namespace TimeManagementSystem.Utils
{
    public static class EventExporter
    {
        public static string GetHtml(SortedDictionary<string, DayNotes> notes)
        {
            var columns = new[] { "Date", "Total time", "Notes" };

            var stb = new StringBuilder();

            stb.AppendLine("<html>");
            stb.AppendLine("<body>");
            stb.AppendLine("    <h1>Export events</h1>");

            foreach (var dayNote in notes)
            {
                stb.AppendLine($"    <p>Date: {dayNote.Key}</p>");
                stb.AppendLine($"    <p style=\"text-indent:1em\">Total time: {dayNote.Value.TotalHours}</p>");
                stb.AppendLine($"    <p style=\"text-indent:1em\">Notes:</p>");
                for (int i = 0; i < dayNote.Value.Notes.Count; i++)
                    stb.AppendLine($"    <p style=\"text-indent:2em\">{i + 1}) {dayNote.Value.Notes[i]}</p>");
            }

            //table footer & end of html file
            stb.AppendLine("</body>");
            stb.AppendLine("</html>");
            return stb.ToString();
        }
    }
}