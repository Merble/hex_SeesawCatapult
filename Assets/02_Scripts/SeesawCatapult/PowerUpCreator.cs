using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeesawCatapult
{
    public class PowerUpCreator : MonoBehaviour
    {
        
        public event Action<List<Human>> DidInstantiateHumans;

        [SerializeField] private List<PowerUp> _PowerUps = new List<PowerUp>();
        [SerializeField] private List<Transform> _PowerUpSpawnPositions;

        private List<PowerUp> PowerUpPrefabs => Game.Config._PowerUpPrefabs;

        [ReadOnly] public int _PowerUpsToCreate;
        
        public List<PowerUp> PowerUps => _PowerUps;
        public List<Human[]> HumanGroupList { get; } = new List<Human[]>();

        private void Awake()
        {
            StartCoroutine(CreatePowerUpsRoutine());
        }

        private IEnumerator CreatePowerUpsRoutine()
        {
            while (_PowerUpsToCreate > 0)
            {
                yield return new WaitForSeconds(Game.Config._WaitDurationBeforeNewPowerUp);

                _PowerUpsToCreate--;
                
                // Randomizing the initialization
                var prefab = PowerUpPrefabs[Random.Range(0, PowerUpPrefabs.Count)];
                var spawnPointTransform = _PowerUpSpawnPositions[Random.Range(0, _PowerUpSpawnPositions.Count)];
                
                var powerUp = Instantiate(prefab, spawnPointTransform.position, Quaternion.identity);
                
                // Some Settings
                powerUp.DidUsePowerUp += OnPowerUpUse;
                _PowerUps.Add(powerUp);
                _PowerUpSpawnPositions.Remove(spawnPointTransform);
            }
        }

        private void OnPowerUpUse(Human human, PowerUp powerUp, int powerUpEffectNumber)
        {
            foreach (var humanGroup in HumanGroupList.Where(humanGroup => humanGroup.Contains(human)))
            {
                switch (powerUp.PowerUpType)
                {
                    case PowerUpType.Addition:
                        AddHumans(humanGroup, powerUp.transform.position, powerUpEffectNumber);
                        break;
                    case PowerUpType.Multiplication:
                        MultiplyHumans(humanGroup, powerUp.transform.position, powerUpEffectNumber);
                        break;
                    default:
                        return;
                }
            }
            
            _PowerUps.Remove(powerUp);
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
            
                instantiatedHumans.Add(InstantiateHuman(powerUpPos, humanPrefab));
            }
        
            DidInstantiateHumans?.Invoke(instantiatedHumans);
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
                    instantiatedHumans.Add(InstantiateHuman(powerUpPos, humanPrefab));
                }
            }
        
            DidInstantiateHumans?.Invoke(instantiatedHumans);
        }
        
        private Human InstantiateHuman(Vector3 powerUpPos, Human humanPrefab)
        {
            var pos = Random.insideUnitCircle * Game.Config.PowerUpHumanInstantiateRadius;
            var newPos = new Vector3(powerUpPos.x + pos.x, humanPrefab.transform.position.y, powerUpPos.z + pos.y);
            var newHuman = Instantiate(humanPrefab, newPos, Quaternion.identity);

            newHuman.SetState(HumanState.OnOtherSide);

            return newHuman;
        }
    }
}
