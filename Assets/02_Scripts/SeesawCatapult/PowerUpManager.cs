using System;
using System.Collections;
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

        [SerializeField] private List<PowerUp> _PowerUps = new List<PowerUp>();
        [SerializeField] private List<Transform> _PowerUpSpawnPositions;
        [Space] 
        [SerializeField] private Team _Team;

        private List<PowerUp> PowerUpPrefabs => Game.Config._PowerUpPrefabs;
        private float InstantiateCircleRadius => Game.Config.PowerUpHumanInstantiateRadius;
        private float WaitDurationBeforePowerUp => Game.Config._WaitDurationBeforePowerUp;
        
        private int PowerUpsToCreate { get; set; }
        
        public List<PowerUp> PowerUps => _PowerUps;
        public List<Human[]> HumanGroupList { get; } = new List<Human[]>();

        private void Awake()
        {
            PowerUpsToCreate = Game.Config._PowerUpsToCreate;
            StartCoroutine(CreatePowerUpsRoutine());
            
            foreach (var powerUp in _PowerUps)
            {
                powerUp.DidUsePowerUp += OnPowerUpUse;
            }
        }

        private IEnumerator CreatePowerUpsRoutine()
        {
            while (PowerUpsToCreate > 0)
            {
                yield return new WaitForSeconds(WaitDurationBeforePowerUp);

                PowerUpsToCreate--;
                
                // Randomizing initialization
                var prefab = PowerUpPrefabs[Random.Range(0, PowerUpPrefabs.Count)];
                var spawnPointTransform = _PowerUpSpawnPositions[Random.Range(0, _PowerUpSpawnPositions.Count)];
                
                var powerUp = Instantiate(prefab, spawnPointTransform.position, Quaternion.identity);
                
                // Settings
                powerUp.DidUsePowerUp += OnPowerUpUse;
                powerUp.Team = _Team;
                PowerUps.Add(powerUp);
                _PowerUpSpawnPositions.Remove(spawnPointTransform);
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
            var pos = Random.insideUnitCircle * InstantiateCircleRadius;
            var newPos = new Vector3(powerUpPos.x + pos.x, humanPrefab.transform.position.y, powerUpPos.z + pos.y);
            var newHuman = Instantiate(humanPrefab, newPos, Quaternion.identity);

            instantiatedHumans.Add(newHuman);
            newHuman.SetState(HumanState.OnOtherSide);

            return instantiatedHumans;
        }
    }
}
