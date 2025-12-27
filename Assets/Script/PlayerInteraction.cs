using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3.0f;
    public LayerMask interactLayer; // เลือก Layer ของดอกไม้
    public Camera playerCamera;
    public TextMeshProUGUI interactText; // ลาก UI Text เช่น "Press E to Burn" มาใส่

    void Update()
    {
        RaycastHit hit;
        // ยิง Ray จากกลางจอ
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange, interactLayer))
        {
            // ถ้าเจอวัตถุที่มี Script FlowerObject
            FlowerObject flower = hit.collider.GetComponent<FlowerObject>();
            if (flower != null)
            {
                if(interactText) interactText.gameObject.SetActive(true);
                if(interactText) interactText.text = "Press [E] to Burn";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    flower.Burn();
                }
            }
            else
            {
                if(interactText) interactText.gameObject.SetActive(false);
            }
        }
        else
        {
            if(interactText) interactText.gameObject.SetActive(false);
        }
    }
}