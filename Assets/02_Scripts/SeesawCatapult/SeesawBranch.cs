using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class SeesawBranch : MonoBehaviour
    {
        public Action<float, bool> DidMassChange;
    
        [SerializeField] private List<SeesawSeat> _SeesawSeats;
        [Space]
        [SerializeField, ReadOnly] private List<Human> _Humans = new List<Human>();
        [Space]
        [SerializeField] private bool _IsPlayer;
        [SerializeField, ReadOnly] private float _TotalMass;
    
    
        public void AddHuman(Human human)
        {
            _TotalMass += human.Mass;
        
            _Humans.Add(human);
        
            DidMassChange?.Invoke(human.Mass, _IsPlayer);
        }
    
        public SeesawSeat GetSeesawSeat()
        {
            return _SeesawSeats.FirstOrDefault(seesawSeat => !seesawSeat.IsSeatFull);
        }

        public void ClearSeats()
        {
            foreach (var human in _Humans)
            {
                human.DestroySelf();
            }

            foreach (var seat in _SeesawSeats)
            {
                seat.ClearSeat();
            }
        
            _Humans.Clear();
        }
    }
}
