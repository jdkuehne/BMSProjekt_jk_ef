using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Klasse, welche für das Laden der NeverDestroyed-Scene in Levels zuständig ist
public class Initializer : MonoBehaviour
{
    public static Initializer InitInstance;
    public string FirstSceneName;
    [SerializeField] private string _persistentSceneName;    
    public Transform StartPoint;
    public bool NeverDestroyLoaded = false;

    private void Awake() //Unity-Singleton-Erstellung => nur eine Instanz, Start wird nur einmal ausgeführt
    {
        if (InitInstance != null)
        {
            Destroy(gameObject);
            return;
        }
        InitInstance = this;
        DontDestroyOnLoad(gameObject);
                             
    }

    private IEnumerator Start()
    {
        if (!InitInstance.NeverDestroyLoaded)
        {
            FirstSceneName = SceneManager.GetActiveScene().name;
            yield return null;
            AsyncOperation async = SceneManager.LoadSceneAsync(_persistentSceneName, LoadSceneMode.Additive); //Laden der Neverdestroyed-Scene           
            yield return new WaitUntil(() => async.isDone);
            if (SaveHandler.Instance.theSave.firstSessionOnThisSave) //Setzt abhängig davon ob es die erste Session mit einem Speicherstand ist diverse Werte
            {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<Collider2D>().enabled = false;
                player.transform.position = StartPoint.position;
                player.GetComponent<Collider2D>().enabled = true;
                SaveHandler.Instance.theSave.StartShrinePosition = (StartPoint.position.x, StartPoint.position.y, StartPoint.position.z);                
            }
            else
            {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<Collider2D>().enabled = false;
                player.transform.position = new Vector3(SaveHandler.Instance.theSave.StartShrinePosition.x, SaveHandler.Instance.theSave.StartShrinePosition.y, SaveHandler.Instance.theSave.StartShrinePosition.z);             
                player.GetComponent<Collider2D>().enabled = true;
                StartPoint.position = new Vector3(SaveHandler.Instance.theSave.StartShrinePosition.x, SaveHandler.Instance.theSave.StartShrinePosition.y, SaveHandler.Instance.theSave.StartShrinePosition.z);
            }
            ShrineHandler.shrinehandler.ShrineSceneName = FirstSceneName;
            LoadingManagerAdditive.loadingManager.CurrentSceneName = FirstSceneName;
            ShrineHandler.shrinehandler.ShrineSpawnPosition = StartPoint.position;
            ShrineHandler.shrinehandler.LanternSpawnPosition = StartPoint.position;
            GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().SetBordersToLevel();            
            InitInstance.NeverDestroyLoaded = true;            
        }
    }
}
