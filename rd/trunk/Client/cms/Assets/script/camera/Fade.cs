using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    public static Material ma;

    static float targetRatio = 1.0f;
    static float fadeBeginTime = 0.0f;
    static float fadeSpeed = 0.0f;
    static bool fadeBegin = false;
    const string fadeRatioName = "_Float1";
    //---------------------------------------------------------------------------------------------
    //初始化
    void Start()
    {
        ma = new Material(Shader.Find("Custom/Fade"));
    }
    //---------------------------------------------------------------------------------------------
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, ma);
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        if (ma.GetFloat(fadeRatioName) != targetRatio)
        {
            float curRatio = ma.GetFloat(fadeRatioName) + fadeSpeed * Time.deltaTime;
            if ((fadeSpeed < 0 && curRatio <= targetRatio) || (fadeSpeed > 0 && curRatio >= targetRatio))
            {
                curRatio = targetRatio;
            }

            ma.SetFloat(fadeRatioName, curRatio);
        }

        fadeBegin = false;
    }
    //---------------------------------------------------------------------------------------------
    public static void SetFadeTarget(float ratio, float duration)
    {
        if (duration == 0.0f)
        {
            ma.SetFloat(fadeRatioName, ratio);
        }
        else
        {
            fadeBegin = true;
            targetRatio = ratio;
            fadeBeginTime = Time.time;
            fadeSpeed = (targetRatio - ma.GetFloat(fadeRatioName)) / duration;
        }
    }
    //---------------------------------------------------------------------------------------------
    public static void FadeIn(float duration)
    {
        SetFadeTarget(1.0f, duration);
    }
    //---------------------------------------------------------------------------------------------
    public static void FadeOut(float duration)
    {
        SetFadeTarget(0.0f, duration);
    }
    //---------------------------------------------------------------------------------------------
}
