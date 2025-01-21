using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected Player player;
    protected SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;

    [Header("Pop up text")]
    [SerializeField] private GameObject popupTextPrefab;

    [Header("Ailment Color")]
    [SerializeField] private Color[] igniteColors;
    [SerializeField] private Color[] chillColors;
    [SerializeField] private Color[] shockColors;



    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void ExecuteFlashFX()
    {
        StartCoroutine(FlashFX());
    }

    //todo 当造成伤害时调用
    private IEnumerator FlashFX()
    {
        sr.material = flashMaterial;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.material = originalMaterial;
        sr.color = currentColor;
    }

    public void CreatePopupText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1.5f, 3);
        Vector3 poppositionOffset = new Vector3(randomX, randomY, 0);
        GameObject newText = Instantiate(popupTextPrefab, transform.position + poppositionOffset, Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        //todo
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColors[0])
            sr.color = igniteColors[0];
        else
            sr.color = igniteColors[1];
    }

    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating(nameof(IgniteColorFx), 0, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColors[0])
            sr.color = chillColors[0];
        else
            sr.color = chillColors[1];
    }

    public void ChillFxFor(float _seconds)
    {
        InvokeRepeating(nameof(ChillColorFx), 0, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColors[0])
            sr.color = shockColors[0];
        else
            sr.color = shockColors[1];
    }

    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating(nameof(ShockColorFx), 0, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }
}
