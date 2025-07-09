using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace SmartApplication.Controllers
{
    public class TextSummaryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Summarize(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ViewBag.Error = "Please enter some text.";
                return View("Index");
            }

            string summary = GetSummary(inputText);
            ViewBag.Original = inputText;
            ViewBag.Summary = summary;

            return View("Index");
        }

        private string GetSummary(string text)
        {
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+"); // split by sentence

            // Pick first 2 sentences or keyword-based
            var summary = sentences.Take(2).ToList();

            foreach (var s in sentences)
            {
                if ((s.Contains("important") || s.Contains("key")) && !summary.Contains(s))
                {
                    summary.Add(s);
                }
            }

            return string.Join(" ", summary.Distinct());
        }
    }
}
