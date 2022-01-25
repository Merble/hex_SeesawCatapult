using System;
using UnityEngine;

namespace SeesawCatapult
{
    public class CatapultSeat : MonoBehaviour
    {
        private bool _isSeatFull;
        private Vector3 _defaultPosition;

        public Vector3 DefaultPosition => _defaultPosition;

        private void Awake()
        {
            _defaultPosition = transform.position;
        }

        public bool IsSeatFull()
        {
            return _isSeatFull;
        }

        public void SetIsSeatFull(bool isFull)
        {
            _isSeatFull = isFull;
        }
        
    }
}
