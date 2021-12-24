using Unity.Collections;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class SeesawSeat : MonoBehaviour
    {
        [SerializeField] private int _MaxHumanToSit;
        [SerializeField, ReadOnly] private int _SatHumans;
        [Space]
        [SerializeField, ReadOnly] private bool _IsSeatFull;
        
        public bool IsSeatFull => _IsSeatFull;

        public void HumanAddedToSeat()
        {
            _SatHumans++;

            if (_SatHumans >= _MaxHumanToSit)
                _IsSeatFull = true;
        }

        public void ClearSeat()
        {
            _SatHumans = 0;
            _IsSeatFull = false;
        }
        
    }
}
