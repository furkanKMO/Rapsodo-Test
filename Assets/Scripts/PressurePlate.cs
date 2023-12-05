using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    // Levhanýn bastýrýlýp bastýrýlmadýðýný kontrol eden deðiþken
    public bool pressed = false;

    // Bastýrýldýðýnda etkileþime geçecek diken nesnelerin listesi
    public List<GameObject> spikesList = new List<GameObject>();

    // Animator'da tetiklenecek bool deðiþkenin adý
    public string boolToTrigger;

    // Start is called before the first frame update
    void Start()
    {
        // Baþlangýçta bir þey yapma
    }

    // Update is called once per frame
    void Update()
    {
        // Levha bastýrýlmýþsa
        if (pressed)
        {
            // Diken nesnelerin her biri için animasyonu tetikle
            foreach (var item in spikesList)
            {
                item.GetComponentInChildren<Animator>().SetBool(boolToTrigger, true);
            }

            // Levha bastýrýldýðýnda tekrar tetiklenmemesi için sýfýrla
            pressed = false;
        }
    }
}
