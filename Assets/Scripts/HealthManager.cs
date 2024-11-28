using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int Starthealth = 24;
    private NumberStringDisplay numberStringDisplay;
    void Start()
    {
        numberStringDisplay = GetComponent<NumberStringDisplay>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Starthealth > 0)
        {
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        }
    }
}
