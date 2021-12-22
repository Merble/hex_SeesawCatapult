using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame._02_Scripts.SeesawCatapult;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanManager : MonoBehaviour
{
    [SerializeField] private Catapult _Catapult;
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
    [SerializeField] private int _WaitDurationBeforeNewHuman;
    [SerializeField] private int _HumansToCreate;
    

    public List<Human> HumansOnRandomMove { get; } = new List<Human>();

    public List<Human> HumansOnOtherSide { get; } = new List<Human>();

    private void Awake()
    {
        foreach (var human in _Humans)
        {
            HumansOnRandomMove.Add(human);
        }
        
        CreateNewHumans();
        
        MoveHumansRandomly();
        MoveHumansToCatapult();
    }
    
    private void Update()
    {
        foreach (var human in HumansOnOtherSide)
        {
            var state = human.GetState();
            switch (state)
            {
                case HumanState.OnOtherSide:
                    
                    human.SetState(HumanState.IsMovingToSeesaw);
                    MoveHumanToNearestSeesaw(human);
                    
                    break;
                
                case HumanState.OnSameSide:
                    HumansOnOtherSide.Remove(human);
                    
                    MoveNewHumanRandomly(human);

                    break;
                
                default:
                    continue;
            }
        }
    }

    private void CreateNewHumans()
    {
        StartCoroutine(CreateNewHumansRoutine(_WaitDurationBeforeNewHuman));
    }
    
    private IEnumerator CreateNewHumansRoutine(int waitTime)
    {
        while (_HumansToCreate > 0)
        {
            yield return new WaitForSeconds(waitTime);

            _HumansToCreate--;

            var number = Random.Range(0, 51);
            var prefab = number % 5 == 0 ? _FatHumanPrefab : _ThinHumanPrefab;
            var pos = new Vector3(Random.Range(_MinX, _MaxX), 0, Random.Range(_MinZ, _MaxZ));

            var newHuman = Instantiate(prefab, pos, Quaternion.identity);
            newHuman.Team = _Team;
            
            MoveNewHumanRandomly(newHuman);
        }
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
            human.SetState(HumanState.RandomMove);
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, _MinHumanSpeed, _MaxHumanSpeed);
            StartCoroutine(human.MoveRandomLocation());
            
            
            HumansOnRandomMove.Add(human);
        }
    }

    private void MoveHumansRandomly()
    {
        foreach (var human in HumansOnRandomMove)
        {
            human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, _MinHumanSpeed, _MaxHumanSpeed);

            human.SetState(HumanState.RandomMove);
            StartCoroutine(human.MoveRandomLocation());
        }
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
        human.MoveToCatapult(_Catapult);
    }

    private void MoveHumanToNearestSeesaw(Human human)
    {
        human.SetMinAndMaxValues(_MinX, _MaxX, _MinZ, _MaxZ, _MinHumanSpeed, _MaxHumanSpeed);
        
        StartCoroutine(DoAfterCoroutine.DoAfter(_HumanToSeesawWaitDuration, () =>
        {
            var humanPos = human.transform.position;
            var nearestSeesaw = GetNearestSeesaw(humanPos);

            human.MoveToSeesaw(nearestSeesaw);
            
            human.Rigidbody.isKinematic = true;
            human.MakeColliderSmaller();
        }));
    }

    private SeesawSeat GetNearestSeesaw(Vector3 humanPos)
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
            nearestSeat.IsSeatFull = true;
        }

        return nearestSeat;
    }

    private List<SeesawSeat> CheckAvailableSeats()
    {
        return _SeesawBranches.Select(seesawBranch => seesawBranch.GetSeesawSeat()).Where(seesawSeat => seesawSeat != null).ToList();
    }
}
