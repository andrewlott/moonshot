using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bank : NetworkBehaviour {
    [SerializeField]
    [SyncVar]
    public bool isEnabled;
    public int tokens = 0;
    public Banker banker;
}
