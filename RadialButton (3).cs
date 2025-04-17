using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]


public class RadialButton : MonoBehaviour
{
    private Image image;
    private UnityEvent function;
    private float pushedDownTimer = 0;
    private float pushedDownTimeToCountTo = 0.2f;
    [SerializeField] private Color buttonColor = Color.white;
    private Vector3 radialPartEulerAngle;

    private float lerpDuration = 0.35f;
    private Vector3 startRotation = Vector3.zero;
    private float elapsedTime = 0f;
    private bool shouldUnfold = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (pushedDownTimer > 0)
        {
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Image>().color = buttonColor;
        }

        pushedDownTimer -= Time.deltaTime;

        Unfold();
    }


    void Unfold()
    {
        if(shouldUnfold)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > lerpDuration)
            {
                elapsedTime = lerpDuration;
            }
        }
        else
        {
            elapsedTime -= Time.deltaTime;
            if (elapsedTime < 0)
            {
                elapsedTime = 0;
            }
        }

        float foldPercentage = elapsedTime / lerpDuration;
        transform.localEulerAngles = Vector3.Lerp(startRotation, radialPartEulerAngle, foldPercentage);
    }


    public void StartFold(bool aBool)
    {
        shouldUnfold = aBool;
    }

    public void StartPushDownTimer()
    {
        pushedDownTimer = pushedDownTimeToCountTo;
        function.Invoke();
    }

    public void SetText(string name, float angle, Color aColor)
    {
        Transform childTransform = transform.Find("Button Text");
        TMPro.TextMeshProUGUI buttonText = childTransform.GetComponent<TMPro.TextMeshProUGUI>();

        float radius = 40;
        float radian = Mathf.Deg2Rad * angle;
        Vector2 position = new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);

        childTransform.localPosition = position;
        childTransform.localRotation = Quaternion.Euler(0, 0, angle - 90f);


        GetComponent<Image>().color = aColor;
        buttonColor = aColor;
        buttonText.text = name;
    }

    public void SetFunctionToButton(UnityEvent aFunction)
    {
        function = aFunction;
    }

    public void SetAngle(Vector3 aEulerAngle)
    {
        radialPartEulerAngle = aEulerAngle;
    }

    public bool IsAlphaVisible(PointerEventData eventData)
    {
        if (image.sprite == null || image.fillAmount <= 0)
        {
            return false;
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

        Rect rect = rectTransform.rect;
        Vector2 normalizedPoint = new Vector2(
            (localPoint.x - rect.x) / rect.width,
            (localPoint.y - rect.y) / rect.height
        );

        if (!IsPointInRadialFill(normalizedPoint))
        {
            return false;
        }

        Texture2D texture = image.sprite.texture;
        Vector2 pixelCoords = new Vector2(
            normalizedPoint.x * texture.width,
            normalizedPoint.y * texture.height
        );

        Color pixelColor = texture.GetPixel((int)pixelCoords.x, (int)pixelCoords.y);
        return pixelColor.a > 0.1f;
    }



    private bool IsPointInRadialFill(Vector2 normalizedPoint)
    {
        float angle = Mathf.Atan2(normalizedPoint.y - 0.5f, normalizedPoint.x - 0.5f) * Mathf.Rad2Deg;
        angle = (angle + 450) % 360;
        angle = 360 - angle;
        float fillAngle = 360 * image.fillAmount;

        if (image.fillClockwise)
        {
            bool temp = angle <= fillAngle;
            return angle <= fillAngle;
        }
        else
        {
            return angle >= (360 - fillAngle);
        }
    }
}