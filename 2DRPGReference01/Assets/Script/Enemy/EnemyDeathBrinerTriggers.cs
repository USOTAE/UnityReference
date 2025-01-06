using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathBrinerTriggers : Enemy_AnimationTriggers
{
    private Enemy_DeathBriner enemyDeathBriner => GetComponentInParent<Enemy_DeathBriner>();

    private void Relocate() => enemyDeathBriner.FindPosition();

    private void MakeInvisible() => enemyDeathBriner.fx.MakeTransparent(true);
    private void MakeVisible() => enemyDeathBriner.fx.MakeTransparent(false);


}
