using UnityEngine;
using UnityEngine.EventSystems; 

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Cài đặt âm thanh")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip hoverSound; 
    [SerializeField] AudioClip clickSound; 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (source != null && hoverSound != null)
        {
            source.PlayOneShot(hoverSound);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (source != null && clickSound != null)
        {
            source.PlayOneShot(clickSound);
        }
    }
}