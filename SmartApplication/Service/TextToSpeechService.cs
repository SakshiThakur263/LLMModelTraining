using System.Speech.Synthesis;

public class TextToSpeechService
{
    public string ConvertTextToAudio(string text, string fileName)
    {
        string outputPath = Path.Combine("wwwroot/audio", $"{fileName}.wav");

        using var synth = new SpeechSynthesizer();
        synth.SetOutputToWaveFile(outputPath);
        synth.Speak(text);

        return $"/audio/{fileName}.wav";
    }
}
