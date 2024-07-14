using FeedbackApp.Data;
using FeedbackApp.Extensions;
using FeedbackApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace FeedbackApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private static List<FeedbackModel> _feedback = new() { };

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
            _context = context;
		}

        [HttpGet]
        public async Task<IActionResult> Index()
		{
            return View(await _context.Feedbacks.OrderByDescending(f => f.CreatedDate).ToListAsync());
        }

        [HttpGet]
        public ActionResult CreateFeedback()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeedback(FeedbackModel model)
		{
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.CreatedBy);
            if (user == null)
            {
                return NotFound();
            }

            if (user.isBlocked)
            {
                return View("BlockedUserError");
            }

            FeedbackModel feedback = new FeedbackModel
            {
                Title = model.Title,
                Body = model.Body,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeedback(int feedbackId, string feedbackCreatedBy)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.isBlocked)
            {
                return View("BlockedUserError");
            }

            if (user.UserName == feedbackCreatedBy || user.isAdmin)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
                
            }

            return Forbid();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(int feedbackId, string feedbackCreatedBy)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == feedbackCreatedBy);
            if (user == null)
            {
                return NotFound();
            }

            user.isBlocked = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFeedback(string searchString)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set Feedbacks is null.");
            }

            var feedbacks = from feedback in _context.Feedbacks select feedback;

            if (!String.IsNullOrEmpty(searchString))
            {
                feedbacks = feedbacks.Where(f => (f.Title != null && f.Title.Contains(searchString))
                || (f.Body != null && f.Body.Contains(searchString)));
            }

            return View("Index", await feedbacks.ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FilterFeedbackByAuthor(string author)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set Feedbacks is null.");
            }

            var feedbacks = from feedback in _context.Feedbacks select feedback;

            if (!String.IsNullOrEmpty(author))
            {
                feedbacks = feedbacks.Where(f => (f.CreatedBy != null && f.CreatedBy.Contains(author)));
            }

            return View("Index", await feedbacks.OrderByDescending(f => f.CreatedDate).ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}