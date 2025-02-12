using Mirror;

public interface IInteractable
{
    bool CanInteract(Player player);
    [Command(requiresAuthority = false)] void OnInteract(Player player);
}