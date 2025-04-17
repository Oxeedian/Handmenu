using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
using System.Globalization;
using UnityEngine.EventSystems;

public class TextMeshWrapAround : MonoBehaviour
{
    TMP_Text textComponent;

    [SerializeField] private Vector3 center = new Vector3(0, -129.5f, 0);
    [SerializeField] private float radius = 130f;
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float endAngle = Mathf.PI;
    [SerializeField] private float letterSpacing = 0.0f; // Letter spacing used to work but not any longer, is it still needed i wonder? - Oscar 
    [SerializeField] private float extraAngleForLetters = 2.05f;
    [SerializeField] private float offsetFromBorder = 0.05f;

    private int maxLetters = 55;
    private float buttonSizeInPercentage = 1f;



    void Start()
    {
        center = new Vector3(0, -129.5f, 0);
        radius = 130f;
    }


    void Update()
    {
    }

    private void Awake()
    {
    }

    void CalculateAnglesForTextPos(TMP_TextInfo textInfo)
    {
        float middleAngle = Mathf.PI / 2;
        float percentageInRadial = buttonSizeInPercentage * (2 * Mathf.PI);
        float letterspercent = textInfo.characterCount / (maxLetters * buttonSizeInPercentage);
        float extraAngle = Mathf.Lerp(extraAngleForLetters * buttonSizeInPercentage, 0.0f, letterspercent);
        float middleOfButton = (percentageInRadial / 2);

        startAngle = middleAngle - middleOfButton + offsetFromBorder + extraAngle;
        endAngle = middleAngle + middleOfButton - offsetFromBorder - extraAngle;

        CalculateWidth(textInfo);
    }

    void CalculateWidth(TMP_TextInfo textInfo)
    {
        float leftmost = float.MaxValue;
        float rightmost = float.MinValue;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            leftmost = Mathf.Min(leftmost, charInfo.bottomLeft.x);
            rightmost = Mathf.Max(rightmost, charInfo.bottomRight.x);
        }

        float totalWidth = rightmost - leftmost;

        if (totalWidth <= 0f)
            totalWidth = 1f;

        CalculatePosition(textInfo, leftmost, totalWidth);
    }

    void CalculatePosition(TMP_TextInfo textInfo, float leftmost, float totalWidth)
    {
        int letterCount = textInfo.characterCount;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            float charMidX = (charInfo.bottomLeft.x + charInfo.bottomRight.x) / 2f;
            float letterspacing = (charMidX - leftmost) / totalWidth;
            float indexFraction = (letterCount > 1) ? (float)i / (letterCount - 1) : 0f;
            float fraction = Mathf.Lerp(letterspacing, indexFraction, letterSpacing);
            float effectiveFraction = 1f - fraction;
            float angle = Mathf.Lerp(startAngle, endAngle, effectiveFraction);

            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius,
                                                  Mathf.Sin(angle) * radius, 0);

            Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90f);

            Vector3 charCenter = (verts[charInfo.vertexIndex] +
                                  verts[charInfo.vertexIndex + 1] +
                                  verts[charInfo.vertexIndex + 2] +
                                  verts[charInfo.vertexIndex + 3]) / 4f;


            for (int j = 0; j < 4; j++)
            {
                verts[charInfo.vertexIndex + j] = newPos + rotation * (verts[charInfo.vertexIndex + j] - charCenter);
            }
        }

        UpdateChanges(textInfo);
    }

    void UpdateChanges(TMP_TextInfo textInfo)
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void SetupTextWrap(float percent)
    {
        buttonSizeInPercentage = percent;
    }

    public void InitializeTextWrap()
    {
        textComponent = GetComponent<TMP_Text>();

        float minTextSize = 18.5f;
        float maxTextSize = 30.0f;
        textComponent.fontSize = Mathf.Lerp(minTextSize, maxTextSize, buttonSizeInPercentage);

        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        if (textInfo.characterCount == 0)
            return;

        CalculateAnglesForTextPos(textInfo);
    }
}