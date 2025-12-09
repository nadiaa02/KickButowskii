using UnityEngine;

public class SimpleSynthesizer
{
    private int sampleRate = 44100;

    public float MidiToFrequency(int midiNote)
    {
        return 440f * Mathf.Pow(2f, (midiNote - 69) / 12f);
    }

    public AudioClip GenerateSineWave(int midiNote, float duration)
    {
        float frequency = MidiToFrequency(midiNote);
        int samples = (int)(sampleRate * duration);

        AudioClip clip = AudioClip.Create($"Note_{midiNote}", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;

            // ADSR envelope (Attack-Decay-Sustain-Release)
            float envelope;
            if (t < 0.02f) // Attack (быстрое нарастание)
                envelope = t / 0.02f;
            else if (t < 0.1f) // Decay
                envelope = 1f - (t - 0.02f) / 0.08f * 0.3f;
            else // Sustain + Release
                envelope = 0.7f * Mathf.Exp(-2f * (t - 0.1f));

            // Основной тон + легкие гармоники для богатства звука
            float sine = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonics = 0.15f * Mathf.Sin(2f * Mathf.PI * frequency * 3f * t);

            data[i] = (sine + harmonics) * envelope * 0.4f;
        }

        clip.SetData(data, 0);
        return clip;
    }

    public AudioClip GenerateSoftBass(int midiNote, float duration)
    {
        float frequency = MidiToFrequency(midiNote);
        int samples = (int)(sampleRate * duration);

        AudioClip clip = AudioClip.Create($"SoftBass_{midiNote}", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;

            // Очень плавное затухание
            float envelope = Mathf.Exp(-1.2f * t);

            // Чистая синусоида + гармоники
            float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonic2 = Mathf.Sin(2f * Mathf.PI * frequency * 2f * t) * 0.3f;
            float harmonic3 = Mathf.Sin(2f * Mathf.PI * frequency * 3f * t) * 0.1f;

            data[i] = (fundamental + harmonic2 + harmonic3) * envelope * 0.2f;
        }

        clip.SetData(data, 0);
        return clip;
    }


    public AudioClip GenerateSquareWave(int midiNote, float duration)
    {
        float frequency = MidiToFrequency(midiNote);
        int samples = (int)(sampleRate * duration);

        AudioClip clip = AudioClip.Create($"Bass_{midiNote}", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;

            // Плавное затухание
            float envelope = Mathf.Exp(-1.5f * t);

            // Мягкая синусоида вместо квадратной волны
            float sine = Mathf.Sin(2f * Mathf.PI * frequency * t);

            // Добавляем немного гармоник для "жирности"
            float harmonics = 0.3f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t);

            data[i] = (sine + harmonics) * envelope * 0.25f; // Уменьшена громкость
        }

        clip.SetData(data, 0);
        return clip;
    }


    public AudioClip GenerateKick()
    {
        int samples = sampleRate / 4;
        AudioClip clip = AudioClip.Create("Kick", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float frequency = 150f * Mathf.Exp(-10f * t) + 40f;
            float envelope = Mathf.Exp(-8f * t);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope;
        }

        clip.SetData(data, 0);
        return clip;
    }

    public AudioClip GenerateHiHat()
    {
        int samples = sampleRate / 10;
        AudioClip clip = AudioClip.Create("HiHat", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Exp(-20f * t);
            data[i] = Random.Range(-1f, 1f) * envelope * 0.3f;
        }

        clip.SetData(data, 0);
        return clip;
    }

    public AudioClip GenerateSnare()
    {
        int samples = sampleRate / 5;
        AudioClip clip = AudioClip.Create("Snare", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Exp(-15f * t);

            float tone = Mathf.Sin(2f * Mathf.PI * 200f * t) * 0.5f;
            float noise = Random.Range(-1f, 1f) * 0.5f;

            data[i] = (tone + noise) * envelope * 0.4f;
        }

        clip.SetData(data, 0);
        return clip;
    }


}
