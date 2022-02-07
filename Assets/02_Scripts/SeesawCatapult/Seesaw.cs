using System;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using Unity.Collections;
using UnityEngine;

namespace SeesawCatapult
{
    public class Seesaw : MonoBehaviour
    {
        public event Action DidBalanceChange;
        public event Action DidSeesawStuck;
    
        [SerializeField] private SeesawBranch _PlayerSeesawBranch;
        [SerializeField] private SeesawBranch _EnemySeesawBranch;
        [Space]
        [SerializeField, ReadOnly] private SeesawState _State;
        [Space]
        [SerializeField, ReadOnly] private float _BalanceValue;

        private float _balancePoint = .5f;
        
        public SeesawState State => _State;
        
        private void Awake()
        {
            _State = SeesawState.Balance;
            _BalanceValue = _balancePoint;
        
            _PlayerSeesawBranch.DidMassChange += BalanceChange;
            _EnemySeesawBranch.DidMassChange += BalanceChange;

            _PlayerSeesawBranch.ParentSeesaw = this;
            _EnemySeesawBranch.ParentSeesaw = this;
        }
    
        private void BalanceChange(float mass, bool isPlayer)
        {
            _BalanceValue = ((_PlayerSeesawBranch.TotalMass - _EnemySeesawBranch.TotalMass) * Game.Config._MassEffectOnSeesawBalance) + .5f;
            _BalanceValue = Mathf.Clamp(_BalanceValue, 0, 1);
        
            RotateBoardToCurrentBalance();
        }

        private void RotateBoardToCurrentBalance()
        {
            var newAngle = Mathf.Lerp(-Game.Config._MaxRotationAngle, Game.Config._MaxRotationAngle, _BalanceValue);
            var angle = transform.rotation.eulerAngles.x;

            if (angle > 180)
            {
                angle -= 360;
            }
            
            var duration = Mathf.Abs(newAngle - angle) / Game.Config._RotationSpeed;
            var rotationEuler = new Vector3(newAngle, 0, 0);
        
            LeanTween.cancel(gameObject);
            LeanTween.rotate(gameObject, rotationEuler, duration).setOnComplete(CheckBalanceAfterRotate);
        }

        private void CheckBalanceAfterRotate()
        {
            if (_BalanceValue > _balancePoint)
            {
                _State = SeesawState.PlayerWins;
            }
            else if (_BalanceValue < _balancePoint)
            {
                _State = SeesawState.EnemyWins;
            }
            else
            {
                if (_PlayerSeesawBranch.GetSeesawSeat() != null || _EnemySeesawBranch.GetSeesawSeat() != null)  return;

                _State = SeesawState.Balance;
            
                _PlayerSeesawBranch.ClearSeats();
                _EnemySeesawBranch.ClearSeats();
            }

            DidBalanceChange?.Invoke();
        }

        public void CheckSeesawSituation()
        {
            if (_PlayerSeesawBranch.GetSeesawSeat() != null || _EnemySeesawBranch.GetSeesawSeat() != null) return;
            
            _State = SeesawState.Stuck;
            DidSeesawStuck?.Invoke();
        }

        public SeesawSeat GetSeesawSeatForEnemyAI()
        {
            return _EnemySeesawBranch.GetSeesawSeat();
        }

        public void ResetSeesaw()
        {
            _State = SeesawState.Balance;
            
            _PlayerSeesawBranch.ClearSeats();
            _EnemySeesawBranch.ClearSeats();

            _BalanceValue = _balancePoint;
            
            RotateBoardToCurrentBalance();
        }
    }
}
