using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour {
    [SerializeField] private bool isEnabled;
    public int tokens = 0;
    public Banker banker;
}
