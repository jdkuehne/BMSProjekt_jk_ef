using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Der Troll ist unser Spin auf das "Door cannot open from this side" der Troll hat auf dem Rücken Steine und ein ParryBox.cs Objekt, er kann nur von vorne getroffen werden => dient nach Plattforming als Abkürzung
public class Troll : MonoBehaviour
{
    [SerializeField] GameObject deadTroll;
    private float time;
    private float hitColorStatus;
    private float currentColorStatus;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color baseColor;
    [SerializeField] private float durationUntilHitColor;
    private SpriteRenderer sr;
    
    public void DestroyTroll()
    {
        GameObject deadtrollinst = Instantiate(deadTroll, transform.parent/*transform.position, transform.rotation*/); //Erstellt Troll ohne Collider usw. (nur Bild)
        //deadtrollinst.transform.localScale = transform.localScale;
        deadtrollinst.GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(/*-transform.localScale.x*/-transform.parent.localScale.x * 5f, 6f), 
        new Vector2(deadtrollinst.transform.position.x, deadtrollinst.transform.position.y) + new Vector2(transform.parent.localScale.x * 1.5f, -0.2f), ForceMode2D.Impulse); //Impuls an dieses Objekt => Troll fliegt aus dem Bildbereich
        Destroy(gameObject);
    }
    public void AddToBosses() //wird momentan auch zu Bossen hinzugefügt, um respawn zu verhindern
    {
        SpawnItem thisitem = GameObject.FindWithTag("Spawner").GetComponent<LevelSpawnManager>().spawnItems.Find(x => x.itemName == transform.name);
        SpawningDataHandler.Instance.killedBossEnemies.Add(thisitem.itemName);
    }

    private void Start() 
    {
        sr = GetComponent<SpriteRenderer>();
        time = 0;        
    }

    private void Update() //Farbänderung bei Treffer
    {
        if(hitColorStatus > 0.5f)
        {
            time += Time.deltaTime;
            if(time > durationUntilHitColor)
            {
                hitColorStatus -= 1f;
                time = 0;
            }
        }
        currentColorStatus += (hitColorStatus - currentColorStatus) * 3f * Time.deltaTime;
        sr.color = Color.Lerp(baseColor, hitColor, currentColorStatus);
    }

    public void IncrementColorStatus() //Sets Lerp Goal => to hit color
    {
        hitColorStatus += 1f;        
    }
}
