using System;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using UnityEngine;

namespace SeesawCatapult
{
    public class PowerUp : MonoBehaviour
    {
        public event Action<Human, PowerUpType, Vector3, int> DidUsePowerUp;
    
        [SerializeField] private Team _Team;
        [SerializeField] private PowerUpType _PowerUpType;
        [Space]
        [SerializeField] private int _PowerUpEffectNumber;
        
        private float PowerUpMinScale => Game.Config.PowerUpMinScaleRate;
        private float PowerUpScaleChangeDuration => Game.Config.PowerUpScaleChangeDuration;

        public Team Team
        {
            get => _Team;
            set => _Team = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            var human = other.GetComponentInParent<Human>();
        
            if (!human) return;

            if (human.Team != Team) return;
        
            DidUsePowerUp?.Invoke(human, _PowerUpType, transform.position,_PowerUpEffectNumber);
            
            DestroySelf();
        }
        
        private void DestroySelf()
        {
            LeanTween.scale(gameObject, Vector3.one * PowerUpMinScale, PowerUpScaleChangeDuration).setOnComplete(() =>
            {
                // TODO: Destroy Effect
                Destroy(gameObject);
            });
        }
    }
}
