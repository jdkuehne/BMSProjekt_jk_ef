using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Klasse für das Laden von Scenes
public class LoadingManagerAdditive : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht Anhängen an GameObject
{
    public static LoadingManagerAdditive loadingManager; //statisches Objekt für einfacheren Zugriff
    public string CurrentSceneName; //benötigt für schreiben der Checkpoint-Scene (Shrine.cs) und Unloading der aktiven Scene

    //Events für vorbereitende & abschliessende Anweisungen bei Laden
    public static event Action OnSceneLoadStart;
    public static event Action OnSceneLoadComplete;

    public GameObject loadingScreenCanvas; //GameObject mit Ladebilschirm
    public CanvasGroup LoadingCanvasGroup; //Canvas Group Komponente für Verändern von Transparenz (Canvas UI)

    bool gameStarted; //nicht relevant, wäre wichtig, wenn das Script nicht in der NeverDestroyed-Scene wäre

    private void Awake()
    {
        loadingManager = this; //Diese Instanz der statischen zuordnen
    }

    IEnumerator Start()
    {
        if(!gameStarted)
        {
            yield return null;
            CurrentSceneName = Initializer.InitInstance.FirstSceneName; //Setzen der aktiven Scene auf die erste geladene Scene der Session
            gameStarted = true;
        }
        yield return null;
    }

    //Aufgerufen, wenn der "To Main Menu"-Button im Hauptmenu gedrückt wird, setzt statische Instanzen auf null, um bei direktem Weiterspielen keine falschen/doppelten Referenzen zu erhalten,
    //Ladet die Startmenu-Scene im "Single"-Modus, wodurch "NeverDestroyed" und der Level zerstört werden.
    public IEnumerator ToStartMenu()
    {
        Initializer.InitInstance.NeverDestroyLoaded = false;
        Destroy(GameObject.FindWithTag("Initializer"));
        Initializer.InitInstance = null;        
        
        Debug.Log("Continues after load");
        OnSceneLoadStart = null;
        OnSceneLoadComplete = null;        
        LoadingScreen.Instance = null;
        ShrineHandler.shrinehandler = null;
        SpawningDataHandler.Instance = null;
        LoadIndicators.LastLoadOption = LoadIndicators.LoadOption.WaitingForLoad.ToString();
        AsyncOperation load = SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Single); //Laden der Startmenu-Scene
        while (!load.isDone)
        {
            yield return null;
        }        
    }

    //Methode zum Laden der Kammern
    public IEnumerator StartLoadAdditive(string sceneName) 
    {
        GameObject.FindWithTag("Player").GetComponent<Collider2D>().enabled = false; //deaktivieren des Spieler-Colliders um in der nächsten Scene unerwünschtes Auslösen von Trigger usw. zu vermeiden          
        yield return StartCoroutine(FadeLoadingScreen(1, 0.15f, 0f)); //Ladebildschirm einblenden
        OnSceneLoadStart?.Invoke(); //Auslösen des Ladestart-Events
        yield return null;
        SceneManager.UnloadSceneAsync(CurrentSceneName); //Unloading der aktiven Scene
        StartCoroutine(LoadAdditiveStage2(sceneName)); //zu LoadAdditiveStage2
    }    

    private IEnumerator LoadAdditiveStage2(string sceneName2)
    {
        yield return new WaitForSeconds(0.1f); 
        AsyncOperation loadasync = SceneManager.LoadSceneAsync(sceneName2, LoadSceneMode.Additive); //Laden des neuen Level
        loadasync.allowSceneActivation = false; //direkte Aktivierung der Scene nicht zulassen
        while (!loadasync.isDone) //warten bis erstellt
        {
            if (loadasync.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
        loadasync.allowSceneActivation = true; /*Scene aktivieren*/ yield return null; Debug.Log("Even reached the end");
        CurrentSceneName = sceneName2; //Überschreiben des Namen von aktiver Scene
        yield return new WaitUntil(() => ShrineHandler.shrinehandler.TriggerLoadEvent); //Warten bis ein bestimmtes GameObject sicher erstellt ist
        yield return new WaitForSeconds(0.1f);
        
        StartCoroutine(FadeLoadingScreen(0, 0.2f, 0.1f)); //Ausblenden des Ladebildschirms     
        Debug.Log("Got to Invoke");
        yield return null;
        OnSceneLoadComplete?.Invoke(); //Auslösen des Complete-Events
        ShrineHandler.shrinehandler.TriggerLoadEvent = false;
    }

    //Methode für Lerp von CanvasGroup-opacity
    public IEnumerator FadeLoadingScreen(float targetValue, float duration, float delay)
    {
        float startValue = LoadingCanvasGroup.alpha;
        float time = 0;

        yield return new WaitForSeconds(delay);

        while (time < duration)
        {
            LoadingCanvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        LoadingCanvasGroup.alpha = targetValue;
        //loadingScreenCanvas.SetActive(false);
    }
}
