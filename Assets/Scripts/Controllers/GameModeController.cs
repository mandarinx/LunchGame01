﻿using System.Collections.Generic;
using GameEvents;
using JetBrains.Annotations;
using UnityEngine;

namespace Deflector {
    public class GameModeController : MonoBehaviour, IOnUpdate {

        [SerializeField]
        private GameEvent                       onGameWon;
        [SerializeField]
        private GameEvent                       onGameLost;
        [SerializeField]
        private StringEvent                     onGameModeDescription;
        [SerializeField]
        private UHooks                          hooks;

        private GameMode                        gameMode;
        private Level                           curLevel;
        private readonly Dictionary<Level, int> playCounts = new Dictionary<Level, int>();

        /// <summary>
        /// Handler for onLevelLoaded
        /// </summary>
        /// <param name="level"></param>
        [UsedImplicitly]
        public void PrepareGameMode(Level level) {
            curLevel = level;

            int playCount;
            if (!playCounts.TryGetValue(curLevel, out playCount)) {
                playCounts.Add(level, 0);
            } else {
                playCounts[curLevel] = 0;
                playCount = 0;
            }

            int m = playCount % curLevel.NumGameModes;
            gameMode = curLevel.GetGameMode(m);
            gameMode.onGameLost = OnGameLost;
            gameMode.onGameWon = OnGameWon;
            onGameModeDescription?.Invoke(gameMode.title);
        }

        /// <summary>
        /// Handler for onGameReady
        /// </summary>
        [UsedImplicitly]
        public void StartCurrentGameMode() {
            gameMode.Activate();
            hooks.AddOnUpdate(this);
        }

        /// <summary>
        /// Handler for onLevelWillLoad
        /// </summary>
        [UsedImplicitly]
        public void ResetCurrentGameMode() {
            gameMode?.Reset();
        }

        public void UOnUpdate() {
            gameMode.Validate();
        }

        private void OnGameWon() {
            if (playCounts.ContainsKey(curLevel)) {
                playCounts[curLevel] += 1;
            }
            hooks.RemoveOnUpdate(this);
            onGameWon.Invoke();
        }

        private void OnGameLost() {
            if (playCounts.ContainsKey(curLevel)) {
                playCounts[curLevel] = 0;
            }
            hooks.RemoveOnUpdate(this);
            onGameLost.Invoke();
        }
    }
}
