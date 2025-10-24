using System;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class Identifier : MonoBehaviour
    {
        public Guid id { get; private set; }

        public void Setup(Guid id)
        {
            this.id = id;
        }
    }
}