using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public class AudioData
    {
        public bool loop;
        public AudioClip audioClip;
    }

    public AudioClip overworldMusic;
    public AudioClip barMusic;
    public GameObject barEntryLevel;

    private AudioSource audioSource;
    private Queue<AudioData> clipQueue;

    private AudioData currentAudioData;

    private bool isTransitioning = false;

    private bool wasInBar = false;
    private bool starting = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clipQueue = new Queue<AudioData>();
    }

    void Update()
    {
        if (starting)
        {
            starting = false;
            if (!barEntryLevel.activeSelf)
            {
                wasInBar = true;
                EnqueueAudio(barMusic, true);
            }
            else
            {
                wasInBar = false;
                EnqueueAudio(overworldMusic, true);
            }

            return;
        }

        if (!barEntryLevel.activeSelf && !wasInBar)
        {
            wasInBar = true;
            ForcePlayWithTransition(barMusic, true);
        }
        else if (barEntryLevel.activeSelf && wasInBar)
        {
            wasInBar = false;
            ForcePlayWithTransition(overworldMusic, true);
        }
    }

    void FixedUpdate()
    {
        if (audioSource.isPlaying || isTransitioning)
        {
            return;
        }

        if (clipQueue != null && clipQueue.Count > 0)
        {
            currentAudioData = clipQueue.Dequeue();
            audioSource.clip = currentAudioData.audioClip;
            audioSource.loop = currentAudioData.loop;
            audioSource.Play();
        }
    }

    public void EnqueueAudio(AudioClip audioClip, bool loop)
    {
        AudioData audioData = new AudioData();
        audioData.audioClip = audioClip;
        audioData.loop = loop;

        clipQueue.Enqueue(audioData);
    }

    public void ForcePlayWithTransition(AudioClip audioClip, bool loop)
    {
        if (audioSource.isPlaying)
        {
            // Empty the queue
            clipQueue.Clear();
            StartCoroutine(TransitionAudio(audioClip, loop));
        }
        else
        {
            audioSource.clip = audioClip;
            audioSource.loop = loop;
            audioSource.Play();

            currentAudioData = new AudioData();
            currentAudioData.audioClip = audioClip;
            currentAudioData.loop = loop;

            // Empty the queue
            clipQueue.Clear();
        }
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private IEnumerator TransitionAudio(AudioClip audioClip, bool loop)
    {
        isTransitioning = true;

        float transitionTime = 0.5f;
        float elapsedTime = 0.0f;

        float startVolume = audioSource.volume;

        while (elapsedTime < transitionTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0.0f, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;

        yield return new WaitForSeconds(0.5f);

        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();

        currentAudioData = new AudioData();
        currentAudioData.audioClip = audioClip;
        currentAudioData.loop = loop;

        isTransitioning = false;
    }
}
