using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerStatsTests
{
    private GameObject playerObject;
    private MonoBehaviour playerStats;
    private System.Type playerStatsType;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("PlayerTest");
        
        // ĐIỂM SÁNG GIÁ NHẤT: Bổ sung Cơ chế Reflection
        // Vì script PlayerStats của bạn nằm ở Assembly mặc định (Assembly-CSharp)
        // mà thư mục Test lại có Asmdef riêng, nên nó không gọi trực tiếp được.
        // Dùng cách Reflection này, bạn KHÔNG CẦN sửa Asmdef của project, và 100% không bị lỗi đỏ mảng nữa.
        playerStatsType = System.Type.GetType("PlayerStats, Assembly-CSharp");
        
        if (playerStatsType == null)
        {
            Assert.Fail("Không tìm thấy component PlayerStats trong hệ thống.");
            return;
        }

        playerStats = playerObject.AddComponent(playerStatsType) as MonoBehaviour;

        // Cài đặt thông số máu thông qua Reflection
        SetField("maxHealth", 100f);
        SetField("currentHealth", 100f);
        SetField("invincibilityDuration", 1.5f); 
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
    }

    // --- Các Hàm Hỗ Trợ Đóng Gói (Reflection Wrappers) ---
    private void SetField(string fieldName, object value)
    {
        var field = playerStatsType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null) field.SetValue(playerStats, value);
    }

    private float GetHealth()
    {
        var field = playerStatsType.GetField("currentHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (float)field.GetValue(playerStats);
    }

    private void CallMethod(string methodName, params object[] parameters)
    {
        var method = playerStatsType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null) method.Invoke(playerStats, parameters);
    }
    // --------------------------------------------------------

    [UnityTest]
    public IEnumerator TakeDamage_ReducesHealth_WhenNotInvincible()
    {
        // Act: Gây 30 sát thương (từ 100)
        CallMethod("TakeDamage", 30f);
        yield return null;

        // Assert: Máu phải còn 70
        Assert.AreEqual(70f, GetHealth(), "Máu phải giảm đúng với sát thương nhận vào.");
    }

    [UnityTest]
    public IEnumerator TakeDamage_DoesNotReduceHealth_WhenInvincible()
    {
        // Act: Đánh đòn thứ nhất kích hoạt cơ chế bất tử
        CallMethod("TakeDamage", 20f);
        
        // Đánh đòn thứ hai ngay tắp lự
        CallMethod("TakeDamage", 50f);
        
        yield return null;

        // Assert: Sát thương lần 2 phải bị chặn lại. Tổng máu chỉ bị trừ ở lần đầu (còn 80)
        Assert.AreEqual(80f, GetHealth(), "Sát thương lần 2 phải bị vô hiệu hóa do cờ bất tử (Invincible) đang bật.");
    }

    [UnityTest]
    public IEnumerator Heal_IncreasesHealth_NotExceedingMax()
    {
        // Arrange
        SetField("currentHealth", 50f);

        // Act
        CallMethod("Heal", 200f);
        yield return null;

        // Assert
        Assert.AreEqual(100f, GetHealth(), "Hàm Heal không được buff máu vượt qua maxHealth.");
    }
}
