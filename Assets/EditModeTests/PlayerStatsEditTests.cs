using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public class PlayerStatsEditTests
{
    private GameObject playerObject;
    private MonoBehaviour playerStats;
    private System.Type playerStatsType;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("PlayerTest_EditMode");
        
        // Dùng Reflection giống PlayMode để tránh lỗi mất tham chiếu Assembly
        playerStatsType = System.Type.GetType("PlayerStats, Assembly-CSharp");
        
        if (playerStatsType == null)
        {
            Assert.Fail("Không tìm thấy component PlayerStats.");
            return;
        }

        playerStats = playerObject.AddComponent(playerStatsType) as MonoBehaviour;

        // Đặt max health và max stamina
        SetField("maxHealth", 100f);
        SetField("maxStamina", 100f);
        
        // Trạng thái dàn dựng (Arrange): Nhân vật đang gần chết và kiệt sức
        SetField("currentHealth", 10f); 
        SetField("currentStamina", 5f); 
        SetField("isInvincible", true); 
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

    private float GetFloatField(string fieldName)
    {
        var field = playerStatsType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (float)field.GetValue(playerStats);
    }
    
    private bool GetBoolField(string fieldName)
    {
        var field = playerStatsType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (bool)field.GetValue(playerStats);
    }

    private void CallMethod(string methodName)
    {
        var method = playerStatsType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null) method.Invoke(playerStats, null);
    }
    // --------------------------------------------------------

    [Test]
    public void ResetStats_RestoresHealthAndStaminaToMax()
    {
        // Act: Gọi hàm ResetStats() - giả lập việc người chơi hồi sinh
        CallMethod("ResetStats");

        // Assert: Kiểm tra cả 3 chỉ số phải được Reset về chuẩn
        Assert.AreEqual(100f, GetFloatField("currentHealth"), "Máu phải được hồi đầy về maxHealth.");
        Assert.AreEqual(100f, GetFloatField("currentStamina"), "Thể lực phải được hồi đầy về maxStamina.");
        Assert.AreEqual(false, GetBoolField("isInvincible"), "Trạng thái bất tử (cờ isInvincible) bắt buộc phải tắt.");
    }
}
