using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NumberStringDisplay : MonoBehaviour
{
    public Sprite[] numberSprites; // ������ �������� ���� (0�9)
    public Image tensDigitImage;   // Image ��� ��������
    public Image unitsDigitImage;  // Image ��� ������
    public RectTransform targetRect;
    public GameObject backgroundImage1;  // ���
    public GameObject backgroundImage2;  // ���
    public GameObject Key1;
    // ����� ��� ����������� ����������� �����
    public void SetDoubleDigitNumber(int number)
    {
        RectTransform rectTransformTens = tensDigitImage.GetComponent<RectTransform>();
        RectTransform rectTransformUnits = unitsDigitImage.GetComponent<RectTransform>();

        if (number < 0 || number > 99)
        {
            Debug.LogError("Number out of range! Must be between 0 and 99.");
            return;
        }

        int tens = number / 10;  // ������� ������
        int units = number % 10; // ������� �� �������

        if (tens == 0)
        {
            rectTransformTens.anchoredPosition = new Vector2(92, -34);
        }
        else
        {
            rectTransformTens.anchoredPosition = new Vector2(95, -34);
        }

        switch (units)
        {
            case 0:
                rectTransformUnits.anchoredPosition = new Vector2(124, -34);
                break;
            case 4:
                rectTransformUnits.anchoredPosition = new Vector2(129, -34);
                break;
            case 5:
                rectTransformUnits.anchoredPosition = new Vector2(128, -34);
                break;
            case 7:
                rectTransformUnits.anchoredPosition = new Vector2(125, -34);
                break;
            default:
                rectTransformUnits.anchoredPosition = new Vector2(127, -34);
                break;
        }

        tensDigitImage.sprite = numberSprites[tens];
        unitsDigitImage.sprite = numberSprites[units];
    }

    // ��������� ��� ������� ��������
    public IEnumerator BlinkEffect()
    {
        // ���������� ������ ��������
        int blinkCount = 30;
        float blinkDuration = 0.03f;// ������������ ������ �������

        for (int i = 0; i < blinkCount; i++)
        {
            // ��������
            tensDigitImage.enabled = false;
            unitsDigitImage.enabled = false;
            backgroundImage1.SetActive(false);
            backgroundImage2.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);

            // ����������
            tensDigitImage.enabled = true;
            unitsDigitImage.enabled = true;
            backgroundImage1.SetActive(true);
            backgroundImage2.SetActive(true);
            yield return new WaitForSeconds(blinkDuration);
        }
    }
    public void SetKey(bool Keyenabler) 
    {
        Key1.SetActive(Keyenabler);
    }
}
