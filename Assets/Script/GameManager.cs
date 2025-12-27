using UnityEngine;
using UnityEngine.UI; // ถ้าใช้ Text Mesh Pro ให้เปลี่ยนเป็น using TMPro;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int currentPhase = 1;
    public float timeRemaining = 60f; // เวลาเริ่มต้น Phase 1
    public bool isGameOver = false;

    [Header("Objectives")]
    public int flowersBurnedInPhase = 0;
    public int flowersRequiredInPhase = 3;

    [Header("UI References")]
    public TextMeshProUGUI timerText;       // ลาก UI Text มาใส่
    public TextMeshProUGUI objectiveText;   // ลาก UI Text มาใส่ (เช่น "Burned: 0/3")
    public TextMeshProUGUI phaseText;       // ลาก UI Text มาใส่ (บอก Phase ปัจจุบัน)
    public GameObject gameOverPanel; // Panel ที่จะเด้งตอนแพ้
    public GameObject winPanel;      // Panel ที่จะเด้งตอนชนะ

    [Header("Environment Controls")]
    public GameObject[] phase1Objects; // ลาก Parent ของดอกไม้ Phase 1 มาใส่
    public GameObject[] phase2Objects;
    public GameObject[] phase3Objects;
    public GameObject[] phase4Objects;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPhase(1);
    }

    void Update()
    {
        if (isGameOver) return;

        // Timer Logic
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUI();
        }
        else
        {
            GameOver("Time Out!");
        }
    }

    void UpdateUI()
    {
        // ถ้าใช้ TextMeshPro ให้เปลี่ยน .text เป็นคำสั่งของ TMP
        if(timerText) timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
        if(objectiveText) objectiveText.text = "Flowers Burned: " + flowersBurnedInPhase + "/" + flowersRequiredInPhase;
        if(phaseText) phaseText.text = "Phase: " + currentPhase;
    }

    public void BurnFlower(int flowerPhase)
    {
        // เช็คว่าเผาถูก Phase หรือไม่
        if (flowerPhase == currentPhase)
        {
            flowersBurnedInPhase++;
            
            // เช็คว่าครบหรือยัง
            if (flowersBurnedInPhase >= flowersRequiredInPhase)
            {
                NextPhase();
            }
        }
    }

    void StartPhase(int phase)
    {
        currentPhase = phase;
        flowersBurnedInPhase = 0;

        // ตั้งค่าแต่ละ Phase
        switch (phase)
        {
            case 1:
                timeRemaining = 60f;
                flowersRequiredInPhase = 3;
                SetObjectsActive(phase1Objects, true);
                break;
            case 2:
                timeRemaining = 90f;
                flowersRequiredInPhase = 3; // หรือ 4 ตามโจทย์
                SetObjectsActive(phase1Objects, false); // ปิดของเก่า
                SetObjectsActive(phase2Objects, true);
                // TODO: ใส่ Code เปลี่ยนแสง/Post Processing ตรงนี้
                break;
            case 3:
                timeRemaining = 120f;
                flowersRequiredInPhase = 3;
                SetObjectsActive(phase2Objects, false);
                SetObjectsActive(phase3Objects, true);
                break;
            case 4:
                timeRemaining = 180f;
                flowersRequiredInPhase = 1; // Boss/Ending logic
                SetObjectsActive(phase3Objects, false);
                SetObjectsActive(phase4Objects, true);
                break;
            case 5:
                GameWin();
                break;
        }
    }

    void NextPhase()
    {
        StartPhase(currentPhase + 1);
    }

    void SetObjectsActive(GameObject[] objs, bool state)
    {
        foreach (var obj in objs)
        {
            if(obj != null) obj.SetActive(state);
        }
    }

    public void GameOver(string reason)
    {
        isGameOver = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(gameOverPanel) gameOverPanel.SetActive(true);
        Debug.Log("Game Over: " + reason);
    }

    public void GameWin()
    {
        isGameOver = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(winPanel) winPanel.SetActive(true);
    }
    
    // ไว้กดปุ่ม Restart
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}