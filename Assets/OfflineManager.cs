using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineManager : MonoBehaviour {
    [Header("Game Objects")]
    [SerializeField] private Animator anyKeyAnimator;

    private void Update() {
        if (Input.anyKeyDown && anyKeyAnimator != null) {
            anyKeyAnimator.SetTrigger("anyKey");
        }
    }
}
