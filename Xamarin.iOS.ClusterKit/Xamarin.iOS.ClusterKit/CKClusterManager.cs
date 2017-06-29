using System;
using CoreLocation;
using MapKit;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Xamarin.iOS.ClusterKit.Tree;
using Xamarin.iOS.ClusterKit.Algorithms;

namespace Xamarin.iOS.ClusterKit
{
    public class CKClusterManager : ICKAnnotationTreeDelegate
    {
        private const double kCKMarginFactorWorld = -1;
        private List<CKCluster> clusters;
        private ICKMap map;

        public CKClusterAlgorithm Algorithm { get; set; }

        public nfloat MaxZoomLevel { get; set; }

        public double MarginFactor { get; set; }

        public double AnimationDuration { get; set; }

        public UIViewAnimationOptions AnimationOptions { get; set; }

        public CKCluster SelectedCluster { get; set; }

        public ICKAnnotationTree Tree { get; set; }

        public ICKClusterManagerDelegate Delegate { get; set; }

        public MKMapRect VisibleMapRect { get; set; }

        public CKAnnotation SelectedAnnotation
        {
            get
            {
                if (this.SelectedCluster?.Annotations != null && this.SelectedCluster.Annotations.Count > 0)
                {
                    return this.SelectedCluster.Annotations[0];
                }

                return null;
            }
        }

        private List<CKCluster> Clusters
        {
            get
            {
                if (this.clusters == null)
                {
                    this.clusters = new List<CKCluster>();
                }

                if (this.SelectedCluster != null)
                {
                    this.clusters.Add(this.SelectedCluster);
                }

                return this.clusters;
            }
            set { this.clusters = value; }
        }

        public CKClusterManager()
        {
            this.Algorithm = new CKClusterAlgorithm();
            this.MaxZoomLevel = 20;
            this.MarginFactor = kCKMarginFactorWorld;
            this.AnimationDuration = 0.5f;
            this.AnimationOptions = UIViewAnimationOptions.CurveEaseOut;
            this.Clusters = new List<CKCluster>();
        }

        public void SetMap(ICKMap map)
        {
            this.map = map;
            this.VisibleMapRect = map.VisibleMapRect;
        }

        public void UpdateClustersIfNeeded()
        {
            if (this.map == null)
            {
                return;
            }

            MKMapRect visibleMapRect = this.map.VisibleMapRect;
            if (Math.Abs(this.VisibleMapRect.Width - visibleMapRect.Width) > 0.1f)
            {
                this.UpdateMapRect(visibleMapRect, this.AnimationDuration > 0);
            }
            else if (this.MarginFactor != kCKMarginFactorWorld)
            {
                if (Math.Abs(this.VisibleMapRect.Origin.X - visibleMapRect.Origin.X) > this.VisibleMapRect.Width * this.MarginFactor / 2 ||
                    Math.Abs(this.VisibleMapRect.Origin.Y - visibleMapRect.Origin.Y) > this.VisibleMapRect.Height * this.MarginFactor / 2)
                {
                    this.UpdateMapRect(visibleMapRect, false);
                }
            }
        }

        public void UpdateClusters()
        {
            if (this.map == null)
            {
                return;
            }

            MKMapRect visibleMapRect = this.map.VisibleMapRect;
            bool animated = (this.AnimationDuration > 0) && Math.Abs(this.VisibleMapRect.Width - visibleMapRect.Width) > 0.1f;
            this.UpdateMapRect(visibleMapRect, animated);
        }

        public List<CKAnnotation> Annotations
        {
            get { return this.Tree != null ? this.Tree.Annotations : new List<CKAnnotation>(); }
            set
            {
                this.Tree = new CKQuadTree(value);
                this.Tree.Delegate = this;
                this.UpdateClusters();
            }
        }

        public void AddAnnotation(CKAnnotation annotation)
        {
            this.Annotations.Add(annotation);
            this.Annotations = this.Annotations;
        }

        public void AddAnnotations(List<CKAnnotation> annotations)
        {
            this.Annotations.AddRange(annotations);
            this.Annotations = this.Annotations;
        }

        public void RemoveAnnotation(CKAnnotation annotation)
        {
            var _annotations = this.Annotations;
            _annotations.Remove(annotation);
            this.Annotations = _annotations;
        }

