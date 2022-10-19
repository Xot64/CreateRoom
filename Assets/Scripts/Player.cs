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
    public RaycastHit hit;
    public bool rotated;
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

            if (inHand != null && onFocus != null)
            {
                Drag();
                inHand.Recolor(true);
                inHand.transform.position = hit.point + (inHand.canStand ? offset : Vector3.zero);
            }
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
            inHand.Put(onFocus.tag == "Floor" ? defaultItemParent : onFocus.transform);

            inHand = null;
        }
    }
    void Drag()
    {
        rotated = false;
        //”казывает ли на поверхность, куда можно поставить
        inHand.canStand = onFocus.flatTop && (Mathf.Abs(hit.point.y - (onFocus.transform.position.y + onFocus.size.y)) < 0.01f) && (onFocus != null);

        text.text = Mathf.Abs(hit.point.y - (onFocus.transform.position.y + onFocus.size.y)) < 0.01f ? "" : Mathf.Abs(hit.point.y - (onFocus.transform.position.y + onFocus.size.y)).ToString();
        

        if (!inHand.canStand) return;
        
        //проверка поставки на объект
        offset = Vector3.zero; 

        inHand.canStand &= checkSpaceUnder(false,true);
        
        //if (inHand.canStand) inHand.canStand &= checkAround();
        
        //if (inHand.canStand) inHand.canStand &= checkSpaceUnder(false, false);
        
        //если нельз€ поставить, то пробуем повернуть
        if (!inHand.canStand)
        {
            
            inHand.canStand = true;
            rotated = true;
            offset = Vector3.zero;
            inHand.canStand &= checkSpaceUnder(true,true);
        
            //if (inHand.canStand) inHand.canStand &= checkAround(true);
        
            //if (inHand.canStand) inHand.canStand &= checkSpaceUnder(true,false);
        
            if (inHand.canStand)
            {
                inHand.rotate();
            }
            else
            {
                offset = Vector3.zero;
            }
        }
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
    public bool checkSpaceUnder(bool rotated = false, bool correcting = true)
    {
        Vector3 size = !rotated ? inHand.size : new Vector3(inHand.size.z, inHand.size.y, inHand.size.x);
        if (onFocus.size.x - size.x < -0.001f || onFocus.size.z - size.z < -0.001f) return false;

        Vector3 delta = Vector3.zero;
        RaycastHit hit1;
        if (Physics.Raycast(hit.point + offset + Vector3.left * size.x / 2 + Vector3.down * 0.01f,          Vector3.right,   out hit1, size.x, 1 << 6))
            delta -= Vector3.left * hit1.distance;
        else if  (Physics.Raycast(hit.point + offset + Vector3.right * size.x / 2 + Vector3.down * 0.01f,   Vector3.left,    out hit1, size.x, 1 << 6))
            delta -= Vector3.right * hit1.distance;
        if (Physics.Raycast(hit.point + offset + Vector3.forward * size.z / 2 + Vector3.down * 0.01f,       Vector3.back,    out hit1, size.z, 1 << 6))
            delta -= Vector3.forward * hit1.distance;
        else if (Physics.Raycast(hit.point + offset + Vector3.back * size.z / 2 + Vector3.down * 0.01f,     Vector3.forward, out hit1, size.z, 1 << 6))
            delta -= Vector3.back * hit1.distance;
        if (correcting)
        {
            offset += delta;
            return true;
        }
        else
        {
            return (delta == Vector3.zero);
        }
        /*
        bool space = false;
        if ((hit.point.x + offset.x - size.x / 2) < onFocus.getBorder(Border.Left))
        {
            space = true;
            if (!offseting  && offset.x != 0) return false;
        
            delta -= Vector3.left * (onFocus.getBorder(Border.Left) - (hit.point.x - size.x / 2));
        }
        if ((hit.point.x + offset.x + size.x / 2) > onFocus.getBorder(Border.Right))
        {
            if (!offseting && offset.x != 0) return false;
            if (space) return false;
        
            delta += Vector3.right * (onFocus.getBorder(Border.Right) - (hit.point.x + size.x / 2));
        }
        space = false;
        if ((hit.point.z + offset.z - size.z / 2) < onFocus.getBorder(Border.Front)) 
        {
            if (!offseting && offset.z != 0) return false;
            delta -= Vector3.back * (onFocus.getBorder(Border.Front) - (hit.point.z - size.z / 2));
            space = true;
        
        }
        if ((hit.point.z + offset.z + size.z / 2) > onFocus.getBorder(Border.Back))
        {
            if (!offseting && offset.z != 0) return false;
            if (space) return false;
            delta += Vector3.forward * (onFocus.getBorder(Border.Back) - (hit.point.z + size.z / 2));
      
        }
        offset += delta;
        return true;
        */
      
    }
    
    
    public bool checkAround(bool rotated = false)
    {
        
        Vector3 size = !rotated ? inHand.size : new Vector3(inHand.size.z, inHand.size.y, inHand.size.x);
        float[] hits = new float[4];
        Vector3 rayStart = hit.point + offset;
        hits[0] = localRays(rayStart, 4, Vector3.right);
        hits[1] = localRays(rayStart, 4, Vector3.forward);
        hits[2] = localRays(rayStart, 4, Vector3.left, hits[0] < size.x);
        hits[3] = localRays(rayStart, 4, Vector3.back, hits[1] < size.z);

        Vector3 delta = Vector3.zero;
        if (hits[0] < size.x) delta += Vector3.left * (size.x - hits[0]);
        else if (hits[2] < size.x) delta += Vector3.right * (size.x - hits[2]);
        if (hits[1] < size.z) delta += Vector3.back * (size.z - hits[1]);
        else if (hits[3] < size.z) delta += Vector3.forward * (size.z - hits[3]);
        


        //text.text = string.Format("R:{0:0.0}/{1:0.0}\nF:{2:0.0}/{3:0.0}\nL:{4:0.0}/{5:0.0}\nB:{6:0.0}/{7:0.0}", hits[0], delta.x, hits[1], delta.z, hits[2], delta.x, hits[3], delta.z);
        if (((hits[0] < size.x) || (hits[2] < size.x)) &&
            ((hits[1] < size.z) || (hits[3] < size.z)))
        {
            if (size.x - Mathf.Abs(delta.x) > size.z - Mathf.Abs(delta.z))
                delta.z = 0;
            else delta.x = 0;
        }
        
        if (hits[2] + hits[0] < size.x && Mathf.Abs(delta.z) > 0) return false;
        if (hits[3] + hits[1] < size.z && Mathf.Abs(delta.x) > 0) return false;
        offset += delta;
        return true;
    }

    public float localRays(Vector3 center, int rays, Vector3 direction, bool invert = false)
    {
        RaycastHit hit1;
        Vector3 size = !rotated ? inHand.size : new Vector3(inHand.size.z, inHand.size.y, inHand.size.x);
        Vector3 raysLine = new Vector3(direction.z * size.x, 0, direction.x * size.z) / rays;
        float retVal = 10f;
        for (int i = - rays / 2; i <= rays / 2; i++)
        {
            if (Physics.Raycast(center + raysLine * i  - new Vector3 (direction.x * size.x / 2, 0, direction.z * size.z / 2) * (invert ? -1 : 1), direction, out hit1, new Vector3(size.x * direction.x,0, size.z * direction.z).magnitude * 2.5f, 1 << 6))
            {
                
                if (hit1.distance < retVal) retVal = hit1.distance;
            }
        }
        return retVal;
    }
}
