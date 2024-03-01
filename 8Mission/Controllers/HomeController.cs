using _8Mission.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace _8Mission.Controllers
{
    public class HomeController : Controller
    {
        private AddTaskContext _context;

        public HomeController(AddTaskContext temp) //Constructor
        {
            _context = temp;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewTasks()
        {

            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.CategoryName)
                .ToList();

            //Linq
            var applications = _context.AddTask.Include("Category")
                .Where(x => x.Completed == false)
                .OrderBy(x => x.TaskId).ToList();

            return View(applications);
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.CategoryId)
                .ToList();

            return View("AddTask", new AddTask()); //create new application to get rid of the error that says " is not a valid input

        }
        [HttpPost]
        public IActionResult AddTask(AddTask response)
        {
            if (ModelState.IsValid)
            {
                _context.AddTask.Add(response); // EF Core should handle TaskId
                _context.SaveChanges();

                return View("Confirmation", response);
            }
            else
            {
                ViewBag.Categories = _context.Categories.OrderBy(x => x.CategoryName).ToList();
                return View(response);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id) //Edits selected record
        {
            var recordToEdit = _context.AddTask
                .Single(x => x.TaskId == id);

            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.CategoryName)
                .ToList();

            return View("AddTask", recordToEdit);
        }

        [HttpPost]
        public IActionResult Edit(AddTask updatedInfo)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                _context.Update(updatedInfo);
                _context.SaveChanges();

                return RedirectToAction("ViewTasks");
            }
            else // Model state is not valid, return to the view with the current information
            {
                ViewBag.Categories = _context.Categories
                    .OrderBy(x => x.CategoryName)
                    .ToList();

                return View("AddTask", updatedInfo); // Use the same AddMovie view for editing, ensuring validation messages are displayed
            }
        }

        [HttpGet]
        public IActionResult Delete(int id) //Deletes selected record
        {
            var recordToDelete = _context.AddTask
                .Single(x => x.TaskId == id);

            return View(recordToDelete);
        }

        [HttpPost]

        public IActionResult Delete(AddTask application)
        {
            _context.AddTask.Remove(application);
            _context.SaveChanges();

            return RedirectToAction("ViewTasks");
        }

    }
}