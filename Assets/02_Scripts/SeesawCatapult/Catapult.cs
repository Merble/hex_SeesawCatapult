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
        
        private float ThrowForce => Game.Config.CatapultThrowForce;
        private float DirectionValueY => Game.Config.HumanThrowDirectionValueY;

        private readonly float _gravity = Math.Abs(Physics.gravity.y);
        private static readonly int ThrowAnimParam = Animator.StringToHash("Throw");

        public List<Human> HumansOnCatapult { get; } = new List<Human>();

        
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
            HumansOnCatapult.Clear();
        }
        public Vector3 FindTrajectoryFinishPosition(Vector2 direction)
        {
            var forceY = DirectionValueY * ThrowForce;
            var time = (forceY * 2) /_gravity;
            var distance = time * direction.magnitude * ThrowForce;
        
            var groundDirection = new Vector3(direction.x, 0, direction.y);
            var finishPos = transform.position + (distance * groundDirection);
        
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
        
        public Vector3 GetSeatPosition()
        {
            return _CatapultSeat.position;
        }
    }
}
