using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IBlockChainObject
    {
        GameObject gameObject{ get; }
        string hexCode{ get; set; }
        uint gameId{ get; set; }
    }
}
