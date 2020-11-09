using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BodyComponent : IComponentData {
    public float mass;
    public Vector2 velocity;
    public Vector2 acceleration;
    public float rotation;
}
