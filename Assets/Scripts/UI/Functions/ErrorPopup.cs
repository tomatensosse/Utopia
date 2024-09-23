using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorPopup : MonoBehaviour
{
    public GameObject popupPanel; // Reference to the popup panel
    public GameObject popupTextHolderTMP;
    private TMP_Text popupText; // Reference to the text component

    private void Start()
    {
        // Ensure the popup is hidden at the start
        popupPanel.SetActive(false);
    }

    // Function to show the error popup
    public void ShowError(string message, int timeoutSeconds)
    {
        popupText = popupTextHolderTMP.GetComponent<TMP_Text>();
        popupText.text = message; // Set the error message
        popupPanel.SetActive(true); // Show the popup
        if (timeoutSeconds < 1 || timeoutSeconds > 15) {
            timeoutSeconds = 3;
        } 
        StartCoroutine(HideAfterDelay(timeoutSeconds));
    }

    // Coroutine to hide the popup after a delay
    private IEnumerator HideAfterDelay(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        popupPanel.SetActive(false); // Hide the popup
    }

    // Function to hide the popup
    public void HidePopup()
    {
        popupPanel.SetActive(false); // Hide the popup
    }
}