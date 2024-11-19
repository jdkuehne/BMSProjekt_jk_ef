using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface für Stats, das einige Grundfunktionen vorschreibt
public interface IStat<T>
{
    public void StatReset();
    public int Value {  get; set; }
    public void Increase(T amount); 
    public void Decrease(T amount);    
}
