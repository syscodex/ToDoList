using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    /// <summary>
    /// Represents the possible states of a task in the todo list.
    /// </summary>
    public enum TaskStatus
    {
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "In Progress")]
        InProgress,
        [Display(Name = "On Hold")]
        OnHold,
        [Display(Name = "Completed")]
        Completed,
    }

    /// <summary>
    /// Represents a single To-Do item with comprehensive validation and metadata.
    /// </summary>
    public class ToDoItem
    {
        /// <summary>
        /// Unique identifier for the todo item.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The title of the task.
        /// </summary>
        [DisplayName("Task Title")]
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public required string Title { get; set; } 
        /// <summary>
        /// The date when the task was started.
        /// </summary>
        [DisplayName("Start Date")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime DateStarted { get; set; } = DateTime.Now;

        /// <summary>
        /// The date when the task was or is expected to be completed.
        /// </summary>
        [DisplayName("Completion Date")]
        [DataType(DataType.DateTime)]
        [CompletionDateValidation]
        public DateTime? DateOfCompletion { get; set; }

        /// <summary>
        /// Detailed description of the task.
        /// </summary>
        [DisplayName("Task Details")]
        [Required(ErrorMessage = "Details are required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Details must be between 5 and 200 characters.")]
        public required string Details { get; set; }

        /// <summary>
        /// The person or entity responsible for the task.
        /// </summary>
        [DisplayName("Assigned To")]
        [Required(ErrorMessage = "Please specify who the task is assigned to.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Assigned to must be between 2 and 100 characters.")]
        public required string AssignedTo { get; set; }


        /// <summary>
        /// Current status of the task.
        /// </summary>
        [DisplayName("Task Status")]
        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        /// <summary>
        /// Custom validation attribute for completion date.
        /// </summary>
        public class CompletionDateValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var toDoItem = validationContext.ObjectInstance as ToDoItem;

                if (toDoItem == null)
                    return new ValidationResult("Invalid to-do item.");

                if (toDoItem.DateStarted == default)
                {
                    return new ValidationResult(
                        "Start date must be specified.",
                        [nameof(ToDoItem.DateStarted)]
                    );
                }

                if (toDoItem.DateOfCompletion.HasValue &&
                    toDoItem.DateOfCompletion.Value <= toDoItem.DateStarted)
                {
                    return new ValidationResult(
                        "The task's completion date cannot be earlier than the start date.",
                        [nameof(ToDoItem.DateOfCompletion)]
                    );
                }

                if (toDoItem.Status == TaskStatus.Completed && !toDoItem.DateOfCompletion.HasValue)
                {
                    return new ValidationResult(
                        "Please provide a completion date for completed tasks.",
                        [nameof(ToDoItem.DateOfCompletion)]
                    );
                }

                return ValidationResult.Success;
            }
        }
    }
}