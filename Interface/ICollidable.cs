using UnityEngine;

public interface ICollidable
{
    public void OnCollision(bool isColliding, GameObject ob);
}
