using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // สำคัญมากสำหรับเชื่อม Fungus

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Status")]
    public int currentPhase = 0;
    public float timer = 0f;
    public bool isTimerRunning = false;
    public int burnedCount = 0;
    public int requiredCount = 0;

    [Header("Environment Groups")]
    public GameObject[] phase1Flowers; // ทานตะวัน
    public GameObject[] phase2Flowers; // ทิวลิปม่วง
    public GameObject[] phase3Flowers; // กุหลาบ
    public GameObject[] phase4Flowers; // ฮิกันบานะ
    // ลากกลุ่ม GameObject ของแต่ละ Phase มาใส่เพื่อสั่ง เปิด/ปิด

    [Header("UI")]
    public Text timerText;
    public Text objectiveText;

    [Header("Fungus Events (Liaison)")]
    // ลาก Flowchart Block มาใส่ในช่องพวกนี้ที่ Inspector
    public UnityEvent onPhase1Complete; 
    public UnityEvent onPhase2Complete;
    public UnityEvent onPhase3Complete;
    public UnityEvent onPhase4Complete; 
    public UnityEvent onGameOver;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            UpdateUI();

            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                onGameOver.Invoke(); // สั่งจบเกม (Game Over)
            }
        }
    }

    void UpdateUI()
    {
        if(timerText) timerText.text = Mathf.Ceil(timer).ToString();
        if(objectiveText) objectiveText.text = $"{burnedCount} / {requiredCount}";
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก InteractableFlower
    public void OnFlowerBurned(int flowerPhase)
    {
        // เช็คว่าเผาถูก Phase หรือไม่
        if (flowerPhase == currentPhase)
        {
            burnedCount++;
            UpdateUI();

            if (burnedCount >= requiredCount)
            {
                CompleteCurrentPhase();
            }
        }
    }

    void CompleteCurrentPhase()
    {
        isTimerRunning = false; // หยุดเวลา
        
        // สั่ง Fungus ให้เล่น Cutscene ตาม Phase
        switch (currentPhase)
        {
            case 1: onPhase1Complete.Invoke(); break;
            case 2: onPhase2Complete.Invoke(); break;
            case 3: onPhase3Complete.Invoke(); break;
            case 4: onPhase4Complete.Invoke(); break; // จบเกม (Win)
        }
    }

    // --- ฟังก์ชันสำหรับให้ Fungus เรียกใช้ (Invoke Method) ---

    // เรียกเมื่อเริ่มเกม หรือเริ่ม Phase ใหม่
    public void StartPhase(int phase)
    {
        currentPhase = phase;
        burnedCount = 0;
        
        // Reset Map (ปิดทั้งหมดก่อน)
        SetObjects(phase1Flowers, false);
        SetObjects(phase2Flowers, false);
        SetObjects(phase3Flowers, false);
        SetObjects(phase4Flowers, false);

        // Setup แต่ละ Phase
        switch (phase)
        {
            case 1: // ทานตะวัน 3 ดอก
                requiredCount = 3;
                timer = 60f;
                SetObjects(phase1Flowers, true);
                break;
            case 2: // ทิวลิป 4 ดอก (มืด)
                requiredCount = 4;
                timer = 90f;
                SetObjects(phase2Flowers, true);
                // TODO: ใส่ Code เปลี่ยนแสงเป็นมืดตรงนี้
                break;
            case 3: // กุหลาบ 4 ดอก (สลับมั่ว)
                requiredCount = 4;
                timer = 120f;
                SetObjects(phase3Flowers, true);
                break;
            case 4: // ฮิกันบานะ 1 ดอก (ชั้น 1)
                requiredCount = 1;
                timer = 180f;
                SetObjects(phase4Flowers, true);
                // TODO: ใส่ Code ปิดบันได/ไฟไหม้
                break;
        }

        isTimerRunning = true;
        UpdateUI();
    }

    void SetObjects(GameObject[] objs, bool active)
    {
        foreach (var o in objs) if(o) o.SetActive(active);
    }
}