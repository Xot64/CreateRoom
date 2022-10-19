using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatObject : MonoBehaviour
{
    public GameObject body;
    public Vector3 size;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        size = body.GetComponent<Renderer>().bounds.size;
   
    }

    public float getBorder(Border b)
    {
        switch (b)
        {
            case Border.Left:
                return transform.position.x - size.x / 2;
                
            case Border.Right:
                return transform.position.x + size.x / 2;

            case Border.Front:
                return transform.position.z - size.z / 2;

            case Border.Back:
                return transform.position.z + size.z / 2;

            default:
                return getBorder(Border.Left);
        }
    }
}
public enum Border
{
    Left,
    Front,
    Right,
    Back
}
