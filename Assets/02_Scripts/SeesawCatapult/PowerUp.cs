using System;
using GameStructure;
using HexGames;
using SeesawCatapult.Enums;
using SeesawCatapult.ThisGame.Main;
using UnityEngine;

namespace SeesawCatapult
{
    public class PowerUp : MonoBehaviour
    {
        public event Action<Human, PowerUp> DidUsePowerUp;
        
        [SerializeField] private PowerUpType _PowerUpType;
        [Space]
        [SerializeField] private int _PowerUpEffectNumber;

        private bool _isHit;
        public PowerUpType PowerUpType => _PowerUpType;
        public int PowerUpEffectNumber => _PowerUpEffectNumber;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isHit) return;
            _isHit = true;
            
            var human = other.GetComponentInParent<Human>();
        
            if (!human) return;
        
            DidUsePowerUp?.Invoke(human, this);
            
            DestroySelf();
        }
        
        private void DestroySelf()
        {
            LeanTween.scale(gameObject, Vector3.one * Game.Config.PowerUpMinScaleRate, Game.Config.PowerUpScaleChangeDuration).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
