using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueNode
{
    public string nodeID; // ID ของประโยคนี้ (เช่น "intro_1")
    [TextArea(3, 10)] public string text; // ข้อความที่ Flowey พูด
    public string speakerName = "Flowey";
    public List<DialogueChoice> choices; // ทางเลือกของผู้เล่น
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // ข้อความในปุ่ม (เช่น "เธอเป็นใคร?")
    public string nextNodeID; // พอกดแล้วไป Node ไหนต่อ
    public bool isQuestionToTrack; // ติ๊กถูกถ้าข้อนี้ต้องถามให้ครบเพื่อปลดล็อคเนื้อเรื่อง
    public bool requiresAllQuestionsAsked; // ติ๊กถูกถ้าข้อนี้จะโผล่ก็ต่อเมื่อถามครบแล้วเท่านั้น
}