using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] cartSounds, monstersSounds, weaponsSounds;
    public AudioSource cartSrc, monstersSrc, weaponsSrc;

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

    public void PlayCart(string name)
    {
        Sound s = FindSound(cartSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            cartSrc.clip = s.clip;
            cartSrc.Play();
        }
    }

    public void PlayMonsters(string name)
    {
        Sound s = FindSound(monstersSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            monstersSrc.clip = s.clip;
            monstersSrc.Play();
        }
    }

    public void PlayWeapons(string name)
    {
        Sound s = FindSound(weaponsSounds, name);
        if(s == null){
            Debug.Log("Sound not found");
        }else{
            weaponsSrc.clip = s.clip;
            weaponsSrc.Play();
        }
    }

    public void CartVolume(float volume){
        cartSrc.volume = volume;
    }

    public void MonstersVolume(float volume){
        monstersSrc.volume = volume;
    }

    public void WeaponsVolume(float volume){
        weaponsSrc.volume = volume;
    }


}
