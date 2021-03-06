﻿using System;
using UnityEngine;
using Heyzap;

public class MyHeyzap : MonoBehaviour {
	public static event Action <bool> OnGiveReward;
	public static event Action <bool> OnVideoAdsAvailable;
	public static event Action GiveReward;
	[HideInInspector] public bool IsRewardedVideoReady;
	[HideInInspector] public int VideoAdCointer;
	private bool _firstInterstitialShowed;
	public long _counter { get; set; }
	// Use this for initialization
	void Start ()
	{
		IsRewardedVideoReady = false;
		VideoAdCointer = 0;
		HeyzapAds.Start("70d6db5109295d28b9ab83165d3fa95c", HeyzapAds.FLAG_NO_OPTIONS);
//		HeyzapAds.ShowMediationTestSuite();
		
		// Interstitial ads are automatically fetched
		HZInterstitialAd.Fetch("app-launch");
		HZVideoAd.Fetch("video");
		HZIncentivizedAd.Fetch("rewarded");
		HZIncentivizedAd.AdDisplayListener listener = delegate(string adState, string adTag){
			if ( adState.Equals("incentivized_result_complete") ) {
				// The user has watched the entire video and should be given a reward.
				GameEvents.Send(GiveReward);
			}
			if ( adState.Equals("incentivized_result_incomplete") ) {
				// The user did not watch the entire video and should not be given a   reward.
				
			}
			if ( adState.Equals("hide") ) {
				// Sent when an ad has been removed from view.
				// This is a good place to unpause your app, if applicable.
				Defs.MuteSounds (false);
			}
			if ( adState.Equals("available") )
			{
				IsRewardedVideoReady = true;
				// Sent when an ad has been loaded and is ready to be displayed,
				//   either because we autofetched an ad or because you called
				//   `Fetch`.
			}
		};
 
		HZIncentivizedAd.SetDisplayListener(listener);
		
		HZVideoAd.AdDisplayListener listenerVideo = delegate(string adState, string adTag){
			if ( adState.Equals("hide") ) {
				// Sent when an ad has been removed from view.
				// This is a good place to unpause your app, if applicable.
				Defs.MuteSounds (false);
			}
		};
 
		HZVideoAd.SetDisplayListener(listenerVideo);
	}
	
	private void OnEnable() {
		ScreenGame.ShowVideoAds += ShowVideo;
		ScreenGame.ShowRewardedAds += ShowRewarded;
		ScreenMenu.ShowRewardedAds += ShowRewarded;
		ScreenCoins.ShowRewardedAds += ShowRewarded;
		ScreenMenu.ShowBannerAds += ShowBanner;
		ScreenMenu.HideBannerAds += HideBanner;
	}

	private void OnDisable() {
		ScreenGame.ShowVideoAds -= ShowVideo;
		ScreenGame.ShowRewardedAds -= ShowRewarded;
		ScreenMenu.ShowRewardedAds -= ShowRewarded;
		ScreenCoins.ShowRewardedAds -= ShowRewarded;
		
		ScreenMenu.ShowBannerAds -= ShowBanner;
		ScreenMenu.HideBannerAds -= HideBanner;
	}

	private void ShowInterstitial()
	{
		++_counter;
		if (_counter >= Umbrella.ServerSettings.Manager.Get<Int64>("interstitialFrequency", 5))
		{
			// show ad
			_counter = 0;
		}
		else
		{
			return;
		}
		if (HZInterstitialAd.IsAvailable(/*"app-launch"*/))
		{
//			HZShowOptions showOptions = new HZShowOptions();
//			showOptions.Tag = "app-launch";
//			HZInterstitialAd.ShowWithOptions(showOptions);
			HZInterstitialAd.Show();

			_firstInterstitialShowed = true;
		}
	}

	private void ShowVideo()
	{
		// Later, such as after a level is completed
		if (HZVideoAd.IsAvailable("video")) {
			HZShowOptions showOptions = new HZShowOptions();
			showOptions.Tag = "video";
			HZVideoAd.ShowWithOptions(showOptions);
			VideoAdCointer = 0;
			Defs.MuteSounds (true);
		}
	}
	
	private void ShowRewarded()
	{
		// Later, such as after a level is completed
		if (HZIncentivizedAd.IsAvailable("rewarded")) {
			HZIncentivizedShowOptions showOptions = new HZIncentivizedShowOptions();
			showOptions.Tag = "rewarded";
			HZIncentivizedAd.ShowWithOptions(showOptions);
			
			Defs.MuteSounds (true);
			VideoAdCointer = 0;
		}
	}

	private void ShowBanner()
	{
		HZBannerShowOptions showOptions = new HZBannerShowOptions();
		showOptions.Position = HZBannerShowOptions.POSITION_BOTTOM;
		HZBannerAd.ShowWithOptions(showOptions);
	}

	private void HideBanner()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
//		if (!_firstInterstitialShowed)
//		{
//			ShowStartInterstitial();
//		}
	}
}
