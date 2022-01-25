using System.Collections;
using HexGames;
using SeesawCatapult.Main;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
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
        [SerializeField] private GameObject _CountdownCanvas;
        [SerializeField] private TextMeshProUGUI _CountdownText;
        [SerializeField] private Image _BaseColor;
        [Space] 
        [SerializeField, ReadOnly] private GameState _State;
        [Space]
        [SerializeField] private int _CountdownDuration;

        private Coroutine _countdownRoutine;

        protected override void DidStartGame(LevelHelper levelHelper)
        {
            _State = GameState.Balance;
            
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
                _CountdownCanvas.gameObject.SetActive(false);
                
                return;
            }
            
            if (playerPoint > enemyPoint)
            {
                if (_State == GameState.PlayerIsAhead) return;

                if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);

                _State = GameState.PlayerIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimerRoutine(_CountdownDuration));
                _BaseColor.color = Config.PlayerColor;
            }
            else if(playerPoint < enemyPoint)
            {
                if (_State == GameState.EnemyIsAhead) return;

                if(_countdownRoutine != null) StopCoroutine(_countdownRoutine);
                
                _State = GameState.EnemyIsAhead;
                _countdownRoutine = StartCoroutine(CountdownTimerRoutine(_CountdownDuration));
                _BaseColor.color = Config.EnemyColor;
            }
        }
        
        private IEnumerator CountdownTimerRoutine(int countdown)
        {
            _CountdownCanvas.gameObject.SetActive(true);
            
            while (countdown >= 0)
            {
                _CountdownText.text = countdown.ToString();
                
                yield return new WaitForSeconds(1f);
                
                countdown--;
            }

            _CountdownCanvas.gameObject.SetActive(false);

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