using System;
using System.IO;
using NAudio.Wave;

namespace BoxAverageTracker
{
    /// <summary>
    /// Convertit n'importe quel fichier audio/vidéo lisible par Windows (mp3, mp4, m4a, aac, wma, wav, mov...)
    /// en .wav PCM 16 bits 44,1 kHz stéréo, le seul format que System.Media.SoundPlayer sait lire.
    /// Utilise Media Foundation (intégré à Windows) via NAudio.
    /// </summary>
    public static class SoundConverter
    {
        /// <returns>Chemin du .wav créé</returns>
        public static string ConvertToWav(string inputPath, string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            string baseName = MakeSafeFileName(Path.GetFileNameWithoutExtension(inputPath));
            string outputPath = Path.Combine(outputFolder, baseName + ".wav");
            for (int i = 2; File.Exists(outputPath); i++)
                outputPath = Path.Combine(outputFolder, $"{baseName}_{i}.wav");

            string tmpPath = outputPath + ".tmp";
            try
            {
                using (var reader = new MediaFoundationReader(inputPath))
                using (var resampler = new MediaFoundationResampler(reader, new WaveFormat(44100, 16, 2)) { ResamplerQuality = 60 })
                {
                    WaveFileWriter.CreateWaveFile(tmpPath, resampler);
                }

                if (new FileInfo(tmpPath).Length <= 44)
                    throw new InvalidDataException("aucune piste audio trouvée dans ce fichier");

                File.Move(tmpPath, outputPath);
                return outputPath;
            }
            finally
            {
                try { if (File.Exists(tmpPath)) File.Delete(tmpPath); } catch { }
            }
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return string.IsNullOrWhiteSpace(name) ? "son" : name;
        }
    }
}
