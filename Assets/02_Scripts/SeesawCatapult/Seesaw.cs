using System;
using AwesomeGame.Enums;
using Unity.Collections;
using UnityEngine;

namespace AwesomeGame
{
    public class Seesaw : MonoBehaviour
    {
        public event Action DidBalanceChange;
    
        [SerializeField] private SeesawBranch _PlayerSeesawBranch;
        [SerializeField] private SeesawBranch _EnemySeesawBranch;
        [Space]
        [SerializeField] private float _MassEffectOnBalance;
        [SerializeField] private float _MaxRotationAngle;
        [SerializeField] private float _BalanceValue;   // Between (0, 1)
        [SerializeField] private float _RotationSpeed;
        [Space]
        [SerializeField, ReadOnly]private SeesawState _State;
        public SeesawState State => _State;


        private void Awake()
        {
            _State = SeesawState.Balance;
            _BalanceValue = .5f;
        
            _PlayerSeesawBranch.DidMassChange += BalanceChange;
            _EnemySeesawBranch.DidMassChange += BalanceChange;
        }
    
        private void BalanceChange(float mass, bool isPlayer)
        {
            _BalanceValue += (isPlayer ? 1 : -1) * mass * _MassEffectOnBalance;
            _BalanceValue = Mathf.Clamp(_BalanceValue, 0, 1);
        
            RotateBoardToCurrentBalance();
        }

        private void RotateBoardToCurrentBalance()
        {
            var newAngle = Mathf.Lerp(-_MaxRotationAngle, _MaxRotationAngle, _BalanceValue);
            var angle = transform.rotation.eulerAngles.x;

            if (angle > 180)
            {
                angle -= 360;
            }
            
            var angleDistance = Mathf.Abs(newAngle - angle) / _RotationSpeed;
            var rotationEuler = new Vector3(newAngle, 0, 0);
        
            LeanTween.cancel(gameObject);
            LeanTween.rotate(gameObject, rotationEuler, angleDistance).setOnComplete(CheckBalanceAfterRotate);
        }

        private void CheckBalanceAfterRotate()
        {
            if (_BalanceValue > .5f)
            {
                _State = SeesawState.PlayerWins;
            }
            else if (_BalanceValue < .5f)
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
    }
}
