using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class ShapeDeleteMove : MonoBehaviour
{

    private Vector3 screenPoint;
    private Vector3 offset;
    public DDAAlgorithm dDA;
    public RenderShapes controller;
    public GameObject penaltyPrefab;
    public GameObject canvas;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Debug.Log("Mouse Down");
        

    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

    }
    void Update()
    {
       try
       {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.transform.tag == "Square")
                    {
                        dDA.sqrCount--;
                        Destroy(hit.collider.gameObject);
                        if (dDA.choiceShape != "Square")
                        {
                            IncorrectAnswer();
                        }

                    }
                    else if (hit.collider.gameObject.transform.tag == "Rectangle")
                    {
                        dDA.rectCount--;
                        Destroy(hit.collider.gameObject);
                        if (dDA.choiceShape != "Rectangle")
                        {
                            IncorrectAnswer();
                        }
                    }
                    else if (hit.collider.gameObject.transform.tag == "Circle")
                    {
                        dDA.circleCount--;
                        Destroy(hit.collider.gameObject);
                        if (dDA.choiceShape != "Circle")
                        {
                            IncorrectAnswer();
                        }
                    }

             
                }
              
            }

        }
       catch {}
                
    }

    void IncorrectAnswer()
    {
        controller.secondsLeft -= 1f;
        Debug.Log("-1s penalty incorrect answer");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 position = hit.point;
            position.z = 0f;
            GameObject penaltyText = Instantiate(penaltyPrefab, position, Quaternion.identity);
            dDA.IncrementDecrementIntelligenceGlobal(-0.1f);
            dDA.IncrementDecrementIntelligence("shapes", -0.1f);
            penaltyText.transform.parent = canvas.transform;
            penaltyText.transform.localPosition = position;
            Destroy(penaltyText, 2.5f);
        }
           
    }
  
}
