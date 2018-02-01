﻿using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class CreateNewLevelScene {

    [MenuItem("Tools/Create New Level Scene")]
    public static void Create() {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(scene);

        GameObject root = new GameObject("LevelRoot");
        root.transform.position = Vector3.left * 0.5f;
        AddComponent<Grid>(root);
        AddComponent<Layers>(root);

        GameObject floor = CreateGO("Floor", root);
        AddComponent<Tilemap>(floor, SetTilemap);
        AddComponent<TilemapRenderer>(floor,
                                      tr => { tr.sortingOrder = 0; });

        GameObject spawnPointsPlayer = CreateGO("SpawnPointsPlayer", root);
        AddComponent<Tilemap>(spawnPointsPlayer, SetTilemap);
        AddComponent<TilemapRenderer>(spawnPointsPlayer,
                                      tr => { tr.sortingOrder = 1; });

        GameObject spawnPointsTurrets = CreateGO("SpawnPointsTurrets", root);
        AddComponent<Tilemap>(spawnPointsTurrets, SetTilemap);
        AddComponent<TilemapRenderer>(spawnPointsTurrets,
                                      tr => { tr.sortingOrder = 1; });

        GameObject walls = CreateGO("Walls", root);
        AddComponent<Tilemap>(walls, SetTilemap);
        AddComponent<TilemapRenderer>(walls,
                                      tr => { tr.sortingOrder = 2; });
        AddComponent<TilemapCollider2D>(walls,
                                        tc => { tc.usedByComposite = true; });
        AddComponent<Rigidbody2D>(walls,
                                  rb => {
                                      rb.bodyType = RigidbodyType2D.Static;
                                      rb.simulated = true;
                                  });
        AddComponent<CompositeCollider2D>(walls,
                                          cc => {
                                              cc.isTrigger = false;
                                              cc.usedByEffector = false;
                                              cc.offset = Vector2.zero;
                                              cc.geometryType = CompositeCollider2D.GeometryType.Outlines;
                                              cc.generationType = CompositeCollider2D.GenerationType.Synchronous;
                                          });

        GameObject obstacles = CreateGO("Obstacles", root);
        AddComponent<Tilemap>(obstacles, SetTilemap);
        AddComponent<TilemapRenderer>(obstacles,
                                      tr => { tr.sortingOrder = 3; });
        AddComponent<TilemapCollider2D>(obstacles,
                                        tc => {
                                            tc.usedByComposite = false;
                                            tc.isTrigger = true;
                                            tc.usedByEffector = false;
                                        });
        AddComponent<Hurt>(obstacles);

        GameObject projectileKiller = CreateGO("ProjectileKiller", root);
        AddComponent<Tilemap>(projectileKiller, SetTilemap);
        AddComponent<TilemapRenderer>(projectileKiller,
                                      tr => { tr.sortingOrder = 2; });
        AddComponent<TilemapCollider2D>(projectileKiller,
                                        tc => { tc.usedByComposite = true; });
        AddComponent<Rigidbody2D>(projectileKiller,
                                  rb => {
                                      rb.bodyType = RigidbodyType2D.Static;
                                      rb.simulated = true;
                                  });
        AddComponent<CompositeCollider2D>(projectileKiller,
                                          cc => {
                                              cc.isTrigger = true;
                                              cc.usedByEffector = false;
                                              cc.offset = Vector2.zero;
                                              cc.geometryType = CompositeCollider2D.GeometryType.Outlines;
                                              cc.generationType = CompositeCollider2D.GenerationType.Synchronous;
                                          });
    }

    private static void SetTilemap(Tilemap tm) {
        tm.animationFrameRate = 1;
        tm.tileAnchor = new Vector3(.5f, .5f, 0f);
        tm.orientation = Tilemap.Orientation.XY;
    }

    private static GameObject CreateGO(string name, GameObject parent) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    private static void AddComponent<T>(GameObject go, Action<T> cb = null) where T : Component {
        T comp = go.AddComponent<T>();
        cb?.Invoke(comp);
    }
}