using System;
using MapKit;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public class CKTree
    {
        public CKNode Root { get; set; }

        public static CKTree New(MKMapRect rect, int capacity)
        {
            CKTree tree = new CKTree();
            tree.Root = CKNode.New(rect, capacity);
            return tree;
        }

        public static void Free(CKTree tree)
        {
            if (tree.Root != null)
            {
                CKNode.NodeFree(tree.Root);
                tree = null;
            }
        }

        public static void Insert(CKTree tree, MKAnnotation annotation)
        {
            CKNode.Insert(tree.Root, annotation);
        }

        public static void Remove(CKTree tree, MKAnnotation annotation)
        {
            CKNode.Remove(tree.Root, annotation);
        }

        public static void Cleart(CKTree tree)
        {
            var bound = tree.Root.Bound;
            var capacity = tree.Root.Capacity;
            CKNode.NodeFree(tree.Root);
            tree.Root = CKNode.New(bound, capacity);
        }

        public static void FindInRange(CKTree tree, MKMapRect range, Action<IMKAnnotation> find)
        {
            CKNode.GetInRange(tree.Root, range, find);
        }
    }
}
