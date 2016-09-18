using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    public Material ma;

    static float targetRatio = 1.0f;
    static float fadeBeginTime = 0.0f;
    static float fadeSpeed = 0.0f;
    static bool fadeBegin = false;
    const string fadeRatioName = "_Float1";
    const string fadeColorName = "_FadeColor";
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
    public void SetFadeTarget(float ratio, float duration)
    {
        if (Mathf.Abs(duration) <= BattleConst.floatZero)
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
    public void FadeIn(float duration)
    {
        SetFadeTarget(1.0f, duration);
    }
    //---------------------------------------------------------------------------------------------
    public void FadeOut(float duration)
    {
        SetFadeTarget(0.0f, duration);
    }
    //---------------------------------------------------------------------------------------------
}
