using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleElevator : MonoBehaviour
{
    [Header("--- CÀI ĐẶT THANG MÁY ---")]
    public Animator elevatorAnimator;
    [Tooltip("Tên Trigger trong Animator (Mở xong tự đóng)")]
    public string openTriggerName = "Open";

    [Header("--- DỊCH CHUYỂN ---")]
    [Tooltip("Điểm Player sẽ hiện ra (Đặt Transform bên trong thang máy đích)")]
    public Transform destination;
    [Tooltip("Kéo thang máy đích vào đây để nó tự mở lúc tới nơi")]
    public SimpleElevator destinationElevator;

    [Header("--- THỜI GIAN ---")]
    [Tooltip("Chờ bao lâu (sau khi chạm vùng) thì bắt đầu tối màn hình")]
    public float waitBeforeFade = 2f;
    [Tooltip("Thời gian màn hình từ từ đen lại (Giây)")]
    public float fadeDuration = 1f;

    [Header("--- UI ĐEN MÀN HÌNH ---")]
    [Tooltip("Kéo cục Image màu đen trùm kín màn hình vào đây")]
    public Image blackScreen;

    private bool isUsing = false;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && !isUsing)
        {
            StartCoroutine(ElevatorRoutine(other.gameObject));
        }
    }

    
    public void OpenDoorOnly()
    {
        if (elevatorAnimator != null)
        {
            elevatorAnimator.SetTrigger(openTriggerName);
        }
    }

    private IEnumerator ElevatorRoutine(GameObject player)
    {
        isUsing = true;

        
        OpenDoorOnly();

        
        yield return new WaitForSeconds(waitBeforeFade);

        
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                blackScreen.color = new Color(0, 0, 0, timer / fadeDuration);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 1);
        }

        
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; 

        player.transform.position = destination.position;
        player.transform.rotation = destination.rotation; 

        if (cc != null) cc.enabled = true; 

        
        if (destinationElevator != null)
        {
            destinationElevator.OpenDoorOnly();
        }

        
        if (blackScreen != null)
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                blackScreen.color = new Color(0, 0, 0, 1 - (timer / fadeDuration));
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 0);
            blackScreen.gameObject.SetActive(false); 
        }

        
        isUsing = false;
    }
}