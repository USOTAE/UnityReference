using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [Header("Currency info")]
    [SerializeField] private TextMeshProUGUI currentCurrency;
    [SerializeField] private float currencyAmount;
    [SerializeField] private float increaseRate = 100;


    //todo 技能及其他的图标

    void Start()
    {
        if (playerStats != null)
            playerStats.onHpChanged += UpdateHpUI;

    }

    void Update()
    {
        UpdateCurrencyUI();
    }

    private void UpdateHpUI()
    {
        slider.maxValue = playerStats.GetMaxHp();
        slider.value = playerStats.currentHp;
    }

    private void UpdateCurrencyUI()
    {
        if (currencyAmount < PlayerManager.instance.player.GetCurrency())
            currencyAmount += Time.deltaTime * increaseRate;
        else
            currencyAmount = PlayerManager.instance.player.GetCurrency();

        float epsilon = .0001f;
        if (Mathf.Abs(currencyAmount) < epsilon)
            currentCurrency.text = "0";
        else
            currentCurrency.text = ((int)currencyAmount).ToString("#,#");
    }


}
