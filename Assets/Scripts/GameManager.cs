using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

public class GameManager : MonoBehaviour {
    [SerializeField] private Mesh planetMesh;
    [SerializeField] private Material planetMaterial;

    private void Start() {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        CreatePlanets(entityManager, 1);
    }

    private void CreatePlanets(EntityManager entityManager, int numPlanets) {
        EntityArchetype planetArchetype = entityManager.CreateArchetype(
            typeof(BodyComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Rotation)
        );

        NativeArray<Entity> planets = new NativeArray<Entity>(numPlanets, Allocator.Temp);
        entityManager.CreateEntity(planetArchetype, planets);
        foreach (Entity planet in planets) {
            entityManager.SetComponentData(planet, new Rotation {
                Value = quaternion.identity
            });
            entityManager.SetComponentData(planet, new BodyComponent {
                mass = 5,
                velocity = Vector2.zero,
                acceleration = Vector2.one * 10.0f,
                rotation = -25.0f
            });
            entityManager.SetSharedComponentData(planet, new RenderMesh {
                mesh = planetMesh,
                material = planetMaterial,
            }); ;
        }
        planets.Dispose();
    }
}
