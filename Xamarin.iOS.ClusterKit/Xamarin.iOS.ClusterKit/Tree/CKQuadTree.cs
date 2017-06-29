using System;
using System.Collections.Generic;
using MapKit;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public class CKQuadTree : ICKAnnotationTree
    {
        public List<CKAnnotation> Annotations { get; set; } = new List<CKAnnotation>();

        public CKTree Tree { get; set; }

        public ICKAnnotationTreeDelegate Delegate { get; set; }

        public CKQuadTree() : this(new List<CKAnnotation>())
        {
        }

        public CKQuadTree(List<CKAnnotation> annotations)
        {
            this.Annotations = annotations;
            this.Tree = CKTree.New(new MKMapRect().World, 4);

            foreach (var annotation in this.Annotations)
            {
                CKTree.Insert(this.Tree, annotation);
            }
        }

        public List<CKAnnotation> AnnotationsInRect(MKMapRect rect)
        {
            var result = new List<CKAnnotation>();
            if (rect.Spans180thMeridian)
            {
                CKTree.FindInRange(this.Tree, rect.Remainder(), (IMKAnnotation annotation) =>
                {
                    if (this.Delegate.AnnotationTree(this, (CKAnnotation)annotation))
                    {
                        result.Add((CKAnnotation)annotation);
                    }
                });

                rect = MKMapRect.Intersection(rect, new MKMapRect().World);
            }

            CKTree.FindInRange(this.Tree, rect, (IMKAnnotation annotation) =>
             {
                 if (this.Delegate.AnnotationTree(this, (CKAnnotation)annotation))
                 {
                     result.Add((CKAnnotation)annotation);
                 }
             });

            return result;
        }
    }
}
