using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    //todo ����Ч��

    [SerializeField] private GameObject inGameUI;

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject optionsUI;

    public UI_ItemToolTip itemToolTip;
    public UI_ItemToolTip equipmentToolTip;
    //public UI_StatsToolTip statsToolTip;
    public UI_SkillToolTip skillToolTip;

    //[SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI);  //we need this to assign events on skill tree slots before we assign events on skill scripts
        //todo
    }

    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        equipmentToolTip.gameObject.SetActive(false);
        //statsToolTip.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.I))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(inventoryUI);

        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)
    {
        //todo ����Ч��
        for (int i = 0;i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            //todo ��Ч
            _menu.SetActive(true);
        }

        //todo �򿪲˵���ͣ��Ϸ
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu!=null&&_menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }
        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                return;
        }
        SwitchTo(inGameUI);
    }

    //todo restart game
}
