using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using ShadowQuality = UnityEngine.ShadowQuality;
using ShadowResolution = UnityEngine.ShadowResolution;
using Slider = UnityEngine.UIElements.Slider;

public class MenuUI : MonoBehaviour {

    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Volume volume;
    
    private ColorAdjustments colorAdjustments;
    
    VisualElement mainMenuRoot;
    VisualElement optionsMenuRoot;

    Resolution[] allResolutions;
    FullScreenMode currentScreenMode;
    int selectedResIndex;
    
    private void Start() {
        if(volume.profile.TryGet(out colorAdjustments)) LogFunction("Found colorAdjustments");
        
        currentScreenMode = Screen.fullScreenMode;
        allResolutions = Screen.resolutions;

        var resOptions = new List<string>();
        foreach (var res in allResolutions) {
            resOptions.Add(res.width + " x " + res.height);
        }

        var qualities = new List<string>(QualitySettings.names);
        
        mainMenuRoot = UIDocument.rootVisualElement;
        
        var menuPanel = mainMenuRoot.Q<VisualElement>("MenuPanel");
        var optionPanel = mainMenuRoot.Q<VisualElement>("OptionsPanel");
        var creditPanel = mainMenuRoot.Q<VisualElement>("CreditPanel");
        var levelPanel = mainMenuRoot.Q<VisualElement>("ChooseLevelPanel");
        
        
        //===Buttons Menu===
        var playBtt = mainMenuRoot.Q<Button>("PlayButton");
        playBtt.RegisterCallback<ClickEvent>(evt =>  ShowPanelMenu(true, menuPanel, levelPanel));
        
        var settingsBtt = mainMenuRoot.Q<Button>("OptionsButton");
        settingsBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(true, menuPanel, optionPanel));
        
