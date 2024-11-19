using UnityEngine;
using UnityEngine.SceneManagement;
using static LoadIndicators.LoadOption;

//Diese Klasse besitzt die Informationen, die für das Laden einer neuen Kammer notwendig sind und löst bei Kollision mit dem Spieler den Ladeprozess aus.
public class SceneGate : MonoBehaviour
{
    private GameObject _loader;
    [SerializeField] private string _nameOfSceneToLoad;
    [SerializeField] private LevelConnection _connectionOfThisGate;
    [SerializeField] private Transform _spawnSpot;
    [SerializeField] private int _spawnDirection;

    void Start()
    {
        if(LoadIndicators.LastLoadOption == Gate.ToString() && _connectionOfThisGate == LevelConnection.ActiveConnection)
        {
            GameObject.FindWithTag("Player").transform.position = _spawnSpot.position;
            GameObject.FindWithTag("Player").GetComponent<Collider2D>().enabled = true;
            GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().LastDirectionalInput = _spawnDirection;
            ShrineHandler.shrinehandler.LanternSpawnPosition = _spawnSpot.position;
            LoadIndicators.LastLoadOption = WaitingForLoad.ToString();            
        }        
    }    

    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //LoadIndicators.LastSceneLoadIndicator = LoadIndicators.LoadOption.ToGate.ToString();
            _loader = GameObject.FindWithTag("Loader");
            LoadIndicators.LastLoadOption = Gate.ToString();
            LevelConnection.ActiveConnection = _connectionOfThisGate;
            StartCoroutine(_loader.GetComponent<LoadingManagerAdditive>().StartLoadAdditive(_nameOfSceneToLoad)); //Ladestart
        }        
    }
}
