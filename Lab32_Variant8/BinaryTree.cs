using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab32Variant8;

public class BinaryTree<T> : IEnumerable<T>
    where T : class, IComparable<T>
{
    public sealed class Node
    {
        public T Data { get; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(T data)
        {
            Data = data;
        }
    }

    private Node root;

    public Node Root => root;

    public void Add(T data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        root = AddRecursive(root, data);
    }

    private static Node AddRecursive(Node node, T data)
    {
        if (node is null)
            return new Node(data);

        if (data.CompareTo(node.Data) < 0)
            node.Left = AddRecursive(node.Left, data);
        else
            node.Right = AddRecursive(node.Right, data);

        return node;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new PostOrderEnumerator<T>(root);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
