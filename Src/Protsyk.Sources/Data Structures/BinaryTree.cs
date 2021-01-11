using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.Sources.DataStructures
{
    public class BinaryTree
    {
        public class Node<T>
        {
            public T Value { get; set; }

            public Node<T> Left { get; set; }

            public Node<T> Right { get; set; }

            public static Node<T> From(T value)
            {
                return new Node<T> { Value = value };
            }

            public static Node<T> From(T value, Node<T> left, Node<T> right)
            {
                return new Node<T>
                {
                    Value = value,
                    Left = left,
                    Right = right
                };
            }

        }

        public enum TraverseType
        {
            PreOrder,
            InOrder,
            PostOrder
        }

        public static void Traverse<T>(Node<T> root, TraverseType type, Action<T> visitor)
        {
            if (root == null)
            {
                throw new ArgumentException();
            }
            var stack = new Stack<(Node<T>, T)>();
            stack.Push((root, default(T)));

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // Invoke visitor
                if (current.Item1 == null)
                {
                    visitor(current.Item2);
                    continue;
                }

                // Visit node
                var node = current.Item1;
                if (type == TraverseType.PostOrder)
                {
                    stack.Push((null, node.Value));
                }
                if (node.Right != null)
                {
                    stack.Push((node.Right, default(T)));
                }
                if (type == TraverseType.InOrder)
                {
                    stack.Push((null, node.Value));
                }
                if (node.Left != null)
                {
                    stack.Push((node.Left, default(T)));
                }
                if (type == TraverseType.PreOrder)
                {
                    stack.Push((null, node.Value));
                }
            }
        }
    }
}
