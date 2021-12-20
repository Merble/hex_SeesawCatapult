using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour
{
    public event Action<List<Human>> DidInstantiateHumans; 
    [SerializeField] private List<PowerUp> _PowerUps = new List<PowerUp>();
    [Space]
    [SerializeField] private float _InstantiateCircleRadius;
    [SerializeField] private float _ColliderSizeChangeWaitDuration;
    

    public List<Human[]> HumanGroupList { get; } = new List<Human[]>();

    private void Awake()
    {
        foreach (var powerUp in _PowerUps)
        {
            powerUp.DidUsePowerUp += OnPowerUpUse;
        }
    }

    private void OnPowerUpUse(Human human, PowerUpType powerUpType, Vector3 powerUpPos,int powerUpEffectNumber)
    {
        foreach (var humanGroup in HumanGroupList.Where(humanGroup => humanGroup.Contains(human)))
        {
            switch (powerUpType)
            {
                case PowerUpType.Addition:
                    AddHumans(humanGroup, powerUpPos,powerUpEffectNumber);
                    break;
                case PowerUpType.Multiplication:
                    MultiplyHumans(humanGroup, powerUpPos,powerUpEffectNumber);
                    break;
                default:
                    return;
            }
        }
    }
    
    private void MultiplyHumans(Human[] humanGroup, Vector3 powerUpPos,int powerUpEffectNumber)
    {
        var instantiatedHumans = new List<Human>();
        
        // Find all human types by prefab
        var distinctPrefabs = humanGroup.Select(human => human.Prefab).Distinct();

        foreach (var humanPrefab in distinctPrefabs)
        {
            // Multiply and instantiate at pos
            var count = humanGroup.Count(human => human.Prefab == humanPrefab);
            var instantiateCount = count * (powerUpEffectNumber - 1);
            
            for (var i = 0; i < instantiateCount; i++)
            {
                instantiatedHumans.AddRange(InstantiateHuman(powerUpPos, humanPrefab));
            }
        }
        
        DidInstantiateHumans?.Invoke(instantiatedHumans);
    }
    private void AddHumans(Human[] humanGroup, Vector3 powerUpPos, int powerUpEffectNumber)
    {
        var instantiatedHumans = new List<Human>();
        
        Human humanPrefab = null;
        foreach (var human in humanGroup)
        {
            if (human.Type == HumanType.Fat)
            {
                humanPrefab = human.Prefab;
                break;
            }
            
            humanPrefab = human.Prefab;
        }
        
        for (var i = 0; i < powerUpEffectNumber; i++)
        {
            if (humanPrefab is null) continue;
            
            instantiatedHumans.AddRange(InstantiateHuman(powerUpPos, humanPrefab));
        }
        
        DidInstantiateHumans?.Invoke(instantiatedHumans);
    }
    
    private List<Human> InstantiateHuman(Vector3 powerUpPos, Human humanPrefab)
    {
        var instantiatedHumans = new List<Human>();
        var pos = Random.insideUnitCircle * _InstantiateCircleRadius;
        var newPos = new Vector3(powerUpPos.x + pos.x, humanPrefab.transform.position.y, powerUpPos.z + pos.y);
        var newHuman = Instantiate(humanPrefab, newPos, Quaternion.identity);

        instantiatedHumans.Add(newHuman);
        newHuman.SetState(HumanState.OnOtherSide);

        // Scale down the human collider to avoid flying humans
        newHuman.MakeColliderSmaller();

        StartCoroutine(DoAfter(_ColliderSizeChangeWaitDuration, () =>
        {
            // Scale up the human collider for normality
            newHuman.MakeColliderBigger();
        }));
        
        return instantiatedHumans;
    }
    
    private IEnumerator DoAfter(float waitTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);
            
        callback?.Invoke();
    }
}
