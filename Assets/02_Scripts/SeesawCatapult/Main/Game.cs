using System.Collections;
using HexGames;
using SeesawCatapult.Main;
using TMPro;
using UnityEngine;

namespace SeesawCatapult.ThisGame.Main
{
    public class Game: Game<Config, LevelHelper>
    {
        
        [SerializeField] private VariableJoystick _Joystick;
        [SerializeField] private TextMeshProUGUI _CountdownText;
        [Space] 
        [SerializeField] private int _CountdownDuration;
        
        private bool _isCountdownStop;

        protected override void DidStartGame(LevelHelper levelHelper)
        {
            levelHelper.SeesawManager.DidBalanceChange += OnWinPoint;
            levelHelper.EnemyAI.RivalHumanManager = levelHelper.Player.HumanManager;
            
            levelHelper.Player.Init(_Joystick);
        }

        protected override void WillStopGame(LevelHelper levelHelper, bool isSuccess)
        {
            
        }

        private void OnWinPoint(int playerPoint, int enemyPoint)
        {
            if (playerPoint > enemyPoint)
            {
                StartCoroutine(CountdownTimer(_CountdownDuration, true));
            }
            else if(playerPoint < enemyPoint)
            {
                StartCoroutine(CountdownTimer(_CountdownDuration, false));
            }
            else
            {
                _isCountdownStop = true;
            }
        }
        
        private IEnumerator CountdownTimer(int countdown, bool isPlayerWin)
        {
            _CountdownText.gameObject.SetActive(true);
            
            while (countdown >= 0)
            {
                if(_isCountdownStop) break;
                
                _CountdownText.text = countdown.ToString();
                
                yield return new WaitForSeconds(1f);
                
                countdown--;
            }

            _CountdownText.gameObject.SetActive(false);
            
            if (countdown <= 0)
            {
                StopGame(isPlayerWin);
            }
            
            _isCountdownStop = false;
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