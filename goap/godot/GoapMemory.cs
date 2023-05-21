using Godot;
using System;
using ReGoap.Core;

public class GoapMemory<T, W> : IReGoapMemory<T, W>
{
        protected ReGoapState<T, W> state = ReGoapState<T, W>.Instantiate();

	public virtual ReGoapState<T, W> GetWorldState() => state;
}
