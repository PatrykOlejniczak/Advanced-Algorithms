﻿using System;

namespace Advanced.Algorithms.DataStructures
{
    internal class SplayTreeNode<T> : BSTNodeBase<T>  where T : IComparable
    {
        internal new SplayTreeNode<T> Parent
        {
            get { return (SplayTreeNode<T>)base.Parent; }
            set { base.Parent = value; }
        }

        internal new SplayTreeNode<T> Left
        {
            get { return (SplayTreeNode<T>)base.Left; }
            set { base.Left = value; }
        }

        internal new SplayTreeNode<T> Right
        {
            get { return (SplayTreeNode<T>)base.Right; }
            set { base.Right = value; }
        }

        internal SplayTreeNode(SplayTreeNode<T> parent, T value)
        {
            Parent = parent;
            Value = value;
        }

    }

    //TODO support initial  bulk loading if possible
    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    public class SplayTree<T> where T : IComparable
    {
        internal SplayTreeNode<T> Root { get; set; }
        public int Count { get; private set; }

        //O(log(n)) worst O(n) for unbalanced tree
        public bool HasItem(T value)
        {
            if (Root == null)
            {
                return false;
            }

            return find(Root, value) != null;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        public int GetHeight()
        {
            return GetHeight(Root);
        }

        //O(log(n)) worst O(n) for unbalanced tree
        private int GetHeight(SplayTreeNode<T> node)
        {
            if (node == null)
            {
                return -1;
            }

            return Math.Max(GetHeight(node.Left), GetHeight(node.Right)) + 1;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        public void Insert(T value)
        {
            if (Root == null)
            {
                Root = new SplayTreeNode<T>(null, value);
                Count++;
                return;
            }

            var newNode = insert(Root, value);

            splay(newNode);
            Count++;
        }

        //O(log(n)) always
        private SplayTreeNode<T> insert(SplayTreeNode<T> currentNode, T newNodeValue)
        {
            while (true)
            {
                var compareResult = currentNode.Value.CompareTo(newNodeValue);

                //current node is less than new item
                if (compareResult < 0)
                {
                    //no right child
                    if (currentNode.Right == null)
                    {
                        //insert
                        currentNode.Right = new SplayTreeNode<T>(currentNode, newNodeValue);
                        return currentNode.Right;
                    }

                    currentNode = currentNode.Right;
                }
                //current node is greater than new node
                else if (compareResult > 0)
                {
                    if (currentNode.Left == null)
                    {
                        //insert
                        currentNode.Left = new SplayTreeNode<T>(currentNode, newNodeValue);
                        return currentNode.Left;
                    }

                    currentNode = currentNode.Left;
                }
                else
                {
                    throw new Exception("Item exists");
                }
            }
        }

        //remove the node with the given identifier from the descendants 
        //O(log(n)) worst O(n) for unbalanced tree
        public void Delete(T value)
        {
            if (Root == null)
            {
                throw new Exception("Empty SplayTree");
            }

            delete(Root, value);
            Count--;
        }

        //O(log(n)) worst O(n) for unbalanced tree
        private void delete(SplayTreeNode<T> node, T value)
        {
            while (true)
            {
                var compareResult = node.Value.CompareTo(value);

                //node is less than the search value so move right to find the deletion node
                if (compareResult < 0)
                {
                    node = node.Right ?? throw new Exception("Item do not exist");
                    continue;
                }
                //node is less than the search value so move left to find the deletion node

                if (compareResult > 0)
                {
                    node = node.Left ?? throw new Exception("Item do not exist");
                    continue;
                }

                var parent = node.Parent;
                //node is a leaf node
                if (node.IsLeaf)
                {
                    deleteLeaf(node);
                }
                else
                {
                    //case one - right tree is null (move sub tree up)
                    if (node.Left != null && node.Right == null)
                    {
                        deleteLeftNode(node);
                    }
                    //case two - left tree is null  (move sub tree up)
                    else if (node.Right != null && node.Left == null)
                    {
                        deleteRightNode(node);
                    }
                    //case three - two child trees 
                    //replace the node value with maximum element of left subtree (left max node)
                    //and then delete the left max node
                    else
                    {
                        var maxLeftNode = FindMax(node.Left);

                        node.Value = maxLeftNode.Value;

                        //delete left max node
                        delete(node.Left, maxLeftNode.Value);
                    }
                }

                if (parent != null)
                {
                    splay(parent);
                }

                break;
            }
        }

        private void deleteLeaf(SplayTreeNode<T> node)
        {
            //if node is root
            if (node.Parent == null)
            {
                Root = null;
            }
            //assign nodes parent.left/right to null
            else if (node.IsLeftChild)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }
        }

        private void deleteRightNode(SplayTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Right.Parent = null;
                Root = Root.Right;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Right;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Right;
            }

            node.Right.Parent = node.Parent;
        }

        private void deleteLeftNode(SplayTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Left.Parent = null;
                Root = Root.Left;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Left;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Left;
            }

            node.Left.Parent = node.Parent;
        }

