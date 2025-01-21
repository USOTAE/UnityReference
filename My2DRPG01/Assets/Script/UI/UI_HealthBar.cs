using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private RectTransform myTransform;
    private Slider slider;

    void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        //todo 更新血量数据
    }



}
