using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab32Variant8;

public class PostOrderEnumerator<T> : IEnumerator<T>
    where T : class
{
    private readonly List<T> items = new();
    private int position = -1;

    public PostOrderEnumerator(BinaryTree<T>.Node root)
    {
        FillPostOrder(root);
    }

    private void FillPostOrder(BinaryTree<T>.Node node)
    {
        if (node is null)
            return;

        // postorder: ліве піддерево -> праве піддерево -> корінь
        FillPostOrder(node.Left);
        FillPostOrder(node.Right);
        items.Add(node.Data);
    }

    public T Current
    {
        get
        {
            if (position < 0 || position >= items.Count)
                throw new InvalidOperationException();

            return items[position];
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (position + 1 >= items.Count)
            return false;

        position++;
        return true;
    }

    public void Reset()
    {
        position = -1;
    }

    public void Dispose()
    {
    }
}
