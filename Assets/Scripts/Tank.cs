using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tank : MonoBehaviour
{
    //Visible
    public float health;
    public float moveSpeed;
    public Color tankColor;
    public SpriteRenderer tankSkin;
    public SpriteRenderer outline;
    public Transform flankOrigin;
    public List<Flank> tankFlanks = new List<Flank>();


    //Invisible
    Vector2 mv;
    Vector2 mp;
    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetTankColor();
        SetTankFlanks();
    }

    void Update()
    {
        mv.x = Input.GetAxisRaw("Horizontal");
        mv.y = Input.GetAxisRaw("Vertical");

        mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.Mouse0))
            foreach (Flank flanks in tankFlanks)
                flanks.Shoot();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + mv.normalized * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mp - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void SetTankColor()
    {
        tankSkin.color = tankColor;
        outline.color = Color.Lerp(tankColor, Color.black, .25f);
    }

    void SetTankFlanks()
    {
        foreach (Transform child in flankOrigin)
            tankFlanks.Add(child.GetComponent<Flank>());
    }
}
