using System;
using AwesomeGame.Enums;
using UnityEngine;

namespace AwesomeGame
{
    public class PowerUp : MonoBehaviour
    {
        public event Action<Human, PowerUpType, Vector3, int> DidUsePowerUp;
    
        [SerializeField] private Team _Team;
        [SerializeField] private PowerUpType _PowerUpType;
        [Space]
        [SerializeField] private int _PowerUpEffectNumber;

        public PowerUpManager.PowerUpFeatures GeneralFeatures;

        private void OnTriggerEnter(Collider other)
        {
            var human = other.GetComponentInParent<Human>();
        
            if (!human) return;

            if (human.Team != _Team) return;
        
            DidUsePowerUp?.Invoke(human, _PowerUpType, transform.position,_PowerUpEffectNumber);
            
            DestroySelf();
        }
        
        private void DestroySelf()
        {
            LeanTween.scale(gameObject, Vector3.one * GeneralFeatures.minScale, GeneralFeatures.scaleChangeDuration).setOnComplete(() =>
            {
                //Features.destroyEffect.Play();
                Destroy(gameObject);
            });
        }
    }
}
