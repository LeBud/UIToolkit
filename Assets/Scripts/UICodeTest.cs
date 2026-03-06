using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;

public class UICodeTest : MonoBehaviour {

    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private AudioMixer audioMixer;
    
    VisualElement mainMenuRoot;
    VisualElement optionsMenuRoot;

    Resolution[] allResolutions;
    FullScreenMode currentScreenMode;
    int selectedResIndex;
    
    private void Start() {
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
        
        
        //===Buttons Menu===
        var playBtt = mainMenuRoot.Q<Button>("PlayButton");
        playBtt.RegisterCallback<ClickEvent>(evt => LogFunction("Play Button Clicked"));
        
        var settingsBtt = mainMenuRoot.Q<Button>("OptionsButton");
        settingsBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(true, menuPanel, optionPanel));
        
        var creditBtt = mainMenuRoot.Q<Button>("CreditButton");
        creditBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(true, menuPanel, creditPanel));
        
        var opMenuBtt = mainMenuRoot.Q<Button>("OptionsMenuBtt");
        opMenuBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(false, menuPanel, optionPanel));
        
        var crMenuBtt = mainMenuRoot.Q<Button>("CreditMenuBtt");
        crMenuBtt.RegisterCallback<ClickEvent>(evt => ShowPanelMenu(false, menuPanel, creditPanel));
        
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

        var fovSlider = mainMenuRoot.Q<SliderInt>("FOV");
        fovSlider.RegisterValueChangedCallback(SetFov);
        
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
