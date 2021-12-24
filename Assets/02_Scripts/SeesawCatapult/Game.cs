using System.Collections;
using TMPro;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Player _Player;
        [SerializeField] private EnemyAI _EnemyAI;
        [SerializeField] private SeesawManager _SeesawManager;
        [SerializeField] private TextMeshProUGUI _CountdownText;
        [Space] 
        [SerializeField] private int _CountdownDuration;

        private bool _isCountdownStop;
        
        private void Awake()
        {
            _SeesawManager.DidBalanceChange += OnWinPoint;
            _EnemyAI.RivalHumanManager = _Player.HumanManager;
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
                if (isPlayerWin)
                    Debug.Log("Player Won");     // TODO: Player Wins
                else
                    Debug.Log("Player Won");     // TODO: Enemy Wins
            }
            
            _isCountdownStop = false;
        }
    }
}