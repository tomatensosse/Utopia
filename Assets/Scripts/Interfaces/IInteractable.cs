using Mirror;

public interface IInteractable
{
    [Command (requiresAuthority = false)]
    public void CmdInteract(NetworkConnectionToClient conn);
}
