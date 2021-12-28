using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame.Enums;
using UnityEngine;

namespace AwesomeGame
{
    public class SeesawManager : MonoBehaviour
    {
        public event Action<int, int> DidBalanceChange;
        [SerializeField] private List<Seesaw> _Seesaws;
    
    
        private void Awake()
        {
            foreach (var seesaw in _Seesaws)
            {
                seesaw.DidBalanceChange += OnBalanceChange;
            }
        }

        private void OnBalanceChange()
        {
            var playerWinCount = 0;
            var enemyWinCount = 0;
        
            foreach (var seesaw in _Seesaws)
            {
                switch (seesaw.State)
                {
                    case SeesawState.PlayerWins:
                    
                        playerWinCount++;
                        break;
                
                    case SeesawState.EnemyWins:
                        
                        enemyWinCount++;
                        break;
                }
            }
            DidBalanceChange?.Invoke(playerWinCount, enemyWinCount);
        }

        public List<Seesaw> GetLosingSeesawsForEnemy()
        {
            return _Seesaws.Where(seesaw => seesaw.State == SeesawState.PlayerWins).ToList();
        }
        public List<Seesaw> GetBalancedSeesaws()
        {
            return _Seesaws.Where(seesaw => seesaw.State == SeesawState.Balance).ToList();
        }
    }
}
