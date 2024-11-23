using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoListDbContext _context;

        public ToDoController(ToDoListDbContext context)
        {
            _context = context;
        }

        // **INDEX**: Display all To-Do Items
        public async Task<IActionResult> Index()
        {
            var toDoItems = await _context.ToDoItems.ToListAsync();
            if (toDoItems == null)
            {
                toDoItems = new List<ToDoItem>(); // Ensure it's never null
            }
            if (!toDoItems.Any())
            {
                ViewData["Message"] = "No tasks available.";
            }
            return View(toDoItems);
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
            // Validate that DateOfCompletion is later than DateStarted
            if (todoItem.DateOfCompletion <= todoItem.DateStarted)
            {
                ModelState.AddModelError("DateOfCompletion", "Date of Completion must be later than Date Started.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem); // Return to form with validation errors
        }

        // **EDIT**: Display the edit form for a specific item (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDoItems.FindAsync(id);
            if (todoItem == null)
            {
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
                return NotFound();
            }

            // Validate that DateOfCompletion is later than DateStarted
            if (todoItem.DateOfCompletion <= todoItem.DateStarted)
            {
                ModelState.AddModelError("DateOfCompletion", "Date of Completion must be later than Date Started.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    throw; // Let the exception propagate for logging/debugging
                }
            }

            return View(todoItem); // Return to form with validation errors
        }

        // **DELETE**: Confirm deletion for a specific item (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDoItems.FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
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
