using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Playlist : MonoBehaviour
{
    [SerializeField] AudioClip[] playlist;
    [SerializeField] AudioClip[] bossMusic;
    [SerializeField] GameObject slider;
    [SerializeField] int musicNum = 0;
    int bossMusicNum;
    AudioSource source;
    bool switching;
    bool isBossBattle;
    bool paused = false;

    float lastMusicTime = 0;
    AudioClip lastMusic; 

    Random rand;
    void Start()
    {
        rand = new Random();
        musicNum = rand.Next(0, playlist.Length);
        bossMusicNum = 0;
        switching = false;
        isBossBattle = false;
        source = GetComponent<AudioSource>();
        if (PlayerPrefs.GetFloat("mVolume") == 0)
        {
            PlayerPrefs.SetFloat("mVolume", source.volume);
        }

        setVolume();
        slider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("mVolume") / 0.4f;
        playNext();  
    }

    void Update()
    {
        if (isBossBattle)
        {
            if (!source.isPlaying && !switching) {
                if (!(source.time < source.clip.length - 5f))
                {
                    source.Play();
                    if (bossMusicNum == 0) // Lemon Fight
                    {
                        source.time = 101f;
                    }
                    else if (bossMusicNum == 1) // Phonk
                    {
                        source.time = 60f;
                    }
                    else if (bossMusicNum == 2)
                    {
                        source.time = 3f;
                    }
                }
            }
        }
        else
        {
            if (!source.isPlaying && !switching)
            {
                if (!(source.time < source.clip.length))
                {
                    playNext();
                }
            }
        }  

        if (Time.timeScale == 0 && !paused)
        {
            paused = true;
            source.Pause();
        }
        else if (Time.timeScale != 0 && paused)
        {
            paused = false;
            source.UnPause();
        }
    }

    public void playNext()
    {
        if (isBossBattle)
        {
            return;
        }

        switching = true;
        AudioClip music = playlist[musicNum];
        source.clip = music;
        source.Play();

        if (musicNum < playlist.Length-1)
        {
            musicNum += 1;
        }
        else
        {
            musicNum = 0;
        }

        switching = false;
    }

    public void setVolume()
    {
        source.volume = PlayerPrefs.GetFloat("mVolume");
    }

    public void playBossMusic(int bossNum)
    {
        switching = true;

        bossMusicNum = bossNum;
        AudioClip music = bossMusic[bossNum];
        lastMusic = source.clip;
        lastMusicTime = source.time;
        source.Stop();
        source.clip = music;
        isBossBattle = true;

        source.Play();
        if (bossMusicNum == 0) // Aero Chord
        {
            source.time = 22f;
        }
        else if (bossMusicNum == 1) // Lemon Fight
        {
            source.time = 60f;
        }
        else if (bossMusicNum == 2) // Phonk
        {
            source.time = 3f;
        }
        else if (bossMusicNum == 3) // rhythm game music
        {
            source.time = 17f;
        }

        switching = false;
    }

    public void stopMusic()
    {
        source.Stop();
        isBossBattle = false;
        source.clip = lastMusic;
        source.time = lastMusicTime;
        source.Play();
    }
}
