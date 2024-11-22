using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserInformationApp.Models;
using UserInformationApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UserInformationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return View(users);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            // Populate ViewBag with Genders and Nationalities
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "GenderName"); // Ensure GenderName is correctly passed
            ViewBag.Nationalities = new SelectList(_context.Nationalities, "Id", "NationalityName"); // Ensure NationalityName is correctly passed
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Repopulate dropdowns
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "GenderName", user.GenderId);
            ViewBag.Nationalities = new SelectList(_context.Nationalities, "Id", "NationalityName", user.NationalityId);
            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Ensure the user exists
            }

            // Populate dropdowns for Genders and Nationalities
            ViewBag.Genders = new SelectList(_context.Genders, "Id", "Name", user.GenderId);
            ViewBag.Nationalities = new SelectList(_context.Nationalities, "Id", "Name", user.NationalityId);

            return View(user); // Pass the user to the view
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,DateOfBirth,GenderId,Phone,Email,Street,City,NationalityId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to Index after successful update
            }

            // If something went wrong, re-load Gender and Nationality dropdowns
            ViewData["GenderId"] = new SelectList(_context.Genders, "GenderId", "GenderName", user.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Nationalities, "NationalityId", "NationalityName", user.NationalityId);

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"User with id {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                // Optional: Handle the error and return an appropriate response
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
