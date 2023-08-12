using UnityEngine;

public class ChainLink : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private SpringJoint springJoint;
    
    public Rigidbody Rigidbody {get {return rigidbody;}}
    public SpringJoint SpringJoint {get {return springJoint;}}

    public void ConnectLink(ChainLink link)
    {
        springJoint.connectedBody = link.Rigidbody;
    }

    public void DestroySpringJoint()
    {
        Destroy(springJoint);
    }
}
