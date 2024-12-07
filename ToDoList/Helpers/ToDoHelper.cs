using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Helpers
{
    public static class ToDoHelpers
    {
        /// <summary>
        /// Retrieves the badge class and icon class for the specified TaskStatus.
        /// </summary>
        public static (string BadgeClass, string IconClass) GetStatusClass(ToDoList.Models.TaskStatus status)
        {
            return status switch
            {
                ToDoList.Models.TaskStatus.Completed => ("badge-success text-white", "fas fa-check-circle text-white"),
                ToDoList.Models.TaskStatus.InProgress => ("badge-inprogress text-white", "fas fa-hourglass-half text-white"),
                ToDoList.Models.TaskStatus.Pending => ("badge-warning text-white", "fas fa-circle-notch text-white"),
                ToDoList.Models.TaskStatus.OnHold => ("badge-secondary text-dark", "fas fa-pause-circle text-dark"),
                _ => ("badge-secondary text-dark", "fas fa-question-circle text-white")
            };
        }

        /// <summary>
        /// Retrieves the display name of a TaskStatus value using its Display attribute.
        /// </summary>
        public static string GetStatusDisplayName(ToDoList.Models.TaskStatus status)
        {
            var field = status.GetType().GetField(status.ToString());
            var displayAttribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                         .FirstOrDefault() as DisplayAttribute;
            return displayAttribute?.Name ?? status.ToString();
        }

        /// <summary>
        /// Formats the DateTime for display with both date and time (e.g., "MM/dd/yyyy hh:mm tt").
        /// </summary>
        public static string FormatDateTimeWithTime(DateTime? dateTime)
        {
            return dateTime?.ToString("MM/dd/yyyy hh:mm tt") ?? "N/A";
        }
    }
}
