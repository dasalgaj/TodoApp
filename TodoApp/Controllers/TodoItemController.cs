using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class TodoItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodoItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return "";
            }
            return user.Id;
        }

        // GET: TodoItem
        public async Task<IActionResult> Index(int listId)
        {
            if (listId <= 0)
            {
                return NotFound();
            }

            var todo_list = _context.TodoLists.FirstOrDefault(tl => tl.Id == listId && tl.UserId == GetUserId());
            if (todo_list == null)
            {
                return NotFound();
            }
            ViewBag.ListId = listId;
            ViewBag.ListName = todo_list.Name;
            var todo_items = await _context.TodoItems.Where(ti => ti.TodoListId == listId).ToListAsync();

            return View(todo_items);
        }

        // GET: TodoItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var todo_list = _context.TodoLists.FirstOrDefault(tl => tl.Id == todoItem.TodoListId);
            if (todo_list == null || todo_list.UserId != GetUserId()) { return NotFound(); }

            ViewBag.ItemName = todoItem.Name;

            return View(todoItem);
        }

        // GET: TodoItem/Create
        public IActionResult Create(int listId)
        {
            TodoItem todoItem = new TodoItem();
            todoItem.TodoListId = listId;

            var todo_list = _context.TodoLists.FirstOrDefault(tl => tl.Id == listId);
            if (todo_list == null || todo_list.UserId != GetUserId()) { return NotFound(); }
            ViewBag.Name = todo_list.Name;

            return View(todoItem);
        }

        // POST: TodoItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Status,TodoListId")] TodoItem todoItem)
        {
            ModelState.Remove("TodoList");
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { listId = todoItem.TodoListId });
            }

            return View(todoItem);
        }

        // GET: TodoItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var todo_list = _context.TodoLists.FirstOrDefault(tl => tl.Id == todoItem.TodoListId);
            if (todo_list == null || todo_list.UserId != GetUserId()) { return NotFound(); }

            ViewBag.Name = todoItem.Name;

            return View(todoItem);
        }

        // POST: TodoItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Status,TodoListId")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            ModelState.Remove("TodoList");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { listId = todoItem.TodoListId });
            }
            
            return View(todoItem);
        }

        // GET: TodoItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var todo_list = _context.TodoLists.FirstOrDefault(tl => tl.Id == todoItem.TodoListId);
            if (todo_list == null || todo_list.UserId != GetUserId()) { return NotFound(); }

            ViewBag.ItemName = todoItem.Name;

            return View(todoItem);
        }

        // POST: TodoItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { listId = todoItem.TodoListId });
        }

        // POST: TodoItem Update Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    todoItem.Status = status;
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index), new { listId = todoItem.TodoListId });
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
