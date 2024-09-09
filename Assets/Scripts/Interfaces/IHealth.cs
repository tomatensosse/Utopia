using Mirror;

public interface IHealth
{
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }

    [Command (requiresAuthority = false)]
    void CmdTakeDamage(int damage, NetworkConnectionToClient conn);

    // Maybe add a CmdHeal method here

    bool IsDead();

    void Die(NetworkConnectionToClient conn);
}
