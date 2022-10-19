using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Item inHand;
    public Transform defaultItemParent;
    public StatObject onFocus;
    public 
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Raying();
        if (Input.GetMouseButton(0))
        {
            
            if (inHand != null) Drag();
        }
        if (Input.GetMouseButtonDown(0)) TryTake();
        if (Input.GetMouseButtonUp(0)) Put();
        
        
    }

    void TryTake()
    {
        if (onFocus.body.tag == "Item")
        {
            inHand = (Item)onFocus;
            inHand.Take();
        }
        else
        {
            Debug.Log("Nothing to take");
        }

    }
    void Put()
    {
        if (inHand != null)
        {
            inHand.Put(hit.transform.gameObject.tag == "Floor" ? defaultItemParent : hit.transform);
            
            inHand = null;
        }
    }
    void Drag()
    {
        if (onFocus != null)
        {
            inHand.canStand = (Mathf.Abs(hit.point.y - (onFocus.transform.position.y + onFocus.size.y)) < 0.01) && (onFocus != null);

            inHand.transform.position = hit.point;
        }
        inHand.Recolor(true);
    }
    void Raying()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude, 1 << 6))
        {
            onFocus = hit.transform.GetComponentInParent<StatObject>();
            
        }
        else
        {
            onFocus = null;
        }
        
    }
    
    
}
