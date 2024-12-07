using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Controllers;
using ToDoList.Data;
using ToDoList.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskStatus = ToDoList.Models.TaskStatus;

namespace ToDoList.test
{
    public class ToDoControllerTests
    {
        /// <summary>
        /// Tests the 'Create' action of the ToDoController.
        /// Ensures that when a new task is created:
        /// 1. The task is added to the database.
        /// 2. The user is redirected to the 'Index' page.
        /// </summary>
        [Fact]
        public async Task Create_ReturnsRedirectToIndex_WhenSuccessful()
        {
            // Arrange: Set up an in-memory database for testing
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            // Mock the logger dependency
            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Clear any existing data and ensure the database starts empty
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Create a controller instance, passing the mocked logger
                var controller = new ToDoController(context, mockLogger.Object);

                // Create a new ToDoItem to add
                var newItem = new ToDoItem
                {
                    Title = "New Task",
                    Details = "Task Details",
                    AssignedTo = "Charlie",
                    Status = TaskStatus.Pending
                };

                // Act: Call the 'Create' method to add the task
                var result = await controller.Create(newItem) as RedirectToActionResult;

                // Assert: Ensure that the result is a redirect to 'Index'
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);

                // Assert that only 1 item exists in the database after the creation
                Assert.Single(context.ToDoItems);
            }
        }

        /// <summary>
        /// Tests the 'Delete' action of the ToDoController.
        /// Verifies that a ToDoItem is deleted from the database.
        /// 1. It first adds a task to be deleted.
        /// 2. Calls the 'DeleteConfirmed' action to remove the task.
        /// 3. Ensures that the task is no longer in the database after deletion.
        /// </summary>
        [Fact]
        public async Task Delete_RemovesItem_WhenValidId()
        {
            // Arrange: Set up an in-memory database and add a task to be deleted
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ToDoListDbContext(options))
            {
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Add a ToDoItem to the database
                context.ToDoItems.Add(new ToDoItem { Title = "Task to Delete", Details = "Details", Status = TaskStatus.Pending, AssignedTo = "Alice" });
                await context.SaveChangesAsync();
            }

            // Mock the logger dependency
            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Act: Call the DeleteConfirmed method to delete the task
                var controller = new ToDoController(context, mockLogger.Object);
                var itemToDelete = context.ToDoItems.FirstOrDefault();
                var result = await controller.DeleteConfirmed(itemToDelete.Id) as RedirectToActionResult;

                // Assert: Ensure the result is a redirect to 'Index'
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);

                // Assert that the task is removed from the database
                Assert.Empty(context.ToDoItems);
            }
        }

        /// <summary>
        /// Unit test for the 'Edit' action of the ToDoController.
        /// Verifies that when a task is edited:
        /// 1. The correct task is retrieved and displayed in the edit view.
        /// 2. The model passed to the view is a valid ToDoItem.
        /// </summary>
        [Fact]
        public async Task Edit_ReturnsViewWithToDoItem_WhenIdIsValid()
        {
            // Arrange: Set up an in-memory database and add a task to be edited
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ToDoListDbContext(options))
            {
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Add a ToDoItem for testing the edit functionality
                context.ToDoItems.Add(new ToDoItem
                {
                    Title = "Test Task",
                    Details = "Test details",
                    AssignedTo = "Test Assignee",
                    Status = TaskStatus.Pending
                });
                await context.SaveChangesAsync();
            }

            // Mock the logger dependency
            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Act: Retrieve the item to edit and pass it to the Edit method
                var controller = new ToDoController(context, mockLogger.Object);
                var item = context.ToDoItems.FirstOrDefault();
                var result = await controller.Edit(item.Id) as ViewResult;

                // Assert: Ensure the view is returned with the correct model
                Assert.NotNull(result);
                var model = result.Model as ToDoItem;
                Assert.NotNull(model);
                Assert.Equal("Test Task", model.Title);
            }
        }
    }
}
