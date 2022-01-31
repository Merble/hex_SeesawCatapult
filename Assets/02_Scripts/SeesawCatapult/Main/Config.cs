using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HexGames;
using UnityEngine;
using UnityEngine.Serialization;

namespace SeesawCatapult.Main
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Hex/Game Config", order = 0)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Config: ConfigBase
    {
        // Lists
        public List<PowerUp> _PowerUpPrefabs; // PowerUpManager
        
        // Countdown Base Color
        [Space]
        public Color CountdownBgPlayerColor = Color.blue;
        public Color CountdownBgEnemyColor = Color.red;
        public Color CountdownBgNeutralColor = Color.white;
        [Space] 
        public Color PlayerWinBarWeakColor = Color.cyan;
        public Color PlayerWinBarStrongColor = Color.blue;
        public Color EnemyWinBarWeakColor = Color.red;
        public Color EnemyWinBarStrongColor = Color.red;
        public float WinBarLerpSpeed = 1f;
        
        [Space]
        // Joystick
        public float JoystickRadiusMultiplier = 1f;

        [Space]
        // Trajectory
        public int TrajectoryDotNumber = 30;
        public float TrajectoryDotSpacing = 0.05f;
        public float TrajectoryDotMinScale = .2f;
        public float TrajectoryDotMaxScale = 1f;
        
        [Space]
        // Human
        public float HumanScaleChangeDuration = 1.5f;
        public float HumanMaxScaleRate = 1.7f;
        
        [Space] 
        // PowerUp Manager
        public float _WaitDurationBeforeNewPowerUp = 6;
        // PowerUp
        public float PowerUpHumanInstantiateRadius = 2;
        public float PowerUpMinScaleRate = .15f;
        public float PowerUpScaleChangeDuration = .6f;

        [Space]
        // Catapult
        public float CatapultThrowForce = 9;
        public float HumanThrowDirectionValueY = 1.1f;
        public float _MaxXOfBoard;
        public float _MinXOfBoard;
        public float _MaxZOfBoard;
        public float _MinZOfBoard;
        
        [Space]
        // EnemyAI
        public float _EnemyHumanThrowPointRadiusMultiplier = 1;
        public int _EnemyHumanNumberBeforeThrow = 2;
        
        [Space]
        // Human Manager
        public float _MinHumanSpeed = 2;
        public float _MaxHumanSpeed = 6;
        public float _HumanToCatapultWaitDuration = 2;
        public float _HumanToSeesawWaitDuration = 1;
        public float _WaitDurationBeforeNewHuman = 4;

        [Space]
        // Seesaw
        public float _MassEffectOnSeesawBalance = 0.1f;
        public float _MaxRotationAngle = 25;
        public float _RotationSpeed = 10;
        // SeesawSeat
        public int _MaxHumanToSitOnSeat = 2;
    }
}