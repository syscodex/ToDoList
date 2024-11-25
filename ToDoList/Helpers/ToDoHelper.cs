namespace ToDoList.Helpers
{
    public static class ToDoHelpers
    {
        // Method to get status class for badges
        public static string GetStatusClass(ToDoList.Models.TaskStatus status)
        {
            return status switch
            {
                ToDoList.Models.TaskStatus.Completed => "completed",
                ToDoList.Models.TaskStatus.InProgress => "in-progress",
                ToDoList.Models.TaskStatus.Pending => "pending",
                _ => "secondary"
            };
        }
    }
}