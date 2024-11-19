using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnums : MonoBehaviour
{
    //Enum, der Kontakt zu Boden und Wänden beschreibt, wird in DetectCompact.cs geschrieben
    public enum ColState
    {
        IsGrounded, IsGroundCloseToLeft, IsGroundCloseToRight, IsAirborne, 
        IsOnRightWall, IsOnLeftWall, IsOnChain, IsHeadButting //Headbutting is never really used but is an exit option for Jump state (although not necessary at this point, new Jump solution)
    }
    public ColState terrain = new();
    
}
