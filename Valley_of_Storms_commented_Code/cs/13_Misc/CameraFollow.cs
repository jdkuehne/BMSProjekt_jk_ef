using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerEnums;

//Script für Kamera (definiert Blickfeld), bewegt sich mit Spieler, kann per S/W-Input auf/ab verschoben werden, bleibt an Level-Rändern stehen
//und kann in Bossräumen separat angepasst werden
public class CameraFollow : MonoBehaviour
{
    private GameObject _player; 
    private MyCharacterCtrl _characterCtrl;
    public Transform pTransform;    
    [SerializeField] Vector3 camOffset;
    [SerializeField] float YAxisOffsetFactor;    
    public Transform leftBorder;
    public Transform rightBorder;
    [SerializeField] float BorderOffset;
    private float currentVerticalInputOffset; //This offset is controlled by the y-Axis => S & W-Buttons
    private float currentBossRoomOffset; //This offset is Triggered alongside the fogwall and boss-ai by some trigger in the bossroom
    private float bossRoomOffsetInput; //if change this value when boss room is entered, the offset parameter will change to this one smoothly

    float verticalAxisLv1;
    float verticalAxisLv2;
    float verticalAxisLv3;
    void Start()
    {
        currentVerticalInputOffset = 0;
        currentBossRoomOffset = 0;
        bossRoomOffsetInput = 0;
        verticalAxisLv1 = 0f;
        verticalAxisLv2 = 0f;
        verticalAxisLv3 = 0f;
        _player = GameObject.FindWithTag("Player");
        _characterCtrl = _player.GetComponent<MyCharacterCtrl>();
        pTransform = _player.GetComponent<Transform>();
        LoadingManagerAdditive.OnSceneLoadStart += SetBordersToDefault;
        LoadingManagerAdditive.OnSceneLoadComplete += SetBordersToLevel;
    }

    // Update is called once per frame
    
    void LateUpdate()
    {
        if(_player != null) 
        {            
            if(_characterCtrl.pe.terrain == ColState.IsGrounded && pTransform.GetComponent<Rigidbody2D>().velocity.x > -0.5f && pTransform.GetComponent<Rigidbody2D>().velocity.x < 0.5f && _characterCtrl._active.GetType() == typeof(MyIdle))
            {
                verticalAxisLv1 = _characterCtrl.PInputs.VerticalAxis() * 5f;                
            }
            else { verticalAxisLv1 = 0f;}
            verticalAxisLv2 += (verticalAxisLv1 - verticalAxisLv2) * 2f * Time.deltaTime;
            switch (verticalAxisLv2)
            {
                case > 4f: verticalAxisLv3 = 1f; break;
                case < -4f: verticalAxisLv3 = -1f; break;
                default: verticalAxisLv3 = 0f; break;

            }            
            currentVerticalInputOffset += (verticalAxisLv3 - currentVerticalInputOffset) * 3f * Time.deltaTime;
            currentBossRoomOffset += (bossRoomOffsetInput - currentBossRoomOffset) * 2f * Time.deltaTime;                        
            transform.position = new Vector3(Mathf.Clamp((pTransform.position + camOffset).x, leftBorder.position.x + BorderOffset, rightBorder.position.x - BorderOffset), 
                pTransform.position.y + camOffset.y + YAxisOffsetFactor * currentVerticalInputOffset + currentBossRoomOffset, 
                (pTransform.position + camOffset).z);            
        }        
    }

    public void SetBossRoomCamOffset(float thisBossRoomOffset)
    {
        bossRoomOffsetInput = thisBossRoomOffset;
    }

    public void ResetBossRoomCam()
    {
       bossRoomOffsetInput = 0f;
    }

    public void SetBordersToDefault()
    {
        leftBorder = GameObject.FindWithTag("DefaultCamBorders").transform.Find("Left");
        rightBorder = GameObject.FindWithTag("DefaultCamBorders").transform.Find("Right");
    }

    public void SetBordersToLevel()
    {
        leftBorder = GameObject.FindWithTag("CamBorders").transform.Find("Left");
        rightBorder = GameObject.FindWithTag("CamBorders").transform.Find("Right");
    }
}
