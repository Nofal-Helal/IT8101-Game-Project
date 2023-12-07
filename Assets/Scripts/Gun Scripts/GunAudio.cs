using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public AudioClip shootSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;
    private Gun gunController;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gunController = GetComponent<Gun>();
        gunController.OnShoot += PlayShootAudio;
        gunController.OnReload += PlayReloadAudio;
    }

    private void PlayShootAudio()
    {
        audioSource.clip = shootSound;
        audioSource.Play();
    }
    private void PlayReloadAudio()
    {
        audioSource.clip = reloadSound;
        audioSource.Play();
    }
}
