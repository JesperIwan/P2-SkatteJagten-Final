using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Audio[] backgroundSfx, sfxSounds;
    public AudioSource BGSource, sfxSource;

   // public AudioClip defaultClickSound;


    private void Awake()
    {
        // --- Singleton setup ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // --- Ensure AudioSources exist ---
        if (BGSource == null)
            BGSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        BGSource.loop = true;
    }

    private void Start()
    {
        PlayMusic("Dyt1");
    }

    public void PlayMusic(string name)
    {
        Audio s = Array.Find(backgroundSfx, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");

        }
        else
        {
            BGSource.clip = s.clip;
            BGSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        Audio s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");

        }
        else
        {
            //sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(s.clip);
        }
    }
   /* public void PlayClick()
    {
        if (defaultClickSound != null)
            sfxSource.PlayOneShot(defaultClickSound);
    }
    */
    public void ToogleMusic()
    {
        BGSource.mute = !BGSource.mute; 
    }

    public void ToogleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void BgVolume(float volume)
    {
        BGSource.volume = volume;
    }
    public void SfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
