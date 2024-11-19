using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Ui Manager für das Hauptmenu
public class OptionsMenuUIManager : MonoBehaviour
{
    public UIDocument optionsMenuUI;
    private Label _continueLabel;
    private Label _controlsLabel;
    private Label _settingsLabel;
    private Label _quitLabel;

    private MyCharacterCtrl _player;
    IEnumerator Start()
    {
        optionsMenuUI = GetComponent<UIDocument>();
        yield return StartCoroutine(GOExtensions.WaitForComponentOnGOWithTag<MyCharacterCtrl>("Player"));
        _player = GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>();
        _continueLabel = optionsMenuUI.rootVisualElement.Q<Label>("continue"); //Finden der Elemente für Buttons: Continue, Controls...
        _controlsLabel = optionsMenuUI.rootVisualElement.Q<Label>("controls");
        _settingsLabel = optionsMenuUI.rootVisualElement.Q<Label>("options");
        _quitLabel = optionsMenuUI.rootVisualElement.Q<Label>("quit");

        _continueLabel.RegisterCallback<MouseDownEvent>(ContinueGame); //Registrieren der Button-Events
        _controlsLabel.RegisterCallback<MouseDownEvent>(OpenControls);
        _settingsLabel.RegisterCallback<MouseDownEvent>(OpenSettings);
        _quitLabel.RegisterCallback<MouseDownEvent>(QuitToMain);

        optionsMenuUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    //Button-Callback-Methoden, welche die entsprechenden Methoden im Player auslösen um Fenster zu öffnen usw.
    private void ContinueGame(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            Debug.Log((evt.target as Label).text);
            _player.CloseMainMenuByContinue();
        }

    }
    
    private void OpenSettings(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            Debug.Log((evt.target as Label).text);
            _player.OpenSettings();
        }

    }

    private void OpenControls(MouseDownEvent evt) 
    {
        if (evt.button == 0)
        {
            Debug.Log((evt.target as Label).text);
            _player.OpenControls();
        }

    }

    private void QuitToMain(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            StartCoroutine(QuitHandler());              
        }
    }

    //Speichert Fortschritt und löst im Loading Manager die ToStartMenu()-Coroutine aus
    private IEnumerator QuitHandler()
    {
        if (SaveMode.Saving)
        {
            yield return StartCoroutine(SaveHandler.Instance.Save()); 
        }
            
        yield return StartCoroutine(LoadingManagerAdditive.loadingManager.ToStartMenu());      
    }


}
