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
        // Kiểm tra xem có phải Player chạm vào không và thang máy đang rảnh
        if (other.CompareTag("Player") && !isUsing)
        {
            StartCoroutine(ElevatorRoutine(other.gameObject));
        }
    }

    // Hàm riêng để ra lệnh mở cửa (Dùng cho thang máy đích)
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

        // 1. Mở cửa cho Player bước vào
        OpenDoorOnly();

        // 2. Chờ Player đi vào và cửa đóng lại (theo thời gian tùy chỉnh)
        yield return new WaitForSeconds(waitBeforeFade);

        // 3. Hiệu ứng làm tối màn hình từ từ
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
            blackScreen.color = new Color(0, 0, 0, 1); // Đen kịt 100%
        }

        // 4. DỊCH CHUYỂN PLAYER (Xử lý mượt để không kẹt CharacterController)
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // Tắt tạm để dịch chuyển không bị giật ngược

        player.transform.position = destination.position;
        player.transform.rotation = destination.rotation; // Xoay mặt Player nhìn ra cửa luôn

        if (cc != null) cc.enabled = true; // Bật lại

        // 5. Báo cho thang máy đích tự mở cửa ra
        if (destinationElevator != null)
        {
            destinationElevator.OpenDoorOnly();
        }

        // 6. Sáng màn hình lên lại
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
            blackScreen.gameObject.SetActive(false); // Ẩn luôn cho nhẹ máy
        }

        // Xong xuôi, reset trạng thái
        isUsing = false;
    }
}