using System;
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

        private Vector3 _seatPosition;
        public bool IsSeatFull => _IsSeatFull;

        private void Awake()
        {
            _seatPosition = transform.position;
        }

        public void HumanAddedToSeat()
        {
            _SatHumans++;

            if (_SatHumans >= _MaxHumanToSit)
                _IsSeatFull = true;
        }
        public void SetSeatPosition(float newY)
        {
            _seatPosition += new Vector3(0, newY, 0);
        }
        public Vector3 GetSeatPosition()
        {
            return _seatPosition;
        }
        public void ClearSeat()
        {
            _SatHumans = 0;
            _IsSeatFull = false;
        }
        
    }
}
