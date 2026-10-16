using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData dialogueData;

    private bool playerInRange;
    private bool dialogueOpen;

    public void StartInteraction()
    {
        if (!playerInRange)
            return;

        if (dialogueOpen)
            return;

        if (dialogueManager == null)
        {
            Debug.LogWarning(
                "Dialogue Manager chưa được gán!",
                this
            );
            return;
        }

        if (dialogueData == null)
        {
            Debug.LogWarning(
                "Dialogue Data chưa được gán!",
                this
            );
            return;
        }

        dialogueOpen = true;

        dialogueManager.StartDialogue(
            dialogueData,
            this
        );

        if (dialogueUI != null)
            dialogueUI.HideInteractionPrompt();
    }

    public void EndInteraction()
    {
        dialogueOpen = false;

        if (dialogueUI != null && playerInRange)
            dialogueUI.ShowInteractionPrompt();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        DialogueManager.SetNearbyNPC(this);

        if (!dialogueOpen && dialogueUI != null)
            dialogueUI.ShowInteractionPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        DialogueManager.ClearNearbyNPC(this);

        if (dialogueUI != null)
            dialogueUI.HideInteractionPrompt();
    }
}