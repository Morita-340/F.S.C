using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PartStatusHUD : HUD
{
    private bool slotFlag = false;
    private PartConnectToUI_IF part;
    private Sprite partIcon;
    private bool inRange = false;
    private float nowChargeTime = 0;
    private float maxChargeTime = 1;
    private Image image;
    private LineRenderer LR;
    protected override void Start()
    {
        base.Start();
        if (!TryGetComponent<Image>(out image))
        {
            image = this.AddComponent<Image>();
        }
        if (!TryGetComponent(out LR))
        {
            LR = this.AddComponent<LineRenderer>();
        }
    }
    protected override void Update()
    {
        base.Update();
        StatusDisplay();
    }
    public void Init(bool slotFlag, PartConnectToUI_IF part)
    {
        this.slotFlag = slotFlag;
        gameObject.SetActive(slotFlag);
        this.part = part;
        partIcon = part?.GetThisPartIcon();
        inRange = part?.GetInRange() ?? false;
        nowChargeTime = part?.GetNowChargeTime() ?? 0;
        maxChargeTime = part?.GetMaxChargeTime() ?? 1;
        maxChargeTime = maxChargeTime <= 0 ? 1 : maxChargeTime;
        if (!TryGetComponent<Image>(out image))
        {
            image = this.AddComponent<Image>();
        }
        image.sprite = partIcon;
        if (!TryGetComponent(out LR))
        {
            LR = this.AddComponent<LineRenderer>();
        }
    }
    public void Init(bool slotFlag, Sprite NoEquipPartIcon)
    {
        this.slotFlag = slotFlag;
        gameObject.SetActive(slotFlag);      
        partIcon = NoEquipPartIcon;
        inRange = false;
        nowChargeTime = 0;
        maxChargeTime = 1;
        maxChargeTime = maxChargeTime <= 0 ? 1 : maxChargeTime;
        if (!TryGetComponent<Image>(out image))
        {
            image = this.AddComponent<Image>();
        }
        image.sprite = partIcon;
        if (!TryGetComponent(out LR))
        {
            LR = this.AddComponent<LineRenderer>();
        }
    }
    private void StatusDisplay()
    {
        if (part != null)
        {
            var prop = part.GetActionProperty();
            this.inRange = prop.Item1;
            this.nowChargeTime = prop.Item2;
        }
        if (!slotFlag) { image.color = new Color(0, 0, 0, 0); }
        //アクション出来る武装はアイコンの色替え
        if (inRange) { image.color = Color.red; }
        else { image.color = Color.white; }
        //リチャージ時間も表示
    }
}
