using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform mainCamera;
    [SerializeField] RadialMenu menu;
    [SerializeField] GameObject menuBase;
    [SerializeField] GameObject startButtonBase;

    public float threshold = -0.25f;
    float currentAlpha = 0;
    bool rampHandMenuFold = false;
    bool rampStartMenuButtonAlpha = false;
    private float lerpDuration = 0.35f;
    private float elapsedStartMenuButtonAlphaTime = 0f;
    private float elapsedHandMenuFoldTime = 0f;
    private Vector3 originalScale;
    private Vector3 originalStartButtonScale;
    private Vector3 zeroScale = new Vector3(0, 0, 0);
    private bool unfoldMenu = false;

    void Start()
    {
        originalScale = transform.localScale;
        originalStartButtonScale = startButtonBase.GetComponent<Transform>().localScale;
    }

    void Update()
    {
        Vector3 directionA = mainCamera.forward;
        Vector3 directionB = transform.forward;
        float dot = Vector3.Dot(directionA.normalized, directionB.normalized);

        rampStartMenuButtonAlpha = false;
        menu.OnLookAway();

        if (dot < threshold)
        {
            rampStartMenuButtonAlpha = true;
        }
        else
        {
            rampHandMenuFold = false;
        }

        if (unfoldMenu)
        {
            menu.OnGaze();
        }

        IncreaseAlpha();
        RampHandMenu();
    }

    void IncreaseAlpha()
    {
        if (rampStartMenuButtonAlpha && rampHandMenuFold == false)
        {
            startButtonBase.SetActive(true);
            elapsedStartMenuButtonAlphaTime += Time.deltaTime;

            if (elapsedStartMenuButtonAlphaTime > 1)
            {
                elapsedStartMenuButtonAlphaTime = 1;
            }

            currentAlpha = Mathf.Lerp(0, 1f, elapsedStartMenuButtonAlphaTime / lerpDuration);
            startButtonBase.GetComponent<Transform>().localScale = Vector3.Lerp(zeroScale, originalStartButtonScale, elapsedStartMenuButtonAlphaTime / lerpDuration);
        }
        else
        {
            elapsedStartMenuButtonAlphaTime -= Time.deltaTime * 2;

            if (elapsedStartMenuButtonAlphaTime < 0)
            {
                startButtonBase.SetActive(false);
                elapsedStartMenuButtonAlphaTime = 0;
            }

            currentAlpha = Mathf.Lerp(0, 1f, elapsedStartMenuButtonAlphaTime / lerpDuration);
            startButtonBase.GetComponent<Transform>().localScale = Vector3.Lerp(zeroScale, originalStartButtonScale, elapsedStartMenuButtonAlphaTime / lerpDuration);
        }
    }

    void RampHandMenu()
    {
        if (rampHandMenuFold)
        {
            elapsedHandMenuFoldTime += Time.deltaTime;

            if (elapsedHandMenuFoldTime > 1)
            {
                elapsedHandMenuFoldTime = 1;
            }

            currentAlpha = Mathf.Lerp(0, 1f, elapsedHandMenuFoldTime / lerpDuration);
            menuBase.GetComponent<Transform>().localScale = Vector3.Lerp(zeroScale, originalScale, elapsedHandMenuFoldTime / lerpDuration);
        }
        else
        {
            elapsedHandMenuFoldTime -= Time.deltaTime * 2;

            if (elapsedHandMenuFoldTime < 0)
            {
                elapsedHandMenuFoldTime = 0;
            }
            unfoldMenu = false;

            currentAlpha = Mathf.Lerp(0, 1f, elapsedHandMenuFoldTime / lerpDuration);
            menuBase.GetComponent<Transform>().localScale = Vector3.Lerp(zeroScale, originalScale, elapsedHandMenuFoldTime / lerpDuration);
        }
    }

    public void ActivateMenuButton()
    {
        unfoldMenu = true;
        //menuBase.SetActive(true);
        rampHandMenuFold = true;
    }
}
