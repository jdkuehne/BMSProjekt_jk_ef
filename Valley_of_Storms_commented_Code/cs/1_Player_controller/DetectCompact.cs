using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEnums;
//Diese Klasse wird vom Spieler GameObject genutzt, um festzustellen ob Boden, Waende beruehrt werden.
public class DetectCompact : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht anhängen an GameObject
{
    //Externe Komponenten
    [SerializeField] private GameObject _movingPlatformParent; //Damit wird ein GameObject referenziert, welches das mitbewegen mit beweglichen Plattformen erlaubt
    public GameObject MovingPlatform; //temporaeres Speichern einer beruerten beweglichen Plattform
    public CapsuleCollider2D PlayerCol; //Spieler-Collider
    public RingAttach2 ringAttach; //obsolet
    public PlayerEnums PEnums; //hier werden Daten hineingeschrieben
    public ScriptableLayerMasks MyLayerMasks; //Filtern der Casts (Detektion von Boden/Waende) nach Layer wie Gegner-Layer, Terrain-Layer...
    public PlayerParams MyPlayerParams; //Spieler-Parameter (ScriptableObject), hier fuer WallJump-Winkelberechnung   

    //Allgemeine Variablen
    private Vector2 PColOffset; 
    private Vector2 PColSize;

    //Resultate
    //Collider2D[] groundCollisions; falls doch mit OverlapCircle
    private int numberPointsOnGround; //Kontaktzahlen
    private int numberHeadHits;
    private int numberLeftWallPoints;
    private int numberRightWallPoints;
    private bool setGround; //Anzahl Kontatkte > 0?
    private bool setHeadButt;
    private bool setLeftWall;
    private bool setRightWall;   
    public bool IsMovingPlat; //Wird von beweglichen Plattformen abgefragt, um herauszufinden, ob das Parent des Spielers sich mitbewegen soll
    private bool _movingPlatParentSet;
    public List<RaycastHit2D> gndHits = new List<RaycastHit2D>(); //gesamte Cast-Ergebnisse
    public List<RaycastHit2D> headbuttHits = new List<RaycastHit2D>();
    public List<RaycastHit2D> wallHitsLeft = new List<RaycastHit2D>();
    public List<RaycastHit2D> wallHitsRight = new List<RaycastHit2D>();
    //Cast Geometrie
    private Vector2 GndDetCapOrig; //Ursprung der Kapsel für Bodendetektion
    private Vector2 GndDetCapSize; //Grösse der Kapsel, wird anhand von Spieler-Collider berechnet
    public float GndDetCapHeight = 1f;
    private float GndCastDistance; 
    [Header("Normals and Vectors")]
    public Vector2 WallJumpVector; //Vektor für Sprung von Wand => wird abhängig von Seite und Winkel berechnet
    public Vector2 SlideVector; //Vektor für Wand Slide => wird abhängig von Seite und Winkel berechnet
    public Vector2 WallNormalPerpendicular; //Variable für speichern der Wandnormale (wenn Wand Slide), jedoch obsolet
    //Cast Geometries Wall
    private Vector2 WallDetectionBoxSize; //obsolet    

    
    void Start()
    {
        PlayerCol = GetComponent<CapsuleCollider2D>(); //Finde Collider auf selbem GameObject wie dieses Script
        ringAttach = GetComponent<RingAttach2>(); //obsolet
        PEnums = GetComponent<PlayerEnums>(); //Finde PlayerEnums-Objekt auf demselben GameObject wie dieses Script
        PEnums.terrain = ColState.IsAirborne; //Setze Status zuerst auf Luft (Spieler wird immer leicht über dem Boden gespawnt)

        //Detection Geometry Calcs
        PColOffset = PlayerCol.offset * transform.localScale; //Berechne Offset, beziehe Grösse des Spielers mitein, Zwischenwert
        PColSize = PlayerCol.size * transform.localScale; //Skaliere Grösse mit Spieler, Zwischenwert
        GndDetCapSize = new Vector2(PColSize.x - 0.2f, GndDetCapHeight); //Verkleinern des Casts in x-Achse => an den Radien sollte der Spieler abrutschen, nicht springen
        GndCastDistance = (PColSize.y - GndDetCapHeight) / 2f + 0.05f; //Casts Distanz wird so gesetzt, dass die Cast-Geometrie minimal aus dem Collider ragt       
        WallDetectionBoxSize = new Vector2(PColSize.x / 2f, PColSize.y/* - 0.1f*/); 
        _movingPlatParentSet = false;
    }

    
    void Update() //Jeden Frame ausgeführt
    {
        DetectionCasts(); //Führe alle Casts aus
        //******************************************
        if (setGround) //wenn Anzahl Bodenkontakte > 0 wird Status (terrain enum) auf ColState.IsGrounded gesetzt
        {
            PEnums.terrain = ColState.IsGrounded;
        }
        else
        {
            if (setRightWall) //analog Ground, ausserdem Berechnung von Wall Jump relevanten Werten (Winkel etc.)
            {
                PEnums.terrain = ColState.IsOnRightWall;  
                GetNormalAndAddDegrees(-MyPlayerParams.WallJumpAngle, -MyPlayerParams.SlideAngle, wallHitsRight[0].normal, false); //Slide und Wall Jump Winkel werden mit zwei der PlayerParams (ScriptableObject) und der Normale der berührten Wand berechnet
            }         
            else if (setLeftWall)//"
            {
                PEnums.terrain = ColState.IsOnLeftWall;
                GetNormalAndAddDegrees(MyPlayerParams.WallJumpAngle, MyPlayerParams.SlideAngle, wallHitsLeft[0].normal, true);
            }
            else if (setHeadButt)//"
            {
                PEnums.terrain = ColState.IsHeadButting;
            }
            else//anderenfalls muss der Spieler in der Luft sein
            {
                PEnums.terrain = ColState.IsAirborne;
            }
        }                
    }

