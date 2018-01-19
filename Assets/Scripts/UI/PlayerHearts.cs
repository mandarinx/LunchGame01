﻿using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class PlayerHearts : MonoBehaviour {

    public HealthAsset health;
    public GameObject  heartPrefab;
    public GameEvent   onHeartFilled;

    private List<UIHeart> hearts;
    
    private void Awake() {
        health.onLivesChanged += OnLivesChanged;
        hearts = new List<UIHeart>(health.maxLives);
        
        for (int i = 0; i < health.maxLives; ++i) {
            AddHeart(false);
        }
    }

    public IEnumerator RenderHearts() {
        int heart = 0;
        while (heart < hearts.Count) {
            onHeartFilled?.Invoke();
            yield return new WaitForSeconds(0.2f);
            hearts[heart].isAlive = heart < health.numLives;
            ++heart;
        }
    }

    private void OnLivesChanged(int lives, int max) {
        // If player gets more health during playtime,
        // add more hearts
        if (max > hearts.Count) {
            AddHeart(true);
        }
        
        // If player loses hearts during playtime,
        // remove hearts
        if (max < hearts.Count) {
            int remove = hearts.Count - max;
            hearts.RemoveRange(max, remove);
            for (int i = 0; i < remove; ++i) {
                Destroy(transform.GetChild(max));
            }
        }
        
        for (int i = 0; i < max; ++i) {
            hearts[i].isAlive = i < lives;
        }
    }

    private void AddHeart(bool alive) {
        GameObject instance = Instantiate(heartPrefab);
        instance.transform.SetParent(transform, false);
        UIHeart heart = instance.GetComponent<UIHeart>();
        hearts.Add(heart);
        heart.isAlive = alive;
    }
}
