using System;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
internal sealed class Node<T>
{
	public Node<T>? Link;

	public T Value = default(T);

	private static readonly Pool<Node<T>> _pool = new Pool<Node<T>>(64, Recycle);

	private Node()
	{
	}

	public static void Recycle(Node<T> node)
	{
		node.Link = null;
		node.Value = default(T);
	}

	public void Initialize(Node<T>? link, T value)
	{
		Link = link;
		Value = value;
	}

	internal static void Donate(Node<T> node)
	{
		_pool.Donate(node);
	}

	internal static Node<T> GetNode(Node<T>? link, T item)
	{
		if (!_pool.TryGet(out var result))
		{
			result = new Node<T>();
		}
		result.Initialize(link, item);
		return result;
	}
}
