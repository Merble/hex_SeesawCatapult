using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumanManager : MonoBehaviour
{
    [SerializeField] private Catapult _Catapult;
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

    public List<Human> HumansOnRandomMove { get; } = new List<Human>();

    public List<Human> HumansOnOtherSide { get; } = new List<Human>();

    private void Awake()
    {
        foreach (var human in _Humans)
        {
            HumansOnRandomMove.Add(human);
        }
        MoveHumansRandomly();
        MoveHumansToCatapult();
    }

    private void Update()
    {
        foreach (var human in HumansOnOtherSide.Where(human => human.GetState() == HumanState.OnOtherSide))
        {
            human.SetState(HumanState.IsMovingToSeesaw);
            MoveHumanToNearestSeesaw(human);
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
        StartCoroutine(DoAfter(_HumanToSeesawWaitDuration, () =>
        {
            var humanPos = human.transform.position;
            var nearestSeesaw = GetNearestSeesaw(humanPos);

            human.MoveToSeesaw(nearestSeesaw);
            
            human.Rigidbody.isKinematic = true;
            human.MakeColliderSmaller();
        }));
    }

    private SeesawBranch GetNearestSeesaw(Vector3 humanPos)
    {
        SeesawBranch nearestSeesaw = null;
        var closestDistanceSqr = Mathf.Infinity;
        
        foreach(var seesawBranch in _SeesawBranches)
        {
            var directionToTarget = seesawBranch.GetSeesawPad().transform.position - humanPos;
            var dSqrToTarget = directionToTarget.sqrMagnitude;

            if (!(dSqrToTarget < closestDistanceSqr)) continue;
            
            closestDistanceSqr = dSqrToTarget;
            nearestSeesaw = seesawBranch;
        }
     
        return nearestSeesaw;
    }

    private IEnumerator DoAfter(float waitTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);
            
        callback?.Invoke();
    }
}
