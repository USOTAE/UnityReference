using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;


    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;


    void Start()
    {
        if (playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
        }

        skills = SkillManager.instance;
    }


    void Update()
    {
        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
        {
            SetCooldownOf(dashImage);
        }

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
        {
            SetCooldownOf(parryImage);
        }

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
        {
            SetCooldownOf(crystalImage);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
        {
            SetCooldownOf(swordImage);
        }

        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackHoleUnlocked)
        {
            SetCooldownOf(blackholeImage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
        {
            SetCooldownOf(flaskImage);
        }


        CheckCooldown(dashImage, skills.dash.cooldown);
        CheckCooldown(parryImage, skills.parry.cooldown);
        CheckCooldown(crystalImage, skills.crystal.cooldown);
        CheckCooldown(swordImage, skills.sword.cooldown);
        CheckCooldown(blackholeImage, skills.blackhole.cooldown);

        CheckCooldown(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrency())
        {
            soulsAmount += Time.deltaTime * increaseRate;
        }
        else
        {
            soulsAmount = PlayerManager.instance.GetCurrency();
        }

        //personal:修改格式化显示时如果soulsAmount是0不会显示的问题
        float epsilon = 0.0001f;
        if (Mathf.Abs(soulsAmount) < epsilon)
        {
            currentSouls.text = "0"; // 或者你可以自定义其他显示内容
        }
        else
        {
            currentSouls.text = ((int)soulsAmount).ToString("#,#");
        }
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCooldown(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
        }
    }
}
