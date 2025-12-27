using UnityEngine;
using UnityEngine.UI; // ถ้าใช้ Text Mesh Pro ให้เปลี่ยนเป็น TMPro

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float range = 3.0f;
    public LayerMask flowerLayer;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public Text interactText; // ลาก Text "Press E to Burn" มาใส่
    public Camera cam;

    void Update()
    {
        // ยิง Raycast
        RaycastHit hit;
        bool isHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, flowerLayer);

        if (isHit)
        {
            InteractableFlower flower = hit.collider.GetComponent<InteractableFlower>();
            if (flower != null)
            {
                if(interactText) interactText.enabled = true;
                if(interactText) interactText.text = "เผา [E]";

                if (Input.GetKeyDown(interactKey))
                {
                    flower.Burn();
                }
                return; // จบการทำงานรอบนี้
            }
        }

        // ถ้าไม่เจออะไรเลย
        if(interactText) interactText.enabled = false;
    }
}