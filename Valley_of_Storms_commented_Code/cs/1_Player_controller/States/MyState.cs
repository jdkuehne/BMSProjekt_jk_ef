using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MyState<T, U> : ScriptableObject where T : MonoBehaviour
{
    public abstract void Init(Rigidbody2D rigid, T charactercontroller, U variation);
    public abstract void WhileRunning();
    public abstract void Exit();
    public abstract void HorizontalMovement();
}
