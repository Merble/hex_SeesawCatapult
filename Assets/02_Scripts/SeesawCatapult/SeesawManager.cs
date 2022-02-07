using System;
using System.Collections.Generic;
using System.Linq;
using SeesawCatapult.Enums;
using UnityEngine;

namespace SeesawCatapult
{
    public class SeesawManager : MonoBehaviour
    {
        public event Action<int, int> DidBalanceChange;
        public event Action DidGameStuck;
        
        [SerializeField] private List<Seesaw> _Seesaws;

        public int SeesawCount => _Seesaws.Count;
        
        private void Awake()
        {
            foreach (var seesaw in _Seesaws)
            {
                seesaw.DidBalanceChange += OnBalanceChange;
                seesaw.DidSeesawStuck += OnSeesawStuckSituation;
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
                    
                    case SeesawState.Balance:
                        break;
                    case SeesawState.Stuck:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            DidBalanceChange?.Invoke(playerWinCount, enemyWinCount);
        }

        private void OnSeesawStuckSituation()
        {
            var stuckSeesawCount = _Seesaws.Count(seesaw => seesaw.State == SeesawState.Stuck);

            if (stuckSeesawCount != _Seesaws.Count) return;
            
            DidGameStuck?.Invoke();
            ResetAllSeesaws();
        }

        private void ResetAllSeesaws()
        {
            foreach (var seesaw in _Seesaws)
            {
                seesaw.ResetSeesaw();
            }
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
