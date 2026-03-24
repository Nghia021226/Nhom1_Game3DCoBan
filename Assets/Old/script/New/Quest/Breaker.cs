using UnityEngine;

public class Breaker : InteractableObject
{
    
    public override void Start()
    { 
        base.Start();
        ToggleMarker(false);
    }

    public void ToggleMarker(bool state)
    {
        if (questMarker != null)
        {
            questMarker.SetActive(state);
        }
    }
    public override string GetHintText()
    {
        if (PowerManager.instance != null && !PowerManager.instance.isPowerOff)
            return "Hệ thống điện ổn định.";

        return "Giữ E để BẬT CẦU DAO";
    }

    public override void PerformAction()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StopLoading();
            GameManager.instance.HideHint();
        }

        // Logic riêng của bạn
        if (PowerManager.instance != null && PowerManager.instance.isPowerOff)
        {
            PowerManager.instance.RestorePower();
            Debug.Log("Đã bật cầu dao!");
        }
    }
}