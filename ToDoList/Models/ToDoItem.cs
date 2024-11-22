using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        OnHold
    }

    public class ToDoItem
    {
        public int Id { get; set; } // primary key

        [Required]
        [StringLength(100)]
        public required string Title { get; set; } // required property

        public DateTime DateStarted { get; set; } = DateTime.Now;

        public DateTime? DateOfCompletion { get; set; } // nullable DateTime

        [StringLength(500)]
        public required string Details { get; set; } // required property

        public required string AssignedTo { get; set; } // required property

        public TaskStatus Status { get; set; } = TaskStatus.Pending;
    }
}