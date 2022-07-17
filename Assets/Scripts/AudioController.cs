using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SOUND
{
        NONE,
        HIT,
        TICK,
        ACTION,
        BUMP,
        SPAWN,
        DEATH,
        COLLECT,
        DOOR,
}

public enum MUSIC
{
    NONE,
    WIN,
    LOSE,
    NEW_ROOM
}

public class AudioController : MonoBehaviour
{
    private static AudioController _instance;
    
    [SerializeField]
    private AudioSource SFXAudioSource;
    [SerializeField]
    private AudioSource MusicAudioSource;

    [SerializeField, Header("Sound Clips")]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip tickSound;
    [SerializeField]
    private AudioClip ActionSound;
    [SerializeField]
    private AudioClip bumpSound;
    [SerializeField]
    private AudioClip spawnSound;
    [SerializeField]
    private AudioClip DeathSound;
    
    [SerializeField]
    private AudioClip DoorSound;
    [SerializeField]
    private AudioClip CollectSound;
    
    [SerializeField, Header("Music Clips")]
    private AudioClip winMusicSound;
    [SerializeField]
    private AudioClip loseMusicSound;
    [SerializeField]
    private AudioClip newRoomMusicSound;



    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void PlaySound(SOUND sound, in float volume = 1f)
    {
        _instance.TryPlaySound(sound, volume);
    }

    private void TryPlaySound(SOUND sound, in float volume)
    {
        AudioClip foundClip;
        switch (sound)
        {
            case SOUND.NONE:
                return;
            case SOUND.HIT:
                foundClip = hitSound;
                break;
            case SOUND.TICK:
                foundClip = tickSound;
                break;
            case SOUND.ACTION:
                foundClip = ActionSound;
                break;
            case SOUND.BUMP:
                foundClip = bumpSound;
                break;
            case SOUND.SPAWN:
                foundClip = spawnSound;
                break;
            case SOUND.DEATH:
                foundClip = DeathSound;
                break;
            case SOUND.DOOR:
                foundClip = DoorSound;
                break;
            case SOUND.COLLECT:
                foundClip = CollectSound;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }

        SFXAudioSource.PlayOneShot(foundClip, volume);
    }
    
    public static void PlayMusic(MUSIC music, in float volume = 1f)
    {
        _instance.TryPlaySound(music, volume);
    }

    private void TryPlaySound(MUSIC music, in float volume)
    {
        AudioClip foundClip;
        switch (music)
        {
            case MUSIC.NONE:
                return;
            case MUSIC.WIN:
                foundClip = winMusicSound;
                break;
            case MUSIC.LOSE:
                foundClip = loseMusicSound;
                break;
            case MUSIC.NEW_ROOM:
                foundClip = newRoomMusicSound;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(music), music, null);
        }

        SFXAudioSource.PlayOneShot(foundClip, volume);
    }
}
