using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.XR;

//Controllerscript des Bosses
public class BossController : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht anhängen an GameObject
{    
    //Components and Children
    private Animator animator;
    private SpriteRenderer sr;
    private ParticleSystem deathPs;
    private List<Collider2D> bossCols = new(); //Collider des Bosses  
    private GameObject hand1;
    private GameObject hand2; 
    private GameObject sword; //obsolet
    [SerializeField] List<Light2D> flashLights = new(); //um Augen zum Aufleuchten zu bringen, wird Unity Light2D gebraucht und dessen Intensity Parameter verändert
    [SerializeField] BossParams bossParams; //Parameter in ScriptableObject

    //Lerp stuff
    private float time; //Zeit für Lerp bei Treffer
    private float hitColorStatus; 
    private float currentColorStatus;
    [SerializeField] private Color hitColor; //Farbe bei Treffer
    [SerializeField] private Color baseColor; //Farbe normal
    [SerializeField] private Color deathColor; //Farbe bei Tod => Boss wird schwarz
    [SerializeField] private float durationUntilHitColor; //Zeit bis Lerp wieder zurück Richtung baseColor wechselt

    //External components
    private Transform playerTransform; //Transform-Komponente des Spielers

    //misc variables
    [SerializeField] Color visible; //Opake Farbe für Fades
    [SerializeField] Color invisible; //Transparente Farbe für Fades
    public bool BossTrig = false; //Trigger für Boss AI => wird durch Trigger in der Arena getoggelt
    public bool TeleportTrigger = false; //Ausgelöst von Animation-Event in TeleportOut Animation, ermöglicht warten auf Animationsende in Coroutine
    public bool IdleTrigger = false; //Ende von Hereinteleportieren
    private bool DeathTrigger = false; //Ende der Todesanimation
    private bool transistioning = false; //State-Transistions laufen bei Tod zuerst noch fertig
    private Coroutine currentAttack; //Kann Coroutine speichern, für Beenden der derzeitigen Attacken-Coroutine per StopCoroutine
    private bool deathCoroutineStarted = false; //Flag das anzeigt, dass Boss in Todesanimation ist
    //weiteres Eventhandling mit BladeEvent.cs => Animation-Event wenn Klingen (Upwards Sword Barrage) an Ende von Animation sind

    //****************************************************************************************************************************************************************************************
    //children transform values
    [System.Serializable] //Macht struct oder class im Editor verfügbar
    private struct ChildTransforms //Für setzen gewisser Positionen von Children im Editor
    {
        private BossController thisBoss; 

        public TypeTransform LeftHand;
        public TypeTransform RightHand;
        public TypeTransform Sword;
        public Transform Teleport; //Unfortunately only applicable for upwards sword barrage

        public void SetParentTo(BossController bossController)
        {
            thisBoss = bossController;
        }

        public void Write()
        {
            thisBoss.hand1.transform.localPosition = LeftHand.localPosition;
            thisBoss.hand2.transform.localPosition = RightHand.localPosition;
            thisBoss.sword.transform.localPosition = Sword.localPosition;

            thisBoss.hand1.transform.eulerAngles = LeftHand.rotation;
            thisBoss.hand2.transform.eulerAngles = RightHand.rotation;
            thisBoss.sword.transform.eulerAngles = Sword.rotation;

            thisBoss.hand1.transform.localScale = LeftHand.scale;
            thisBoss.hand2.transform.localScale = RightHand.scale;
            thisBoss.sword.transform.localScale = Sword.scale;
        }
    }

    [System.Serializable] 
    public struct TypeTransform //Transform-Ersatz struct, um einfach Transform Werte zu speichern
    {
        public Vector3 localPosition;
        public Vector3 rotation;
        public Vector3 scale;
    }    
    
    //****************************************************************************************************************************************************************************************

    //Attack related

    //Erste Attacke
    [Header("Upwards Sword Barrage")] [SerializeField] ChildTransforms uSBTransforms; //Start Positionen der Child-Objects (Hände usw.)
    [SerializeField] List<GameObject> GroundBlades = new(); //Liste mit Bodenklingen => So können alle gleichzeitig in einem foreach-loop adressiert werden

    //Zweite Attacke
    [Header("Side Hover"), SerializeField] ChildTransforms sHTransforms; //"
    [SerializeField] List<Transform> SH_TeleportOptions; //Side Hover kann von Linker zu Rechter Ecke und umgekehrt geschehen, das sind beide Startpunkte/Ziele

