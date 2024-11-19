using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Events;
using static PlayerEnums;
//Spieler Controller
public class MyCharacterCtrl : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht Anhängen an GameObject
{
    //SerializeField bedeutet im Editor verfügbar, aber nicht public
    [SerializeField] private float _playerScaleFactor; //Faktor für Spielergrösse, schlechte Lösung
    public InputSystem PInputs;
    public PlayerEnums pe; //Informationen zu Bodenkontakt usw.
    public PlayerParams PParams; //Diverse Parameter (ScriptableObject)
    public DetectCompact PDetect; //Detektionskomponente
    [SerializeField] private GameObject _bloodParticle; //Vorlage/Prefab für Erstellung von Blut bei Treffer
    
    public MyState<MyCharacterCtrl, int> _active; //Aktiver State 
    
    [Header("States")] public MyState<MyCharacterCtrl, int> myIdle; //restliche States, Typ ist gleich, jedoch werden eigentlich abstrakte Klassenmember eingesetzt, der Code der States ist nicht derselbe
    //jedoch besitzen sie identische Methodenaufrufe  (abstract void) wodurch der Code unterschiedliche Methoden unter demselben Namen auslösen kann.
    public MyState<MyCharacterCtrl, int> myDash;
    public MyState<MyCharacterCtrl, int> myJump;
    public MyState<MyCharacterCtrl, int> myWallJump;
    public MyState<MyCharacterCtrl, int> myGetOff; //von Wand abspringen
    public MyState<MyCharacterCtrl, int> myWallSlide;
    public MyState<MyCharacterCtrl, int> myGrappleHook; //obsolet
    public MyState<MyCharacterCtrl, int> myRagdoll;
    public MyState<MyCharacterCtrl, int> myAttack;
    public MyState<MyCharacterCtrl, int> myHeal;
    public MyState<MyCharacterCtrl, int> myPickUp; //für Aufnehmen von Objekten
    public MyState<MyCharacterCtrl, int> myPray; //für Rasten an Schrein

    public MyState<MyCharacterCtrl, int> TestState; //obsolet

    public GameObject Sword; //Schwertobjekt, nur von Attackenstate genutzt, dieser erhält eine Referenz zu diesem Script

    public Rigidbody2D _rigidbody; //Physikkomponente des Spielers   

    public SpriteRenderer _renderer; //Renderer/Optische Darstellung des Spielers
    public Animator animator; //Verwendet um Animator-Parameter zu schreiben und damit Animationen zu wechseln, primär durch States genutzt   

    public List<GameObject> EnemyHitboxes; //Gegner Hitboxes für Abfrage nach Ende von IFrames

    //*****************************************************************************************************************Item Pickup Solution
    public List<GameObject> PickupObjects = new List<GameObject>(); //Objekte, die momentan aufgesammelt werden können (Spieler ist im Bereich)

    public delegate void PickupHandler(object sender, PickupEventArgs e); //Ein Event-Typ für standardisierte Pickup-Datenpakete => auch für Infoschild

    public static event PickupHandler OnPickup; //Event, löst Info-Popup aus

    public void PickupFunc()
    {
        GameObject[] destroyObjects = new GameObject[10]; //Array, jedes Objekt, das aufgenommen wird am Schluss zerstört, max 10 Objekte, da mehr unwahrscheinlich, sonst Error
        int top = 0;
        foreach (var item in PickupObjects)
        {
            var script = item.GetComponent<ItemForPickUp>(); //Hole Informationen von Objekt
            OnPickup?.Invoke(this, new PickupEventArgs(script.pickup, script.item)); //Löse Event mit Infos aus
            SpawnItem thisitem = GameObject.FindWithTag("Spawner").GetComponent<LevelSpawnManager>().spawnItems.Find(x => x.itemName == item.name); //Finde Objekt in Spawner für weitere Daten
            //Debug.Log(thisitem.itemName);
            SpawningDataHandler.Instance.pickedUpItems.Add(thisitem.itemName); //Füge Name des Objekts der "aufgesammelten Liste" hinzu => Objekt wird nicht mehr gespawnt 
            destroyObjects[top] = item; //Zu zerstören
            top++;
        }
        for (int i = 0; i < top; i++)
        {
            Destroy(destroyObjects[i]); //Zerstöre alle Objekte in Array
        }
        PickupObjects.Clear();
    }

