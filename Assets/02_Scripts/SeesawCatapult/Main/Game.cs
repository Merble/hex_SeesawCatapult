using System.Collections;
using HexGames;
using SeesawCatapult.Main;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SeesawCatapult.ThisGame.Main
{
    public enum GameState
    {
        Balance,
        PlayerIsAhead,
        EnemyIsAhead
    }
    public class Game: Game<Config, LevelHelper>
    {
        [SerializeField] private VariableJoystick _Joystick;
        [SerializeField] private TextMeshProUGUI _CountdownText;
        [Space] 
        [SerializeField, ReadOnly] private GameState _State;
        [Space]
        [SerializeField] private int _CountdownDuration;

        private Coroutine _countdownRoutine;

        protected override void DidStartGame(LevelHelper levelHelper)
        {
            levelHelper.SeesawManager.DidBalanceChange += OnBalanceChange;
            levelHelper.EnemyAI.RivalHumanManager = levelHelper.Player.HumanManager;
            
            levelHelper.Player.Init(_Joystick);
        }

        protected override void WillStopGame(LevelHelper levelHelper, bool isSuccess)
        {
            
        }

        private void OnBalanceChange(int playerPoint, int enemyPoint)
        {
            if(playerPoint == enemyPoint)
            {
                _State = GameState.Balance;
                if(_countdownRoutine != null) StopCoroutine(_countdownRoutine);
                _CountdownText.gameObject.SetActive(false);
                
                return;
            }
            
            var isPlayerAhead = playerPoint > enemyPoint;
            
            if ((_State == GameState.PlayerIsAhead) == isPlayerAhead) return;
                
            _State = isPlayerAhead ? GameState.PlayerIsAhead : GameState.EnemyIsAhead;
                
            if(_countdownRoutine != null) StopCoroutine(_countdownRoutine);
            _countdownRoutine = StartCoroutine(CountdownTimer(_CountdownDuration));
            
            /*if (playerPoint > enemyPoint)
            {
                if (_State == GameState.PlayerIsAhead) return;

                if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);

                _State = GameState.PlayerIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimer(_CountdownDuration));
            }
            else if(playerPoint < enemyPoint)
            {
                if (_State == GameState.EnemyIsAhead) return;

                if(_countdownRoutine != null) StopCoroutine(_countdownRoutine);
                
                _State = GameState.EnemyIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimer(_CountdownDuration));
            }*/
        }
        
        private IEnumerator CountdownTimer(int countdown)
        {
            _CountdownText.gameObject.SetActive(true);
            
            while (countdown >= 0)
            {
                _CountdownText.text = countdown.ToString();
                
                yield return new WaitForSeconds(1f);
                
                countdown--;
            }

            _CountdownText.gameObject.SetActive(false);

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