using UnityEngine;
using TMPro;
using System.Collections.Generic;
//using NaughtyAttributes;



public class DirectionIndicator : MonoBehaviour
{
    private Transform player;
    
    [SerializeField]
    private GameObject targetObject;
    [SerializeField]
    private List<GameObject> targetObjects;
    public float nearDistance = 0.5f; // Near distance threshold
    public float farDistance = 7f; // Far distance threshold

    public List<GameObject> TargetObjects
    {
        get { return targetObjects; }
        set { targetObjects = value; }
    }

    public GameObject TargetObject
    {
        get { return targetObject; }
        set { targetObject = value; }
    }

    //public Image indicatorArrow;
    public GameObject arrowIndicator,pivotObject;

    public float closeThreshold = 0.2f;   // Distance for green color
    public float middleThreshold = 0.5f;  // Distance for orange color
    public float farThreshold = 1.0f;     // Distance for red color
    public TextMeshProUGUI distanceText;
    private float distance = 0;

    private int currentTargetIndex=0;


    private static DirectionIndicator _instance;
    public static DirectionIndicator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DirectionIndicator>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DirectionIndicator");
                    _instance = go.AddComponent<DirectionIndicator>();
                }
            }
            return _instance;
        }
    }
    private void Start(){
    
        player = this.transform;

        
        if(targetObject == null || targetObjects == null){
            arrowIndicator.SetActive(false);
            distanceText.gameObject.SetActive(false);
        }
        else{
             arrowIndicator.SetActive(true);
            distanceText.gameObject.SetActive(true);
        }
        

        //indicatorArrow.transform.gameObject.SetActive(true);
        
    }

    public void TargetListUpdated()
    {
        if(targetObjects.Count == 0)
            return;
        arrowIndicator.SetActive(true);
        targetObject = targetObjects[0];
    }

   	//[Button("NextTarget")]
    public void NextTarget(){

        currentTargetIndex++;

        Debug.Log(targetObjects.Count);
        
        if(currentTargetIndex < targetObjects.Count){
            targetObject  = targetObjects[currentTargetIndex];
        }
        else{
            targetObject = null;
            targetObjects = null;
            currentTargetIndex = 0;
            arrowIndicator.SetActive(false);
            distanceText.gameObject.SetActive(false);
        }

    }



    private void Update()
    {
        if (targetObject)
        {
            
            distance = Vector3.Distance(player.position, targetObject.transform.position);
            distanceText.text = $"Distance:\n {distance:F2} m";
            UpdateIndicator();

        }
        
    }

    private void UpdateIndicator()
    {
       
        // Use LookAt to point the arrow towards the target
        pivotObject.transform.LookAt(TargetObject.transform.position);
        arrowIndicator.transform.rotation = pivotObject.transform.rotation;
        // Calculate direction and distance
        Vector3 direction = (TargetObject.transform.position - player.position).normalized;
        float distance = Vector3.Distance(player.position, TargetObject.transform.position);
        UpdateTextColor(distance);
        

        // Change color based on distance thresholds
        if (distance > farThreshold)
        {
            //indicatorArrow.color = Color.red;
        }
        else if (distance > middleThreshold)
        {
            //indicatorArrow.color = Color.yellow;
        }
        else
        {
            //indicatorArrow.color = Color.green;
        }
    }

    public void SetTarget(GameObject target)
    {
        TargetObject = target;
        arrowIndicator.SetActive(true);
        distanceText.gameObject.SetActive(true);

    }
    public void RemoveTarget(){
        TargetObject = null;
        arrowIndicator.SetActive(false);
        distanceText.gameObject.SetActive(false);
    }


    private  void UpdateTextColor(float distance)
    {
        if (distance <= nearDistance)
        {
            NextTarget();

        }
        else if (distance > nearDistance && distance<farDistance)
        {
            distanceText.color = Color.green; // Middle
            return;
        }
        else if (distance >= middleThreshold)
        {
                distanceText.color = Color.red; // Middle

        }
        else
        {
            distanceText.color = Color.red; // Far
        }
    }
}
