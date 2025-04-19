using UnityEngine;

public class BotRespawner : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _maxDistance = 60;
    [SerializeField] float _respawnXRange = 5;
    [SerializeField] float _respawnDistance = 20;


    void Update()
    {
        float distance = _player.position.z - transform.position.z;
        if (distance > _maxDistance)
        {
            Vector3 newPos = _player.position;
            newPos.z -= _respawnDistance;
            newPos.x = Random.Range(-_respawnXRange, _respawnXRange);
            transform.SetPositionAndRotation(newPos, Quaternion.Euler(0, 0, 0));
        }
    }
}
