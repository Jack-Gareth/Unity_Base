using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu(fileName = "ChannelName", menuName = "SO/Audio/MixerChannelController")]
public class AudioMixerSO : ScriptableObject
{

   
    [SerializeField] AudioMixer mix;
    [SerializeField] string MixName;
    [SerializeField] float initialValue = 1f;
    public event Action<bool> OnToggleMixer;
    public event Action<float> OnSetMixerVolume;
    [NonSerialized][Range(0f, 1f)] float currentVolume;
    public float lastSetMixValue { get; private set; }
 
    public void OnEnable()
    {
        LoadMixerPrefs();
    }

    public void LoadMixerPrefs()
    {
        lastSetMixValue = PlayerPrefs.GetFloat(MixName, initialValue);
        UpdateVolume(lastSetMixValue);
    }

    public void SaveMixerPrefs()
    {
        PlayerPrefs.SetFloat(MixName, currentVolume);
    }

    public bool isMixEnabled()
    {
        float value;
        mix.GetFloat(MixName, out value);
        return value > -80f;
    }

    public void ToggleMix() //For button interaction
    {
        float value;
        mix.GetFloat(MixName, out value);
        if (value > -80f)
        {
            UpdateVolume(0f);
        }
        else
        {
            if (lastSetMixValue != 0f) UpdateVolume(lastSetMixValue);
            else { UpdateVolume(0.5f); };
        }

    }

    public void SetMixVolume(float vol) //For slider interaction if we use sliders
    {
        UpdateVolume(vol);
        lastSetMixValue = vol;
    }

    void UpdateVolume(float vol)
    {
        currentVolume = vol;
        if (currentVolume <= 0.002)
        {
            OnSetMixerVolume?.Invoke(0f);
            OnToggleMixer?.Invoke(false);
            mix.SetFloat(MixName, -80f);
            SaveMixerPrefs();
            return;
        }

        OnToggleMixer?.Invoke(true);
        OnSetMixerVolume?.Invoke(vol);
        mix.SetFloat(MixName, Mathf.Log(vol) * 20);
        SaveMixerPrefs();
    }

}

