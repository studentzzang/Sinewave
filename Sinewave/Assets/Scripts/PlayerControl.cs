using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeriControl : MonoBehaviour
{

    public float speed = 3;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        VerticalControl();
    }

    void VerticalControl() //임시 / w s키로 함 -> 나중에 터치 슬라이더로 바꿀예정
    {
        float ver = Input.GetAxis("Vertical");

        Vector2 vector = new(0, ver);

        rb.MovePosition(rb.position + vector*speed*Time.deltaTime);
        
    }
}
