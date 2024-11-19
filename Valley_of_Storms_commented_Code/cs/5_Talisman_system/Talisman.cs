using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using static MyCharacterCtrl;

[CreateAssetMenu(fileName = "TalismanData", menuName = "Talisman/Data")]

public class Talisman : IItem
{
    [Header("Popup Message")]
    [SerializeField] private Texture2D setImage; //diese Variablen gehören zu Properties
    [SerializeField] private string setName;
    [SerializeField] private int setNumber;
    [SerializeField] private ItemType setType;
    public override Texture2D Image { get => setImage; set => setImage = value; } //aus Interface, werden zum Schreiben der Popup-Messages benötigt
    public override string Name {  get => setName; set => setName = value; }
    public override int Number { get => setNumber; set => setNumber = value; }
    public override ItemType ItemType { get => setType; set => setType = value; }

    [Header("Gameplay/Handling")]
    public TalismanEffect effect; //Effekt des Talisman => abstract class
    public string TalismanName;

    [Header("UI Window")]
    public string TalismanWindowTitle; //Titel des Talisman-Infofensters
    public string ItemDescription; //Beschreibung des Talisman (in Selektor)
    public int SortNumber; //Sortiernummer
    
}
