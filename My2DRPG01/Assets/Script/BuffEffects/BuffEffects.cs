using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Effect/Buff effect")]
public class BuffEffects : ItemEffects
{
    private PlayerStats playerStats;
    [SerializeField] private StatsType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.IncreaseStatsBy(buffAmount, buffDuration, playerStats.GetStats(buffType));
    }
}
