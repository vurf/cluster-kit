using System;
using System.Collections.Generic;
using MapKit;
using CoreLocation;
using Xamarin.iOS.ClusterKit.Tree;

namespace Xamarin.iOS.ClusterKit.Algorithms
{
    public class CKClusterAlgorithm
    {
        private CKCluster clusterClass;

        public CKClusterAlgorithm()
        {
            this.clusterClass = new CKCluster();
        }

        public virtual List<CKCluster> ClustersInRect(MKMapRect rect, double zoom, ICKAnnotationTree tree)
        {
            var annotations = tree.AnnotationsInRect(rect);
            var clusters = new List<CKCluster>();

            foreach (var annotation in annotations)
            {
                var clustr = this.ClusterWithCoordinate(annotation.Coordinate);
                clustr.AddAnnotation(annotation);
                clusters.Add(clustr);
            }

            return clusters;
        }

        public void RegisterClusterClass(CKCluster clusterClass)
        {
            if (clusterClass != null)
            {
                this.clusterClass = clusterClass;
            }
        }

        public CKCluster ClusterWithCoordinate(CLLocationCoordinate2D coordinate)
        {
            return CKCluster.ClusterWithCoordinate(coordinate);
        }
    }
}
