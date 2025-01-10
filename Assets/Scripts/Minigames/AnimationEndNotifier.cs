using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEndNotifier : MonoBehaviour
{
    public UnityEvent onAnimationEnd = new UnityEvent();

    public void NotifyAnimationEnd()
    {
        onAnimationEnd.Invoke();
    }
}
