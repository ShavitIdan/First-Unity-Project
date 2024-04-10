using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SC_SliderValue : MonoBehaviour
{
    public TextMeshProUGUI text; 

    public void SetFloatNumberText(float value) {
        text.text = value.ToString("0.0");
    }
    public void SetWholeNumberText(float value) {
        text.text = value.ToString();
    }
}