    public void OnDestroy() 
    {
        OnPickup = null; //Wenn Spieler zerstört wird (Ende des Spiels) sollten Event bereinigt werden, sie sind static und könnten bei Neustart des Spiels während der Session zu Fehlern führen
    }

    public void PickupListClear()
    {
        PickupObjects.Clear();
    }

    public enum ItemType //Vorbereitung für verschiedene Item Typen, nie genutzt, immer Talisman im Moment
    {
        Talisman,
        Heal,
        SilverStags
    }    
    //****************************************************************************************************************Events
    public UnityEvent OnPray; //Unity Event für Heilen, wiederherstellen von Flasks und Reset Gegner in anderen Räumen
    //*******************************************************************************************************************diverse Variablen
    public bool IsPogo = false;
    public bool IsDead = false;
    public bool HasStoredHorizontalInput = false;
    private float _storageTime = 0f;
    public float LastDirectionalInput;
    public bool CanDash = false; //Ground?
    public bool DashCooldown = false;
    private const float DashCooldownDuration = 0.3f;
    private float _dashCooldownTime = 0f;
    public bool CanGrapple;
    public Vector2 GrappleVector;
    public float GrappleDistance;
    public bool CanPray;    
    public bool IsInitialized = false;

    // Start is called before the first frame update

    //**********************************************************************************************************************************************************************Unity Event Functions
    void Start()
    {
        IsPogo = false;
        CanPray = false;
        pe = GetComponent<PlayerEnums>(); //Referenz Komponenten auf Spielerobjekt           
        _rigidbody = GetComponent<Rigidbody2D>();        
        PDetect = GetComponent<DetectCompact>();

        PInputs.InventoryPerformed += OpenInventory; //Füge "I" Inventar Öffnen hinzu
        PInputs.EscapePerformed += OpenMainMenu; //Füge "Esc" Hauptmenu Öffnen hinzu
        Sword = GameObject.FindWithTag("Player-Weapon"); //Finde Schwert per Tag (im Editor definiert)
        _active = myIdle; //Erster State

        LoadingManagerAdditive.OnSceneLoadStart += PickupListClear; //Bei Start von Scene load wird Liste aufnehmbarer Objekte geleert, da sonst Referenz zu zerstörtem Objekt Error auslösen könnte (theoretisch)
        LoadingManagerAdditive.OnSceneLoadStart += PInputs.Disable; //Aktionen bei Scene load sperren
        LoadingManagerAdditive.OnSceneLoadComplete += ResetHittingAfterDeath; //Spieler kann nach Laden wieder getroffen werden (Treffen wird bei Tod deaktiviert)
        LoadingManagerAdditive.OnSceneLoadComplete += PInputs.Enable;

        //PickupHandler.Pickup(this, new PickupEventArgs()); Test of Pickup c# event
        (TestState as MyIdle).Hello();
        IsInitialized = true; //Flag für FixedUpdate(States) und andere Objekte, dass vollständiges Laden des Spielers bestätigt

    }

    private void OnEnable() //Nur für Testzwecke, aktiviert Input, bei Aktivierung des Spielerobjekts im Editor
    {
        PInputs = new InputSystem(); 
        PInputs.Enable();
        InitIdle(0);
    }

    private void OnDisable() //&vice versa
    {
        PInputs.Disable();
        _active.Exit();
    }

    void Update()
    {
        SpriteHandler(); //Richtung des Spielers setzen
        if (DashCooldown) //Dash kurzzeitig nach Dash sperren => Timer
        {
            _dashCooldownTime += Time.deltaTime;
        }
        if (_dashCooldownTime > DashCooldownDuration)
        {
            DashCooldown = false;
            _dashCooldownTime = 0f;           
        } //bis hier Timer
    }

