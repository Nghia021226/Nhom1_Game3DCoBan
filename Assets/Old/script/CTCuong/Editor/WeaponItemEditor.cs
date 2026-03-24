using UnityEngine;
using UnityEditor;

// Khai báo rằng đây là Editor dành cho script WeaponItem
[CustomEditor(typeof(WeaponItem))]
[CanEditMultipleObjects]
public class WeaponItemEditor : Editor
{
    // Khai báo các thuộc tính muốn hiển thị
    SerializedProperty typeProp;
    SerializedProperty holdTimeProp;
    SerializedProperty interactSoundProp; // Thêm âm thanh nếu bạn cần dùng sau này

    void OnEnable()
    {
        // Tìm các biến từ script gốc (InteractableObject)
        typeProp = serializedObject.FindProperty("type");
        holdTimeProp = serializedObject.FindProperty("holdTime");
        interactSoundProp = serializedObject.FindProperty("interactSound");
    }

    public override void OnInspectorGUI()
    {
        // Cập nhật dữ liệu từ script
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CÀI ĐẶT VŨ KHÍ (RÚT GỌN)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Chỉ hiển thị các trường cần thiết cho việc nhặt súng.", MessageType.Info);

        // Hiển thị các trường bạn yêu cầu
        EditorGUILayout.PropertyField(typeProp, new GUIContent("Loại đối tượng"));
        EditorGUILayout.PropertyField(holdTimeProp, new GUIContent("Thời gian giữ E (giây)"));

        // Bạn có thể hiện thêm âm thanh nếu muốn súng có tiếng khi nhặt
        EditorGUILayout.PropertyField(interactSoundProp, new GUIContent("Âm thanh nhặt súng"));

        EditorGUILayout.Space();

        // Áp dụng các thay đổi vào Object
        serializedObject.ApplyModifiedProperties();
    }
}