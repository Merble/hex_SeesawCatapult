using System.Collections;
using System.Collections.Generic;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeesawCatapult
{
    public class PowerUpCreator : MonoBehaviour
    {
        [SerializeField] private PowerUpManager _PlayerPowerUpManager;
        [SerializeField] private PowerUpManager _EnemyPowerUpManager;
        [Space]
        [SerializeField] private List<PowerUp> _PowerUps = new List<PowerUp>();
        [SerializeField] private List<Transform> _PowerUpSpawnPositions;

        private List<PowerUp> PowerUpPrefabs => Game.Config._PowerUpPrefabs;

        [ReadOnly] public int _PowerUpsToCreate;
        
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
                
                //var powerUp = Instantiate(prefab, spawnPointTransform.position, Quaternion.identity);
                var powerUp = prefab.InstantiateInLevel(spawnPointTransform.position);
                
                // Some Settings
                powerUp.DidUsePowerUp += OnPowerUpUse;
                _PowerUps.Add(powerUp);
                _PowerUpSpawnPositions.Remove(spawnPointTransform);
            }
        }

        private void OnPowerUpUse(Human human, PowerUp powerUp, int powerUpEffectNumber)
        {
            var isPlayer = human.Team == Team.PlayerTeam;
            
            if(isPlayer)
                _PlayerPowerUpManager.OnPowerUpUse(human, powerUp, powerUpEffectNumber);
            else
                _EnemyPowerUpManager.OnPowerUpUse(human, powerUp, powerUpEffectNumber);
        }
    }
}
