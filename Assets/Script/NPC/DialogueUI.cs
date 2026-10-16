using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject choices;
    [SerializeField] private GameObject interactionPrompt;

    private void Awake()
    {
        CloseDialogue();
        HideInteractionPrompt();
    }

    public void OpenDialogue()
    {
        dialogueBox.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public void CloseDialogue()
    {
        dialogueBox.SetActive(false);

        if (choices != null)
            choices.SetActive(false);
    }

    public void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}