using Microsoft.AspNetCore.Mvc;
using System.Speech.Recognition;
using NAudio.Wave;

namespace SmartApplication.Controllers
{
    public class AudioToTextController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Transcribe(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("Audio file is missing.");

            string uploadedPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_uploaded.wav");
            string cleanPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_converted.wav");

            try
            {
                // Save raw uploaded audio
                using (var fs = new FileStream(uploadedPath, FileMode.Create))
                {
                    await audioFile.CopyToAsync(fs);
                }

                // Convert to PCM 16-bit Mono 16kHz
                using (var reader = new WaveFileReader(uploadedPath))
                {
                    var outFormat = new WaveFormat(16000, 16, 1);
                    using var resampler = new MediaFoundationResampler(reader, outFormat)
                    {
                        ResamplerQuality = 60
                    };
                    WaveFileWriter.CreateWaveFile(cleanPath, resampler);
                }

                // Recognize using Windows Speech
                using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.SetInputToWaveFile(cleanPath);

                var result = recognizer.Recognize();
                return Content(result?.Text ?? "No speech detected.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error transcribing audio: " + ex.Message);
            }
            finally
            {
                try { if (System.IO.File.Exists(uploadedPath)) System.IO.File.Delete(uploadedPath); } catch { }
                try { if (System.IO.File.Exists(cleanPath)) System.IO.File.Delete(cleanPath); } catch { }
            }
        }
    }
}
