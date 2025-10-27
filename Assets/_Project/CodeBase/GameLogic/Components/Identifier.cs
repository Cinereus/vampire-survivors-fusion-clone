using System;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class Identifier : MonoBehaviour
    {
        public uint id { get; private set; }

        public void Setup(uint id)
        {
            this.id = id;
        }
    }
}