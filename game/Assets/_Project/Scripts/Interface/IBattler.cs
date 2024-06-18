using System;
using UnityEngine;

public interface IBattler
{
    public GameObject gameObject{ get;}
    public void Attack();
    public void Skill();
    public void Damage();
}