using System.Collections.Generic;
using JetBrains.Annotations;

namespace Objects
{
    public class Triangle
    {
        private Edge[] edges = new Edge[3];

        public Triangle()
        {
            edges[0] = new Edge();
            edges[1] = new Edge();
            edges[2] = new Edge();
        }

        public Triangle(Edge a, Edge b, Edge c)
        {
            edges[0] = a;
            edges[1] = b;
            edges[2] = c;
        }
        
        public Triangle(Edge a, Edge b)
        {
            edges[0] = a;
            edges[1] = b;
            edges[2] = a.FindPointsDifference(b);
        }

        public Triangle(List<Edge> newEdges)
        {
            if (newEdges == null || newEdges.Count <= 2) return;
            edges[0] = newEdges[0];
            edges[1] = newEdges[1];
            edges[2] = newEdges[2];
        }

        [CanBeNull]
        public Edge GetSimilarEdge(Triangle triangleToCompare)
        {
            foreach (Edge currentEdge in edges)
            {
                foreach (Edge edgeToCompare in triangleToCompare.edges)
                    if (currentEdge == edgeToCompare)
                        return currentEdge;
            }

            return null;
        }

        public void DisplayTriangle()
        {
            
        }
    }
}