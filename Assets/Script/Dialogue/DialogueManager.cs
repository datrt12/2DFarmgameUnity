using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image npcPortrait;

    [Header("Continue")]
    [SerializeField] private Button continueButton;

    [Header("Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private Button[] choiceButtons;

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.03f;

    private static NPCInteraction nearbyNPC;

    private DialogueData currentDialogue;
    private NPCInteraction currentNPC;

    private int currentLineIndex;

    private bool isDialogueOpen;
    private bool isTyping;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        CloseDialogue();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        // =========================================
        // CHƯA CÓ HỘI THOẠI
        // =========================================

        if (!isDialogueOpen)
        {
            if (nearbyNPC != null)
                nearbyNPC.StartInteraction();

            return;
        }

        // =========================================
        // ĐANG GÕ CHỮ
        // =========================================

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // =========================================
        // ĐANG HIỆN CHOICE
        // =========================================

        if (HasChoices())
        {
            return;
        }

        // =========================================
        // KHÔNG CÓ CHOICE → SANG CÂU TIẾP
        // =========================================

        NextLine();
    }

    // =====================================================
    // NPC NEARBY
    // =====================================================

    public static void SetNearbyNPC(NPCInteraction npc)
    {
        nearbyNPC = npc;
    }

    public static void ClearNearbyNPC(NPCInteraction npc)
    {
        if (nearbyNPC == npc)
            nearbyNPC = null;
    }

    // =====================================================
    // START DIALOGUE
    // =====================================================

    public void StartDialogue(
        DialogueData dialogue,
        NPCInteraction npc)
    {
        if (dialogue == null)
        {
            Debug.LogWarning(
                "DialogueData chưa được gán!"
            );

            return;
        }

        if (dialogue.lines == null ||
            dialogue.lines.Length == 0)
        {
            Debug.LogWarning(
                "Dialogue này chưa có câu thoại!"
            );

            return;
        }

        currentDialogue = dialogue;
        currentNPC = npc;

        currentLineIndex = 0;
        isDialogueOpen = true;

        // NPC đứng im
        if (currentNPC != null)
        {
            currentNPC
                .GetComponent<NPCController>()
                ?.SetTalking(true);
        }

        dialogueBox.SetActive(true);

        npcNameText.text =
            dialogue.npcName;

        if (npcPortrait != null)
        {
            npcPortrait.sprite =
                dialogue.npcPortrait;
        }

        HideChoices();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        ShowCurrentLine();
    }

    // =====================================================
    // SHOW CURRENT LINE
    // =====================================================

    private void ShowCurrentLine()
    {
        if (currentDialogue == null)
            return;

        if (currentLineIndex < 0 ||
            currentLineIndex >= currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }

        // Khi sang câu mới → reset UI
        HideChoices();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        string line =
            currentDialogue
                .lines[currentLineIndex]
                .text;

        StartTyping(line);
    }

    // =====================================================
    // TYPEWRITER
    // =====================================================

    private void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine =
            StartCoroutine(
                TypeText(line)
            );
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        isTyping = false;
        typingCoroutine = null;

        // Sau khi gõ xong:
        // nếu có Choice → hiện Choice
        // nếu không → hiện Continue
        ShowButtonsAfterTyping();
    }

    // =====================================================
    // SAU KHI GÕ XONG
    // =====================================================

    private void ShowButtonsAfterTyping()
    {
        if (!isDialogueOpen ||
            currentDialogue == null)
            return;

        if (HasChoices())
        {
            ShowChoices();

            if (continueButton != null)
                continueButton.gameObject.SetActive(false);
        }
        else
        {
            HideChoices();

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
    }

    // =====================================================
    // CHECK CHOICE
    // =====================================================

    private bool HasChoices()
    {
        if (currentDialogue == null)
            return false;

        if (currentLineIndex < 0 ||
            currentLineIndex >= currentDialogue.lines.Length)
            return false;

        DialogueChoice[] choices =
            currentDialogue
                .lines[currentLineIndex]
                .choices;

        return choices != null &&
               choices.Length > 0;
    }

    // =====================================================
    // SHOW CHOICES
    // =====================================================

    private void ShowChoices()
    {
        if (choicesPanel == null)
            return;

        choicesPanel.SetActive(true);

        DialogueChoice[] choices =
            currentDialogue
                .lines[currentLineIndex]
                .choices;

        // Tắt toàn bộ button trước
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            choiceButtons[i].gameObject.SetActive(false);

            // Xóa event cũ
            choiceButtons[i].onClick.RemoveAllListeners();
        }

        // Hiện các Choice thực sự tồn tại
        for (int i = 0;
             i < choices.Length &&
             i < choiceButtons.Length;
             i++)
        {
            if (choiceButtons[i] == null)
                continue;

            int choiceIndex = i;

            choiceButtons[i].gameObject.SetActive(true);

            // Đổi text
            TMP_Text buttonText =
                choiceButtons[i]
                    .GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text =
                    choices[i].choiceText;
            }

            // Gắn sự kiện click
            choiceButtons[i]
                .onClick
                .AddListener(
                    () => SelectChoice(choiceIndex)
                );
        }
    }

    // =====================================================
    // HIDE CHOICES
    // =====================================================

    private void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        if (choiceButtons == null)
            return;

        for (int i = 0;
             i < choiceButtons.Length;
             i++)
        {
            if (choiceButtons[i] == null)
                continue;

            choiceButtons[i].gameObject.SetActive(false);

            choiceButtons[i]
                .onClick
                .RemoveAllListeners();
        }
    }

    // =====================================================
    // SELECT CHOICE
    // =====================================================

    private void SelectChoice(int choiceIndex)
    {
        if (!isDialogueOpen ||
            currentDialogue == null)
            return;

        if (isTyping)
            return;

        if (currentLineIndex < 0 ||
            currentLineIndex >= currentDialogue.lines.Length)
            return;

        DialogueChoice[] choices =
            currentDialogue
                .lines[currentLineIndex]
                .choices;

        if (choices == null ||
            choiceIndex < 0 ||
            choiceIndex >= choices.Length)
            return;

        int nextIndex =
            choices[choiceIndex]
                .nextLineIndex;

        // Kiểm tra index
        if (nextIndex < 0 ||
            nextIndex >= currentDialogue.lines.Length)
        {
            Debug.LogWarning(
                "Next Line Index không hợp lệ: "
                + nextIndex,
                this
            );

            return;
        }

        // Chuyển sang câu được chọn
        currentLineIndex = nextIndex;

        HideChoices();

        ShowCurrentLine();
    }

    // =====================================================
    // COMPLETE TYPING
    // =====================================================

    private void CompleteTyping()
    {
        if (!isTyping ||
            currentDialogue == null)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(
                typingCoroutine
            );

            typingCoroutine = null;
        }

        dialogueText.text =
            currentDialogue
                .lines[currentLineIndex]
                .text;

        isTyping = false;

        ShowButtonsAfterTyping();
    }

    // =====================================================
    // NEXT LINE
    // =====================================================

    private void NextLine()
    {
        if (!isDialogueOpen)
            return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (HasChoices())
            return;

        currentLineIndex++;

        if (currentLineIndex >=
            currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }

        ShowCurrentLine();
    }

    // =====================================================
    // CONTINUE BUTTON
    // =====================================================

    public void OnContinueButtonPressed()
    {
        if (!isDialogueOpen)
            return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (HasChoices())
            return;

        NextLine();
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void CloseDialogue()
    {
        isDialogueOpen = false;
        isTyping = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(
                typingCoroutine
            );

            typingCoroutine = null;
        }

        HideChoices();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        // Cho NPC đi lại
        if (currentNPC != null)
        {
            currentNPC
                .GetComponent<NPCController>()
                ?.SetTalking(false);
        }

        if (currentNPC != null)
        {
            currentNPC.EndInteraction();
        }

        currentDialogue = null;
        currentNPC = null;
        currentLineIndex = 0;
    }

    public bool IsDialogueOpen()
    {
        return isDialogueOpen;
    }
}