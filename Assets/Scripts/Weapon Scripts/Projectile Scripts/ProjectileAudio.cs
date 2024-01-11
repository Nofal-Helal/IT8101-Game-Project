using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAudio : MonoBehaviour
{

    private Projectile projectile;
    public AudioClip throwClip;
    private AudioSource audioSource;
    public SettingsData settingsData;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume *= settingsData.weaponsVolume;
        projectile = GetComponent<Projectile>();
        projectile.OnThrow += PlayThrowAudio;
    }

    void OnDestroy()
    {
        projectile.OnThrow -= PlayThrowAudio;
    }

    void PlayThrowAudio()
    {
        audioSource.clip = throwClip;
        audioSource.Play();
    }
}
