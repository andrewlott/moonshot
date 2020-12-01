using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Colorable : NetworkBehaviour {
    [SerializeField] public Color color = Color.white;
    [SerializeField] private List<Color> colorChoices = new List<Color> { Color.white };
    private Color prevColor = Color.white;
    [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();


    private void Start() {
        if (gameObject.CompareTag("Player")) {
            color = Random.ColorHSV();
            //color = colorChoices[Random.Range(0, colorChoices.Count - 1)];
        }
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
