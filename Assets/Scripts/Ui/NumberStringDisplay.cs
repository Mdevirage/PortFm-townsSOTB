using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NumberStringDisplay : MonoBehaviour
{
    public Sprite[] numberSprites; // Массив спрайтов цифр (0–9)
    public Image tensDigitImage;   // Image для десятков
    public Image unitsDigitImage;  // Image для единиц
    public RectTransform targetRect;
    public GameObject backgroundImage1;  // Фон
    public GameObject backgroundImage2;  // Фон
    public GameObject Key1;
    // Метод для отображения двузначного числа
    public void SetDoubleDigitNumber(int number)
    {
        RectTransform rectTransformTens = tensDigitImage.GetComponent<RectTransform>();
        RectTransform rectTransformUnits = unitsDigitImage.GetComponent<RectTransform>();

        if (number < 0 || number > 99)
        {
            Debug.LogError("Number out of range! Must be between 0 and 99.");
            return;
        }

        int tens = number / 10;  // Деление нацело
        int units = number % 10; // Остаток от деления

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

    // Коррутина для эффекта мерцания
    public IEnumerator BlinkEffect()
    {
        // Количество циклов мерцания
        int blinkCount = 30;
        float blinkDuration = 0.03f;// Длительность одного мигания

        for (int i = 0; i < blinkCount; i++)
        {
            // Скрываем
            tensDigitImage.enabled = false;
            unitsDigitImage.enabled = false;
            backgroundImage1.SetActive(false);
            backgroundImage2.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);

            // Показываем
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
