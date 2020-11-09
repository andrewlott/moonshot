using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class GravitySystem : SystemBase {
    private float dampeningFactor = 0.97f;

    protected override void OnUpdate() {
        Entities.ForEach((ref BodyComponent bodyComponent, ref Translation translationComponent, ref Rotation rotationComponent) => {
            bodyComponent.acceleration *= 0.9f;
            bodyComponent.velocity += (bodyComponent.acceleration - (bodyComponent.velocity * dampeningFactor)) * Time.DeltaTime;
            translationComponent.Value.x += bodyComponent.velocity.x * Time.DeltaTime;
            translationComponent.Value.y += bodyComponent.velocity.y * Time.DeltaTime;
            rotationComponent.Value = math.mul(rotationComponent.Value, quaternion.RotateZ(math.radians(bodyComponent.rotation * Time.DeltaTime)));
        }).WithoutBurst().Run();
    }

}
