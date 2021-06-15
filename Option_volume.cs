using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Option_volume : MonoBehaviour {

    public AudioMixer mixer;
    

    
public void ChangeBGMVolume(float vol)
    {
        mixer.SetFloat("BGMVol", vol);
    }

    public void ChangeSEVolume(float vol)
    {
        mixer.SetFloat("SEVol", vol);
    }


}
