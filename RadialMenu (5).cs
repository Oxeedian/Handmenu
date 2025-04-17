using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    struct ClickEvents
    {
        public string buttonName;
        public Color buttonColor;
        public float setSize;
        public UnityEvent function;
    }

    [SerializeField] List<ClickEvents> clickEvents = new List<ClickEvents>();
    [SerializeField] Transform radialPartCanvas;
    [SerializeField] float spaceBetweenParts = 5;
    [SerializeField] GameObject radialButtonPrefab;

    private List<GameObject> spawnedRadialParts = new List<GameObject>();
    int numberOfRadialPart;


    void Start()
    {
     
    }
    private void Awake()
    {
        numberOfRadialPart = clickEvents.Count;
        if (numberOfRadialPart == 1)
        {
            spaceBetweenParts = 0;
        }

        SpawnRadialPart();
    }

    void Update()
    {

    }

    public void OnGaze()
    {
        foreach (GameObject go in spawnedRadialParts)
        {
            RadialButton button = go.GetComponent<RadialButton>();
            button.StartFold(true);
        }
    }
    public void OnLookAway()
    {
        foreach (GameObject go in spawnedRadialParts)
        {
            RadialButton button = go.GetComponent<RadialButton>();
            button.StartFold(false);
        }
    }

    public bool IsUnfoldDone()
    {
        foreach (GameObject go in spawnedRadialParts)
        {
            RadialButton button = go.GetComponent<RadialButton>();
            return button.IsUnfoldDone();
        }

        return false;
    }

    public void KickStartTimer()
    {
        foreach (GameObject go in spawnedRadialParts)
        {
            RadialButton button = go.GetComponent<RadialButton>();
            button.KickStartTimer();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (GameObject radialPart in spawnedRadialParts)
        {
            RadialButton imageComponent = radialPart.GetComponent<RadialButton>();

            if (imageComponent.IsAlphaVisible(eventData))
            {
                radialPart.GetComponent<RadialButton>().StartPushDownTimer();

                break;
            }
        }
    }

    public void SpawnRadialPart()
    {
        int autoSizedButtonsAmount = 0;
        float fixedAngle = 0;

        for (int i = 0; i < clickEvents.Count; i++)
        {
            if (clickEvents[i].setSize > 0)
            {
                fixedAngle += clickEvents[i].setSize;
            }
            else
            {
                autoSizedButtonsAmount++;
            }
        }

        if (fixedAngle == 0)
        {
            fixedAngle = 360;
        }

        float combinedAngle = 0;

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = 0;
            float fillPercentage;

            if (clickEvents[i].setSize > 0)
            {
                combinedAngle += clickEvents[i].setSize;
                angle = combinedAngle;

                fillPercentage = (clickEvents[i].setSize / 360) - (spaceBetweenParts / 360);

            }
            else
            {
                combinedAngle += (360 - fixedAngle) / autoSizedButtonsAmount;
                angle = combinedAngle;
                fillPercentage = (((360 - fixedAngle) / autoSizedButtonsAmount) / 360) - (spaceBetweenParts / 360);
            }

            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle - (spaceBetweenParts / 2));

            GameObject spawnedRadialPart = Instantiate(radialButtonPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;

            spawnedRadialPart.GetComponent<Image>().fillAmount = fillPercentage;

            float textAngle = 270 - ((fillPercentage * 360) / 2);

            spawnedRadialPart.GetComponent<RadialButton>().SetText(clickEvents[i].buttonName, textAngle, clickEvents[i].buttonColor);
            spawnedRadialPart.GetComponent<RadialButton>().SetFunctionToButton(clickEvents[i].function);
            spawnedRadialPart.GetComponent<RadialButton>().SetAngle(radialPartEulerAngle);
            spawnedRadialPart.GetComponentInChildren<TextMeshWrapAround>().SetupTextWrap(fillPercentage);
            spawnedRadialParts.Add(spawnedRadialPart);
        }
    }
}
