using System;
using MapKit;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public class CKNode
    {
        public int Capacity { get; set; }

        public int Count { get; set; }   

        public MKMapRect Bound { get; set; }

        public CKPoint Points { get; set; }  

        /// North West
        public CKNode NW { get; set; }    

        public CKNode NE { get; set; }    

        public CKNode SW { get; set; }   

        public CKNode SE { get; set; }    

        public static CKNode New(MKMapRect bound, int capacity)
        {
            var node = new CKNode();
            node.Bound = bound;
            node.Capacity = capacity;
            return node;
        }

        public static void Add(CKNode node, CKPoint point)
        {
            point.Next = node.Points;
            node.Points = point;
            node.Count++;
        }

        public static bool Drop(CKNode node, MKAnnotation annotation)
        {
            CKPoint cur = node.Points;
            CKPoint prev = null;
            while (cur != null)
            {
                prev = cur;
                cur = cur.Next;

                if (cur.Annotation == annotation)
                {
                    if (prev == null)
                    {
                        node.Points = cur.Next;
                    }
                    else
                    {
                        prev.Next = cur.Next;
                    }
                    node.Count--;
                    return true;
                }
            }

            return false;
        }

        public static void Subdivide(CKNode node)
        {
            MKMapRect bd = node.Bound;
            MKMapRect nw;
            MKMapRect ne;
            MKMapRect sw;
            MKMapRect se;

            nw = bd.Divide(bd.Width / 2, CoreGraphics.CGRectEdge.MaxXEdge, out ne);
            nw = nw.Divide(nw.Height / 2, CoreGraphics.CGRectEdge.MaxYEdge, out sw);
            ne = ne.Divide(ne.Height / 2, CoreGraphics.CGRectEdge.MaxYEdge, out se);

            node.NW = CKNode.New(nw, node.Capacity);
            node.NE = CKNode.New(ne, node.Capacity);
            node.SW = CKNode.New(sw, node.Capacity);
            node.SE = CKNode.New(se, node.Capacity);
        }

        public static bool Insert(CKNode node, MKAnnotation annotation)
        {
            var point = MKMapPoint.FromCoordinate(annotation.Coordinate);
            if (!node.Bound.Contains(point))
            {
                return false;
            }

            if (node.Count < node.Capacity)
            {
                CKPoint tPoint = new CKPoint();
                tPoint.Annotation = annotation;
                tPoint.Point = point;
                CKNode.Add(node, tPoint);
                return true;
            }

            if (node.NW == null)
            {
                CKNode.Subdivide(node);
            }

            if (CKNode.Insert(node.NW, annotation))
            {
                return true;
            }

            if (CKNode.Insert(node.NE, annotation))
            {
                return true;
            }

            if (CKNode.Insert(node.SW, annotation))
            {
                return true;
            }

            if (CKNode.Insert(node.SE, annotation))
            {
                return true;
            }

            return false;
        }

        public static bool Remove(CKNode node, MKAnnotation annotation)
        {
            if (CKNode.Drop(node, annotation))
            {
                return true;
            }

            if (node.SW != null)
            {
                if (CKNode.Remove(node.NW, annotation))
                {
                    return true;
                }

                if (CKNode.Remove(node.NE, annotation))
                {
                    return true;
                }

                if (CKNode.Remove(node.SW, annotation))
                {
                    return true;
                }

                if (CKNode.Remove(node.SE, annotation))
                {
                    return true;
                }
            }

            return false;
        }

        public static void GetInRange(CKNode node, MKMapRect range, Action<IMKAnnotation> find)
        {
            if (node.Count != null)
            {
                if (MKMapRect.Intersects(node.Bound, range) == false)
                {
                    return;
                }

                CKPoint point = node.Points;
                while (point != null)
                {
                    if (range.Contains(point.Point))
                    {
                        find?.Invoke(point.Annotation);
                    }

                    point = point.Next;
                }
            }

            if (node.NW != null)
            {
                CKNode.GetInRange(node.NW, range, find);
                CKNode.GetInRange(node.NE, range, find);
                CKNode.GetInRange(node.SW, range, find);
                CKNode.GetInRange(node.SE, range, find);
            }
        }

        public static void PointFree(CKPoint point)
        {
            if (point != null)
            {
                PointFree(point.Next);
                point = null;
            }
        }

        public static void NodeFree(CKNode node)
        {
            CKNode.PointFree(node.Points);
            node.Count = 0;
            if (node.NW != null)
            {
                NodeFree(node.NW);
                NodeFree(node.NE);
                NodeFree(node.SW);
                NodeFree(node.SE);
            }
            node = null;
        }
    }
}
