﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorListButtons : MonoBehaviour
{
     private AppController appController;
    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptions;

    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddAnchor()
    {
        appController.appMode = AppMode.Create;
        AnchorList.SetActive(false);
        AnchorOptions.SetActive(true);
    }
}
