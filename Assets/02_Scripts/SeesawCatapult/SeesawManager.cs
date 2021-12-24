using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
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
            //var balanceCount = 0;
        
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
                
                    // case SeesawState.Balance:
                    //     
                    //     balanceCount++;
                    //     break;
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
    
        /*public List<Seesaw> GetLosingSeesawsForPlayer(bool isPlayer)
    {
        return _Seesaws.Where(seesaw => seesaw.State == SeesawState.EnemyWins).ToList();
        
        
        List<Seesaw> playerLoseList = new List<Seesaw>();
        List<Seesaw> enemyLoseList = new List<Seesaw>();
        
        foreach (var seesaw in _Seesaws)
        {
            switch (seesaw.State)
            {
                case SeesawState.EnemyWins:
                    playerLoseList.Add(seesaw);
                    break;
                case SeesawState.PlayerWins:
                    enemyLoseList.Add(seesaw);
                    break;
            }
        }

        return isPlayer ? playerLoseList : enemyLoseList;
    }*/
    }
}
