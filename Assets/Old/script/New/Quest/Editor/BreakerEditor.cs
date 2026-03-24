using UnityEngine;
using UnityEditor; // Bắt buộc phải có thư viện này

[CustomEditor(typeof(Breaker))] // Định nghĩa đây là Editor riêng cho script Breaker
[CanEditMultipleObjects]
public class BreakerEditor : Editor
{
    // Các biến SerializedProperty để quản lý dữ liệu
    
    SerializedProperty questMarker;
    SerializedProperty holdTime;

    // Nếu bạn muốn hiện thêm biến nào thì khai báo thêm ở đây

    void OnEnable()
    {
        // Tìm các biến trong script InteractableObject (Cha) và Breaker (Con)
       
        questMarker = serializedObject.FindProperty("questMarker");
        holdTime = serializedObject.FindProperty("holdTime");
    }

    public override void OnInspectorGUI()
    {
        // Cập nhật dữ liệu mới nhất
        serializedObject.Update();

        // 1. Vẽ cái Header Script mặc định (để bạn biết đang chỉnh script nào)
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Breaker)target), typeof(Breaker), false);
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CÀI ĐẶT CẦU DAO (RÚT GỌN)", EditorStyles.boldLabel);

        // 2. Chỉ vẽ ra những cái bạn CẦN
        
        EditorGUILayout.PropertyField(questMarker, new GUIContent("Quest Marker"));
        EditorGUILayout.PropertyField(holdTime, new GUIContent("Thời gian giữ E"));

        // Nếu bạn muốn hiện thêm các biến khác, cứ thêm dòng PropertyField tương tự

        // Lưu ý: Các biến như lockerScript, petScript... sẽ KHÔNG được vẽ ra nữa -> Ẩn hoàn toàn.

        // 3. Lưu lại các thay đổi
        serializedObject.ApplyModifiedProperties();
    }
}