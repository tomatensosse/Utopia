using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Player player;
    
    public float interactionRange = 5f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask playerLayer;
    private IInteractable currentInteractable;

    public void Initialize(Player playerRef)
    {
        player = playerRef;
    }

    public void HandleInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("Interacting...");

            RaycastHit hit;
            if (Physics.Raycast(player.playerCamera.transform.position, player.playerCamera.transform.forward, out hit, interactionRange, ~playerLayer))
            {
                Debug.Log("Hit " + hit.collider.name);

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    currentInteractable = interactable;
                    currentInteractable.OnInteract(player);

                    Debug.Log("Interacted with " + hit.collider.name);
                }
            }
        }
    }
}