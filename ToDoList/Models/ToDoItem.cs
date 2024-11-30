using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public enum TaskStatus
    {
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "In Progress")] //change dp name
        InProgress,
        [Display(Name = "Completed")]
        Completed,
        [Display(Name = "On Hold")]
        OnHold
    }

    /// <summary>
    /// Represents a single To-Do item with validation for user input.
    /// </summary>
    public class ToDoItem
    {
        public int Id { get; set; } // Primary key
        [DisplayName("Task Title")]
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [DataType(DataType.DateTime)]

        [DisplayName("Start Date")]

        public DateTime DateStarted { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayName("Completion Date")]
       
        public DateTime? DateOfCompletion { get; set; }

        // Custom validation to ensure DateOfCompletion is after DateStarted
        public bool IsValidCompletionDate()
        {
            return !DateOfCompletion.HasValue || DateOfCompletion > DateStarted;
        }

        public class ToDoItemValidator : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                var toDoItem = value as ToDoItem;
                if (toDoItem == null)
                    return false;

                return toDoItem.IsValidCompletionDate();

            }
        }
        /// <summary>


        [DisplayName("Task Details")]
        [Required(ErrorMessage = "Details are required.")]
        [StringLength(500, ErrorMessage = "Details cannot exceed 500 characters.")]
        public required string Details { get; set; }

        [Required(ErrorMessage = "Please specify who the task is assigned to.")]
        [StringLength(100, ErrorMessage = "Task cannot be longer than 100 characters.")]

        [DisplayName("Assigned To")]
        public required string AssignedTo { get; set; }

        [DisplayName("Task Status")]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        
    }
}