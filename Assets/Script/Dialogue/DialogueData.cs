using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;

    [Tooltip("Index của câu thoại sẽ đi tới sau khi chọn.")]
    public int nextLineIndex;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;

    [Header("Choices")]
    public DialogueChoice[] choices;
}

[CreateAssetMenu(
    fileName = "NewDialogue",
    menuName = "Dialogue/Dialogue Data"
)]
public class DialogueData : ScriptableObject
{
    [Header("NPC Information")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("Dialogue")]
    public DialogueLine[] lines;
}