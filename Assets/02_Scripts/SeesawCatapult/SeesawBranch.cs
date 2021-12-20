using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SeesawBranch : MonoBehaviour
{
    public Action<float, bool> DidMassChange;

    [SerializeField] private SeesawBranch _SeesawBranchPrefab;
    [SerializeField] private SeesawPad _SeesawPad;
    [SerializeField] private Transform _EndPointOfBranch;
    [SerializeField] private Transform _ParentTransform;
    [Space]
    [SerializeField, Sirenix.OdinInspector.ReadOnly] private List<Human> _Humans = new List<Human>();
    [Space]
    [SerializeField] private bool _IsPlayer;
    [SerializeField] private float _MaxMassForASeesawPad;
    [SerializeField] private float _EndPointOfBranchDistanceZ;
    [SerializeField, ReadOnly] private float _TotalMass;
    [Space] 
    [SerializeField] private float _ScaleChangeDuration;
    [SerializeField] private float _MinScale;
    [SerializeField] private float _MaxScale;

    private int _seesawPadCount;
    private bool _isCreatingSeesaw;
    
    //public List<Human> Humans => _Humans;

    private void Awake()
    {
        _seesawPadCount = 1;
    }

    public void AddHuman(Human human)
    {
        _TotalMass += human.Mass;

        if (_TotalMass >= _seesawPadCount * _MaxMassForASeesawPad)
        {
            InstantiateSeesawBranch();
            _seesawPadCount++;
        }
        
        _Humans.Add(human);
        
        DidMassChange?.Invoke(human.Mass, _IsPlayer);
    }

    private void InstantiateSeesawBranch()
    {
        if (_isCreatingSeesaw)
            return;
        
        _isCreatingSeesaw = true;
        var position = _EndPointOfBranch.position;
        var newBranch = Instantiate(_SeesawBranchPrefab, _ParentTransform, false);
        newBranch.transform.position = position;
        
        SetSeesawPad(newBranch, position);
        
        AnimateSeesawBranchInstantiate(newBranch);
    }

    private void AnimateSeesawBranchInstantiate(SeesawBranch newBranch)
    {
        newBranch.transform.localScale = Vector3.one * _MinScale;
        LeanTween.scale(newBranch.gameObject, Vector3.one * _MaxScale, _ScaleChangeDuration).setOnComplete(() =>
        {
            _isCreatingSeesaw = false;
        });
    }

    private void SetSeesawPad(SeesawBranch newBranch, Vector3 endPointOfBranchPos)
    {
        endPointOfBranchPos.z += newBranch._EndPointOfBranchDistanceZ * (_IsPlayer ? 1 : -1);
        newBranch.transform.Rotate(Vector3.up, 180 * (_IsPlayer ? 2 : 1));
        _EndPointOfBranch.transform.position = endPointOfBranchPos;
        
        _SeesawPad = newBranch._SeesawPad;
    }
    public SeesawPad GetSeesawPad()
    {
        return _SeesawPad;
    }
}
