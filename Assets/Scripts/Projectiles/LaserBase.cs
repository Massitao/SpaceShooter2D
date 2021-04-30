using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaserBase : MonoBehaviour
{
    #region Variables
    [Header("Laser Properties")]
    [SerializeField] protected float laserSpeed = 6f;
    [SerializeField] protected int damage = 1;
    #endregion


    #region MonoBehaviour Methods
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        LaserCollision(collision);
    }
    #endregion

    #region Custom Methods
    protected abstract void LaserCollision(Collider2D collision);
    protected abstract void Move();
    #endregion
}