        public T FindMax()
        {
            return FindMax(Root).Value;
        }


        private SplayTreeNode<T> FindMax(SplayTreeNode<T> node)
        {
            while (true)
            {
                if (node.Right == null) return node;
                node = node.Right;
            }
        }

        public T FindMin()
        {
            return FindMin(Root).Value;
        }

        private SplayTreeNode<T> FindMin(SplayTreeNode<T> node)
        {
            while (true)
            {
                if (node.Left == null) return node;
                node = node.Left;
            }
        }

        //find the node with the given identifier among descendants of parent and parent
        //uses pre-order traversal
        //O(log(n)) worst O(n) for unbalanced tree
        private SplayTreeNode<T> find(SplayTreeNode<T> parent, T value)
        {
            while (true)
            {
                if (parent == null)
                {
                    return null;
                }

                if (parent.Value.CompareTo(value) == 0)
                {
                    return parent;
                }

                var left = find(parent.Left, value);

                if (left != null) return left;
                parent = parent.Right;
            }
        }

        private void splay(SplayTreeNode<T> x)
        {
            while (x.Parent != null)
            {
                if (x.Parent.Parent == null)
                {
                    //zig step
                    x = x.IsLeftChild ? rightRotate(x.Parent) : leftRotate(x.Parent);
                }
                //zig-zig step
                else if (x.IsLeftChild && x.Parent.IsLeftChild)

                {
                    rightRotate(x.Parent.Parent);
                    x = rightRotate(x.Parent);
                }
                //zig-zig step mirror
                else if (x.IsRightChild && x.Parent.IsRightChild)
                {
                    leftRotate(x.Parent.Parent);
                    x = leftRotate(x.Parent);
                }
                //zig-zag step
                else if (x.IsLeftChild && x.Parent.IsRightChild)
                {
                    rightRotate(x.Parent);
                    x = leftRotate(x.Parent);
                }
                //zig-zag step mirror
                else //if (x.IsRightChild && x.Parent.IsLeftChild)
                {
                    leftRotate(x.Parent);
                    x = rightRotate(x.Parent);
                }
            }
        }

        /// <summary>
        /// Rotates current root right and returns the new root node
        /// </summary>
        /// <param name="currentRoot"></param>
        /// <returns></returns>
        private SplayTreeNode<T> rightRotate(SplayTreeNode<T> currentRoot)
        {
            var prevRoot = currentRoot;
            var leftRightChild = prevRoot.Left.Right;

            var newRoot = currentRoot.Left;

            //make left child as root
            prevRoot.Left.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Left;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Left;
                }
            }

            //move prev root as right child of current root
            newRoot.Right = prevRoot;
            prevRoot.Parent = newRoot;

            //move right child of left child of prev root to left child of right child of new root
            newRoot.Right.Left = leftRightChild;
            if (newRoot.Right.Left != null)
            {
                newRoot.Right.Left.Parent = newRoot.Right;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }

            return newRoot;

        }

        /// <summary>
        /// Rotates the current root left and returns new root
        /// </summary>
        /// <param name="currentRoot"></param>
        /// <returns></returns>
        private SplayTreeNode<T> leftRotate(SplayTreeNode<T> currentRoot)
        {
            var prevRoot = currentRoot;
            var rightLeftChild = prevRoot.Right.Left;

            var newRoot = currentRoot.Right;

            //make right child as root
            prevRoot.Right.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Right;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Right;
                }
            }


            //move prev root as left child of current root
            newRoot.Left = prevRoot;
            prevRoot.Parent = newRoot;

            //move left child of right child of prev root to right child of left child of new root
            newRoot.Left.Right = rightLeftChild;
            if (newRoot.Left.Right != null)
            {
                newRoot.Left.Right.Parent = newRoot.Left;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }

            return newRoot;
        }

    }
}
