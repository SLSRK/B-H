using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.LevelPlay;
using System;


public class AdController : MonoBehaviour
{
    private string interstitialAdUnitId = "pwkamvcrm7p6v1z8"; //
    LevelPlayInterstitialAd interstitialAd;

    private float adTimer;
    private static bool initialized;
    private static bool _initCallbackHandled;

    private IGameController currentGameController;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        //InitAd();
    }

    private void Update()
    {
        adTimer += Time.deltaTime;
    }

    public void InitAd()
    {
        if (initialized) return;
        initialized = true;

        Debug.Log("Ad iniatilization");
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        LevelPlay.Init("21c5114bd"); //
    }

    public void InitGameController(IGameController controller)
    {
        currentGameController = controller;
    }

    public void ClearGameController()
    {
        currentGameController = null;
    }

    void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
    {
        if (_initCallbackHandled) return;
        _initCallbackHandled = true;

        Debug.Log("Ad Init Seccessfel");
        LoadAd();
    }

    void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
    {
        Debug.Log("Ad Init failed");
    }

    private void LoadAd()
    {
        Debug.Log("Ad loading");

        ResetAdTimer();
        if (interstitialAd != null)
        {
            interstitialAd.DestroyAd();
            interstitialAd = null;
        }

        /*var configBuilder = new LevelPlayInterstitialAd.Config.Builder();
        configBuilder.SetBidFloor(0.6); // Minimum bid price in USD
        var isConfig = configBuilder.Build();

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId, isConfig);*/

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

        interstitialAd.LoadAd();
    }

    public void ShowAd()
    {
        Debug.Log("Ad opening");
        interstitialAd.ShowAd();
        Time.timeScale = 0f;
    }

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        //ShowAd();
        Debug.Log("Ad loaded");
    }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) 
    {
        Debug.Log("Ad loading failed " + error.ErrorCode + " " + error.ErrorMessage);
        LoadAd();
    }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) 
    {
        Debug.Log("Ad Opened");
    }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log("Ad failed to open");
        StartCoroutine(currentGameController.OnOverDelay());
        Time.timeScale = 1f;
        //LoadAd();
        Invoke(nameof(LoadAd), 5f);
    }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo) 
    {
        Debug.Log("Ad Closed");
        StartCoroutine(currentGameController.OnOverDelay());
        Time.timeScale = 1f;
        LoadAd();
    }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }

    public float GetAdTimer()
    {
        return adTimer;
    }

    public void ResetAdTimer()
    {
        adTimer = 0;
    }
}