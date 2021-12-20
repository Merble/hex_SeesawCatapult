using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    public event Action<Human> HumanDidComeToCatapult;
    public event Action<Human[]> DidThrowHumans;
    
    [SerializeField] private float _ThrowForce;
    [SerializeField] private float _DirectionValueY;

    private readonly float _gravity = Math.Abs(Physics.gravity.y);

    public List<Human> HumansOnCatapult { get; } = new List<Human>();

    public void DidHumanCome(Human human)
    {
        HumanDidComeToCatapult?.Invoke(human);
    }

    public void ThrowHumansByDirection(Vector2 direction)
    {
        foreach (var human in HumansOnCatapult)
        {
            human.Rigidbody.AddForce(new Vector3(direction.x, _DirectionValueY, direction.y) * _ThrowForce, ForceMode.VelocityChange);
            human.SetState(HumanState.IsFlying);
            //human.MakeColliderSmaller();
        }
        DidThrowHumans?.Invoke(HumansOnCatapult.ToArray());
        HumansOnCatapult.Clear();
    }
    public Vector3 FindTrajectoryFinishPosition(Vector2 direction)
    {
        var forceY = _DirectionValueY * _ThrowForce;
        var time = (forceY * 2) /_gravity;
        var distance = time * direction.magnitude * _ThrowForce;
        
        var groundDirection = new Vector3(direction.x, 0, direction.y);
        var finishPos = transform.position + (distance * groundDirection);
        
        return finishPos;
    }
    
    [Button]
    public void ThrowHumansByPosition(Vector3 position)
    {
        var direction = FindDirectionFromFinishPosition(position);
        ThrowHumansByDirection(direction);
    }

    private Vector2 FindDirectionFromFinishPosition(Vector3 position)
    {
        var forceY = _DirectionValueY * _ThrowForce; 
        var time = (forceY * 2) /_gravity;
        var tempPos  = (position - transform.position);
        var direction = new Vector2(tempPos.x, tempPos.z) / (time * _ThrowForce); // * Mathf.Sqrt(2);

        return direction;
        
        // var forceY = _DirectionValueY * _ThrowForce;
        // var time = (forceY * 2) /_gravity;
        // var distance = time * direction.magnitude * _ThrowForce;
        //
        // var groundDirection = new Vector3(direction.x, 0, direction.y);
        // var finishPos = transform.position + (distance * groundDirection);
        //
        //return finishPos;
    }
    

    public void AddHuman(Human human)
    {
        HumansOnCatapult.Add(human);
    }
}
