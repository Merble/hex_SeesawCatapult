using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public event Action<Human, PowerUpType, Vector3, int> DidUsePowerUp;
    
    [SerializeField] private Team _Team;
    [SerializeField] private PowerUpType _PowerUpType;
    [Space]
    [SerializeField] private int _PowerUpEffectNumber;


    private void OnTriggerEnter(Collider other)
    {
        var human = other.GetComponentInParent<Human>();
        
        if (!human) return;

        if (human.Team != _Team) return;
        
        DidUsePowerUp?.Invoke(human, _PowerUpType, transform.position,_PowerUpEffectNumber);
        gameObject.SetActive(false);
    }
}
