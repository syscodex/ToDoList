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
        /// Unit test to verify that the Create action:
        /// 1. Successfully adds a new ToDoItem to the database.
        /// 2. Redirects to the Index page after the item is created.
        /// </summary>
        [Fact]
        public async Task Create_ReturnsRedirectToIndex_WhenSuccessful()
        {
            // Arrange - Setup in-memory database and mock ILogger
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Ensure the database starts empty
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Create controller with the mock logger
                var controller = new ToDoController(context, mockLogger.Object);

                // Create a new ToDoItem
                var newItem = new ToDoItem
                {
                    Title = "New Task",
                    Details = "Task Details",
                    AssignedTo = "Charlie",
                    Status = TaskStatus.Pending
                };

                // Act - Call the 'Create' method
                var result = await controller.Create(newItem) as RedirectToActionResult;

                // Assert - Check if the result redirects to the Index page
                Assert.NotNull(result); // Ensure a result was returned
                Assert.Equal("Index", result.ActionName); // Ensure it redirects to "Index"

                // Assert - Check if exactly 1 item is added to the database
                Assert.Single(context.ToDoItems); // Expecting only 1 item in the database
            }
        }

        /// <summary>
        /// Unit test to verify that the Delete action:
        /// 1. Removes an existing ToDoItem from the database when a valid ID is provided.
        /// 2. Redirects to the Index page after deletion.
        /// </summary>
        [Fact]
        public async Task Delete_RemovesItem_WhenValidId()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ToDoListDbContext(options))
            {
                // Ensure the database starts empty before adding the test item
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Add a ToDoItem to delete
                context.ToDoItems.Add(new ToDoItem { Title = "Task to Delete", Details = "Details", Status = TaskStatus.Pending, AssignedTo = "Alice" });
                await context.SaveChangesAsync();
            }

            // Arrange - Mock the ILogger<ToDoController>
            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Act - Create controller and call the DeleteConfirmed action
                var controller = new ToDoController(context, mockLogger.Object);
                var itemToDelete = context.ToDoItems.FirstOrDefault();
                var result = await controller.DeleteConfirmed(itemToDelete.Id) as RedirectToActionResult;

                // Assert - Check if the result redirects to the Index page
                Assert.NotNull(result); // Ensure a result was returned
                Assert.Equal("Index", result.ActionName); // Ensure it redirects to "Index"

                // Assert - Check if the item is removed from the database
                Assert.Empty(context.ToDoItems); // The database should be empty after deletion
            }
        }

        /// <summary>
        /// Unit test to verify that the Edit action:
        /// 1. Retrieves the correct ToDoItem from the database.
        /// 2. Returns the Edit view with the ToDoItem model.
        /// </summary>
        [Fact]
        public async Task Edit_ReturnsViewWithToDoItem_WhenIdIsValid()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<ToDoListDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ToDoListDbContext(options))
            {
                // Ensure the database starts empty before adding a test item
                context.ToDoItems.RemoveRange(context.ToDoItems);
                await context.SaveChangesAsync();

                // Add a ToDoItem for testing the Edit action
                context.ToDoItems.Add(new ToDoItem
                {
                    Title = "Test Task",
                    Details = "Test details",
                    AssignedTo = "Test Assignee",
                    Status = TaskStatus.Pending
                });
                await context.SaveChangesAsync();
            }

            // Arrange - Mock the ILogger<ToDoController>
            var mockLogger = new Mock<ILogger<ToDoController>>();

            using (var context = new ToDoListDbContext(options))
            {
                // Act - Create controller and retrieve the item for editing
                var controller = new ToDoController(context, mockLogger.Object);
                var item = context.ToDoItems.FirstOrDefault(); // Retrieve the item added above
                var result = await controller.Edit(item.Id) as ViewResult;

                // Assert - Ensure the Edit view is returned with the correct model
                Assert.NotNull(result); // Ensure the result is not null
                var model = result.Model as ToDoItem; // The model should be of type ToDoItem
                Assert.NotNull(model); // Ensure the model is not null
                Assert.Equal("Test Task", model.Title); // Ensure the title matches
            }
        }
    }
}
