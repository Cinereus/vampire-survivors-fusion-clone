using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotationAxis;

    private void Update() => transform.Rotate(_rotationAxis * Time.deltaTime);
}