using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Verschiebt GameObject in DontDestroyOnLoad
public class DontDestroySingle : MonoBehaviour
{
    public static DontDestroySingle Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }    
}
