using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int Starthealth = 24;
    public NumberStringDisplay numberStringDisplay;
    void Start()
    {
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && Starthealth > 0) 
        {
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            numberStringDisplay.SetKey(true);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            numberStringDisplay.SetKey(false);
        };
    }
    public void TakeDamage(int damage)
    {
        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        StartCoroutine(numberStringDisplay.BlinkEffect());
        if (Starthealth <= 0)
        {
            Die();
        }
    }

    public void Kill()
    {
        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        if (Starthealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Логика смерти: проигрывание анимации или деактивация объекта
        gameObject.SetActive(false);
    }
}

