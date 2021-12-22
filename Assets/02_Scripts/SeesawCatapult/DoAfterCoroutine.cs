using System;
using System.Collections;
using UnityEngine;

namespace AwesomeGame._02_Scripts.SeesawCatapult
{
    public static class DoAfterCoroutine
    {
        public static IEnumerator DoAfter(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            
            callback?.Invoke();
        }
    }
}