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
    public CartSound[] sounds;
    /// <summary>
    /// Amount of overlap between sounds in speed units
    /// </summary>
    public float soundFade = 0.25f;

    void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        railFollower = transform.GetChild(0).GetComponent<RailFollower>();
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

            // TODO: use audio manager for volume control instead of hardcoded value
            sound.source.volume = 0.75f * volume;
        }
    }
}
