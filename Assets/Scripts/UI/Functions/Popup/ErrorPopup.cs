using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorPopup : MonoBehaviour
{
    public TMP_Text popupTitleText;
    public TMP_Text popupMainText; // Reference to the text component
    public Animator anim;

    // Function to show the error popup
    public void ShowError(string title, string message, int timeoutSeconds = 5)
    {
        popupTitleText.text = title; // Set the title of the popup
        popupMainText.text = message; // Set the text of the popup
        StartCoroutine(ErrorCorountine(timeoutSeconds));
        anim.SetTrigger("show"); // Trigger the show animation
    }

    // Coroutine to hide the error popup after a certain amount of time
    private IEnumerator ErrorCorountine(int timeoutSeconds)
    {
        yield return new WaitForSeconds(timeoutSeconds);
        anim.SetTrigger("hide"); // Trigger the hide animation
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject); // Destroy the popup
    }
}