using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorage : MonoBehaviour
{

    //public EnemyDestroyedEvent enemyDestroyedEvent;

   // public SpeedTypeChangedEvent speedTypeChangedEvent;

    public Outliner outliner;

    public QuestLedger ledger;

    public DialogLoader dialog;

    //public InventoryManager inventory;

    private static ObjectStorage _instance;

    public static ObjectStorage Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
