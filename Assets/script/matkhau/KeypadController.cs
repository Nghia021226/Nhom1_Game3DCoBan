using UnityEngine;
using TMPro;
using System.Collections;
using Cinemachine;

public class KeypadController : MonoBehaviour
{
    [Header("Cấu hình")]
    public TextMeshProUGUI screenText;
    public CinemachineVirtualCamera keypadCamera;
    public string correctPassword = "1997";

    [Header("Âm thanh (Mới)")]
    public AudioSource audioSource;
    public AudioClip buttonPressSound; 
    public AudioClip successSound;     
    public AudioClip errorSound;       

    [Header("Kết nối")]
    public InteractableObject myInteractObject;

    private string currentInput = "";
    private bool isLocked = false;
    private bool isSolved = false;

  
    private float activateTime = 0f;

    void Start()
    {
        if (keypadCamera != null) keypadCamera.Priority = 0;

      
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        UpdateScreen();
    }

    void Update()
    {
        
        if (GameManager.instance.isUsingKeypad && Time.time > activateTime + 0.2f)
        {
          
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape))
            {
                ExitKeypad();
            }
        }
    }

    public void ActivateKeypad()
    {
        if (keypadCamera != null) keypadCamera.Priority = 20;
        GameManager.instance.ToggleKeypadMode(true);

      
        activateTime = Time.time;

        if (isSolved)
        {
            screenText.text = "OPEN";
            screenText.color = Color.green;
        }
        else
        {
            currentInput = "";
            screenText.color = Color.white;
            UpdateScreen();
        }
    }

    public void ExitKeypad()
    {
        StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        if (keypadCamera != null) keypadCamera.Priority = 0;
        yield return new WaitForEndOfFrame();
        GameManager.instance.ToggleKeypadMode(false);
    }

    public void InputNumber(string num)
    {
        if (isLocked || isSolved) return;
        if (currentInput.Length >= 4) return;

        
        if (audioSource != null && buttonPressSound != null)
            audioSource.PlayOneShot(buttonPressSound);
        

        currentInput += num;
        UpdateScreen();

        if (currentInput.Length == 4)
        {
            StartCoroutine(CheckPassword());
        }
    }

    void UpdateScreen()
    {
        if (screenText != null && !isSolved) screenText.text = currentInput;
    }

    IEnumerator CheckPassword()
    {
        isLocked = true;
        yield return new WaitForSeconds(0.5f);

        if (currentInput == correctPassword)
        {
            
            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);
           

            screenText.color = Color.green;
            screenText.text = "OPEN";
            isSolved = true;

            yield return new WaitForSeconds(1f);
            ExitKeypad();

            if (myInteractObject != null) myInteractObject.OpenDoorByKeypad();
            isLocked = false;
        }
        else
        {
            
            if (audioSource != null && errorSound != null)
                audioSource.PlayOneShot(errorSound);
          

            screenText.color = Color.red;
            screenText.text = "ERROR";

            yield return new WaitForSeconds(1f);

            currentInput = "";
            screenText.color = Color.white;
            UpdateScreen();
            isLocked = false;
        }
    }
}