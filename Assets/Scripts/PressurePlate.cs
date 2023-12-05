using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    // Levhan�n bast�r�l�p bast�r�lmad���n� kontrol eden de�i�ken
    public bool pressed = false;

    // Bast�r�ld���nda etkile�ime ge�ecek diken nesnelerin listesi
    public List<GameObject> spikesList = new List<GameObject>();

    // Animator'da tetiklenecek bool de�i�kenin ad�
    public string boolToTrigger;

    // Start is called before the first frame update
    void Start()
    {
        // Ba�lang��ta bir �ey yapma
    }

    // Update is called once per frame
    void Update()
    {
        // Levha bast�r�lm��sa
        if (pressed)
        {
            // Diken nesnelerin her biri i�in animasyonu tetikle
            foreach (var item in spikesList)
            {
                item.GetComponentInChildren<Animator>().SetBool(boolToTrigger, true);
            }

            // Levha bast�r�ld���nda tekrar tetiklenmemesi i�in s�f�rla
            pressed = false;
        }
    }
}
