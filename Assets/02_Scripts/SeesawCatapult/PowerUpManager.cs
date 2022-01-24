using System;
using System.Collections.Generic;
using System.Linq;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeesawCatapult
{
    public class PowerUpManager : MonoBehaviour
    {
        public event Action<List<Human>> DidInstantiateHumans;

        public List<Human[]> HumanGroupList { get; } = new List<Human[]>();

       public void OnPowerUpUse(Human human, PowerUp powerUp, int powerUpEffectNumber)
        {
            foreach (var humanGroup in HumanGroupList.Where(humanGroup => humanGroup.Contains(human)))
            {
                switch (powerUp.PowerUpType)
                {
                    case PowerUpType.Addition:
                        AddHumans(humanGroup, powerUp.transform.position, powerUpEffectNumber, human.GetCurrentVelocity());
                        break;
                    case PowerUpType.Multiplication:
                        MultiplyHumans(humanGroup, powerUp.transform.position, powerUpEffectNumber, human.GetCurrentVelocity());
                        break;
                    default:
                        return;
                }
            }
        }
        
        private void AddHumans(Human[] humanGroup, Vector3 powerUpPos, int powerUpEffectNumber, Vector3 velocity)
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
            
                instantiatedHumans.Add(InstantiateHuman(powerUpPos, humanPrefab, velocity));
            }
        
            DidInstantiateHumans?.Invoke(instantiatedHumans);
        }
    
        private void MultiplyHumans(Human[] humanGroup, Vector3 powerUpPos,int powerUpEffectNumber, Vector3 velocity)
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
                    instantiatedHumans.Add(InstantiateHuman(powerUpPos, humanPrefab, velocity));
                }
            }

            DidInstantiateHumans?.Invoke(instantiatedHumans);
        }
        
        private Human InstantiateHuman(Vector3 powerUpPos, Human humanPrefab, Vector3 velocity)
        {
            var pos = Random.insideUnitCircle * Game.Config.PowerUpHumanInstantiateRadius;
            var newPos = new Vector3(powerUpPos.x + pos.x, humanPrefab.transform.position.y, powerUpPos.z + pos.y);
            //var newHuman = Instantiate(humanPrefab, newPos, Quaternion.identity);
            var newHuman = humanPrefab.InstantiateInLevel(newPos);

            newHuman.SetNewHuman(velocity);
            
            return newHuman;
        }
    }
}
