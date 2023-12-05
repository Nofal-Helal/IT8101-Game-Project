using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _Instance;
    public Sound[] cartSounds, monstersSounds, weaponsSounds;
    public AudioSource cartSrc, monstersSrc, weaponsSrc;

    public static AudioManager Instance
    {
        get
        {
            if (_Instance != null) return _Instance;
            else
            {
                GameObject prefab = Resources.Load("Audio Manager") as GameObject;
                if (prefab == null || prefab.GetComponent<AudioManager>() == null)
                {
                    Debug.LogError("Prefab for audio manager is not found.");
                }
                else
                {
                    GameObject gameObject = Instantiate(prefab);
                    DontDestroyOnLoad(gameObject);
                    _Instance = gameObject.GetComponent<AudioManager>();
                }

                return _Instance;
            }
        }
    }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Sound FindSound(Sound[] sounds, string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);

        return s;
    }

    public void PlayCart(string name)
    {
        Sound s = FindSound(cartSounds, name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            cartSrc.clip = s.clip;
            cartSrc.Play();
        }
    }

    public void PlayMonsters(string name)
    {
        Sound s = FindSound(monstersSounds, name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            monstersSrc.clip = s.clip;
            monstersSrc.Play();
        }
    }

    public void PlayWeapons(string name)
    {
        Sound s = FindSound(weaponsSounds, name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            weaponsSrc.clip = s.clip;
            weaponsSrc.Play();
        }
    }

    public void CartVolume(float volume)
    {
        cartSrc.volume = volume;
    }

    public void MonstersVolume(float volume)
    {
        monstersSrc.volume = volume;
    }

    public void WeaponsVolume(float volume)
    {
        weaponsSrc.volume = volume;
    }


}
