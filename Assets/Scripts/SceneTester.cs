using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class SceneTester : MonoBehaviour
{
    public DialogBank starterDialog;
    
    // Start is called before the first frame update
    void Start()
    {
        ObjectStorage.Instance.dialog.Init(starterDialog.dialogObject);
    }


}