        public void RemoveAnnotations(List<CKAnnotation> annotations)
        {
            var _annotations = this.Annotations;
            foreach (var an in annotations)
            {
                _annotations.Remove(an);
            }

            this.Annotations = _annotations;
        }

        public void SelectAnnotation(CKAnnotation annotation, bool animated, CKCluster cluster = null)
        {
            if (annotation != null)
            {
                if (annotation.Cluster == null || annotation.Cluster.Annotations.Count > 1)
                {
                    cluster = this.Algorithm.ClusterWithCoordinate(annotation.Coordinate);
                    cluster.AddAnnotation(annotation);
                    this.map.AddCluster(cluster);
                }
                else
                {
                    cluster = annotation.Cluster;
                }
            }
            this.SetSelectedCluster(cluster, animated);
        }

        public void DeselectAnnotation(CKAnnotation annotation, bool animated)
        {
            if (annotation == this.SelectedAnnotation)
            {
                this.SelectAnnotation(null, animated);
            }
        }

        public void SetSelectedCluster(CKCluster selectedCluster, bool animated)
        {
            if (this.SelectedCluster != null && selectedCluster != this.SelectedCluster)
            {
                this.Clusters.Add(this.SelectedCluster);
                this.map.DeselectCluster(this.SelectedCluster, animated);
            }
            if (selectedCluster != null)
            {
                this.Clusters.Remove(selectedCluster);
                this.map.SelectCluster(selectedCluster, animated);
            }
            this.SelectedCluster = selectedCluster;
        }

        private void UpdateMapRect(MKMapRect visibleMapRect, bool animated)
        {
            if (this.Tree == null || visibleMapRect.IsNull || visibleMapRect.IsEmpty)
            {
                return;
            }

            MKMapRect clusterMapRect = new MKMapRect().World;
            if (this.MarginFactor != kCKMarginFactorWorld)
            {
                clusterMapRect = visibleMapRect.Inset(-this.MarginFactor * visibleMapRect.Width, -this.MarginFactor * visibleMapRect.Height);
            }

            double zoom = this.map.Zoom;
            var algorithm = (zoom < this.MaxZoomLevel) ? this.Algorithm : new CKClusterAlgorithm();
            var clusters = algorithm.ClustersInRect(clusterMapRect, zoom, this.Tree);
            var toRemove = this.clusters.ToList();
            foreach (var newCluster in clusters)
            {
                this.map.AddCluster(newCluster);
                if (!visibleMapRect.Contains(MKMapPoint.FromCoordinate(newCluster.Coordinate)) || !animated)
                {
                    continue;
                }

                var c = this.Clusters.ToList();
                foreach (var oldCluster in c)
                {
                    if (!clusterMapRect.Contains(MKMapPoint.FromCoordinate(oldCluster.Coordinate)))
                    {
                        continue;
                    }

                    if (this.CLLocationCoordinateEqual(newCluster.Coordinate, oldCluster.Coordinate))
                    {
                        continue;
                    }

                    if (oldCluster.Annotations.Contains(newCluster.Annotations[0]))
                    {
                        this.map.MoveCluster(newCluster, oldCluster.Coordinate, newCluster.Coordinate, null);
                    }
                    else if (newCluster.Annotations.Contains(oldCluster.Annotations[0]))
                    {
                        this.map.MoveCluster(oldCluster, oldCluster.Coordinate, newCluster.Coordinate, (finished) => { this.map.RemoveCluster(oldCluster); });
                        toRemove.Remove(oldCluster);
                    }
                }
            }

            foreach (var cluster in toRemove)
            {
                this.map.RemoveCluster(cluster);
            }

            this.Clusters = clusters;
            this.VisibleMapRect = visibleMapRect;
        }

        public bool AnnotationTree(ICKAnnotationTree annotationTree, CKAnnotation annotation)
        {
            if (annotation == this.SelectedAnnotation)
            {
                return false;
            }

            return this.Delegate?.ClusterManager(this, annotation) ?? true;
        }

        private bool CLLocationCoordinateEqual(CLLocationCoordinate2D coordinate1, CLLocationCoordinate2D coordinate2)
        {
            return (Math.Abs(coordinate1.Latitude - coordinate2.Latitude) <= double.Epsilon &&
                    Math.Abs(coordinate1.Longitude - coordinate2.Longitude) <= double.Epsilon);
        }
    }
}