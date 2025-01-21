using UnityEngine;
using UnityEngine.UI;

public class Skill_Lightsaber : Skill
{
    [Header("Lightsaber info")]
    [SerializeField] private GameObject lightsaberPrefab;
    [SerializeField] private float lightsaberExistTime; //光剑的存续时间，在该时间内再次释放技能可以传送
    [SerializeField] private float moveSpeed;   //光剑的移动速度
    [SerializeField] private UI_SkillTreeSlot lightsaberUnlockButton;
    public bool lightsaberUnlocked { get; private set; }
    private GameObject currentLightsaber;

    [Header("Teleport info")]
    [SerializeField] private UI_SkillTreeSlot lightsaberTeleportUnlockButton;
    [SerializeField] private bool canTeleport;
    private bool haveTeleport;  //用来判断是否已经传送，已传送后不能再次传送
    public bool hitTarget;  //是否命中敌人,用来辅助判断能否传送  

    [Header("Explisive info")]
    [SerializeField] private UI_SkillTreeSlot lightsaberExplosiveUnlockButton;
    [SerializeField] private bool canExplode;


    protected override void Start()
    {
        base.Start();

        lightsaberUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockLightsaber);
        lightsaberTeleportUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockLightsaberTeleport);
        lightsaberExplosiveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockLightsaberExplosive);
    }

    protected override void CheckUnlock()
    {
        UnlockLightsaber();
        UnlockLightsaberTeleport();
        UnlockLightsaberExplosive();
    }

    private void UnlockLightsaberExplosive()
    {
        if (lightsaberExplosiveUnlockButton.unlocked)
        {
            canExplode = true;
        }
    }

    private void UnlockLightsaberTeleport()
    {
        if (lightsaberTeleportUnlockButton.unlocked)
            canTeleport = true;
    }

    private void UnlockLightsaber()
    {
        if (lightsaberUnlockButton.unlocked)
            lightsaberUnlocked = true;
    }

    public override void UseSkill()
    {
        if (currentLightsaber == null)
        {
            CreateLightsaber();
            haveTeleport = false;
        }
        else
        {
            if (canTeleport && hitTarget && !haveTeleport)
            {
                haveTeleport = true;
                player.transform.position = currentLightsaber.transform.position;
            }
        }

        if (!canTeleport)
            base.UseSkill();
        else
            Invoke(nameof(DelayCooldownTimer), lightsaberExistTime);    //解锁传送后需要等光剑消失后才计时
    }

    private void CreateLightsaber()
    {
        currentLightsaber = Instantiate(lightsaberPrefab, player.transform.position, Quaternion.identity);
        Controller_Lightsaber currentLightsaberScript = currentLightsaber.GetComponent<Controller_Lightsaber>();
        currentLightsaberScript.SetupLightsaber(lightsaberExistTime, moveSpeed * player.facingDir, player.stats, canExplode);
    }

    private void DelayCooldownTimer()
    {
        cooldownTimer = cooldown;
    }
}
