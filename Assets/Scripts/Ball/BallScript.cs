using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider physicCollider;
    [SerializeField] private SphereCollider triggerCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Light> lights;
    private Material material;
    //[SerializeField] private float speed;
    
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int IsPickedUp = Animator.StringToHash("isPickedUp");

    
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
    }
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            player.combat.OnGrabbingBall.Invoke();
        }
    }

    public void Toggle(bool isEnabled)
    {
        animator.SetBool(IsPickedUp,!isEnabled);
        physicCollider.enabled = isEnabled;
        triggerCollider.enabled = isEnabled;
    }

    public void SetEmissiveColor(Color color)
    {
        foreach (var light in lights)
        {
            light.color = color;
        }
        material.SetColor(EmissionColor,color);
    }

    public void AttachTo(Transform anchor)
    {
        transform.SetParent(anchor);
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
    }


    public void Detach()
    {
        transform.SetParent(null);
    }
}
