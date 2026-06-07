using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;

    public TMPro.TMP_Dropdown resolutionDropdown;

    private List<Resolution> resolutionList;
    Resolution[] resolutions;

// start is updated once on the start 
    void Start(){

        resolutionDropdown.ClearOptions();
        
        resolutionList = new List<Resolution>();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        // geting all avalible resolutions for a PC and adding them to the resolution dropdown
        foreach (Resolution res in Screen.resolutions)
        {
            string option = res.width + " x " + res.height;

            // sorting through useful options and eliminating duplicates
            if (option == "800 x 600" || option == "1280 x 720" || option == "1920 x 1080" || option == "1536 x 864" || option == "1366 x 768" || option == "1440 x 900" || option == "2560 x 1440" || option == "1280 x 1200" || option == "3840 x 2160"){
                if (options.Contains(option)){
                    Debug.Log("caught");
                }
                else{
                    options.Add(option);
                    resolutionList.Add(res);
                    Debug.Log(option);
                }
            }
        }
// creates a optimizedresolution list
        resolutions = resolutionList.ToArray();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(currentResolutionIndex);
    }

    public void SoundSlider(float volume){
// changes audio based on UI
        audioMixer.SetFloat("MasterVolume", volume);
    }    public void MusicSlider(float volume){

// changes audio based on UI
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public void FXSlider(float volume){
// changes audio based on UI
        audioMixer.SetFloat("FXVolume", volume);
    }

    public void SetQuality(int qualityIndex){

// changes qualiti based on UI
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen){

// sets the game fullscrean (a UI button triger)
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex){

// Changes the resolution (called with UI)
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
