using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using Microsoft.Extensions.Logging;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoListDbContext _context;
        private readonly ILogger<ToDoController> _logger; // Add ILogger

        // Inject ILogger into the controller
        public ToDoController(ToDoListDbContext context, ILogger<ToDoController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger
        }

        // **INDEX**: Display all To-Do Items
        public async Task<IActionResult> Index(string searchQuery, string statusFilter, string assigneeFilter, DateTime? startDateFilter, DateTime? endDateFilter)
        {
            var tasks = _context.ToDoItems.AsQueryable();

            // Apply search query filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchQuery) || t.Details.Contains(searchQuery));
            }

            // Apply status filter (Enum to String comparison)
            if (!string.IsNullOrEmpty(statusFilter))
            {
                tasks = tasks.Where(t => t.Status.ToString() == statusFilter);
            }

            // Apply assignee filter
            if (!string.IsNullOrEmpty(assigneeFilter))
            {
                tasks = tasks.Where(t => t.AssignedTo.Contains(assigneeFilter));
            }

            // Apply start date filter
            if (startDateFilter.HasValue)
            {
                tasks = tasks.Where(t => t.DateStarted >= startDateFilter.Value);
            }

            // Apply end date filter
            if (endDateFilter.HasValue)
            {
                tasks = tasks.Where(t => t.DateOfCompletion <= endDateFilter.Value);
            }

            // Pass filter parameters to the view for persistence in the UI
            ViewData["SearchQuery"] = searchQuery;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["AssigneeFilter"] = assigneeFilter;
            ViewData["StartDateFilter"] = startDateFilter;
            ViewData["EndDateFilter"] = endDateFilter;

            // Return the filtered list to the view
            return View(await tasks.ToListAsync());
        }


        // **CREATE**: Display the create form (GET)
        public IActionResult Create()
        {
            return View();
        }

        // **CREATE**: Handle the form submission (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Details,AssignedTo,DateStarted,DateOfCompletion,Status")] ToDoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created a new ToDoItem with Title: {Title}", todoItem.Title);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Failed to create ToDoItem. Validation errors: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            return View(todoItem);
        }

        // **EDIT**: Display the edit form for a specific item (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit action received null id.");
                return NotFound();
            }

            var todoItem = await _context.ToDoItems.FindAsync(id);
            if (todoItem == null)
            {
                _logger.LogWarning("ToDoItem with ID {Id} not found during Edit action.", id);
                return NotFound();
            }

            return View(todoItem);
        }

        // **EDIT**: Handle the form submission for updating an item (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Details,AssignedTo,DateStarted,DateOfCompletion,Status")] ToDoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                _logger.LogWarning("Edit action received mismatched ID: Expected {ExpectedId}, Found {FoundId}", id, todoItem.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Updated ToDoItem with ID {Id} and Title: {Title}", todoItem.Id, todoItem.Title);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(todoItem.Id))
                    {
                        _logger.LogWarning("ToDoItem with ID {Id} not found during update.", todoItem.Id);
                        return NotFound();
                    }

                    _logger.LogError("Error occurred during updating ToDoItem with ID {Id}.", todoItem.Id);
                    throw;
                }
            }

            return View(todoItem); // Return to form with validation errors
        }

        // **DELETE**: Confirm deletion for a specific item (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete action received null id.");
                return NotFound();
            }

            var todoItem = await _context.ToDoItems.FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                _logger.LogWarning("ToDoItem with ID {Id} not found during Delete action.", id);
                return NotFound();
            }

            return View(todoItem);
        }

        // **DELETE**: Handle deletion confirmation (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.ToDoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.ToDoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted ToDoItem with ID {Id} and Title: {Title}", id, todoItem.Title);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent ToDoItem with ID {Id}.", id);
            }

            return RedirectToAction(nameof(Index)); // Redirect after deletion
        }

        // **Helper Method**: Check if an item exists by ID
        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}