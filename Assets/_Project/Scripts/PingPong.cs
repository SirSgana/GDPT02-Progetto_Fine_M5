using UnityEngine;

public class PingPong : MonoBehaviour
{
    [SerializeField] private Vector3 _movementVector;
    [SerializeField] private float _speed = 1f;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _movementFactor = 1f;

    private void Start()
    {
        _startPosition = transform.position;
        _endPosition = _startPosition + _movementVector;
    }

    private void Update()
    {
        _movementFactor = Mathf.PingPong(Time.time * _speed, 1f);
        transform.position = Vector3.Lerp(_startPosition, _endPosition, _movementFactor);
    }
}