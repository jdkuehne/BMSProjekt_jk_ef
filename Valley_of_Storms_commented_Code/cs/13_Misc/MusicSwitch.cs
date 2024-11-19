using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Musik-Wechsler, kann in Scene platziert werden, wenn ein Musikwechsel bei Eintritt in die Kammer benötigt wird
public class MusicSwitch : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Initializer.InitInstance.NeverDestroyLoaded);
        AudioSource musicPlayer = GameObject.FindWithTag("MusicPlayer").GetComponent<AudioSource>();
        musicPlayer.Stop();
        yield return new WaitForSeconds(0.5f);       
        musicPlayer.clip = music;
        musicPlayer.Play();
    }
}
