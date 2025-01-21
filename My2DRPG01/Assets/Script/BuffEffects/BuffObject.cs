using System.Collections;
using UnityEngine;


public class BuffObject : MonoBehaviour
{
    [SerializeField] private ItemData buffData;

    private void SetupVisuals()
    {
        if (buffData == null)
            return;

        gameObject.name = "Item Object - " + buffData.name;
    }

}
