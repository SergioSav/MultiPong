using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GameSettings
    {
        [Header("Game settings")]
        public int WinScore = 10;
        public int MatchStartTimeout = 3;
        public int ButtonReturnShowTimeout = 3;
        public string WinText = "YOU WIN!";
        public string LoseText = "You lose...";

        [Header("Match field settings")]
        public float FieldHeight = 20f;
        public float FieldWidth = 16f;

        [Header("Ball settings")]
        public float BallMaxSpeed = 1f;
        public float BallVelocity = 0.01f;
        public float BallDiameter = 1f;

        [Header("Board settings")]
        public float BoardMaxSpeed = 1f;
        public float BoardVelocity = 0.01f;
        public float BoardWidth = 4f;
        public float BoardHeight = 0.4f;
        public float BoardShiftPosY = 0.2f;

        [Header("Bonus settings")]
        public float BonusGeneratingTimeout = 5f;
        public int BonusOnFieldLimit = 2;
        public float BonusDiameter = 1f;
    }
}