    private void FixedUpdate()
    {
        if (IsInitialized)
        {
            _active.WhileRunning(); //Löse State Methoden aus
            _active.HorizontalMovement();
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -25f, float.MaxValue)); //Max. Fallgeschwindigkeit, bis hier Gravitation simuliert
        }
    }
    //********************************************************************************************************************************************************************************Input specific Init-Functions

    //Folgende Methoden aktivieren/deaktvieren den jeweiligen Screen und schreiben Events für den Esc und I Input, wiederholt bis "Ende"
    private void OpenInventory(InputAction.CallbackContext context) //Beispiel:
    {
        UIManager.OpenScreenWithTag("UI-Inventory"); //Methode aus eigener Utility class, kürzt Befehl für Display auf "flex" setzen => UI öffnen
        PInputs.InventoryPerformed -= OpenInventory; //"I" öffnet Inventar nicht mehr, weil schon offen
        PInputs.EscapePerformed += CloseInventory; //"Esc" schliesst ab jetzt Inventar
        PInputs.EscapePerformed -= OpenMainMenu; //"Esc" öffnet Hauptmenu nicht mehr, Reihenfolge egal, da Input erst auf nächsten Frame möglich wäre
    }

    private void CloseInventory(InputAction.CallbackContext context)
    {
        UIManager.CloseScreenWithTag("UI-Inventory");
        PInputs.InventoryPerformed += OpenInventory;
        PInputs.EscapePerformed -= CloseInventory;
        PInputs.EscapePerformed += OpenMainMenu;
    }

    private void OpenMainMenu(InputAction.CallbackContext context)
    {
        UIManager.OpenScreenWithTag("UI-Main");
        PInputs.EscapePerformed -= OpenMainMenu;
        PInputs.InventoryPerformed -= OpenInventory;
        PInputs.EscapePerformed += CloseMainMenu;
    }

    private void CloseMainMenu(InputAction.CallbackContext context)
    {
        UIManager.CloseScreenWithTag("UI-Main");
        PInputs.EscapePerformed += OpenMainMenu;
        PInputs.InventoryPerformed += OpenInventory;
        PInputs.EscapePerformed -= CloseMainMenu;
    }

    public void CloseMainMenuByContinue()
    {
        UIManager.CloseScreenWithTag("UI-Main");
        PInputs.EscapePerformed += OpenMainMenu;
        PInputs.InventoryPerformed += OpenInventory;
        PInputs.EscapePerformed -= CloseMainMenu;
    }

    public void OpenSettings()
    {
        UIManager.OpenScreenWithTag("UI-Settings");
        UIManager.CloseScreenWithTag("UI-Main");
        PInputs.EscapePerformed -= CloseMainMenu;
        PInputs.EscapePerformed += CloseSettings;
    }

    public void CloseSettings(InputAction.CallbackContext context)
    {
        UIManager.CloseScreenWithTag("UI-Settings");
        GameObject.FindWithTag("UI-Settings").GetComponent<SettingsUIManager>().SetUIToLastApplied();
        UIManager.OpenScreenWithTag("UI-Main");        
        PInputs.EscapePerformed -= CloseSettings;
        PInputs.EscapePerformed += CloseMainMenu;
    }

    public void OpenControls()
    {
        UIManager.OpenScreenWithTag("UI-Controls");
        UIManager.CloseScreenWithTag("UI-Main");
        PInputs.EscapePerformed -= CloseMainMenu;
        PInputs.EscapePerformed += CloseControls;
    }

    public void CloseControls(InputAction.CallbackContext context)
    {
        UIManager.CloseScreenWithTag("UI-Controls");
        UIManager.OpenScreenWithTag("UI-Main");
        PInputs.EscapePerformed -= CloseControls;
        PInputs.EscapePerformed += CloseMainMenu;
    }
    //Ende

    public void OnTalismanSelectorOpen()
    {
        PInputs.EscapePerformed -= CloseInventory; //sperre schliessen während Talisman Selector
    }
    public void OnTalismanSelectorClose()
    {
        PInputs.EscapePerformed += CloseInventory; //entsperre
    }

    /*Die folgenden Methoden folgen immer demselben Muster: löse "Exit"-Code von letztem State aus, schreibe State, löse "Init"-Code von neuem aus, Ausnahmen werden beschrieben*/
    public void InitJump(InputAction.CallbackContext context)
    {        
        if (pe.terrain == ColState.IsGrounded || pe.terrain == ColState.IsGroundCloseToLeft || pe.terrain == ColState.IsGroundCloseToRight) //Input ist nur möglich, wenn Boden
        {
            _active.Exit(); //Exit von letztem
            _active = myJump; //setze State
            _active.Init(_rigidbody, this, 0); //Init von neuem
            StartCoroutine(JumpStateHandler(0, 0)); //Löse Handler für beenden des States aus, Argument zwei: 0 = Jump, 1 = Wall Jump
        }
        if ((pe.terrain == ColState.IsOnRightWall || pe.terrain == ColState.IsOnLeftWall) && _active.GetType() == typeof(MyWallSlide)) //Input ist nur möglich wenn Wände und Slide State
        {
            _active.Exit();
            _active = myWallJump;
            _active.Init(_rigidbody, this, 0);
            StartCoroutine(JumpStateHandler(PParams.WallJumpDuration, 1)); //Handler für Wall Jump weil "1"
        }
    }

    public void InitIdle(int modifier) //0 => Standard, 1 => direkter Wechsel in Slide, 2 => Attacken-Input gespeichert => Wechsel nach 0.1s, damit Spieler kurz laufen kann
    {
        if (_active != null) _active.Exit();
        _active = myIdle;
        _active.Init(_rigidbody, this, modifier);
    }

    public void InitHeal(InputAction.CallbackContext context)
    {
        if (pe.terrain == ColState.IsGrounded && GetComponent<Flask>().Flasks > 0) //Nur wenn auf Boden und Flaschen übrig
        {
            _active.Exit();
            _active = myHeal;
            _active.Init(_rigidbody, this, 0);            
        }

    }

    public void InitPickup(InputAction.CallbackContext context)
    {
        if (PickupObjects.Count > 0) //nur wenn Objekt auszunehmen
        {
            _active.Exit();
            _active = myPickUp;
            _active.Init(_rigidbody, this, 0);
        }
    }

    public void InitPray(InputAction.CallbackContext context) //Rasten an Schrein
    {
        if(pe.terrain == ColState.IsGrounded && CanPray) //Nur wenn auf Boden und im Bereich eines Schreins (CanPray)
        {
            _active.Exit();
            _active = myPray;
            _active.Init(_rigidbody, this, 0);
        }                
    }

    public void InitWallSlide(int side) //side: 0 => right, 1 => left
    {
        _active.Exit();
        _active = myWallSlide;
        _active.Init(_rigidbody, this, side);      
    }

    public void InitDash(InputAction.CallbackContext context)
    {        
        if (CanDash && !DashCooldown && pe.terrain != ColState.IsOnRightWall && pe.terrain != ColState.IsOnLeftWall) //Wenn erster in Luft, nicht im Cooldown und nicht an Wänden
        {
            if (_active.GetType() == typeof(MyJump)) 
            { 
                LeaveStateToIdle(); //Setzt State zuerst auf Idle, wenn auf Jump, wahrscheinlich irrelevant
            }
            _active.Exit();
            _active = myDash;
            _active.Init(_rigidbody, this, 0);
            StartCoroutine(DashStateHandler());
        }        
    }

    public void InitAttack(InputAction.CallbackContext context)
    {
        if (_active != null) _active.Exit();

        if(PInputs.VerticalAxis() < -0.1f) //Attacke wird abhängig von vertikalem Input W/S anders animiert, hier: abwärts
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 1);
        }
        else if(PInputs.VerticalAxis() > 0.1f) //hier: aufwärts
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 2);
        }
        else //hier: vorwärts
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 0);
        }
        
    }

    public void InitChainAttack() //Kopie ohne den Unity Input System Event Kontext => normales Auslösen per Methoden-Call
    {
        if (_active != null) _active.Exit();

        if (PInputs.VerticalAxis() < -0.1f)
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 1);
        }
        else if (PInputs.VerticalAxis() > 0.1f)
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 2);
        }
        else
        {
            _active = myAttack;
            _active.Init(_rigidbody, this, 0);
        }
    }

    public void InitGetOffRight(InputAction.CallbackContext context)
    {
        _active.Exit();
        _active = myGetOff;
        _active.Init(_rigidbody, this, 0); //right is 0 left is 1
        StartCoroutine(GetOffStateHandler());               
    }

    public void InitGetOffLeft(InputAction.CallbackContext context)
    {        
        _active.Exit();
        _active = myGetOff;
        _active.Init(_rigidbody, this, 1); //right is 0 left is 1
        StartCoroutine(GetOffStateHandler());            
    }
    //obsolet
    public void InitGrapple(InputAction.CallbackContext context)
    {        
        GrappleCheck(PParams.GrappleCastDimensions.x, PParams.GrappleCastDimensions.y, PParams.GrappleRange);
        if (CanGrapple && _active.GetType() != typeof(MyGrappleHook)) //Achtung die Logik funktioniert so nicht
        {                     
            _active.Exit();
            _active = myGrappleHook;
            _active.Init(_rigidbody, this, 0);
            StartCoroutine(GrappleStateHandler());
        }
        else if (_active.GetType() == typeof(MyGrappleHook))
        {
            LeaveStateToIdle();
        }
        else
        {
            Debug.Log("No point in sight!");
        }
    }   

    //*****************************************************************************************************************Handlers

    private IEnumerator GetOffStateHandler() //Handler für Wand verlassen => wartet einfach 0.1s
    {
        yield return new WaitForSeconds(0.1f);
        InitIdle(0);

    }

    public IEnumerator DashStateHandler() //Handler für Dash =>  
    {
        yield return new WaitForSeconds(PParams.DashDuration); //wartet Zeit aus PlayerParams
        InitIdle(1); //schaltet auf Idle mit Wandslide-Abfrage
        HitCheckAfterDash(); //kontrolliert ob noch immer Kontakt zu einem Gegner besteht, der während Dash IFrames berührt wurde
    }

    private IEnumerator GrappleStateHandler() //obsolet
    {
        yield return new WaitForSeconds(0.2f);
        var grappletime = GrappleDistance / PParams.GrappleVelocity; 
        for(float time = 0; time < grappletime - 0.1f; time += Time.deltaTime)
        {
            if (_active.GetType() != typeof(MyGrappleHook))
            {
                yield break;
            }
            if (pe.terrain != ColState.IsAirborne)
            {
                break;
            }            
            yield return null;
        }
        InitIdle(0);

    }
    
    public IEnumerator JumpStateHandler(float duration, int variation) //Handler für Jump und Wall Jump
    {
        
        //yield return new WaitUntil(TakenOff); //saubere Lösung die Timer ersetzen sollte, sorgte aber für Bugs
        yield return new WaitForSeconds(0.1f); //min 0.1f => bis Boden/Wand verlassen ist
        float time = 0f; //Initialisiere Timer Variable 
        while (true)
        {
            if (!(_active.GetType() == typeof(MyAttack) || _active.GetType() == typeof(MyJump) || _active.GetType() == typeof(MyWallJump))) { yield break; } //Bei Wechsel von State abgesehen von Jump, Wall Jump, Attack => bricht aus Coroutine

            else if (_rigidbody.velocity.y <= 0f || (variation == 1 && time >= duration)){ //sonst wenn Höhepunkt oder Wall Jump Dauer erreicht => bricht aus loop
                break; 
            }

            else if (!(pe.terrain == ColState.IsAirborne || pe.terrain == ColState.IsOnRightWall || pe.terrain == ColState.IsOnLeftWall) || PInputs.JumpAcceleration() < 0.5f){ //sonst wenn nicht Luft oder Wand oder Space nicht gedrückt => 
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, PParams.JumpStopFactor * _rigidbody.velocity.y); //Geschwindigkeit wird verringert => Fall
                break; //bricht aus loop
            }

            else { }
            time += Time.deltaTime; //Zähler = Zähler + Frametime
            yield return null; //Springe aus Ausführung und mache hier weiter beim nächsten Frame
        }
        
        yield return new WaitUntil(() => _active.GetType() != typeof(MyAttack)); //Warte bis allfällige Attacken ausgeführt sind
        InitIdle(0); //zurück zu Idle
    }
    
    //**********************************************************************************************************************************************************************************Miscellaneous Functions
    
    public void LeaveStateToIdle()
    {        
        InitIdle(0);
    }    
    public void LeaveAttackChainToIdle()
    {
        InitIdle(2); 
    }


    

    public void CaptureLastDirectionalInput() //speichert letzten Horiontalen Input
    {
        if (PInputs.HorizontalAxis() > 0.5f)
        {
            LastDirectionalInput = 1f;
        }
        if(PInputs.HorizontalAxis() < -0.5f)
        {
            LastDirectionalInput = -1f;
        }            
    }
    private void SpriteHandler() //Richtungswechsel anhand letztem hor. Input
    {
        switch (LastDirectionalInput)
        {
            case > 0.5f: transform.localScale = new Vector3(1, 1, 1) * _playerScaleFactor; break;
            case < -0.5f: transform.localScale = new Vector3(-1, 1, 1) * _playerScaleFactor; break;
            default: break;
        }
    }
    public void StoreInputX() //Input wird gespeichert für den Fall, dass Wall Jump Distanz ausreicht A/D aber losgelassen
    {
        if (!HasStoredHorizontalInput) { HasStoredHorizontalInput = true; StartCoroutine(RemoveHorizontalInput()); }
        else { _storageTime = 0; }
    }
    private IEnumerator RemoveHorizontalInput() //Input wird nach x Zeit wieder gelöscht
    {
        for (; _storageTime < 1f; _storageTime += Time.deltaTime) { yield return null; }
        HasStoredHorizontalInput = false;
        _storageTime = 0f;
    }



    private void GrappleCheck(float height, float width, float distance) //obsolet
    {
        Vector2 playerpos = new(transform.position.x, transform.position.y);
        Vector2 mousepos = Get2DMousePosition();
        Vector2 diffmouseplayer = mousepos - playerpos;
        Vector2 origin = playerpos + diffmouseplayer.normalized;
        Vector2 size = new(width, height);
        var angle = Vector2.Angle(Vector2.right, diffmouseplayer.normalized);
        //AngleSquare.transform.SetPositionAndRotation(origin, Quaternion.Euler(0, 0, angle));
        //AngleSquare2.transform.SetPositionAndRotation(origin + distance * diffmouseplayer.normalized, Quaternion.Euler(0, 0, angle));
        //AngleSquare.transform.localScale = size;
        //AngleSquare2.transform.localScale = size;
        RaycastHit2D[] mousehit = new RaycastHit2D[10];
        int nMouseHit = Physics2D.BoxCast(origin, size, angle, diffmouseplayer.normalized, PDetect.MyLayerMasks.GrapplePointFilter, mousehit, distance);
        
        if (nMouseHit > 0) 
        { 
            CanGrapple = true;
            GrappleVector = mousehit[0].point - new Vector2(transform.position.x, transform.position.y);
            GrappleDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), mousehit[0].point);
        }
        else { CanGrapple = false; }
    }    

    private void OnCollisionEnter2D(Collision2D collision) //Aufnahme von Treffern
    {
        if (collision.gameObject.TryGetComponent(out Hitbox hitbox) && !IsDead) //Wenn Komponente Hitbox (kleines Script mit Schadenswert) und nicht Tot, dann:
        {
            if (!EnemyHitboxes.Contains(collision.gameObject)) //Wenn dieses Objekt ausserdem noch nicht im Schadensabfrageprozess ist:
            {
                EnemyHitboxes.Add(collision.gameObject); //Füge zu "jetzt in Abfrage" hinzu
                if (EnemyHitboxes.Count <= 1 && _active.GetType() != typeof(MyDash)) //Wenn nicht Dash und einzige Hitbox
                {
                    GetComponent<HealthV2>().Damage(hitbox.DamageValue, false); //direkt Schaden an Health-Script senden                                     
                }
            }
        }
    }    
    //Diese Methode erlaubt uns nach einer Kollision die Berührung mit einem Gegner weiterzuverfolgen, falls der Spieler im Bereich von dessen Hitbox bleibt und ausserdem IFrames bei Treffer
    private IEnumerator IFrameHandler() //Ausgelöst von OnPlayerDamaged()
    {        
        yield return new WaitForSeconds(PParams.KnockbackDuration); //wartet nach Treffer bis Kontrolle über Spieler zurückerlangt
        InitIdle(0); //gibt Spieler Kontrolle zurück
        yield return new WaitForSeconds(PParams.IFrameTime); //Warte bis IFrame-Zeit vorbei
        int count = EnemyHitboxes.Count;
        for (int i = 0; i < count; i++) //für jede gespeicherte Hitbox überprüfen, ob sie noch immer den Spieler berührt, erste bei der dies zutrifft, löst Damage aus.
        {
            if (_rigidbody.IsTouching(EnemyHitboxes[0].GetComponent<Collider2D>()))
            {
                GetComponent<HealthV2>().Damage(EnemyHitboxes[0].GetComponent<Hitbox>().DamageValue, false);                
                yield break; //bricht aus Coroutine, Rest der Liste wird erst beim nächsten Durchlauf abgefragt
            }
            else
            {
                EnemyHitboxes.RemoveAt(0); //Wenn kein Kontakt, wird der Gegner aus der Liste gelöscht
            }
        }
    }

    public void HitCheckAfterDash() //Call in DashHandler, ähnlich IFrameHandler
    {
        int count = EnemyHitboxes.Count;
        for (int i = 0; i < count; i++)
        {
            if (_rigidbody.IsTouching(EnemyHitboxes[0].GetComponent<Collider2D>()))
            {
                GetComponent<HealthV2>().Damage(EnemyHitboxes[0].GetComponent<Hitbox>().DamageValue, false);
                break;
            }
            else
            {
                EnemyHitboxes.RemoveAt(0);
            }
        }
    }

    public void OnPlayerDamaged() //Ausgelöst von HealthV2.Damaged-Event
    {
        if (_active.GetType() != typeof(MyHeal)) //Wenn nicht in Heilungs-State => Exit-Code
        {
            _active.Exit();
        }
        else { GetComponent<Flask>().Decrement(); } //sonst Heilung wird abgebrochen, die Flasche verliert man aber trotzdem
        _active = myRagdoll; //State ohne Update-Inhalt => der Spieler kann sich nicht bewegen
        _active.Init(_rigidbody, this, 0); //Init-Code
        StartCoroutine(IFrameHandler()); //löse IFrameHandler aus        
    }

    public void HitParticle() //ausgelöst von Damaged-Event => erstellt ein Prefab mit selbstzerstörendem Blut-Particle ohne Parent (in World Space)
    {
        Instantiate(_bloodParticle, transform.position, transform.rotation);
    }

    public void ResetBigEnemySpawns() //
    {
        SpawningDataHandler.Instance.killedBigEnemies.Clear();
    }

    public void ResetHittingAfterDeath()
    {
        if (IsDead)
        {
            IsDead = false;
        }
    }

    public Vector2 Get2DMousePosition()
    {
        var input = PInputs.MousePosition();
        return Camera.main.ScreenToWorldPoint(input);
    }
}
