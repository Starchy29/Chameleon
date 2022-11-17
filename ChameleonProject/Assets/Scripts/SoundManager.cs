using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] mySounds;

    private AudioSource eatingSound;
    private AudioSource bushSound;

    //footsteps
    public AudioSource grassFootstepSound;
    private AudioSource waterFootstepSound;
    private AudioSource rockFootstepSound;
    private AudioSource dirtFootstepSound;
    private AudioSource clayFootstepSound;

    // Start is called before the first frame update
    void Start()
    {
        mySounds = GetComponents<AudioSource>();

        eatingSound = mySounds[0];
        bushSound = mySounds[1];
        grassFootstepSound = mySounds[2];
        waterFootstepSound = mySounds[3];
        rockFootstepSound = mySounds[4];
        dirtFootstepSound = mySounds[5];
        clayFootstepSound = mySounds[6];
    }

    public void PlayEatingSound()
    {
        eatingSound.Play();
    }

    public void PlayBushSound()
    {
        bushSound.Play();
    }

    //grass footstep
    public void PlayGrassFootstep()
    {
        grassFootstepSound.enabled = true;
    }

    public void StopGrassFootstep()
    {
        grassFootstepSound.enabled = false;
    }

    //water footstep
    public void PlayWaterFootstep()
    {
        waterFootstepSound.enabled = true;
    }

    public void StopWaterFootstep()
    {
        waterFootstepSound.enabled = false;
    }

    //rock footstep
    public void PlayRockFootstep()
    {
        rockFootstepSound.enabled = true;
    }

    public void StopRockFootstep()
    {
        rockFootstepSound.enabled = false;
    }

    //dirt footstep
    public void PlayDirtFootstep()
    {
        dirtFootstepSound.enabled = true;
    }

    public void StopDirtFootstep()
    {
        dirtFootstepSound.enabled = false;
    }

    //clay footstep
    public void PlayClayFootstep()
    {
        clayFootstepSound.enabled = true;
    }

    public void StopClayFootstep()
    {
        clayFootstepSound.enabled = false;
    }

    //stop all footsteps
    public void StopFootsteps()
    {
        grassFootstepSound.enabled = false;
        waterFootstepSound.enabled = false;
        rockFootstepSound.enabled = false;
        dirtFootstepSound.enabled = false;
        clayFootstepSound.enabled = false;
    }

    public void InvokeAudio()
    {
        Invoke("StopFootsteps", grassFootstepSound.clip.length / 2);
    }
}
