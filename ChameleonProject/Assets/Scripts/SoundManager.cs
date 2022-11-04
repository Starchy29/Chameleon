using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] mySounds;

    private AudioSource eatingSound;
    private AudioSource bushSound;
    private AudioSource grassFootstepSound;

    // Start is called before the first frame update
    void Start()
    {
        mySounds = GetComponents<AudioSource>();

        eatingSound = mySounds[0];
        bushSound = mySounds[1];
        grassFootstepSound = mySounds[2];
    }

    public void PlayEatingSound()
    {
        eatingSound.Play();
    }

    public void PlayBushSound()
    {
        bushSound.Play();
    }

    public void PlayGrassFootstep()
    {
        grassFootstepSound.enabled = true;
    }

    public void StopFootsteps()
    {
        grassFootstepSound.enabled = false;
    }
}
