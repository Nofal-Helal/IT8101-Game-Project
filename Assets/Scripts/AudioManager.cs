using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] cartSounds, monstersSounds, weaponsSounds;
    public AudioSource[] cartSrcs, monstersSrcs, weaponsSrcs;

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    private Sound FindSound(Sound[] sounds, string name){
        Sound s = Array.Find(sounds, x => x.name == name);

        return s;
    }

    public AudioSource PlayCart(string name)
    {
        AudioSource openSource = cartSrcs[0];
        Sound s = FindSound(cartSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            foreach (AudioSource source in cartSrcs)
            {
                if (!source.isPlaying)
                {
                    source.clip = s.clip;
                    source.Play();
                    openSource = source;
                    break;
                }
            }
        }
        return openSource;
    }

    public AudioSource PlayMonsters(string name)
    {
        AudioSource openSource = monstersSrcs[0];
        Sound s = FindSound(monstersSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            foreach (AudioSource source in monstersSrcs)
            {
                if (!source.isPlaying)
                {
                    source.clip = s.clip;
                    source.Play();
                    openSource = source;
                    break;
                }
            }
        }
        return openSource;
    }

    public AudioSource PlayWeapons(string name)
    {
        AudioSource openSource = weaponsSrcs[0];
        Sound s = FindSound(weaponsSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            
            foreach (AudioSource source in weaponsSrcs)
            {
                if (!source.isPlaying)
                {
                    source.clip = s.clip;
                    source.Play();
                    openSource = source;
                    break;
                }
            }
        }
        return openSource;
    }

    public void CartVolume(float volume){
        foreach (AudioSource source in cartSrcs)
        {
            source.volume = volume;
        }
    }

    public void MonstersVolume(float volume){
        foreach (AudioSource source in monstersSrcs)
        {
            source.volume = volume;
        }
    }

    public void WeaponsVolume(float volume){
        foreach (AudioSource source in weaponsSrcs)
        {
            source.volume = volume;
        }
    }


}
