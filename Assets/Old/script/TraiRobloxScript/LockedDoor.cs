using UnityEngine;
using System.Collections; 

public class LockedDoor : InteractableObject
{
    [Header("Cài đặt Cửa Khóa")]
    public ItemType keyRequired = ItemType.GateCard;

    [Header("Cấu hình Mở Cửa (Điền số vào đây)")]
    public Vector3 openPosition;   
    public Vector3 openRotation;   
    public float openSpeed = 2f;

    private Vector3 closedPosition;
    private Quaternion closedRotation;
    private bool isOpened = false; 

    public override void Start()
    {
        base.Start();
        
        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation;

        type = ObjectType.None;
        holdTime = 3f;
    }
    public override string GetHintText()
    {
        if (isOpened) return ""; 

        if (GameManager.instance.IsHoldingItem(keyRequired))
            return "Đã có Keycard, Giữ E để mở cửa";
        else
            return "Cần có Keycard để mở cửa";
    }
    public override void PerformAction()
    {
        if (isOpened) return;

        if (GameManager.instance.IsHoldingItem(keyRequired))
        {
            StartCoroutine(OpenDoorSmoothly());

            GameManager.instance.RemoveCurrentItem();

            if (GameManager.instance != null)
            {
                GameManager.instance.StopLoading();
                GameManager.instance.HideHint();
            }

            if (questMarker != null) Destroy(questMarker);

            isOpened = true;
        }
        else
        {
            GameManager.instance.ShowHint("Bạn chưa có chìa khóa!");
        }
    }
    IEnumerator OpenDoorSmoothly()
    {
        float time = 0;
        Quaternion targetRot = Quaternion.Euler(openRotation);

        while (time < 1)
        {
            time += Time.deltaTime * openSpeed;

            transform.localPosition = Vector3.Lerp(closedPosition, openPosition, time);

            transform.localRotation = Quaternion.Slerp(closedRotation, targetRot, time);

            yield return null;
        }

        transform.localPosition = openPosition;
        transform.localRotation = targetRot;
    }
}