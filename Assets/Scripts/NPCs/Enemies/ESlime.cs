using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESlime : NPC
{
    public override void SetDefaults()
    {
        entityID = "Slime";
        entityName = "Slime";
        entityDescription = "A slime that is very bouncy. Reminds me of CUM.";

        hasHealth = true;
        maxHealth = 30;
        isHoldable = false;
        isInteractable = false;
    }
}
