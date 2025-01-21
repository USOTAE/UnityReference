using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    //[SerializeField] private float xLimit = 960;
    //[SerializeField] private float yLimit = 540;

    //[SerializeField] private float xOffset = 150;
    //[SerializeField] private float yOffset = 150;

    //public virtual void AdjustPosition()
    //{
    //    Vector2 mousePosition = Input.mousePosition;
    //    float newXOffset = 0;
    //    float newYOffset = 0;

    //}

    public virtual void AdjustFontSize(TextMeshProUGUI _text)
    {
        if (_text.text.Length > 12)
        {
            _text.fontSize *= .8f;
        }
    }
}
