using UnityEngine;

public class InteractableFlower : MonoBehaviour
{
    [Header("Settings")]
    public int phaseIndex = 1; // ดอกไม้นี้สำหรับ Phase ไหน (1, 2, 3, 4)
    public bool isFake = false; // ถ้าติ๊กถูก = เผาแล้วไม่ได้แต้ม (หลอก)
    
    [Header("Effects")]
    public GameObject fireEffectPrefab; // ใส่ Prefab ไฟ

    private bool burned = false;

    public void Burn()
    {
        if (burned) return;
        burned = true;

        // เล่น Effect ไฟ
        if (fireEffectPrefab) Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);

        // แจ้ง GameManager
        if (!isFake)
        {
            GameManager.Instance.OnFlowerBurned(phaseIndex);
        }

        // ซ่อนดอกไม้
        gameObject.SetActive(false);
    }
}