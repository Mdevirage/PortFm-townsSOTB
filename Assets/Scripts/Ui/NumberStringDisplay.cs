using UnityEngine;
using UnityEngine.UI;

public class NumberStringDisplay : MonoBehaviour
{
    public Sprite[] numberSprites; // ������ �������� ���� (0�9)
    public Image tensDigitImage;   // Image ��� ��������
    public Image unitsDigitImage;  // Image ��� ������
    public RectTransform targetRect;
    // ����� ��� ����������� ����������� �����
    public void SetDoubleDigitNumber(int number)
    {
        RectTransform rectTransformTens = tensDigitImage.GetComponent<RectTransform>();
        RectTransform rectTransformUnits = unitsDigitImage.GetComponent<RectTransform>();
        // ����������� �� �������� (0�99)
        if (number < 0 || number > 99)
        {
            Debug.LogError("Number out of range! Must be between 0 and 99.");
            return;
        }
        
        // ������ �������� � ������
        int tens = number / 10;  // ������� ������
        int units = number % 10; // ������� �� �������

        // ���������� ��������
        if( tens == 0)
        {
            rectTransformTens.anchoredPosition = new Vector2(92, -34);
        }
        if (units == 4)
        {
            rectTransformUnits.anchoredPosition = new Vector2(129, -34);
        }
        if (units == 5)
        {
            rectTransformUnits.anchoredPosition = new Vector2(128, -34);
        }
        if (units == 7)
        {
            rectTransformUnits.anchoredPosition = new Vector2(125, -34);
        }
        tensDigitImage.sprite = numberSprites[tens];
        unitsDigitImage.sprite = numberSprites[units];
    }
}
