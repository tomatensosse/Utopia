using Mirror;

public interface IInteractable
{
    bool CanInteract(Player player);
    void OnInteract(Player player);
    [Command(requiresAuthority = false)] void CmdHandleInteraction(Player player);
}