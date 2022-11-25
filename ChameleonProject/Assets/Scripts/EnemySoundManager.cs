using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    private AudioSource[] enemySounds;

    private AudioSource snakeDeathSound;
    private AudioSource birdDeathSound;

    // Start is called before the first frame update
    void Start()
    {
        enemySounds = GetComponents<AudioSource>();

        snakeDeathSound = enemySounds[0];
        birdDeathSound = enemySounds[1];
    }

    public void PlaySnakeDeathSound()
    {
        snakeDeathSound.Play();
    }

    public void PlayBirdDeathSound()
    {
        birdDeathSound.Play();
    }
}
