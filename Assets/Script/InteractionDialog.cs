using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractionDialog : MonoBehaviour
{
    public enum Character { Player, NPC };

    [System.Serializable]
    public class DialogData
    {
        public Character character;
        public string name;
        [TextArea(5, 6)]
        public string dialog;
    }

    [Header("Player Name")]
    public string PlayerName;

    [Header("Active Dialog")]
    [TextArea(5, 3)]
    public string activeDialog;
    public bool typewriting;
    public float delay;

    [Header("Visual Setting")]
    public GameObject ParentObject;
    public Image dialogPanel;
    public Image playerImage;
    public Image npcImage;
    public Text nameText;
    public Text dialogText;

    [Header("Dialog Setting")]
    public bool AutoStartDialog;
    public bool AutoFinishDialog;
    public List<DialogData> playerDialogs = new List<DialogData>();

    [Header("Event Dialog Setting")]
    public UnityEvent StartDialogEvent;
    public UnityEvent FinishDialogEvent;

    string currentText = "";
    private int currentDialogIndex = 0;
    private Coroutine DialogCoroutine;

    public void StartDialog()
    {
        currentDialogIndex = 0;
        ActivateDialog(playerDialogs[currentDialogIndex]);
        StartDialogEvent?.Invoke();
    }

    void Start()
    {
        if (AutoStartDialog) StartDialog();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return))
        {
            NextDialog();
        }
    }

    void NextDialog()
    {
        if (typewriting && DialogCoroutine != null)
        {
            StopCoroutine(DialogCoroutine);
        }
        currentDialogIndex++;
        currentText = "";
        dialogText.text = "";

        if (currentDialogIndex < playerDialogs.Count)
        {
            ActivateDialog(playerDialogs[currentDialogIndex]);
        }
        else
        {
            // Dialogs end
            // Perform any necessary actions or close the dialog window
            if (AutoFinishDialog)
            {
                SetChildStatus(ParentObject, false);
            }
            FinishDialogEvent?.Invoke();
        }
    }

    void ActivateDialog(DialogData dialogData)
    {
        string player = PlayerName;
        SetChildStatus(ParentObject, true);
        if (dialogData.character == Character.Player)
        {
            playerImage.gameObject.SetActive(true);
            npcImage.gameObject.SetActive(false);
        }
        else if (dialogData.character == Character.NPC)
        {
            playerImage.gameObject.SetActive(false);
            npcImage.gameObject.SetActive(true);
        }

        nameText.text = dialogData.name;
        if (dialogData.name == "<name>")
        {
            nameText.text = player;
        }

        //-- transfer dialog
        activeDialog = dialogData.dialog;
        string editedString = activeDialog;
        if (activeDialog.Contains("<name>"))
        {
            editedString = EditString(activeDialog, "<name>", player);
        }
        if (typewriting)
        {
            DialogCoroutine = StartCoroutine(TypeText(editedString));
        }
        else
        {
            dialogText.text = editedString;
        }
    }

    IEnumerator TypeText(string fullString)
    {
        for (int i = 0; i < fullString.Length; i++)
        {
            currentText += fullString[i];
            dialogText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }

    string EditString(string originalString, string targetWord, string replacementWord)
    {
        string[] words = originalString.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == targetWord)
            {
                words[i] = replacementWord;
                break;
            }
        }

        return string.Join(" ", words);
    }

    public void SetChildStatus(GameObject parentObject, bool aValue)
    {
        // Mendapatkan semua komponen Transform dari anak-anak (children) objek
        Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>(true);

        // Melakukan iterasi untuk menonaktifkan semua objek anak
        foreach (Transform childTransform in childTransforms)
        {
            // Pastikan objek tersebut bukan parentObject itu sendiri
            if (childTransform.gameObject != parentObject)
            {
                childTransform.gameObject.SetActive(aValue);
            }
        }
    }
}
