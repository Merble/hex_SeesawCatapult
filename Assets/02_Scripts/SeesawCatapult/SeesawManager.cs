using System;
using System.Collections.Generic;
using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;

public class SeesawManager : MonoBehaviour
{
    public event Action<int, int> DidBalanceChange;
    [SerializeField] private List<Seesaw> _Seesaws;
    
    
    private void Awake()
    {
        foreach (var seesaw in _Seesaws)
        {
            seesaw.DidBalanceChange += OnBalanceChange;

            // seesaw.DidWinPointOccur += () => { DidWinPointOccur?.Invoke(seesaw); };
            // seesaw.DidBalanceOccur += OnBalanceSituation;
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

    // private void OnWinPoint(Seesaw seesaw)
    // {
    //     
    // }
    //
    // private void OnBalanceSituation()
    // {
    //     
    // }
}
