using System;
using System.Collections.Generic;
using MapKit;
using CoreLocation;

namespace Xamarin.iOS.ClusterKit.Algorithms
{
    public class CKNonHierarchicalDistanceBasedAlgorithm : CKClusterAlgorithm
    {
        public nfloat CellSize { get; set; }

        public CKNonHierarchicalDistanceBasedAlgorithm()
        {
            this.CellSize = 100;
        }

        public override List<CKCluster> ClustersInRect(MKMapRect rect, double zoom, Tree.ICKAnnotationTree tree)
        {
            var zoomSpecificSpan = 100 * this.CellSize / Math.Pow(2, zoom + 8);
            var clusters = new List<CKCluster>();
            var visited = new Dictionary<CKAnnotation, CKCandidate>();
            var annotations = tree.AnnotationsInRect(rect);

            foreach (var ann in annotations)
            {
                if (visited.ContainsKey(ann))
                {
                    continue;
                }

                var cluster = this.ClusterWithCoordinate(ann.Coordinate);
                clusters.Add(cluster);

                MKMapRect clusterRect = this.CKCreateRectFromSpan(ann.Coordinate, zoomSpecificSpan);
                var neighbors = tree.AnnotationsInRect(clusterRect);

                foreach (var neighbor in neighbors)
                {
                    var distance = CKDistance.GetDistance(neighbor.Coordinate, cluster.Coordinate);
                    CKCandidate candidate;
                    if (visited.TryGetValue(neighbor, out candidate))
                    {
                        if (candidate.Distance < distance)
                        {
                            continue;
                        }
                        candidate.Cluster.RemoveAnnotation(neighbor);
                    }
                    else
                    {
                        candidate = new CKCandidate();
                        visited.Add(neighbor, candidate);
                    }

                    candidate.Cluster = cluster;
                    candidate.Distance = distance;
                    cluster.AddAnnotation(neighbor);
                }
            }

            return clusters;
        }

        public MKMapRect CKCreateRectFromSpan(CLLocationCoordinate2D center, double span)
        {
            double halfSpan = span / 2;

            double latitude = Math.Min(center.Latitude + halfSpan, 90);
            double longitude = Math.Max(center.Longitude - halfSpan, -180);
            MKMapPoint a = MKMapPoint.FromCoordinate(new CLLocationCoordinate2D(latitude, longitude));

            latitude = Math.Max(center.Latitude - halfSpan, -90);
            longitude = Math.Min(center.Longitude + halfSpan, 180);
            MKMapPoint b = MKMapPoint.FromCoordinate(new CLLocationCoordinate2D(latitude, longitude));

            return new MKMapRect(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }
    }
}
