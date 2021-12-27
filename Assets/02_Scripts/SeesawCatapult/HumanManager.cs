using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class HumanManager : MonoBehaviour
    {
        [SerializeField] private Human _ThinHumanPrefab;
        [SerializeField] private Human _FatHumanPrefab;
        [SerializeField] private Team _Team;
        [SerializeField] private Transform _SpawnPos;
        [Space]
        [SerializeField] private List<Human> _Humans;
        [Space]
        [SerializeField] private List<SeesawBranch> _SeesawBranches;
        [Space]
        [SerializeField] private float _MinHumanSpeed;
        [SerializeField] private float _MaxHumanSpeed;
        [SerializeField] private float _MinX;
        [SerializeField] private float _MaxX;
        [SerializeField] private float _MinZ;
        [SerializeField] private float _MaxZ;
        [Space]
        [SerializeField] private float _HumanToCatapultWaitDuration;
        [SerializeField] private float _HumanToSeesawWaitDuration;
        [SerializeField] private float _WaitDurationBeforeNewHuman;
        [SerializeField] private float _HumanSideCheckWaitDuration;
        [SerializeField] private int _HumansToCreate;
        
        private List<Human> _humansOnOtherSide= new List<Human>();
        private readonly List<int> _humansToRemoveIndices = new List<int>();
        
        public Catapult Catapult { get; set; }
        public List<Human> HumansOnRandomMove { get; } = new List<Human>();
        
        public List<Human> HumansOnOtherSideWaitList { get; } = new List<Human>();
        public int HumansToCreate => _HumansToCreate;

        private void Awake()
        {
            foreach (var human in _Humans)
            {
                HumansOnRandomMove.Add(human);
            }
        
            CreateNewHumans();
        
            MoveAllHumansRandomly();
            MoveHumansToCatapult();
            
            CheckHumansSituation(_HumanToCatapultWaitDuration + _HumanSideCheckWaitDuration);
        }
        
        private void CheckHumansSituation(float waitTime)
        {
            StartCoroutine(CheckHumansSituationRoutine(waitTime));
        }
        
        private IEnumerator CheckHumansSituationRoutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            
            if (HumansOnOtherSideWaitList.Any())
            {
                _humansOnOtherSide.AddRange(HumansOnOtherSideWaitList);
                HumansOnOtherSideWaitList.Clear();
            }

            for (var index = 0; index < _humansOnOtherSide.Count; index++)
            {
                var human = _humansOnOtherSide[index];
                var state = human.GetState();
                
                switch (state)
                {
                    case HumanState.Idle:
                    case HumanState.IsMovingToCatapult:
                    case HumanState.OnCatapult:
                    case HumanState.IsFlying:
                    case HumanState.IsMovingToSeesaw:
                    case HumanState.OnSeesaw:
                        break;
                    
                    case HumanState.RandomMove:
                        if(HumansOnRandomMove.Any())
                            MoveAllHumansRandomly();
                        break;
                    case HumanState.OnOtherSide:
                        human.SetState(HumanState.IsMovingToSeesaw);
                        MoveHumanToNearestSeesaw(human);
                        break;

                    case HumanState.OnSameSide:
                        _humansToRemoveIndices.Add(index);
                        MoveNewHumanRandomly(human);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _humansToRemoveIndices.Reverse();
            
            foreach (var humanIndex in _humansToRemoveIndices)
            {
                _humansOnOtherSide.RemoveAt(humanIndex);
            }
            
            StartCoroutine(CheckHumansSituationRoutine(_HumanSideCheckWaitDuration));
        }

        private void CreateNewHumans()
        {
            StartCoroutine(CreateNewHumansRoutine(_WaitDurationBeforeNewHuman));
        }
    
        private IEnumerator CreateNewHumansRoutine(float waitTime)
        {
            while (_HumansToCreate > 0)
            {
                yield return new WaitForSeconds(waitTime);

                _HumansToCreate--;

                var number = Random.Range(0, 51);
                var prefab = number % 5 == 0 ? _FatHumanPrefab : _ThinHumanPrefab;

                var newHuman = Instantiate(prefab, _SpawnPos.position, Quaternion.identity);
                newHuman.Team = _Team;
            
                HumansOnRandomMove.Add(newHuman);
            }
            MoveAllHumansRandomly();
        }
    
        private void MoveNewHumanRandomly(Human human)
        {
            if (HumansOnRandomMove.Any())
            {
                MoveTheHumanRandomly();
            }
            else
            {
                MoveTheHumanRandomly();
                MoveHumansToCatapult();
            }

            void MoveTheHumanRandomly()
            {
                MoveHumanRandomly(human);
            }
        }

        private void MoveAllHumansRandomly()
        {
            foreach (var human in HumansOnRandomMove)
            {
                MoveHumanRandomly(human);
            }
        }

        private void MoveHumanRandomly(Human human)
        {
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, _MinHumanSpeed, _MaxHumanSpeed);

            human.SetState(HumanState.RandomMove);
            StartCoroutine(human.MoveRandomLocation());
        }

        public void MoveHumansToCatapult()
        {
            StartCoroutine(MoveHumansToCatapultRoutine(_HumanToCatapultWaitDuration));
        }

        private IEnumerator MoveHumansToCatapultRoutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (!HumansOnRandomMove.Any()) yield break;
        
            var human = HumansOnRandomMove[Random.Range(0, HumansOnRandomMove.Count)];
            human.MoveToCatapult(Catapult);
        }

        private void MoveHumanToNearestSeesaw(Human human)
        {
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, _MinHumanSpeed, _MaxHumanSpeed);
        
            StartCoroutine(DoAfterCoroutine.DoAfter(_HumanToSeesawWaitDuration, () =>
            {
                var humanPos = human.transform.position;
                var nearestSeesawSeat = GetNearestSeesawSeat(humanPos);
                
                human.MoveToSeesaw(nearestSeesawSeat);
            
                human.Rigidbody.isKinematic = true;
                human.MakeColliderSmaller();
            }));
        }

        private SeesawSeat GetNearestSeesawSeat(Vector3 humanPos)
        {
            var availableSeats = CheckAvailableSeats();
            SeesawSeat nearestSeat = null;
            var closestDistanceSqr = Mathf.Infinity;
        
            foreach(var seat in availableSeats)
            {
                var directionToTarget = seat.transform.position - humanPos;
                var dSqrToTarget = directionToTarget.sqrMagnitude;

                if (!(dSqrToTarget < closestDistanceSqr)) continue;
            
                closestDistanceSqr = dSqrToTarget;
                nearestSeat = seat;
            }

            if (nearestSeat is null) return null;
        
            nearestSeat.HumanAddedToSeat();
            return nearestSeat;

        }

        private List<SeesawSeat> CheckAvailableSeats()
        {
            return _SeesawBranches.Select(seesawBranch => seesawBranch.GetSeesawSeat()).Where(seesawSeat => seesawSeat != null).ToList();
        }

        public Vector3 GetRandomPointOnBoard()
        {
            var posX = Random.Range(_MinX, _MaxX);
            var posZ = Random.Range(_MinZ, _MaxZ);
            
            return new Vector3(posX, 0, posZ);
        }
    }
}
