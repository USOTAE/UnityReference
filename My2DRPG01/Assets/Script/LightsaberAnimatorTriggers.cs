using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberAnimatorTriggers : MonoBehaviour
{
    private Controller_Lightsaber lightsaber => GetComponentInParent<Controller_Lightsaber>();

    private void AnimationFinishTrigger()
    {
        lightsaber.LightsaberFinishTrigger();
    }

    private void AnimationSpecialAttackTrigger()
    {
        lightsaber.LightsaberExplodeEventTrigger();
    }
}
