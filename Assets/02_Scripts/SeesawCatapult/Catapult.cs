using System;
using System.Collections.Generic;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using UnityEngine;

namespace SeesawCatapult
{
    public class Catapult : MonoBehaviour
    {
        public event Action<Human> HumanDidComeToCatapult;
        public event Action<Human[]> DidThrowHumans;
        
        [SerializeField] private Animator _CatapultAnimator;
        [SerializeField] private Transform _CatapultSeat;
        
        private Vector3 _seatPosition;
        
        private float ThrowForce => Game.Config.CatapultThrowForce;
        private float DirectionValueY => Game.Config.HumanThrowDirectionValueY;
        
        private readonly float _gravity = Math.Abs(Physics.gravity.y);
        private static readonly int ThrowAnimParam = Animator.StringToHash("Throw");

        public List<Human> HumansOnCatapult { get; } = new List<Human>();
        
        private void Awake()
        {
            _seatPosition = _CatapultSeat.position;
        }
        
        public void DidHumanCome(Human human)
        {
            human.SetState(HumanState.OnCatapult);

            HumansOnCatapult.Add(human);
            
            HumanDidComeToCatapult?.Invoke(human);
        }

        public void ThrowHumansByDirection(Vector2 direction)
        {
            foreach (var human in HumansOnCatapult)
            {
                human.Throw(new Vector3(direction.x, DirectionValueY, direction.y) * ThrowForce);
            }
            
            _CatapultAnimator.SetTrigger(ThrowAnimParam);
            
            DidThrowHumans?.Invoke(HumansOnCatapult.ToArray());

            ClearSeat();
        }
        public Vector3 FindTrajectoryFinishPosition(Vector2 direction)
        {
            var forceY = DirectionValueY * ThrowForce;
            var time = (forceY * 2) /_gravity;
            var distance = time * direction.magnitude * ThrowForce;
        
            var groundDirection = new Vector3(direction.x, 0, direction.y);
            var finishPos = transform.position + (distance * groundDirection);
            
            finishPos.x = Mathf.Clamp(finishPos.x, Game.Config._MinXOfBoard, Game.Config._MaxXOfBoard);
            finishPos.z = Mathf.Clamp(finishPos.z, Game.Config._MinZOfBoard, Game.Config._MaxZOfBoard);
        
            return finishPos;
        }
    
        public void ThrowHumansByPosition(Vector3 position, float waitTime)
        {
            var direction = FindDirectionFromFinishPosition(position);
            StartCoroutine(DoAfterCoroutine.DoAfter(waitTime, () => { ThrowHumansByDirection(direction); }));
        }

        private Vector2 FindDirectionFromFinishPosition(Vector3 position)
        {
            var forceY = DirectionValueY * ThrowForce; 
            var time = (forceY * 2) /_gravity;
            var tempPos  = (position - transform.position);
            var direction = new Vector2(tempPos.x, tempPos.z) / (time * ThrowForce);

            return direction;
        }
        
        public void SetSeatPosition(float newY)
        {
            _seatPosition += new Vector3(0, newY, 0);
        }
        
        public Vector3 GetSeatPosition()
        {
            return _seatPosition;
        }
        
        private void ClearSeat()
        {
            HumansOnCatapult.Clear();
            _seatPosition = _CatapultSeat.position;
        }
    }
}
