using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    [Header("Giá trị nút bấm")]
    public string number; // Điền số 1, 2, ... 9 vào đây

    private KeypadController controller;
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        controller = GetComponentInParent<KeypadController>();
        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;
    }

    
    void OnMouseEnter()
    {
        if (GameManager.instance.isUsingKeypad && rend != null)
            rend.material.color = Color.yellow; 
    }

    void OnMouseExit()
    {
        if (rend != null) rend.material.color = originalColor;
    }

    
    void OnMouseDown()
    {
        if (GameManager.instance.isUsingKeypad && controller != null)
        {
            controller.InputNumber(number);

           
            if (rend != null)
            {
                rend.material.color = Color.green;
                Invoke("ResetColor", 0.1f);
            }
        }
    }

    void ResetColor()
    {
        if (rend != null) rend.material.color = Color.yellow;
    }
}