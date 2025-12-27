using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel; // Panel ใหญ่ของบทพูด
    public Text speakerNameText;
    public Text dialogueText;
    public Transform choiceContainer; // ที่วางปุ่ม Choice
    public GameObject choiceButtonPrefab; // Prefab ปุ่มกด

    [Header("Game State Reference")]
    public GameManager gameManager; // เชื่อมกับ GameManager ตัวเดิม
    public GameObject floweyModel;  // โมเดล Flowey (ไว้หันหน้าหาหรือเล่น Anim)

    [Header("Dialogue Data")]
    public List<DialogueNode> allNodes; // ใส่ข้อมูลบทพูดทั้งหมดใน Inspector ตรงนี้เลย (ง่ายและเร็วสุด)
    
    private DialogueNode currentNode;
    private HashSet<string> askedQuestions = new HashSet<string>(); // เก็บรายการคำถามที่ถามไปแล้ว
    private int totalRequiredQuestions = 0; // จำนวนคำถามสำคัญทั้งหมด

    void Start()
    {
        dialoguePanel.SetActive(false);
        // นับจำนวนคำถามที่ "จำเป็น" ทั้งหมดเตรียมไว้
        foreach (var node in allNodes)
        {
            foreach (var choice in node.choices)
            {
                if (choice.isQuestionToTrack) totalRequiredQuestions++;
            }
        }
    }

    public void StartDialogue(string startNodeID)
    {
        dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // ปล่อยเมาส์ให้กดได้
        Cursor.visible = true;
        
        // หยุดเดิน
        GameObject.FindWithTag("Player").GetComponent<HorrorFPSController>().enabled = false;

        ShowNode(startNodeID);
    }

    void ShowNode(string nodeID)
    {
        currentNode = allNodes.Find(n => n.nodeID == nodeID);
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        speakerNameText.text = currentNode.speakerName;
        dialogueText.text = currentNode.text;

        // ล้างปุ่มเก่า
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        // สร้างปุ่มใหม่
        foreach (var choice in currentNode.choices)
        {
            // เช็คเงื่อนไข: ถ้าเป็นปุ่ม "ไปต่อ" (requiresAllQuestionsAsked) จะโผล่เมื่อถามครบแล้วเท่านั้น
            if (choice.requiresAllQuestionsAsked && askedQuestions.Count < totalRequiredQuestions)
            {
                continue; // ข้าม ไม่แสดงปุ่มนี้
            }

            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<Text>().text = choice.choiceText;
            btn.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(DialogueChoice choice)
    {
        // ถ้าเป็นคำถามสำคัญ ให้บันทึกว่าถามแล้ว
        if (choice.isQuestionToTrack)
        {
            if (!askedQuestions.Contains(choice.choiceText)) // เช็คจาก Text หรือ ID ก็ได้
            {
                askedQuestions.Add(choice.choiceText);
                Debug.Log($"Asked {askedQuestions.Count}/{totalRequiredQuestions}");
            }
        }

        // ถ้า Choice บอกให้จบการคุย (เช่น nextNodeID = "EXIT")
        if (choice.nextNodeID == "EXIT")
        {
            EndDialogue();
        }
        else if (choice.nextNodeID == "START_FIGHT")
        {
            EndDialogue();
            // เริ่ม Boss Fight
            gameManager.StartBossFight(); // ต้องไปเพิ่ม function นี้ใน GameManager
        }
        else
        {
            ShowNode(choice.nextNodeID);
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameObject.FindWithTag("Player").GetComponent<HorrorFPSController>().enabled = true;
    }
}