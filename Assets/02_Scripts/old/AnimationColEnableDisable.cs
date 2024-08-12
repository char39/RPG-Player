using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationColEnableDisable : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider = null;
    [SerializeField] MeshRenderer meshRenderer = null;
    [SerializeField] float sword_damage ;
    [SerializeField] float shiled_damage;
    [SerializeField] int swordLayer;
    [SerializeField] int shiledLayer;
    void Start()
    {
        swordLayer = LayerMask.NameToLayer("SWORD");
        shiledLayer = LayerMask.NameToLayer("SHIELD");
        sword_damage = 0;
        shiled_damage = 0;
        boxCollider = GetComponentInChildren<BoxCollider>();
        meshRenderer = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
    }
    public  void AttackHitEnable()
    {
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
        sword_damage = 25f;
        shiled_damage = 15f;
        
    }
    public void AttackHitDisable()
    {
        boxCollider.enabled = false;
        meshRenderer.enabled = false;
    }
}
