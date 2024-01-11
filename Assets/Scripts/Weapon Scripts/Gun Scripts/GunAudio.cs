using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public AudioClip shootSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;
    public SettingsData settingsData;
    private Gun gunController;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = settingsData.weaponsVolume;
        gunController = GetComponent<Gun>();
        gunController.OnShoot += PlayShootAudio;
        gunController.OnReload += PlayReloadAudio;
    }

    void OnDestroy()
    {
        gunController.OnShoot -= PlayShootAudio;
        gunController.OnReload -= PlayReloadAudio;
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
