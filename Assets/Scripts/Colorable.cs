using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Colorable : NetworkBehaviour {
    [SerializeField] public Color color = Color.white;
    [SerializeField] public List<Color> colorChoices = new List<Color> { Color.white };
    private Color prevColor = Color.white;
    [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    private static List<Color> usedPlayerColors = new List<Color>();

    private void Start() {
        if (gameObject.CompareTag("Player")) {
            Color choice = colorChoices[Random.Range(0, colorChoices.Count - 1)];
            while (usedPlayerColors.Contains(choice)) {
                choice = colorChoices[Random.Range(0, colorChoices.Count - 1)];
            }

            color = choice;
            usedPlayerColors.Add(choice);
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
