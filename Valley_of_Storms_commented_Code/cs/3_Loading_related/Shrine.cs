using UnityEngine;
using static LoadIndicators.LoadOption;

//Script das an die Schreine/Altare angefügt wird
public class Shrine : MonoBehaviour
{
    void Start()
    {
        //falls die Ladeoption "zu einem Checkpoint hin" ist, setzt der Schrein die Spielerposition auf seine eigene.
        if(LoadIndicators.LastLoadOption == Checkpoint.ToString()) 
        {
            GameObject.FindWithTag("Player").transform.position = ShrineHandler.shrinehandler.ShrineSpawnPosition;
            GameObject.FindWithTag("Player").GetComponent<Collider2D>().enabled = true;
            LoadIndicators.LastLoadOption = WaitingForLoad.ToString();
        }      
    }

    private void OnTriggerEnter2D(Collider2D other) //Bei Eintritt in den Trigger des Schreins:
    {
        if (other.gameObject.CompareTag("Player")) //Wenn das Objekt den Tag "Player" besitzt => der Spieler ist
        {            
            ShrineHandler.shrinehandler.ShrineSceneName = LoadingManagerAdditive.loadingManager.CurrentSceneName; //setze den Namen der Checkpoint Scene auf die jetzige
            SaveHandler.Instance.theSave.StartSceneName = ShrineHandler.shrinehandler.ShrineSceneName; //dasselbe für die Speicherdaten (JSON)
            ShrineHandler.shrinehandler.ShrineSpawnPosition = new Vector3(transform.position.x, transform.position.y, 0f); //setze die Spawn Position auf die Position dieses Schreins
            //Und schreibe diese Werte auch in das Speicherobjekt
            SaveHandler.Instance.theSave.StartShrinePosition = (ShrineHandler.shrinehandler.ShrineSpawnPosition.x,
                                                                ShrineHandler.shrinehandler.ShrineSpawnPosition.y,
                                                                ShrineHandler.shrinehandler.ShrineSpawnPosition.z);
            ShrineHandler.shrinehandler.LanternSpawnPosition = ShrineHandler.shrinehandler.ShrineSpawnPosition; //Schreine überschreiben die Laternen Checkpoints
            other.gameObject.GetComponent<MyCharacterCtrl>().CanPray = true;  //Die Aktion "Beten/Rasten" wird ausserdem freigeschaltet          
        }
    }

    //Bei Verlassen desselben Triggers:
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<MyCharacterCtrl>().CanPray = false; //wird die "Beten/Rasten"-Aktion wieder gesperrt
        }
    }
}
