using UnityEngine;
using System.Collections;
using Gley.MobileAds;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConsentController : MonoBehaviour
{
    public static bool IsConsentReady = false;
    public static bool AdsInitialized = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(FullConsentFlow());
    }

    private IEnumerator FullConsentFlow()
    {
        // ✅ 0. Pre-initialize Gley (needed for GDPR & ATT)
        bool preInit = false;
        API.Initialize(() =>
        {
            preInit = true;
            Debug.Log("✅ Gley pre-initialized");
        });

        while (!preInit)
            yield return null;

        // ✅ 1. GDPR
        yield return StartCoroutine(GDPRFlow());

        // ✅ 2. ATT (Editor + iOS only)
        yield return StartCoroutine(ATTFlow());

        // ✅ 3. Final Ad Initialization
        AdsInitialized = false;
        API.Initialize(() =>
        {
            AdsInitialized = true;
            Debug.Log("✅ Ads fully initialized");
        });

        while (!AdsInitialized)
            yield return null;

        IsConsentReady = true;
        Debug.Log("✅ FULL CONSENT DONE (GDPR + ATT + ADS INIT)");
    }


    // ✅ GDPR FLOW
    private IEnumerator GDPRFlow()
    {
        if (API.GDPRConsentWasSet())
        {
            Debug.Log("✅ GDPR already set");
            yield break;
        }

#if UNITY_EDITOR
        bool accept = EditorUtility.DisplayDialog(
            "GDPR Consent (Editor Test)",
            "We use ads to support the game.\nDo you accept personalized ads?",
            "Accept Personalized Ads",
            "Reject"
        );

        API.SetGDPRConsent(accept);
#else
        bool closed = false;
        API.ShowBuiltInConsentPopup(() => closed = true);
        while (!closed)
            yield return null;
#endif
    }


  private IEnumerator ATTFlow()
{
    Debug.Log("📢 ATT Flow starting...");

    // ✅ If ATT already asked → skip
    int attState = PlayerPrefs.GetInt("ATT_STATUS", 0);
    if (attState != 0)
    {
        Debug.Log("✅ ATT already set: " + attState);
        yield break;
    }

#if UNITY_IOS && !UNITY_EDITOR
    // ✅ REAL ATT POPUP
    bool attClosed = false;

    API.ShowATTPopup(() =>
    {
        attClosed = true;
        Debug.Log("✅ ATT popup closed");

        // iOS does NOT give us the user choice directly.
        // But we can assume it's been asked:
        PlayerPrefs.SetInt("ATT_STATUS", 1); // asked
        PlayerPrefs.Save();
    });

    while (!attClosed)
        yield return null;

#elif UNITY_EDITOR
    // ✅ FAKE ATT POPUP for Editor
    bool accept = EditorUtility.DisplayDialog(
        "ATT Permission (Editor Simulation)",
        "Allow app tracking to deliver personalized ads?",
        "Allow Tracking",
        "Ask App Not to Track"
    );

    // ✅ Save ATT result in Editor
    PlayerPrefs.SetInt("ATT_STATUS", accept ? 1 : 2);
    PlayerPrefs.Save();

    Debug.Log("✅ Editor ATT saved: " + (accept ? "Allowed" : "Not Allowed"));
#endif

    yield return null;
}

}
