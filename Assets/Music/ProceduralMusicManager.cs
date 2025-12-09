using UnityEngine;
using System.Collections;

public class ProceduralMusicManager : MonoBehaviour
{
    [Header("Timing")]
    [Range(60, 180)] public float bpm = 140f;

    [Header("Layer Timing (seconds)")]
    public float bassStartTime = 5f;
    public float drumsStartTime = 10f;
    public float melodyStartTime = 15f;

    [Header("Volumes")]
    [Range(0, 1)] public float ambientVolume = 0.15f;
    [Range(0, 1)] public float bassVolume = 0.3f;
    [Range(0, 1)] public float drumsVolume = 0.4f;
    [Range(0, 1)] public float melodyVolume = 0.6f;

    private AudioSource ambientSource;
    private AudioSource bassSource;
    private AudioSource drumsSource;
    private AudioSource melodySource;

    private MarkovChain melodyChain;
    private MarkovChain bassChain;
    private SimpleSynthesizer synth;

    private int[] melodySequence;
    private int[] bassSequence;
    private int melodyIndex = 0;
    private int bassIndex = 0;

    private float beatInterval;
    private float nextBeatTime;
    private float startTime;
    private int beatCount = 0;

    private bool bassActive = false;
    private bool drumsActive = false;
    private bool melodyActive = false;

    void Start()
    {
        synth = new SimpleSynthesizer();
        beatInterval = 60f / bpm;
        startTime = Time.time;
        nextBeatTime = Time.time;

        SetupAudioSources();
        TrainMarkovChains();

        melodySequence = melodyChain.Generate(60, 200);
        bassSequence = bassChain.Generate(48, 100);

        StartAmbient();
        StartCoroutine(BeatSystem());
    }

    void SetupAudioSources()
    {
        ambientSource = gameObject.AddComponent<AudioSource>();
        bassSource = gameObject.AddComponent<AudioSource>();
        drumsSource = gameObject.AddComponent<AudioSource>();
        melodySource = gameObject.AddComponent<AudioSource>();

        ambientSource.volume = ambientVolume;
        bassSource.volume = 0;
        drumsSource.volume = 0;
        melodySource.volume = 0;

        // Добавляем реверб для ambient
        AudioReverbFilter reverb = ambientSource.gameObject.AddComponent<AudioReverbFilter>();
        reverb.reverbPreset = AudioReverbPreset.Cave;
        reverb.dryLevel = -1000f;
        reverb.room = -1000f;
        reverb.roomHF = -100f;
    }


    void TrainMarkovChains()
    {
        // Мажорная гамма C major (C, D, E, F, G, A, B)
        // 60=C, 62=D, 64=E, 65=F, 67=G, 69=A, 71=B

        melodyChain = new MarkovChain();

        // Паттерн 1: Восходящая мелодия
        int[] melodyPattern1 = { 60, 62, 64, 65, 67, 65, 64, 62, 60 }; // C D E F G F E D C

        // Паттерн 2: Арпеджио мажорного аккорда
        int[] melodyPattern2 = { 60, 64, 67, 72, 67, 64, 60 }; // C E G C(октавой выше) G E C

        // Паттерн 3: Более живая мелодия
        int[] melodyPattern3 = { 67, 69, 67, 65, 64, 62, 64, 65, 67 }; // G A G F E D E F G

        melodyChain.Train(melodyPattern1);
        melodyChain.Train(melodyPattern2);
        melodyChain.Train(melodyPattern3);

        // Бас - тоника и доминанта (классическая прогрессия)
        bassChain = new MarkovChain();
        int[] bassPattern = { 48, 48, 48, 55, 55, 48, 48, 53, 53, 48 }; // C C C G G C C F F C
        bassChain.Train(bassPattern);
    }


    void StartAmbient()
    {
        AudioClip ambientPad = synth.GenerateSineWave(60, 4f);
        ambientSource.clip = ambientPad;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    IEnumerator BeatSystem()
    {
        while (true)
        {
            yield return new WaitUntil(() => Time.time >= nextBeatTime);

            float elapsedTime = Time.time - startTime;
            CheckLayerActivation(elapsedTime);
            PlayBeat();

            nextBeatTime += beatInterval;
            beatCount++;
        }
    }

    void CheckLayerActivation(float elapsedTime)
    {
        if (!bassActive && elapsedTime >= bassStartTime)
        {
            bassActive = true;
            StartCoroutine(FadeIn(bassSource, bassVolume, 2f));
        }

        if (!drumsActive && elapsedTime >= drumsStartTime)
        {
            drumsActive = true;
            StartCoroutine(FadeIn(drumsSource, drumsVolume, 2f));
        }

        if (!melodyActive && elapsedTime >= melodyStartTime)
        {
            melodyActive = true;
            StartCoroutine(FadeIn(melodySource, melodyVolume, 2f));
        }
    }

    void PlayBeat()
    {
        if (melodyActive)
        {
            int note = melodySequence[melodyIndex];

            // Иногда играем короткие ноты, иногда длинные
            float noteDuration = (melodyIndex % 4 == 0) ? 0.5f : 0.3f;

            AudioClip melodyClip = synth.GenerateSineWave(note, noteDuration);
            melodySource.PlayOneShot(melodyClip);
            melodyIndex = (melodyIndex + 1) % melodySequence.Length;
        }


        if (bassActive && beatCount % 2 == 0)
        {
            int note = bassSequence[bassIndex];
            AudioClip bassClip = synth.GenerateSoftBass(note, 0.8f);
            bassSource.PlayOneShot(bassClip);
            bassIndex = (bassIndex + 1) % bassSequence.Length;
        }

        if (drumsActive)
        {
            if (beatCount % 4 == 0 || beatCount % 4 == 2)
            {
                drumsSource.PlayOneShot(synth.GenerateKick());
            }

            if (beatCount % 4 == 1 || beatCount % 4 == 3)
            {
                drumsSource.PlayOneShot(synth.GenerateSnare());
            }

            drumsSource.PlayOneShot(synth.GenerateHiHat());
        }
    }

    IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        float startTime = Time.time;
        float startVolume = source.volume;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            source.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        source.volume = targetVolume;
    }
}
