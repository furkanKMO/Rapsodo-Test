using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buzzSaw : MonoBehaviour
{
    public GameObject baseObject, buzzsaw;
    public float moveAmount;
    private Vector3 startPosition;
    public string moveAxis;

    // Start is called before the first frame update
    void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Testereyi yavaþça döndür
        buzzsaw.transform.Rotate(Vector3.up, 3f);

        if (moveAxis == "x")
        {
            // Obje X-ekseni boyunca ileri geri hareket etsin
            float newPosition = startPosition.x + Mathf.PingPong(Time.time * 2f, moveAmount * 2f) - moveAmount;
            transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
        }
        else if (moveAxis == "y")
        {
            // Obje Y-ekseni boyunca ileri geri hareket etsin
            float newPosition = startPosition.y + Mathf.PingPong(Time.time * 2f, moveAmount * 2f) - moveAmount;
            transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
        }
        else if (moveAxis == "z")
        {
            // Obje Z-ekseni boyunca ileri geri hareket etsin
            float newPosition = startPosition.z + Mathf.PingPong(Time.time * 2f, moveAmount * 2f) - moveAmount;
            transform.position = new Vector3(transform.position.x, transform.position.y, newPosition);
        }
    }
}
