using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeesawCatapult
{
    public class HumanManager : MonoBehaviour
    {
        [SerializeField] private Human _ThinHumanPrefab;
        [SerializeField] private Human _FatHumanPrefab;
        [SerializeField] private Transform _SpawnPos;
        [Space]
        [SerializeField] private Team _Team;
        [Space]
        [SerializeField] private List<Human> _Humans;
        [Space]
        [SerializeField] private List<SeesawBranch> _SeesawBranches;
        [Space]
        [SerializeField] private float _MinX;
        [SerializeField] private float _MaxX;
        [SerializeField] private float _MinZ;
        [SerializeField] private float _MaxZ;

        private bool _isCatapultAvailable = true;
        public Catapult Catapult { get; set; }

        [ReadOnly] public int _HumansToCreate;
        
        private void Awake()
        {
            StartCoroutine(CreateNewHumansRoutine());
            StartCoroutine(MoveHumansToCatapultRoutine());
            StartCoroutine(MoveHumansToSeesawRoutine());
            
            MoveAllHumansRandomly();
        }
        
        private IEnumerator CreateNewHumansRoutine()
        {
            var duration = Game.Config._WaitDurationBeforeNewHuman;
            while (_HumansToCreate > 0)
            {
                yield return new WaitForSeconds(duration / 2);
                if(!_isCatapultAvailable) continue;
                yield return new WaitForSeconds(duration / 2);

                _HumansToCreate--;

                // Randomizing what prefab is going to be created.
                var number = Random.Range(0, 51);
                var prefab = number % 5 == 0 ? _FatHumanPrefab : _ThinHumanPrefab;
                
                //var newHuman = Instantiate(prefab, _SpawnPos.position, Quaternion.identity);
                var newHuman = prefab.InstantiateInLevel(_SpawnPos.position);
                newHuman.Team = _Team;
            
                _Humans.Add(newHuman);
                MoveHumanRandomly(newHuman);
            }
        }

        private void MoveAllHumansRandomly()
        {
            foreach (var human in _Humans)
            {
                MoveHumanRandomly(human);
            }
        }

        private void MoveHumanRandomly(Human human)
        {
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, Game.Config._MinHumanSpeed, Game.Config._MaxHumanSpeed);

            human.SetState(HumanState.RandomMove);
            StartCoroutine(human.MoveRandomLocation());
        }

        private IEnumerator MoveHumansToCatapultRoutine()
        {
            var duration = Game.Config._HumanToCatapultWaitDuration;
            while(true)
            {
                yield return new WaitForSeconds(duration / 2);
                if(!_isCatapultAvailable) continue;
                yield return new WaitForSeconds(duration / 2);
                
                foreach (var human in _Humans.Where(human => human.State == HumanState.RandomMove))
                {
                    human.MoveToCatapult(Catapult);
                    break;
                }
            }
        }
        
        private IEnumerator MoveHumansToSeesawRoutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(Game.Config._HumanToCatapultWaitDuration);

                foreach (var human in _Humans.Where(human => human.State == HumanState.OnOtherSide))
                {
                    MoveHumanToNearestSeesaw(human);
                }
            }
        }

        private void MoveHumanToNearestSeesaw(Human human)
        {
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, Game.Config._MinHumanSpeed, Game.Config._MaxHumanSpeed);
            var humanState = human.State;
            human.SetState(HumanState.IsMovingToSeesaw);
            
            StartCoroutine(DoAfterCoroutine.DoAfter(Game.Config._HumanToSeesawWaitDuration, () =>
            {
                var humanPos = human.transform.position;
                var nearestSeesawSeat = GetNearestSeesawSeat(humanPos);
                
                if (!nearestSeesawSeat)
                {
                    human.SetState(humanState);
                    return;
                }
                
                human.MoveToSeesaw(nearestSeesawSeat);
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

        public void AddHumans(List<Human> humans)
        {
            _Humans.AddRange(humans);
        }

        public void SetIsCatapultAvailable(bool isAvailable)
        {
            _isCatapultAvailable = isAvailable;
        }
    }
}
