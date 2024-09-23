using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public interface INPCInteractionHandler
{
    public Family Family { get; set; }
    public float ToleranceVariation { get; set; }
    public float Tolerance { get; set; }
    public float Relation { get; set; }
}
