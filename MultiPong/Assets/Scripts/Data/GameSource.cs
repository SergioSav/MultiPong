using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class GameSource
    {
        private const string _GAME_SETTINGS_URL = "GameSettingsScriptableObject";
        private const string _BONUS_SETTINGS_URL = "BonusScriptableObject";

        private const float _VERTICAL_PADDING = 0.1f;

        private static GameSource _instance;

        private GameSettings _settings;
        private List<BonusModel> _bonuses;

        public static GameSource Instance => _instance ?? new GameSource();

        public GameSource()
        {
            _instance = this;
        }
        public GameSettings Settings => _settings;
        public List<BonusModel> BonusModels => _bonuses;

        public float InitialBoardPosY()
        {
            return (Settings.FieldHeight - Settings.BallDiameter - _VERTICAL_PADDING - Settings.BoardHeight - Settings.BoardShiftPosY) / 2;
        }

        public void LoadSettings()
        {
            _settings = Resources.Load<GameSettingsScriptableObject>(_GAME_SETTINGS_URL).GameSettings;
        }

        public void LoadBonuses()
        {
            _bonuses = Resources.Load<BonusScriptableObject>(_BONUS_SETTINGS_URL).BonusModels;
        }
    }
}