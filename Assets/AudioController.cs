using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip fireClip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShotFire()
    {
        audioSource.clip = fireClip;
        audioSource.Play();
    }
    public void Walk()
    {
        audioSource.clip = walkClip;
        audioSource.Play();
    }
}
