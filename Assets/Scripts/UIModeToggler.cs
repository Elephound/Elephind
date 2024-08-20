using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModeToggler : MonoBehaviour
{
    
    [SerializeField] GameObject _memorize;
    [SerializeField] GameObject _search;
    // Start is called before the first frame update
    void Start()
    {
        StorageContainerManager.Instance.SetupPhaseChanged.AddListener(SwitchMode);
    }

    void SwitchMode(bool value)
    {
        if(value==true)
        {
            _memorize.SetActive(true);
            _search.SetActive(false);
        }
        else
        {
            _memorize.SetActive(false);
            _search.SetActive(true);
        }
    }

    
}