    private enum BossAnims
    {
        UpwardsSwordBarrage, SideHover, Attack3, None
    }
    private BossAnims lastAnim = new(); //für Verhindern von Attacken-Wiederholung

    IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        deathPs = GetComponent<ParticleSystem>(); //ParticleSystem, das bei Tod startet, Boss "zerfällt"
        hand1 = transform.Find("Hand1").gameObject; // the left "unflipped" hand
        hand2 = transform.Find("Hand2").gameObject; // the right "flipped" hand       
        sword = transform.Find("Sword").gameObject;
        uSBTransforms.SetParentTo(GetComponent<BossController>());
        sHTransforms.SetParentTo(GetComponent<BossController>()); 
        lastAnim = BossAnims.None;
        GetComponents<Collider2D>(bossCols); //Schreibe Collider des Bosses in bossCols
        //Port in first time
        sr.enabled = false; //deaktiviere Visuelle Darstellung/Renderer aus
        yield return new WaitUntil(() => BossTrig); //Warte bis Eintritt in Startbereich
        playerTransform = GameObject.FindWithTag("Player").transform; //Finde Spieler       
        animator.SetInteger("BossState", 0); //Teleport in       
        sr.enabled = true; //Renderer ein
        animator.enabled = true; //Animator aktiviert
        yield return new WaitUntil(() => IdleTrigger); //Warte auf Ende von Teleport
        IdleTrigger = false; 

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Flash(bossParams.FlashDuration)); //Aufleuchten der Augen        
        StartCoroutine(TransistionToNextMove(0.2f)); //Wechsel in Attacken-Auswahl
    }

    private void Update()
    {
        if (hitColorStatus > 0.5f) //Lerp bei Treffer, vergleichbar mit BigTreeFront.cs
        {
            time += Time.deltaTime;
            if (time > durationUntilHitColor)
            {
                hitColorStatus -= 1f;
                time = 0;
            }
        }
        if (!deathCoroutineStarted)
        {
            currentColorStatus += (hitColorStatus - currentColorStatus) * 8f * Time.deltaTime;
            sr.color = Color.Lerp(baseColor, hitColor, currentColorStatus);
        }   
        
    }

    //****************************************************************************************************************************************************************************************

    public void SetIdleAnim() //Aufgerufen von Teleport in - Animation Event, automatischer Wechsel in Idle State (1) bei Teleport Ende
    {
        IdleTrigger = true;
        animator.SetInteger("BossState", 1);        
    }

    //Moves
    public void SetTeleportTrigger() //Aufgerufen von Teleport out - Animation Event
    {
        TeleportTrigger = true;
    } 
    
    public void SetDeathTrigger() //Aufgerufen von Death - Animation Event
    {
        DeathTrigger = true;
    }
    
    //Methoden zum deaktivieren von Komponenten mehrerer GameObjects (alle GameObjects der jeweiligen Listen)
    private void EnableRenderers(List<GameObject> gameObjects)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go.TryGetComponent(out SpriteRenderer sprite))
            {
                sprite.enabled = true;
            }
        }
    }
    private void DisableRenderers(List<GameObject> gameObjects)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go.TryGetComponent(out SpriteRenderer sprite))
            {
                sprite.enabled = false;
            }
        }
    }
    private void EnableColliders(List<GameObject> gameObjects)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go.TryGetComponent(out Collider2D col))
            {
                col.enabled = true;
            }
        }
    }
    private void DisableColliders(List<GameObject> gameObjects)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go.TryGetComponent(out Collider2D col))
            {
                col.enabled = false;
            }
        }
    }

    //Teleport Coroutine
    IEnumerator TeleportTo(Vector2 destination)
    {
        bossCols.ForEach(x => x.enabled = false); //deaktiviere Boss-Collider
        animator.SetInteger("BossState", 2); //Teleport out Animation
        yield return new WaitUntil(() => TeleportTrigger); //Warte auf Animation
        yield return null;
        TeleportTrigger = false;
        sr.enabled = false;
        transform.position = destination; //Setzt Position auf Ziel
        yield return new WaitForSeconds(bossParams.TeleportTime); //Warte für Teleportdauer in BossParams
        sr.enabled = true;
        animator.SetInteger("BossState", 0); //Teleport in
        yield return new WaitUntil(() => IdleTrigger); 
        IdleTrigger = false;
        bossCols.ForEach(x => x.enabled = true); //aktiviere Boss-Collider
    }
    //Attention: Toggle hand to invisible at the end of every move! 
    IEnumerator UpwardsSwordBarrage() //Erste Attacke, der Boss teleportiert in Raummitte senkt seine Hände und stösst sie anschliessend in die Luft während Klingen aus dem Boden schiessen
    {
        var moveChildren = new List<GameObject> //Children, die für diese Attacke relevant sind
        {
            hand1, hand2
        };
        yield return StartCoroutine(TeleportTo(uSBTransforms.Teleport.position)); //Starte TeleportTo() und warte bis Ende
        StartCoroutine(Flash(bossParams.FlashDuration));
        uSBTransforms.Write(); //Setze Children auf im Editor definierte Startpositionen
        Vector2 bossPos = new(transform.position.x, transform.position.y); //Boss-Position
        Vector2 centerleft = bossPos + bossParams.USB_HandRotationCenter; //Berechnen von Zentren für Rotation der Hände (Hände bewegen sich während Attacke)
        Vector2 centerright = bossPos + new Vector2(-1, 1) * bossParams.USB_HandRotationCenter; 
        EnableRenderers(moveChildren); 
        yield return StartCoroutine(FadeInGOs(moveChildren, bossParams.HandsFadeDuration)); //Hände werden eingeblendet
        EnableColliders(moveChildren);
        bossPos = new(transform.position.x, transform.position.y); 
        Vector2 firstBladePos = new(bossPos.x + bossParams.USB_BladesStartPoint.x, bossParams.USB_BladesStartPoint.y); //Position der Klinge in linker Ecke der Arena berechnen
        Vector2 currentBladePos = firstBladePos; //Setze "Cursor" auf erste Position
        foreach (GameObject blade in GroundBlades) //für jede Klinge in der Liste wird:
        {
            //Blades must have disabled sprite renderer at the beginning            
            blade.transform.position = currentBladePos; //Position auf jetzige "Cursor"-Position gelegt
            blade.GetComponent<SpriteRenderer>().enabled = true; //Renderer aktiviert
            blade.GetComponent<Animator>().SetInteger("State", 1); //Telegraph (also Teil der noch keinen Schaden macht) der Klingenanimation (Barrage_Anticipate)
            currentBladePos += new Vector2(bossParams.USB_BladesSpacing, 0f); //Cursor wird für nächste Klinge um Spacing verschoben
        }
        StartCoroutine(SmoothRotateHand(hand1.transform, 40, centerleft, 0.7f)); //Hände werden abgesenkt
        yield return StartCoroutine(SmoothRotateHand(hand2.transform, -40, centerright, 0.7f)); 
        yield return new WaitForSeconds(bossParams.USB_BladesDelay);        
        foreach (GameObject blade in GroundBlades) //Animation wird nun auf Stoss (Barrage_Up) gesetzt
        {
            blade.GetComponent<Animator>().SetInteger("State", 2);
        }
        StartCoroutine(RotateHand(hand1.transform, -50, centerleft, 0.1f)); //Hände werden so rotiert, dass es aussieht, als heben sie die Klingen magisch aus dem Boden
        StartCoroutine(RotateHand(hand2.transform, 50, centerright, 0.1f));        
        yield return new WaitForSeconds(bossParams.USB_BladesActiveTime);
        foreach (GameObject blade in GroundBlades) //Klingen senken sich wieder ab (Barrage_Down)
        {
            blade.GetComponent<Animator>().SetInteger("State", 3);
        }
        yield return new WaitUntil(() => !GroundBlades.Exists((x) => !x.GetComponent<BladesEvent>().BladeAnimDone)); //Es wird gewartet, bis jede Klinge das entsprechend Animationsevent auslöst
        currentBladePos += Vector2.down * 50f; //Cursor wird unter Arena befördert
        foreach (GameObject blade in GroundBlades) 
        {
            blade.GetComponent<BladesEvent>().BladeAnimDone = false; //Reset des Animationsevent-Flags
            blade.GetComponent<SpriteRenderer>().enabled = false; //Renderer deaktiviert            
            blade.transform.position = new Vector3(blade.transform.position.x, currentBladePos.y, blade.transform.position.z); //Klingen werden in y auf Cursorposition gesetzt            
        }
        DisableColliders(moveChildren); //Hände deaktiviert (Collider, Renderer)
        yield return StartCoroutine(FadeOutGOs(moveChildren, bossParams.HandsFadeDuration));
        DisableRenderers(moveChildren);
        //animator.SetInteger("BossState", 2);
        yield return new WaitForSeconds(bossParams.USB_Downtime);
        if (!deathCoroutineStarted)
        {
            StartCoroutine(TransistionToNextMove(0.05f)); //Wechsel in nächste Attacke
        }
        
    }

    IEnumerator SideHover() //Zweite Attacke: Boss schwebt von einem Ende des Raums zum anderen. Dabei kreisen seine Hände um ihn. Der Spieler soll über die Hände springen können
    {
        var moveChildren = new List<GameObject>
        {
            hand1, hand2
        };
        sHTransforms.Write(); 
        Vector2 teleportDest;
        float dir;
        float randomNo = Random.Range(0, 2f); //Zufallszahl zur Wahl von Richtung
        int hoverDest; //index des Ziel-Transforms
        //Selektieren von Richtung anhand Zufallszahl
        if(randomNo < 1f)
        {
            teleportDest = SH_TeleportOptions[0].position;
            dir = 1f;
            hoverDest = 1;
        }
        else
        {
            teleportDest = SH_TeleportOptions[1].position;
            dir = -1f;
            hoverDest = 0;
        }

        yield return StartCoroutine(TeleportTo(teleportDest)); //Starte TeleportTo() und warte bis Ende
        //Setze Position der Hände 
        Vector2 localPos2 = new Vector2(bossParams.SH_Radius * Mathf.Cos(bossParams.SH_StartAngle * Mathf.PI/180f) * dir, bossParams.SH_Radius * Mathf.Sin(bossParams.SH_StartAngle * Mathf.PI / 180f));//Hand2                
        Vector2 localPos1 = new Vector2(-localPos2.x, -localPos2.y);//Hand1
        hand1.transform.localPosition = localPos1;
        hand2.transform.localPosition = localPos2;
        //Rotation der Hände        
        hand1.transform.eulerAngles = new Vector3(0, 0, hand1.transform.eulerAngles.z + bossParams.SH_StartAngle);
        hand2.transform.eulerAngles = new Vector3(0, 0, hand2.transform.eulerAngles.z + bossParams.SH_StartAngle);
        //Setze Richtung der Hände
        foreach(GameObject hand in moveChildren)
        {
            hand.transform.localScale = new Vector3(hand.transform.localScale.x, hand.transform.localScale.y * dir, hand.transform.localScale.z);
        }
        EnableRenderers(moveChildren);
        yield return StartCoroutine(FadeInGOs(moveChildren, bossParams.HandsFadeDuration));
        yield return new WaitForSeconds(bossParams.SH_MovementDelay);
        EnableColliders(moveChildren);        
        Transform rotationParent = transform.Find("SH_RotationParent"); //Finde das "SH_RotationParent"-Child des Bosses, weise es rotationParent zu
        foreach (GameObject hand in moveChildren)
        {
            hand.transform.parent = rotationParent; //konfiguriere rotationParent als Parent der Hände          
        }
        //Innerhalb der Zeit bossParams.SH_Duration bewegt sich nun der Boss zu dem Zielpunkt, wobei die Hände in dieser Zeit um SH_FullAngle rotieren
        //Um die Rotation immer sauber relativ zur Bewegung des Bosses zu machen, wurde den Händen ein Parent in der Mitte des Bosses zugewiesen, das nun rotiert   
        for (float time = 0; time < bossParams.SH_Duration; time += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(teleportDest, SH_TeleportOptions[hoverDest].position, time / bossParams.SH_Duration); //Lerp der Position
            rotationParent.Rotate(0f, 0f, bossParams.SH_FullAngle * dir / bossParams.SH_Duration * Time.deltaTime); //mit *Time.deltaTime/bossParams.SH_Duration als Faktor können die Schritte so gerechnet werden,
            //dass das Parent insgesamt um den gegebenen Winkel rotiert wird
            yield return null;
        }
        foreach (GameObject hand in moveChildren)
        {
            hand.transform.parent = transform; //Deparenting => jetzt wieder direkte Children von Boss-Object
        }
        yield return null;
        DisableColliders(moveChildren);
        yield return StartCoroutine(FadeOutGOs(moveChildren, bossParams.HandsFadeDuration));
        DisableRenderers(moveChildren);
        yield return new WaitForSeconds(bossParams.SH_Downtime);
        if (!deathCoroutineStarted)
        {
            StartCoroutine(TransistionToNextMove(0.05f)); //Wechsel zu nächster Attacke
        }
    }
    
    //****************************************************************************************************************************************************************************************  
    //Utilities

    //Diese Coroutine wählt zufällig die nächste Aktion aus und verhindert dieselbe Attacke zweimal nacheinander, leider gibt es momentan nur zwei Attacken, wodurch die Logik nicht relevant ist
    IEnumerator TransistionToNextMove(float downtime)
    {
        transistioning = true;
        //wait
        yield return new WaitForSeconds(downtime); //nur kurz, um Tod nicht zu stark zu Verzögern falls mitten in Transition
        
        //randomize
        float randomNo = Random.Range(0f, 10f); //Zufallszahl
        bool chosen = false; //Wenn Zufallszahl auf vorherige Attacke fällt, ist ein weiterer Durchlauf nötig 
        while (!chosen)
        {
            switch (randomNo)
            {
                case < 3f:
                    if (lastAnim != BossAnims.UpwardsSwordBarrage) //wenn nicht schon letzte Attacke
                    {
                        lastAnim = BossAnims.UpwardsSwordBarrage; //Setzt letzte Attacke auf die neu ausgewählte
                        currentAttack = StartCoroutine(UpwardsSwordBarrage()); //Starte Attacke
                        chosen = true; //Ausgangsbedingung
                    }
                    else { randomNo += 1f; } //Sonst Inkrementiere, bis eine andere Attacke gewählt würde
                    break;
                case < 9.5f:
                    if (lastAnim != BossAnims.SideHover) //"
                    {
                        lastAnim = BossAnims.SideHover;
                        currentAttack = StartCoroutine(SideHover());
                        chosen = true;
                    }
                    else { randomNo -= 1f; }
                    break;
                case <= 10f:
                    if (lastAnim != BossAnims.Attack3) //"
                    {
                        lastAnim = BossAnims.Attack3;
                        //chosen = true;
                    }
                    else { randomNo -= 1f; }
                    break;
            }
            yield return null;
        }
        transistioning = false;
    }

    public void Death()
    {
        List<GameObject> deactvivate = new()
        {            
            hand1,
            hand2,
            sword
        };
        DisableColliders(deactvivate); //Deaktiviere children
        bossCols.ForEach(x => x.enabled = false); //Deaktiviere Boss Collider
        var bladeCols = new List<Collider2D>();
        var allBladeCols = new List<Collider2D>();
        //Für jede Klinge in der Liste:
        GroundBlades.ForEach(x =>
        {
            x.GetComponents(bladeCols); //Finde beide Collider
            allBladeCols.AddRange(bladeCols); //Füge sie der Hauptliste hinzu
        });
        allBladeCols.ForEach(x => x.enabled = false); //Alle Klingen-Collider deaktiviert
        AddToBosses(); //Füge Boss der "besiegte Bosse"-Liste hinzu
        StartCoroutine(DeathHandler()); //Starte Todes-State
        deathCoroutineStarted = true;
    }

    private IEnumerator DeathHandler()
    {        
        yield return new WaitUntil(() => transistioning == false); //Warte bis in Attacken-State
        StopCoroutine(currentAttack); //Unterbreche die Attacke
        GroundBlades.ForEach(x => x.GetComponent<Light2D>().enabled = false); //Deaktiviere Licht bei Klingen
        var fade = new List<GameObject> { hand1, hand2, sword }; 
        fade.AddRange(GroundBlades); 
        StartCoroutine(FadeGOsInvisibleFromCurrent(fade, 0.2f)); //Fade von allen Children zu unsichtbarer Farbe        
        deathPs.Play(); //Spiele ParticleSystem ab, das den Zerfall des Bosses darstellt
        float duration = 1f; 
        for(float time = 0; time < duration; time += Time.deltaTime)
        {
            sr.color = Color.Lerp(baseColor, deathColor, time / duration); //der Boss färbt sich schwarz
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        animator.SetInteger("BossState", 3); //Todes-Animation (Frames von Teleport out in langsamer Abfolge)
        yield return new WaitUntil(() => DeathTrigger); //Warte auf Ende der Animation
        sr.enabled = false; //deaktiviere Renderer des Bosses
        yield return new WaitUntil(() => !deathPs.IsAlive()); //Warte bis alle Particles verschwinden
        GameObject[] fogwalls = GameObject.FindGameObjectsWithTag("FogWall"); //Finde Fogwalls (in unserem Raum natürlich nur eine)
        foreach (GameObject fogwall in fogwalls)
        {
            fogwall.GetComponent<FogWall>().Deactivate(); //Collider und Opacity
        }
        GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().ResetBossRoomCam(); //Reset der Kamera (zu Beginn des Kampfs mit Boss zusammen getriggert)
        Destroy(transform.parent.gameObject); //Zerstört Boss
    }

    //Rotate Methoden:

    //Um definierten Punkt im Raum
    IEnumerator RotateHand(Transform hand, float angle, Vector2 center, float duration)
    {
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            hand.RotateAround(center, Vector3.forward, angle * Time.deltaTime / duration); //Rotiert das Objekt schrittweise um das Zentrum
            yield return null;
        }
    }

    //Um das Transform.position eines GameObjects
    IEnumerator RotateHandMoving(Transform hand, float angle, Transform center, float duration) 
    {
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            hand.RotateAround(center.position, Vector3.forward, angle * Time.deltaTime / duration);
            yield return null;
        }
    }

    //Mit weicherer Bewegung (dafür wird nicht der exakte Winkel erreicht)
    IEnumerator SmoothRotateHand(Transform hand, float angle, Vector2 center, float duration)
    {
        float dropoffFactor;
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            dropoffFactor = duration - time;            
            hand.RotateAround(center, Vector3.forward, angle * Time.deltaTime / duration * dropoffFactor);
            yield return null;
        }
    }  

    //Fade Methoden:

    //Von unsichtbar zu opak
    IEnumerator FadeInGOs(List<GameObject> gameObjectsToFade, float duration)
    {
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            foreach (GameObject fadeObject in gameObjectsToFade)
            {
                fadeObject.GetComponent<SpriteRenderer>().color = Color.Lerp(invisible, visible, time / duration);
            }            
            yield return null;
        }
    }    

    //VOn opak zu unsichtbar
    IEnumerator FadeOutGOs(List<GameObject> gameObjectsToFade, float duration)
    {
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            foreach (GameObject fadeObject in gameObjectsToFade)
            {
                fadeObject.GetComponent<SpriteRenderer>().color = Color.Lerp(visible, invisible, time / duration);
            }
            yield return null;
        }
    }

    //Von jetziger Farbe unsichtbar
    IEnumerator FadeGOsInvisibleFromCurrent(List<GameObject> gameObjectsToFade, float duration)
    {
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            foreach (GameObject fadeObject in gameObjectsToFade)
            {
                fadeObject.GetComponent<SpriteRenderer>().color = Color.Lerp(fadeObject.GetComponent<SpriteRenderer>().color, invisible, time / duration);
            }
            yield return null;
        }
    }

    //Lässt Light2D aufleuchten und dann direkt wieder abdunkeln, gelöst mit Sinusfunktion 0-PI
    IEnumerator Flash(float duration)
    {
        for(float time = 0; time < duration; time += Time.deltaTime)
        {
            foreach(Light2D light in flashLights)
            {
                light.intensity = Mathf.Sin(time / duration * Mathf.PI);
            }
            yield return null;
        }
    }

    //Wird bei Treffer ausgelöst, sorgt für Farbänderung in Update
    public void IncrementColorStatus() //Sets Lerp Goal => to hit color
    {
        hitColorStatus += 1f;
    }

    //Fügt den Boss der SpawningDataHandler-Liste hinzu, verhindert, dass der Boss bei neuer Erstellung des Scene spawnt
    public void AddToBosses()
    {
        SpawnItem thisitem = GameObject.FindWithTag("Spawner").GetComponent<LevelSpawnManager>().spawnItems.Find(x => x.itemName == transform.parent.name);
        SpawningDataHandler.Instance.killedBossEnemies.Add(thisitem.itemName);
    }
}
