using System;

namespace Theraot.Collections;

internal sealed class ProgressorProxy
{
	private readonly IClosable _node;

	public bool IsClosed => _node.IsClosed;

	public ProgressorProxy(IClosable node)
	{
		_node = node ?? throw new ArgumentNullException("node");
	}
}
