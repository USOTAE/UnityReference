using System.Collections;
using UnityEngine;


public class ItemEffects : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed");
    }
}
