using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    //Methode für Hinzufügen von Element eines eigenen Template-Types zu einem VisualElement oder einer ScrollList
    public static void AddToElementOfType<ViewType, ElementType, DataSourceType>(UIDocument destinationDoc, VisualTreeAsset template, string destinationName, DataSourceType editsource, Action<ElementType, DataSourceType> editElement) where ViewType : VisualElement where ElementType : VisualElement
    {
        var templateContainer = template.Instantiate();
        var templateElement = templateContainer.Q<ElementType>();

        var view = destinationDoc.rootVisualElement.Q<ViewType>(destinationName);
        view.Add(templateElement);

        editElement(templateElement, editsource);
    }

    //Abkürzung für Einblenden eines UI Fensters
    public static void OpenScreenWithTag(string tag)
    {
        GameObject.FindWithTag(tag).GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }
    //für Ausblenden
    public static void CloseScreenWithTag(string tag)
    {
        GameObject.FindWithTag(tag).GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }
}
