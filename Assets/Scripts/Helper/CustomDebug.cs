using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomDebug : MonoBehaviour
{
    public static CustomDebug Instance { get; private set; }
    private Canvas canvasInScene;
    
    [Header("Error Popup")]
    public GameObject errorPopupPrefab;
    private int popupCount = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        canvasInScene = FindObjectOfType<Canvas>();

        SceneManager.sceneLoaded += (scene, mode) => OnSceneChanged();
    }

    private void OnSceneChanged()
    {
        canvasInScene = FindObjectOfType<Canvas>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PopUp(string title, string message, int timeoutSeconds = 5)
    {
        GameObject errorPopup = Instantiate(errorPopupPrefab, canvasInScene.transform);
        errorPopup.GetComponent<ErrorPopup>().ShowError(title, message, timeoutSeconds);
    }

    [ContextMenu("Test Error Popup")]
    public void TestErrorPopup()
    {
        PopUp("Test Error", "This is a test error message", 5);
    }
}
