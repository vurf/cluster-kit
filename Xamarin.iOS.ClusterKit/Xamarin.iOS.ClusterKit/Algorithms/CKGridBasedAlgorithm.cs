using System;
using System.Collections.Generic;
using MapKit;
using System.Linq;

namespace Xamarin.iOS.ClusterKit.Algorithms
{
    public class CKGridBasedAlgorithm : CKClusterAlgorithm
    {
        public nfloat CellSize { get; set; }

        public CKGridBasedAlgorithm()
        {
            this.CellSize = 100;
            this.RegisterClusterClass(new CKCentroidCluster());
        }

        public override List<CKCluster> ClustersInRect(MapKit.MKMapRect rect, double zoom, Tree.ICKAnnotationTree tree)
        {
            var clusters = new Dictionary<double, CKCluster>();
            var annotations = tree.AnnotationsInRect(rect);
            var numCells = Math.Ceiling(256 * Math.Pow(2, zoom) / this.CellSize);
            foreach (var ann in annotations)
            {
                var point = MKMapPoint.FromCoordinate(ann.Coordinate);
                var col = numCells * point.X / MKMapSize.World.Width;
                var row = numCells * point.Y / MKMapSize.World.Height;

                var key = numCells * row + col;

                CKCluster cluster;
                if (clusters.ContainsKey(key))
                {
                    cluster = clusters[key];
                }
                else
                {
                    cluster = this.ClusterWithCoordinate(ann.Coordinate);
                    clusters[key] = cluster;
                }
                cluster.AddAnnotation(ann);
            }

            return clusters.Values.ToList();
        }
    }
}
