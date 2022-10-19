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

        
            offset = Vector3.zero;
            inHand.canStand &= checkSpaceUnder();
            if (!inHand.canStand) offset = Vector3.zero;
            
            

            inHand.transform.position = hit.point + offset;
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
    public Vector3 offset;
    public bool checkSpaceUnder()
    {
        bool space = false;
        if ((hit.point.x - inHand.size.x / 2) < onFocus.getBorder(Border.Left))
        {
            space = true;
            offset -= Vector3.left * (onFocus.getBorder(Border.Left) - (hit.point.x - inHand.size.x / 2));
        }
        if ((hit.point.x + inHand.size.x / 2) > onFocus.getBorder(Border.Right))
        {
            if (space) return false;
            offset += Vector3.right * (onFocus.getBorder(Border.Right) - (hit.point.x + inHand.size.x / 2));
        }
        space = false;
        if ((hit.point.z - inHand.size.z / 2) < onFocus.getBorder(Border.Front)) 
        {
            offset -= Vector3.back * (onFocus.getBorder(Border.Front) - (hit.point.z - inHand.size.z / 2));
            space = true;
        }
        if ((hit.point.z + inHand.size.z / 2) > onFocus.getBorder(Border.Back))
        {
            if (space) return false;
            offset += Vector3.forward * (onFocus.getBorder(Border.Back) - (hit.point.z + inHand.size.z / 2));
        }
        return true;

    }
    
}
