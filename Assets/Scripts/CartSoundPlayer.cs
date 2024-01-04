using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CartSoundPlayer : MonoBehaviour
{
    [Serializable]
    public struct CartSound
    {
        [Tooltip("The speed at which this sound starts playing")]
        public float startSpeed;
        [Tooltip("The speed at which this sound stops playing")]
        public float endSpeed;
        public AudioSource source;
    }


    RailFollower railFollower;

    /// <summary>
    /// Amount of overlap between sounds in speed units
    /// </summary>
    public float soundFade = 0.25f;
    public CartSound[] sounds;
    public AudioSource crashAudioSource;
    private bool playedCrashSound;

    void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        railFollower = transform.GetChild(0).GetComponent<RailFollower>();
        crashAudioSource.volume *= Global.cartVolume;
    }

    // Update is called once per frame
    void Update()
    {
        var speed = railFollower.speed;
        foreach (var sound in sounds)
        {
            if (speed >= sound.startSpeed - soundFade && speed <= sound.endSpeed + soundFade)
            {
                if (!sound.source.isPlaying) sound.source.UnPause();
            }
            else
            {
                if (sound.source.isPlaying) sound.source.Pause();
            }

            // volume fading
            float volume = 1f;
            if (speed < sound.startSpeed + soundFade)
                volume = Mathf.InverseLerp(sound.startSpeed, sound.startSpeed + soundFade, speed);
            else if (speed > sound.endSpeed)
                volume = Mathf.InverseLerp(sound.endSpeed + soundFade, sound.endSpeed, speed);

            // limit volume from the value set in AudioManager
            sound.source.volume = Global.cartVolume * volume;
        }

        // Play crash sound on impact
        if (speed < Mathf.Epsilon)
        {
            if (!playedCrashSound)
            {
                playedCrashSound = true;
                crashAudioSource.Play();
            }
        }
        else
        {
            playedCrashSound = false;
        }
    }
}
