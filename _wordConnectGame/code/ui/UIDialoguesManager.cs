using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class UIDialoguesManager : MonoBehaviour
{
    public static UIDialoguesManager Instance;

    public List<UIDialogueElement> menuHud = new List<UIDialogueElement>();
    public List<UIDialogueElement> gameHud = new List<UIDialogueElement>();

    public List<UIDialogueElement> uIDialogues = new List<UIDialogueElement>();


    private void Awake()
    {
        Instance = this;
    }

    public void ToggleShowMenuHUD(bool show)
    {
        for (int i = 0; i < menuHud.Count; i++)
        {
            if (show)
            {
                menuHud[i].Show();
            }
            else
            {
                menuHud[i].Hide();
            }
        }

        print("ToggleShowMenuHUD called, show: " + show);
    }

    public void ToggleShowGameHUD(bool show)
    {
        for (int i = 0; i < gameHud.Count; i++)
        {
            if (show)
            {
                gameHud[i].Show();
            }
            else
            {
                gameHud[i].Hide();
            }
        }
    }


    [Button("Assign UIDialoges")]
    public void GetSceneUIElements()
    {
        uIDialogues.Clear();

        //foreach (UIDialogueElement element in FindFirstObjectByType<UIDialogueElement>())
        //{
        //    uIDialogues.Add(element);
        //}
    }



    public UIDialogueElement GetUIDialogue(string id)
    {
        for (int i = 0; i < uIDialogues.Count; i++)
        {
            if (uIDialogues[i].dialogueID == id)
            {
                return uIDialogues[i];
            }
        }

        return null;
    }

}
