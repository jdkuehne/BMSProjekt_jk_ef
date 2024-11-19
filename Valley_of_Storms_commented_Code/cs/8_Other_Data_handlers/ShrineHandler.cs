using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoadIndicators.LoadOption;

//Klasse zum Speichern von Informationen bezüglich Checkpoint-Laden
public class ShrineHandler : MonoBehaviour
{
    public static ShrineHandler shrinehandler;
    public Vector3 ShrineSpawnPosition;
    public string ShrineSceneName;
    public Vector3 LanternSpawnPosition;
    public bool TriggerLoadEvent;

    [SerializeField] private CanvasGroup _trapSpawnScreen;


    private void Awake()
    {
        shrinehandler = this;        
    }

    public void CheckPointLoad()
    {
        LoadIndicators.LastLoadOption = Checkpoint.ToString();
        StartCoroutine(LoadingManagerAdditive.loadingManager.StartLoadAdditive(ShrineSceneName));
    }

    public void StartLaternRespawn()
    {
        StartCoroutine(LanternRespawn());
    }

    public IEnumerator LanternRespawn()
    {
        yield return TrapKillScreen(1f, 0.1f, 0f);
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Collider2D>().enabled = false;
        player.transform.position = LanternSpawnPosition;
        player.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        yield return TrapKillScreen(0f, 0.1f, 0f);
    }

    public IEnumerator TrapKillScreen(float targetValue, float duration, float delay) //Coroutine, die bei Teleport wegen Treffer von Falle, einen schwarzen Screen einblendet (Lerp von CanvasGroup.alpha => Transparenz)
    {

        float startValue = _trapSpawnScreen.alpha;
        float time = 0;

        yield return new WaitForSeconds(delay);

        while (time < duration)
        {
            _trapSpawnScreen.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _trapSpawnScreen.alpha = targetValue;
        //loadingScreenCanvas.SetActive(false);

    }
}

/*Goal (lanterns) we need:
 * a GameObject that deals damage with OnTriggerEnter and does this with "bool trap = true"
 * This causes trapevent, which need a method to call probably on this script, so on contact with a "lantern-trigger" the position is set, also it is set: by init, by shrine =>
 * new Killscreen which can solely be activated by a Trap
 */

