using UnityEngine;
using UnityEditor;

// Khai báo rằng đây là Editor dành cho script AmmoLootItem
[CustomEditor(typeof(AmmoLootItem))]
[CanEditMultipleObjects]
public class AmmoLootItemEditor : Editor
{
    // Khai báo các thuộc tính muốn hiển thị
    SerializedProperty ammoAmountProp;
    SerializedProperty interactSoundProp;
    SerializedProperty questMarkerProp;
    SerializedProperty holdTimeProp;

    void OnEnable()
    {
        // Tìm biến ammoAmount từ script AmmoLootItem
        ammoAmountProp = serializedObject.FindProperty("ammoAmount");

        // Tìm các biến kế thừa từ script cha (InteractableObject)
        interactSoundProp = serializedObject.FindProperty("interactSound");
        questMarkerProp = serializedObject.FindProperty("questMarker");
        holdTimeProp = serializedObject.FindProperty("holdTime");
    }

    public override void OnInspectorGUI()
    {
        // Cập nhật dữ liệu từ script
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CÀI ĐẶT HÒM ĐẠN (RÚT GỌN)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Chỉ hiển thị các trường cần thiết cho việc nhặt đạn năng lượng.", MessageType.Info);

        // Hiển thị các trường bạn thực sự dùng
        EditorGUILayout.PropertyField(ammoAmountProp, new GUIContent("Số lượng đạn cấp"));
        EditorGUILayout.PropertyField(holdTimeProp, new GUIContent("Thời gian giữ E (giây)"));
        EditorGUILayout.PropertyField(interactSoundProp, new GUIContent("Âm thanh khi nhặt"));
        EditorGUILayout.PropertyField(questMarkerProp, new GUIContent("Quest Marker (Vòng sáng)"));

        EditorGUILayout.Space();

        // Áp dụng các thay đổi vào Object
        serializedObject.ApplyModifiedProperties();
    }
}