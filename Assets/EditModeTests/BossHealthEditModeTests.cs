using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class BossHealthEditModeTests
{
    private GameObject bossObject;
    private MonoBehaviour bossHealth;
    private System.Type bossHealthType;

    [SetUp]
    public void SetUp()
    {
        // Khởi tạo Game Object
        bossObject = new GameObject("Boss_Test");
        
        // Dùng Type Reflection để lấy script BossHealth từ Assembly-CSharp
        // Tránh lỗi mất tham chiếu Assembly Definition trong thư mục EditModeTests
        bossHealthType = System.Type.GetType("BossHealth, Assembly-CSharp");
        
        if (bossHealthType == null)
        {
            Assert.Fail("Không tìm thấy component BossHealth. Bạn hãy chắc chắn script BossHealth nằm trong Assembly-CSharp.");
            return;
        }

        bossHealth = bossObject.AddComponent(bossHealthType) as MonoBehaviour;
    }

    [TearDown]
    public void TearDown()
    {
        // Dọn dẹp sau khi Test xong
        Object.DestroyImmediate(bossObject);
    }

    [Test]
    public void TakeDamage_DecreasesHealth_WhenNotInvulnerable()
    {
        // Arrange: Giả lập Boss đã kết thúc Intro, có máu và không còn bất tử
        SetPrivateField("maxHealth", 1000f);
        SetPrivateField("currentHealth", 1000f);
        SetPrivateField("isInvulnerable", false);
        SetPrivateField("isDead", false);
        
        float initialHealth = (float)GetPrivateField("currentHealth");
        float damageAmount = 150f;

        // Act: Gây đam cho Boss qua Reflection Invocation
        CallMethod("TakeDamage", damageAmount);

        // Assert: Xác nhận Boss bị trừ máu đúng
        float currentHealth = (float)GetPrivateField("currentHealth");
        Assert.AreEqual(initialHealth - damageAmount, currentHealth, "Máu của Boss phải bị trừ chính xác số lượng lượng sát thương nhận vào.");
    }

    // =========================================================================
    // Các hàm Hỗ Trợ (Reflection Helper) dùng C# Reflection 
    // Giúp truy xuất, thiết lập và gọi hàm mà không bị phụ thuộc Assenbly
    // =========================================================================

    private void SetPrivateField(string fieldName, object value)
    {
        if (bossHealthType == null) return;
        FieldInfo field = bossHealthType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(bossHealth, value);
        }
        else
        {
            Debug.LogError($"[EditModeTests] Không tìm thấy biến tên '{fieldName}' trong class BossHealth");
        }
    }

    private object GetPrivateField(string fieldName)
    {
        if (bossHealthType == null) return null;
        FieldInfo field = bossHealthType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(bossHealth);
        }
        
        Debug.LogError($"[EditModeTests] Không tìm thấy biến tên '{fieldName}' trong class BossHealth");
        return null;
    }

    private void CallMethod(string methodName, object parameter)
    {
        if (bossHealthType == null) return;
        MethodInfo method = bossHealthType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(bossHealth, new object[] { parameter });
        }
        else
        {
            Debug.LogError($"[EditModeTests] Không tìm thấy hàm tên '{methodName}' trong class BossHealth");
        }
    }
}
