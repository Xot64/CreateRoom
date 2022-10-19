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
            
            inHand.canStand &= checkAround();
            if (!inHand.canStand) offset = Vector3.zero;
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
    
    public bool checkAround()
    {
        float[] hits = new float[4];
        Vector3 rayStart = hit.point + offset;
        hits[0] = localRays(rayStart, 4, Vector3.right);
        hits[1] = localRays(rayStart, 4, Vector3.forward);
        hits[2] = localRays(rayStart, 4, Vector3.left);
        hits[3] = localRays(rayStart, 4, Vector3.back);
        if (hits[2] + hits[0] < inHand.size.x) return false;
        if (hits[3] + hits[1] < inHand.size.z) return false;

        Vector3 delta = Vector3.zero;
        if (hits[0] < inHand.size.x) delta -= Vector3.right * (inHand.size.x - hits[0]);
        if (hits[1] < inHand.size.z) delta += Vector3.back * (inHand.size.z - hits[1]);
        if (hits[2] < inHand.size.x) delta -= Vector3.left * (inHand.size.x - hits[2]);
        if (hits[3] < inHand.size.z) delta += Vector3.forward * (inHand.size.z - hits[3]);
        text.text = string.Format("R:{0:0.0}/{1:0.0}\nF:{2:0.0}/{3:0.0}\nL:{4:0.0}/{5:0.0}\nB:{6:0.0}/{7:0.0}", hits[0], delta.x, hits[1], delta.z, hits[2], delta.x, hits[3], delta.z);
        if (((hits[0] < inHand.size.x / 2) || (hits[2] < inHand.size.x / 2)) &&
            ((hits[1] < inHand.size.z / 2) || (hits[3] < inHand.size.z / 2)))
        {
            if (inHand.size.x - Mathf.Abs(delta.x) > inHand.size.z - Mathf.Abs(delta.z))
                delta.z = 0;
            else delta.x = 0;
        }
        offset += delta;
        return true;
    }

    public float localRays(Vector3 center, int rays, Vector3 direction)
    {
        RaycastHit hit1;
        Vector3 raysLine = new Vector3(direction.z * inHand.size.x, 0, direction.x * inHand.size.z) / rays;
        float retVal = 10f;
        for (int i = - rays / 2; i <= rays / 2; i++)
        {
            if (Physics.Raycast(center + raysLine * i  - new Vector3 (direction.x * inHand.size.x / 2, 0, direction.x * inHand.size.z / 2), direction, out hit1, new Vector3(inHand.size.x * direction.x,0, inHand.size.z * direction.z).magnitude * 2.5f, 1 << 6))
            {
                
                if (hit1.distance < retVal) retVal = hit1.distance;
            }
        }
        return retVal;
    }
}
