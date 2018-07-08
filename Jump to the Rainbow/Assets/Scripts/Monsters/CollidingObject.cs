using UnityEngine;

public class CollidingObject : MonoBehaviour, IObject
{
    protected bool IsPlayer(Collider other)
    {
        return other.gameObject.tag == "Player";
    }

    protected bool IsMonsterTrigger(Collider other)
    {
        return other.gameObject.tag == "MonsterTrigger";
    }
}
