using UnityEngine;
using System.Collections;
using Gley.MobileAds;
using Gley.MobileAds.Internal;

public class AllAdsIntegrations : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        StartCoroutine(WaitForConsent());
    }


    private IEnumerator WaitForConsent()
    {
        while (!ConsentController.IsConsentReady)
        yield return null;

    // ✅ 2. Initialize ads
    bool initDone = false;
    API.Initialize(() =>
    {
        initDone = true;
        Debug.Log("✅ Gley Ads Initialized.");
    });

    // ✅ 3. Wait until ads are initialized
    while (!initDone)
        yield return null;

    Debug.Log("✅ Ads are ready to load/show.");
		ShowBannerAdSquare();
    }

    // --------------------
    // INTERSTITIAL
    // --------------------
    public void ShowInterstitial()
    {
			   Debug.Log("Show Ad GameOver All");
        if (Gley.MobileAds.API.IsInterstitialAvailable()){
					Debug.Log("Show Ad GameOver All");
					Gley.MobileAds.API.ShowInterstitial();
				}
            
    }

    public void RequestInterstitialAd() { }  // ✅ left empty

    // --------------------
    // REWARDED
    // --------------------
    public bool HasSRewardedVideo()
    {
        return Gley.MobileAds.API.IsRewardedVideoAvailable();
    }

    public void ShowRewardedVideo()
    {
        if (Gley.MobileAds.API.IsRewardedVideoAvailable())
            Gley.MobileAds.API.ShowRewardedVideo(OnRewardComplete);
    }

    private void OnRewardComplete(bool success)
    {
        if (success)
            FindObjectOfType<GameManager>().RewardedVideoCallBack();
    }

    public void RequestRewardedAds() { }  // ✅ left empty

    // --------------------
    // BANNER
    // --------------------
    public void RequestBannerAd() { }
    public void RequestBannerAdSquare() { }

    public void ShowBannerAdSquare()
    {
			  Debug.Log("📢 Trying to show banner...");
        Gley.MobileAds.API.ShowBanner(BannerPosition.Bottom, BannerType.Banner);
    }

    public void HideBannerAdSquare()
    {
        Gley.MobileAds.API.HideBanner();
    }

		public void Show_More_Apps()
	{
		//Debug.Log ("Show More Aps");

		#if UNITY_ANDROID

		#elif UNITY_IPHONE
		Application.OpenURL("https://itunes.apple.com/us/developer/uzair-mehmood/id974140777");
		#endif
	}

    // --------------------
    // OLD COMPATIBILITY FUNCTIONS
    // --------------------
    public void ShowAdsMainMenu() => ShowInterstitial();
    public void ShowAdsLevelComplete() => ShowInterstitial();
    public void ShowAdsGameOver() => ShowInterstitial();
    public void ShowAdsOnPauseGame() => ShowInterstitial();
    public void ShowFreeModeAd() => ShowInterstitial();

    public void RequestApOpenAds() { }
}
