using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatsSlot : MonoBehaviour
{
    private UI ui;

    [SerializeField] private string statsName;
    [SerializeField] private StatsType statsType;
    [SerializeField] private TextMeshProUGUI statsValueText;
    [SerializeField] private TextMeshProUGUI statsNameText;

    [TextArea]
    [SerializeField] private string statsDescription;

    private void OnValidate()
    {
        gameObject.name = "Stats - " + statsName;
        if (statsNameText != null)
            statsNameText.text = statsName;
    }

    void Start()
    {
        UpdateStatsValueUI();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatsValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats == null)
            return;

        statsValueText.text = playerStats.GetStats(statsType).GetValue().ToString();
    }
}
