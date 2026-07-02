using UnityEngine;

public class EneyFireBallHolder : MonoBehaviour
{
    [SerializeField] private Transform enemy;
    private void Update()
    {
        transform.localScale = enemy.localScale;
    }
}
