using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorable : MonoBehaviour {
    [SerializeField] public Color color = Color.white;
    private Color prevColor = Color.white;
    [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    private void Start() {
        SetSpriteColor();
    }

    private void Update() {
        if (prevColor != color) {
            SetSpriteColor();
        }
    }

    private void SetSpriteColor() {
        foreach(SpriteRenderer spriteRenderer in spriteRenderers) {
            spriteRenderer.color = color;
        }
        prevColor = color;
    }
}
