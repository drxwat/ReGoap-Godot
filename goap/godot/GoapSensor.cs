using Godot;
using System;
using ReGoap.Core;

public class GoapSensor<T, W> : IReGoapSensor<T, W>
{
        protected IReGoapMemory<T, W> memory;
        public virtual void Init(IReGoapMemory<T, W> memory)
        {
            this.memory = memory;
        }

        public virtual IReGoapMemory<T, W> GetMemory()
        {
            return memory;
        }

        public virtual void UpdateSensor()
        {

        }
}