        var creditBtt = mainMenuRoot.Q<Button>("CreditButton");
        creditBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(true, menuPanel, creditPanel));
        
        var opMenuBtt = mainMenuRoot.Q<Button>("OptionsMenuBtt");
        opMenuBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(false, menuPanel, optionPanel));
        
        var crMenuBtt = mainMenuRoot.Q<Button>("CreditMenuBtt");
        crMenuBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(false, menuPanel, creditPanel));
        
        var lvlMenuBtt = mainMenuRoot.Q<Button>("LevelMenuBtt");
        lvlMenuBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(false, menuPanel, levelPanel));
        
        var quitBtt = mainMenuRoot.Q<Button>("QuitButton");
        quitBtt.RegisterCallback<ClickEvent>(evt => QuitApplication());
        
        //===Video Settings===
        var resolutionSettings = mainMenuRoot.Q<DropdownField>("Resolution");
        resolutionSettings.choices = resOptions;
        resolutionSettings.value = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        resolutionSettings.RegisterValueChangedCallback(SetScreenRes);
        
        var displayMode = mainMenuRoot.Q<EnumField>("DisplayMode");
        displayMode.Init(Screen.fullScreenMode);
        displayMode.value = Screen.fullScreenMode;
        displayMode.RegisterValueChangedCallback(SetFullScreenSettings);
        
        var qualityPreset = mainMenuRoot.Q<DropdownField>("QualityPreset");
        qualityPreset.choices = qualities;
        qualityPreset.index = QualitySettings.GetQualityLevel();
        qualityPreset.RegisterValueChangedCallback(evt => SetQualityPreset(qualityPreset));

        var contrast = mainMenuRoot.Q<SliderInt>("Contrast");
        contrast.RegisterValueChangedCallback(evt => colorAdjustments.contrast.value = evt.newValue);
        
        var brightness = mainMenuRoot.Q<Slider>("Brightness");
        brightness.RegisterValueChangedCallback(evt => colorAdjustments.postExposure.value = evt.newValue);
        
        var fovSlider = mainMenuRoot.Q<SliderInt>("FOV");
        fovSlider.RegisterValueChangedCallback(SetFov);
        
        var textureQuality = mainMenuRoot.Q<DropdownField>("TextureQuality");
        textureQuality.choices = new List<string> {
            "Full Resolution",
            "Half Resolution",
            "Quarter Resolution",
            "Eighth Resolution",
        };
        textureQuality.index = QualitySettings.globalTextureMipmapLimit;
        textureQuality.RegisterValueChangedCallback(evt => QualitySettings.globalTextureMipmapLimit = textureQuality.index);
        
        var shadowQuality = mainMenuRoot.Q<DropdownField>("ShadowQuality");
        shadowQuality.choices = new List<string> {
            "Off",
            "Hard Shadow",
            "Soft Shadow",
        };
        shadowQuality.index = (int)QualitySettings.shadows;
        shadowQuality.RegisterValueChangedCallback(evt => ApplyShadowQuality(shadowQuality.index));
        
        var shadowRes = mainMenuRoot.Q<DropdownField>("ShadowRes");
        shadowRes.choices = new List<string> {
            "Low",
            "Medium",
            "High"
        };
        shadowRes.index = (int)QualitySettings.shadowResolution;
        shadowRes.RegisterValueChangedCallback(evt => ApplyShadowResolution(shadowRes.index));
        
        //===Audio Settings===
        var masterVol = mainMenuRoot.Q<SliderInt>("Master");
        masterVol.RegisterValueChangedCallback(evt => SetVolume(evt, "Master"));

        var musicVol = mainMenuRoot.Q<SliderInt>("Music");
        musicVol.RegisterValueChangedCallback(evt => SetVolume(evt, "Music"));
        
        var sfxVol = mainMenuRoot.Q<SliderInt>("SFX");
        sfxVol.RegisterValueChangedCallback(evt => SetVolume(evt, "SFX"));
        
        var voiceVol = mainMenuRoot.Q<SliderInt>("Voice");
        voiceVol.RegisterValueChangedCallback(evt => SetVolume(evt, "Voice"));
        
        //===Game Settings===
        
        
        // var slider = root.Q<Slider>("Slider");
        // slider.RegisterValueChangedCallback<float>(evt => ExampleFunction("Change Slider" + evt.newValue));
        //
        // var dropDown = root.Q<DropdownField>("Dropdown");
        // dropDown.RegisterValueChangedCallback(evt => ExampleFunction("Change Dropdown" + evt.newValue));
    }

    private void SetFov(ChangeEvent<int> evt) {
        LogFunction($"Set Fov to {evt.newValue}");
        Camera.main.fieldOfView = evt.newValue;
    }

    #region General Settings
    private void ShowPanelMenu(bool show, VisualElement menuPanel, VisualElement openedPanel) {
        LogFunction(show ? $"Switch to panel : {openedPanel.name}" : $"Switch to panel : {menuPanel.name}"); 
        menuPanel.style.display = show ? DisplayStyle.None : DisplayStyle.Flex;
        openedPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private void QuitApplication() {
        LogFunction("Quit Application");
        Application.Quit();
    }
    #endregion
    
    #region Video Settings
    private void SetScreenRes(ChangeEvent<string> evt) {
        var res = evt.newValue.Split('x');
        
        var width = int.Parse(res[0]);
        var height = int.Parse(res[1]);
        
        LogFunction($"Change Screen Resolution to : {width}x{height}");
        
        Screen.SetResolution(width, height, currentScreenMode);
    }
    
    private void SetFullScreenSettings(ChangeEvent<Enum> evt) {
        var mode = (FullScreenMode)evt.newValue;
        LogFunction($"Change FullScreen Mode to : {mode}");
        var res = Screen.currentResolution;
        currentScreenMode = mode;
        Screen.SetResolution(res.width, res.height, mode);
    }
    
    private void SetQualityPreset(DropdownField field) {
        LogFunction($"Change Quality Preset : {field.value}");
        QualitySettings.SetQualityLevel(field.index, true);
    }
    
    private void ApplyShadowQuality(int index) {
        LogFunction($"Updating Shadow Quality");
        var sun = RenderSettings.sun;

        switch (index) {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable;
                if (sun != null) sun.shadows = LightShadows.None;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                if (sun != null) sun.shadows = LightShadows.Hard;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                if (sun != null) sun.shadows = LightShadows.Soft;
                break;
        }
    }
    
    private void ApplyShadowResolution(int index) {
        LogFunction($"Updating Shadow Resolution");
       
        switch(index) {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowDistance = 25f;
                break;

            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                QualitySettings.shadowDistance = 50f;
                break;

            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                QualitySettings.shadowDistance = 100f;
                break;
        }
    }
    
    #endregion

    #region Audio Settings

    private void SetVolume(ChangeEvent<int> evt, string mixerName) {
        LogFunction($"Change Volume {mixerName} : {evt.newValue}");
        
        var vol = Mathf.Log10(evt.newValue / 100f) * 20f;
        if(evt.newValue == 0) audioMixer.SetFloat(mixerName, -80f);
        else audioMixer.SetFloat(mixerName, vol);
    }

    #endregion
    
    private void LogFunction(string args) {
        Debug.Log(args);
    }
}
