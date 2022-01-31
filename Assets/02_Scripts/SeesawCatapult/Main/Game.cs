using System;
using System.Collections;
using System.Collections.Generic;
using HexGames;
using SeesawCatapult.Main;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SeesawCatapult.ThisGame.Main
{
    public enum GameState
    {
        Balance,
        PlayerIsAhead,
        EnemyIsAhead
    }
    
    public class Game: GameGenericBase<Config, LevelHelper, LevelInitData>
    {
        [SerializeField] private VariableJoystick _Joystick;
        [SerializeField] private TextMeshProUGUI _CountdownText;
        [SerializeField] private Image _CountdownBaseColor;
        [SerializeField] private Image _PlayerWinBar;
        [SerializeField] private Image _EnemyWinBar;
        [Space]
        [SerializeField, ReadOnly] private GameState _State;
        [Space]
        [SerializeField] private int _CountdownDuration;
        [SerializeField] private bool _CountdownBgColorChange;
        
        private Coroutine _countdownRoutine;
        private int _seesawCount;
        private int _playerWinPoint;
        private int _enemyWinPoint;

        protected override void DidStartGame(LevelHelper levelHelper)
        {
            _CountdownText.enabled = false;
            _seesawCount = levelHelper.SeesawManager.SeesawCount;
            _State = GameState.Balance;
            
            levelHelper.SeesawManager.DidBalanceChange += OnBalanceChange;
            levelHelper.EnemyAI.RivalHumanManager = levelHelper.Player.HumanManager;
            
            levelHelper.Player.Init(_Joystick);
        }

        private void Update()
        {
            AdjustWinBars();
        }

        private void OnBalanceChange(int playerPoint, int enemyPoint)
        {
            _playerWinPoint = playerPoint;
            _enemyWinPoint = enemyPoint;
            
            AdjustCountdown(playerPoint, enemyPoint);
        }

        private void AdjustWinBars()
        {
            _PlayerWinBar.fillAmount = (float) _playerWinPoint / _seesawCount; // Mathf.Lerp(_PlayerWinBar.fillAmount, (float) _playerWinPoint / _seesawCount, Config.WinBarLerpSpeed);
            _EnemyWinBar.fillAmount = (float) _enemyWinPoint / _seesawCount;   // Mathf.Lerp(_EnemyWinBar.fillAmount, (float) _enemyWinPoint / _seesawCount, Config.WinBarLerpSpeed);

            
            _PlayerWinBar.color = Color.Lerp(Config.PlayerWinBarWeakColor, Config.PlayerWinBarStrongColor,
                (float) _playerWinPoint / _seesawCount);
            
            _EnemyWinBar.color = Color.Lerp(Config.EnemyWinBarWeakColor, Config.EnemyWinBarStrongColor,
                (float) _enemyWinPoint / _seesawCount);
        }

        private void AdjustCountdown(int playerPoint, int enemyPoint)
        {
            if (playerPoint == enemyPoint)
            {
                _State = GameState.Balance;
                if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);
                _CountdownText.enabled = false;
                
                if(_CountdownBgColorChange) 
                    _CountdownBaseColor.color = Config.CountdownBgNeutralColor;

                return;
            }

            if (playerPoint > enemyPoint)
            {
                if (_State == GameState.PlayerIsAhead) return;

                if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);

                _State = GameState.PlayerIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimerRoutine(_CountdownDuration));
                
                if(_CountdownBgColorChange)
                    _CountdownBaseColor.color = Config.CountdownBgPlayerColor;
            }
            else if (playerPoint < enemyPoint)
            {
                if (_State == GameState.EnemyIsAhead) return;

                if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);

                _State = GameState.EnemyIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimerRoutine(_CountdownDuration));
                
                if(_CountdownBgColorChange) 
                    _CountdownBaseColor.color = Config.CountdownBgEnemyColor;
            }
        }

        private IEnumerator CountdownTimerRoutine(int countdown)
        {
            _CountdownText.enabled = true;
            
            while (countdown >= 0)
            {
                _CountdownText.text = countdown.ToString();
                
                yield return new WaitForSeconds(1f);
                
                countdown--;
            }
            
            _CountdownText.enabled = false;

            var isPlayerWin = _State == GameState.PlayerIsAhead;
            
            StopGame(isPlayerWin);
        }
        
        // Use these Game Event methods 
        /*protected override void WillStartGame()
        {
        }

        protected override void DidStartGame(LevelHelper levelHelper)
        {
        }

        protected override void WillStopGame(LevelHelper levelHelper, bool isSuccess)
        {
        }

        protected override void DidStopGame(bool isSuccess)
        {
        }
        */
    }
}