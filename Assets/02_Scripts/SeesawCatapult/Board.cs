using AwesomeGame._02_Scripts.SeesawCatapult.Enums;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Team _Team;

        public Team Team => _Team;
    }
}
