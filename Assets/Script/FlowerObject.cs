using UnityEngine;

public class FlowerObject : MonoBehaviour
{
    [Header("Settings")]
    public int phaseIndex = 1; // ดอกไม้นี้สำหรับ Phase ไหน (1, 2, 3, 4)
    public bool isDecor = false; // ถ้าติ๊กถูก แปลว่าเป็นแค่ของประดับ เผาไม่ได้แต้ม
    
    [Header("Effects")]
    public GameObject fireParticlePrefab; // ลาก Prefab ไฟใส่ตรงนี้ (ถ้ามี)

    private bool isBurned = false;

    public void Burn()
    {
        if (isBurned) return;

        isBurned = true;
        
        // เล่น Effect ไฟ
        if (fireParticlePrefab)
        {
            Instantiate(fireParticlePrefab, transform.position, Quaternion.identity);
        }

        if (!isDecor)
        {
            // แจ้ง GameManager ว่าเผาแล้ว
            GameManager.Instance.BurnFlower(phaseIndex);
        }

        // ซ่อนดอกไม้ทันที (หรือจะหน่วงเวลาก็ได้)
        gameObject.SetActive(false);
        Debug.Log("Burned flower phase: " + phaseIndex);
    }
}