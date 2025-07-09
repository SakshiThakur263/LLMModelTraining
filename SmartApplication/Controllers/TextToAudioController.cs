using Microsoft.AspNetCore.Mvc;
using System.Speech.Synthesis;

namespace SmartApplication.Controllers
{
    public class TextToAudioController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Convert(string text, string voice)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ViewBag.Error = "Text cannot be empty.";
                return View("Index");
            }

            var synth = new SpeechSynthesizer();

            // Choose voice (if available)
            if (voice == "Male")
                synth.SelectVoiceByHints(VoiceGender.Male);
            else if (voice == "Female")
                synth.SelectVoiceByHints(VoiceGender.Female);

            // Unique filename
            string fileName = $"audio_{Guid.NewGuid()}.wav";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, fileName);

            // Generate audio
            synth.SetOutputToWaveFile(filePath);
            synth.Speak(text);
            synth.Dispose();

            ViewBag.AudioPath = $"/audio/{fileName}";
            ViewBag.OriginalText = text;
            return View("Index");
        }
    }
}