    private void GetNormalAndAddDegrees(float angletoadd, float anglesmalltoadd, Vector2 basenormal, bool walldir) //right = false left = true 
    {
        //Debug.Log(basenormal);
        var walljumpsine = Mathf.Sin(Mathf.Deg2Rad * (Vector2.SignedAngle(Vector2.right, basenormal) + angletoadd)); /*Zwischenwinkel des Rechts-Vektors und Wandnormalenvektors wird in Rad umgerechnet & Wall Jump Winkel wird addiert 
        sin um y-Komponente des Sprungvektors*/
        var wallslidesine = Mathf.Sin(Mathf.Deg2Rad * (Vector2.SignedAngle(Vector2.right, basenormal) + anglesmalltoadd)); //"
        var walljumpcos = Mathf.Cos(Mathf.Deg2Rad * (Vector2.SignedAngle(Vector2.right, basenormal) + angletoadd)); 
        var wallslidecos = Mathf.Cos(Mathf.Deg2Rad * (Vector2.SignedAngle(Vector2.right, basenormal) + anglesmalltoadd));
        if (walldir)
        { 
            WallNormalPerpendicular = Vector2.Perpendicular(basenormal); //Berechne Senkrechte, Richtungsabhängig, obsolet
        }
        else
        {
            WallNormalPerpendicular = Vector2.Perpendicular(-basenormal);
        }

        WallJumpVector = new Vector2(walljumpcos, walljumpsine); //Einsetzen der berechneten x/y-Werte
        SlideVector = new Vector2(wallslidecos, wallslidesine);
        //Debug.Log(SlideVector);
    }
        
    private void DetectionCasts()
    {
        GndDetCapOrig = new Vector2(transform.position.x, transform.position.y) + PColOffset;

        numberPointsOnGround = Physics2D.CapsuleCast(GndDetCapOrig, GndDetCapSize, CapsuleDirection2D.Vertical, 0, Vector2.down, MyLayerMasks.GroundFilter, gndHits, GndCastDistance); //Raycast einer Kaspel um Bodenkontakt zu bestimmen
        if (numberPointsOnGround > 0) //Wenn Kontakte, wird set Ground gesetzt
        {
            setGround = true;
        }
        else
        {
            setGround = false;
        }
        if(gndHits.Find(x => x.collider.gameObject.CompareTag("MovingPlatform"))) //Wenn bewegliche Plattformen in den Kontakten vorhanden sind
        {            
            if (!_movingPlatParentSet) //Wenn ausserdem erster Frame mit Kontakt
            {
                MovingPlatform = gndHits.Find(x => x.collider.gameObject.CompareTag("MovingPlatform")).collider.gameObject; //Finde Plattform
                _movingPlatformParent.transform.position = MovingPlatform.transform.position; //Setze temp. Parent auf Position der Plattform
                gameObject.transform.SetParent(_movingPlatformParent.transform, true); //Setze temp. Parent tatsächlich als Parent des Spielers, erlaubt mitbewegen mit Plattform, Bewegung relativ zu Plattformbewegung ausgeführt
                _movingPlatParentSet = true; //erster Frame => check
            }
            IsMovingPlat = true; //Signal für Plattform, dass der Spieler möglicherweise auf ihr steht 
        }
        else //sonst mache alles rückgängig, sodass sich Spieler unabhängig von Plattform bewegt
        {
            IsMovingPlat = false;
            _movingPlatParentSet = false;
            MovingPlatform = null;
            gameObject.transform.SetParent(null, true);
        }

        numberHeadHits = PlayerCol.Cast(Vector2.up, MyLayerMasks.HeadbuttFilter, headbuttHits, 0.1f); //analog Groundcast, nutzt jedoch Spielergeometrie als Castgeometrie, genutzt um Sprung bei Kontakt zu stoppen
        if (numberHeadHits > 0) { setHeadButt = true; } else {  setHeadButt = false; }      

        numberLeftWallPoints = PlayerCol.Cast(Vector2.left, MyLayerMasks.LeftWallFilter, wallHitsLeft, 0.05f); //analog Kopfkontakte
        if (numberLeftWallPoints > 0 && !wallHitsLeft.Find(x => x.collider.gameObject.CompareTag("MovingPlatform"))) { setLeftWall = true; } else { setLeftWall = false; } //schliesst bewegliche Plattformen aus

        numberRightWallPoints = PlayerCol.Cast(Vector2.right, MyLayerMasks.RightWallFilter, wallHitsRight, 0.05f);
        if (numberRightWallPoints > 0 && !wallHitsRight.Find(x => x.collider.gameObject.CompareTag("MovingPlatform"))) { setRightWall = true; } else { setRightWall= false; }
    }

}
