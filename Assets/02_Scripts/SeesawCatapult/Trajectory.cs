using SeesawCatapult.ThisGame.Main;
using UnityEngine;

namespace SeesawCatapult
{
    public class Trajectory : MonoBehaviour
    {
        [SerializeField] private GameObject _DotsParent;
        [SerializeField] private GameObject _DotPrefab;
        [SerializeField] private CatapultSeat _PlayerCatapult;

        private Transform[] _dotsList;
        private float _timeStamp;

        private int _dotsNumber => Game.Config.TrajectoryDotNumber;
        private float _dotSpacing => Game.Config.TrajectoryDotSpacing;

        private void Start ()
        {
            HideTrajectory ();
            PrepareDots ();
        }
        
        private void PrepareDots()
        {
            _dotsList = new Transform[_dotsNumber];
            _DotPrefab.transform.localScale = Vector3.one * Game.Config.TrajectoryDotMaxScale;

            var scale = Game.Config.TrajectoryDotMaxScale;
            var scaleFactor = scale / _dotsNumber;

            for (var i = 0; i < _dotsNumber; i++)
            {
                _dotsList [i] = Instantiate (_DotPrefab, _DotsParent.transform).transform;

                _dotsList [i].localScale = Vector3.one * scale;
                if (scale > Game.Config.TrajectoryDotMinScale)
                    scale -= scaleFactor;
            }
        }

        public void UpdateDots (Vector2 direction)
        {
            _timeStamp = _dotSpacing;
            
            for (var i = 0; i < _dotsNumber; i++)
            {
                var force = new Vector3(direction.x, Game.Config.HumanThrowDirectionValueY, direction.y);
                var pos = force * Game.Config.CatapultThrowForce * _timeStamp;
                pos.y -= Physics.gravity.magnitude * _timeStamp * _timeStamp / 2f;
			
                _dotsList[i].position = pos + _PlayerCatapult.transform.position;
                _timeStamp += _dotSpacing;
            }
        }

        public void ShowTrajectory()
        {
            _DotsParent.SetActive (true);
        }

        public void HideTrajectory ()
        {
            _DotsParent.SetActive (false);
        }
    }
}
