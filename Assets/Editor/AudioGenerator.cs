using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NeonBreaker.EditorTools
{
    public static class AudioGenerator
    {
        private const string AudioFolder = "Assets/Resources/Audio";
        private const int SampleRate = 44100;

        [MenuItem("Neon Breaker/Regenerate Audio")]
        public static void RegenerateAudio()
        {
            GenerateAll();
            AssetDatabase.Refresh();
            ConfigureImports();
            AssetDatabase.Refresh();
            Debug.Log("Neon Breaker audio regenerated.");
        }

        public static void GenerateAll()
        {
            Directory.CreateDirectory(AudioFolder);

            WriteClip("bgm", BuildMusic(), 0.75f);
            WriteClip("hit", BuildHit(), 0.8f);
            WriteClip("break", BuildBreak(), 0.8f);
            WriteClip("launch", BuildLaunch(), 0.8f);
            WriteClip("lost", BuildLost(), 0.7f);
            WriteClip("powerup", BuildPowerUp(), 0.8f);
            WriteClip("gameover", BuildGameOver(), 0.8f);
            WriteClip("victory", BuildVictory(), 0.8f);
            WriteClip("levelclear", BuildLevelClear(), 0.8f);
            WriteClip("click", BuildClick(), 0.7f);

            AssetDatabase.Refresh();
        }

        private static float[] BuildMusic()
        {
            float[] buffer = new float[SampleRate * 16];

            float[][] chords =
            {
                new[] { 220.00f, 261.63f, 329.63f, 440.00f, 329.63f, 261.63f, 329.63f, 440.00f },
                new[] { 174.61f, 220.00f, 261.63f, 349.23f, 261.63f, 220.00f, 261.63f, 349.23f },
                new[] { 261.63f, 329.63f, 392.00f, 523.25f, 392.00f, 329.63f, 392.00f, 523.25f },
                new[] { 196.00f, 246.94f, 293.66f, 392.00f, 293.66f, 246.94f, 293.66f, 392.00f }
            };

            float[] bassNotes = { 110.00f, 87.31f, 130.81f, 98.00f };

            for (int repeat = 0; repeat < 2; repeat++)
            {
                for (int chordIndex = 0; chordIndex < 4; chordIndex++)
                {
                    float chordStart = repeat * 8f + chordIndex * 2f;
                    float[] notes = chords[chordIndex];

                    for (int noteIndex = 0; noteIndex < notes.Length; noteIndex++)
                    {
                        AddTone(buffer, chordStart + noteIndex * 0.25f, 0.22f, notes[noteIndex], notes[noteIndex] * 0.995f, 0.11f, 3.6f, 0.004f);
                    }

                    AddBass(buffer, chordStart, 1.85f, bassNotes[chordIndex], 0.16f);
                    AddNoise(buffer, chordStart + 0.25f, 0.05f, 0.018f, 8f, 7100);
                    AddNoise(buffer, chordStart + 0.75f, 0.05f, 0.014f, 8f, 7101);
                    AddNoise(buffer, chordStart + 1.25f, 0.05f, 0.016f, 8f, 7102);
                    AddNoise(buffer, chordStart + 1.75f, 0.05f, 0.012f, 8f, 7103);
                }
            }

            return buffer;
        }

        private static float[] BuildHit()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.09f)];
            AddSquare(buffer, 0f, 0.09f, 620f, 430f, 0.32f, 5f);
            AddTone(buffer, 0f, 0.06f, 900f, 700f, 0.1f, 6f, 0.002f);
            return buffer;
        }

        private static float[] BuildBreak()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.24f)];
            AddNoise(buffer, 0f, 0.22f, 0.4f, 5f, 2024);
            AddTone(buffer, 0f, 0.18f, 220f, 90f, 0.28f, 4f, 0.002f);
            AddSquare(buffer, 0f, 0.08f, 700f, 400f, 0.12f, 8f);
            return buffer;
        }

        private static float[] BuildLaunch()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.24f)];
            AddTone(buffer, 0f, 0.22f, 320f, 980f, 0.32f, 2.8f, 0.006f);
            AddSquare(buffer, 0f, 0.14f, 240f, 520f, 0.1f, 4f);
            return buffer;
        }

        private static float[] BuildLost()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.32f)];
            AddTone(buffer, 0f, 0.3f, 460f, 120f, 0.32f, 2.4f, 0.004f);
            AddSquare(buffer, 0f, 0.24f, 240f, 90f, 0.1f, 3f);
            return buffer;
        }

        private static float[] BuildPowerUp()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.5f)];
            AddTone(buffer, 0.00f, 0.16f, 659.25f, 659.25f, 0.28f, 3f, 0.004f);
            AddTone(buffer, 0.09f, 0.16f, 880.00f, 880.00f, 0.28f, 3f, 0.004f);
            AddTone(buffer, 0.18f, 0.18f, 1318.51f, 1318.51f, 0.26f, 3f, 0.004f);
            AddTone(buffer, 0.28f, 0.2f, 1760.00f, 1760.00f, 0.2f, 3.2f, 0.004f);
            return buffer;
        }

        private static float[] BuildGameOver()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 1.05f)];
            AddSquare(buffer, 0.00f, 0.28f, 392f, 392f, 0.22f, 2f);
            AddSquare(buffer, 0.22f, 0.3f, 329.63f, 329.63f, 0.22f, 2f);
            AddSquare(buffer, 0.46f, 0.32f, 261.63f, 261.63f, 0.22f, 2f);
            AddSquare(buffer, 0.72f, 0.3f, 196f, 180f, 0.24f, 2.4f);
            AddTone(buffer, 0f, 1f, 110f, 55f, 0.22f, 1.6f, 0.05f);
            return buffer;
        }

        private static float[] BuildVictory()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 1.4f)];
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f, 1318.51f };
            for (int i = 0; i < notes.Length; i++)
            {
                AddTone(buffer, i * 0.14f, 0.5f, notes[i], notes[i], 0.24f, 2.3f, 0.004f);
                AddSquare(buffer, i * 0.14f, 0.2f, notes[i] * 0.5f, notes[i] * 0.5f, 0.07f, 3f);
            }

            AddTone(buffer, 0.72f, 0.62f, 1046.50f, 1046.50f, 0.16f, 1.6f, 0.004f);
            AddTone(buffer, 0.72f, 0.62f, 1318.51f, 1318.51f, 0.12f, 1.6f, 0.004f);
            AddTone(buffer, 0.72f, 0.62f, 1567.98f, 1567.98f, 0.1f, 1.6f, 0.004f);
            return buffer;
        }

        private static float[] BuildLevelClear()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.85f)];
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
            for (int i = 0; i < notes.Length; i++)
            {
                AddTone(buffer, i * 0.12f, 0.35f, notes[i], notes[i], 0.24f, 2.5f, 0.004f);
            }

            AddNoise(buffer, 0.05f, 0.5f, 0.04f, 2f, 4040);
            return buffer;
        }

        private static float[] BuildClick()
        {
            float[] buffer = new float[Mathf.CeilToInt(SampleRate * 0.06f)];
            AddSquare(buffer, 0f, 0.05f, 1050f, 780f, 0.25f, 8f);
            return buffer;
        }

        private static void AddTone(
            float[] buffer,
            float startSeconds,
            float duration,
            float startFrequency,
            float endFrequency,
            float volume,
            float decay,
            float attack)
        {
            int startSample = Mathf.CeilToInt(startSeconds * SampleRate);
            int length = Mathf.CeilToInt(duration * SampleRate);
            double phase = 0.0;

            for (int i = 0; i < length; i++)
            {
                int index = startSample + i;
                if (index < 0 || index >= buffer.Length)
                {
                    continue;
                }

                float progress = (float)i / Mathf.Max(1, length);
                float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
                phase += 2.0 * Mathf.PI * frequency / SampleRate;

                float envelope = Mathf.Min(1f, progress * duration / Mathf.Max(0.001f, attack)) * Mathf.Exp(-decay * progress);
                buffer[index] += (float)System.Math.Sin(phase) * volume * envelope;
            }
        }

        private static void AddSquare(
            float[] buffer,
            float startSeconds,
            float duration,
            float startFrequency,
            float endFrequency,
            float volume,
            float decay)
        {
            int startSample = Mathf.CeilToInt(startSeconds * SampleRate);
            int length = Mathf.CeilToInt(duration * SampleRate);
            double phase = 0.0;

            for (int i = 0; i < length; i++)
            {
                int index = startSample + i;
                if (index < 0 || index >= buffer.Length)
                {
                    continue;
                }

                float progress = (float)i / Mathf.Max(1, length);
                float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
                phase += 2.0 * Mathf.PI * frequency / SampleRate;

                float wave = System.Math.Sin(phase) >= 0.0 ? 1f : -1f;
                float softWave = wave * 0.7f + (float)System.Math.Sin(phase * 2.0) * 0.3f;
                float envelope = Mathf.Exp(-decay * progress);
                buffer[index] += softWave * volume * envelope;
            }
        }

        private static void AddBass(float[] buffer, float startSeconds, float duration, float frequency, float volume)
        {
            int startSample = Mathf.CeilToInt(startSeconds * SampleRate);
            int length = Mathf.CeilToInt(duration * SampleRate);
            double phase = 0.0;

            for (int i = 0; i < length; i++)
            {
                int index = startSample + i;
                if (index < 0 || index >= buffer.Length)
                {
                    continue;
                }

                float progress = (float)i / Mathf.Max(1, length);
                phase += 2.0 * Mathf.PI * frequency / SampleRate;

                float envelope = Mathf.Min(1f, progress * duration / 0.02f) * Mathf.Exp(-1.4f * progress);
                buffer[index] += ((float)System.Math.Sin(phase) * 0.7f + (float)System.Math.Sin(phase * 2.0) * 0.3f) * volume * envelope;
            }
        }

        private static void AddNoise(float[] buffer, float startSeconds, float duration, float volume, float decay, int seed)
        {
            int startSample = Mathf.CeilToInt(startSeconds * SampleRate);
            int length = Mathf.CeilToInt(duration * SampleRate);
            System.Random random = new System.Random(seed);
            float previous = 0f;

            for (int i = 0; i < length; i++)
            {
                int index = startSample + i;
                if (index < 0 || index >= buffer.Length)
                {
                    continue;
                }

                float progress = (float)i / Mathf.Max(1, length);
                float noise = (float)(random.NextDouble() * 2.0 - 1.0);
                previous = Mathf.Lerp(previous, noise, 0.35f);
                float envelope = Mathf.Exp(-decay * progress);
                buffer[index] += previous * volume * envelope;
            }
        }

        private static void WriteClip(string clipName, float[] samples, float gain)
        {
            float maximum = 0.0001f;

            for (int i = 0; i < samples.Length; i++)
            {
                maximum = Mathf.Max(maximum, Mathf.Abs(samples[i]));
            }

            float scale = Mathf.Min(1f, 0.92f / maximum) * gain;
            float peak = 0.0001f;

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Mathf.Clamp(samples[i] * scale, -1f, 1f);
                peak = Mathf.Max(peak, Mathf.Abs(samples[i]));
            }

            string path = Path.Combine(AudioFolder, clipName + ".wav");
            int sampleCount = samples.Length;
            int dataSize = sampleCount * 2;

            using (FileStream stream = File.Create(path))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(SampleRate);
                writer.Write(SampleRate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(dataSize);

                for (int i = 0; i < sampleCount; i++)
                {
                    writer.Write((short)(samples[i] * short.MaxValue));
                }
            }
        }

        public static void ConfigureImports()
        {
            string[] files = Directory.GetFiles(AudioFolder, "*.wav");

            foreach (string file in files)
            {
                string assetPath = file.Replace('\\', '/');
                AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;

                if (importer == null)
                {
                    continue;
                }

                importer.forceToMono = true;
                importer.loadInBackground = false;

                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.PCM;
                settings.quality = 1f;
                settings.preloadAudioData = true;
                importer.defaultSampleSettings = settings;

                importer.SaveAndReimport();
            }
        }
    }
}
