using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : StatObject
{
    public Vector3 oldPosition;
    public bool canStand;
    Renderer[] my_R;
    public Color holdColor = Color.yellow;
    public Color invisibleColor = new Color(1,0,0,0.5f);
    public Color visibleColor = Color.green;
    protected override void Start()
    {
        base.Start();
        oldPosition = transform.position;
        my_R = body.GetComponentsInChildren<Renderer>();
        Recolor(false);
    }
    private void Update()
    {
        
    }
    public void Take()
    {
        MoveToLayer(0);
        Debug.Log("Take: " + name);
    }
    public void Put(Transform Parent)
    {
        Recolor(false);
        MoveToLayer(6);
        if (canStand)
        {
            transform.SetParent(Parent);
            Debug.Log("Put: " + name);
            oldPosition = transform.position;
        }
        else
        {
            transform.position = oldPosition;
        }
    }
    void MoveToLayer(int l)
    {
        foreach (Item it in GetComponentsInChildren<Item>()) it.body.layer = l;
    }
    public void Recolor(bool onDrag)
    {
        if (onDrag)
        {
            foreach(Renderer r in my_R) r.material.color = canStand ? visibleColor : invisibleColor;
        }
        else foreach(Renderer r in my_R) r.material.color = holdColor;
    }

    public void rotate()
    {
        //float t = size.x;
        //size.x = size.z;
        //size.z = t;
        transform.eulerAngles = Vector3.up * 90 - transform.eulerAngles;
        reSize();

    }
}
