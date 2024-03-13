using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        List<double[]> data = GenerateRandomData(30000, 14); //тут цифры на свой варик поменяй

        int k = 15;
        KMeans kmeans = new KMeans(k);
        Stopwatch sw = new Stopwatch();

        sw.Start();
        List<int> clusters = kmeans.Cluster(data);
        sw.Stop();

        for (int i = 0; i < data.Count; i++)
            Console.WriteLine("Элемент {0} принадлежит кластеру {1}", i, clusters[i]);

        Console.WriteLine("\nВремя выполнения алгоритма кластеризации: {0}", sw.Elapsed);
    }

    static List<double[]> GenerateRandomData(int count, int dimensions)
    {
        Random random = new Random();
        List<double[]> data = new List<double[]>();

        for (int i = 0; i < count; i++)
        {
            double[] point = new double[dimensions];

            for (int j = 0; j < dimensions; j++)
                point[j] = random.NextDouble();

            data.Add(point);
        }

        return data;
    }
}

class KMeans
{
    private int k;
    private List<double[]> centroids;

    public KMeans(int k)
    {
        this.k = k;
        this.centroids = new List<double[]>();
    }

    public List<int> Cluster(List<double[]> data)
    {
        InitializeCentroids(data);

        List<int> clusters = new List<int>();
        bool converged = false;

        while (!converged)
        {
            clusters.Clear();
            for (int i = 0; i < data.Count; i++)
                clusters.Add(-1);

            for (int i = 0; i < data.Count; i++)
            {
                int clusterIndex = AssignToCluster(data[i]);
                clusters[i] = clusterIndex;
            }

            converged = UpdateCentroids(data, clusters);
        }

        return clusters;
    }

    private void InitializeCentroids(List<double[]> data)
    {
        Random random = new Random();

        for (int i = 0; i < k; i++)
        {
            int index = random.Next(data.Count);
            centroids.Add(data[index]);
        }
    }

    private int AssignToCluster(double[] point)
    {
        double minDistance = CalculateDistance(point, centroids[0]);
        int clusterIndex = 0;

        for (int i = 1; i < centroids.Count; i++)
        {
            double distance = CalculateDistance(point, centroids[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                clusterIndex = i;
            }
        }

        return clusterIndex;
    }

    private bool UpdateCentroids(List<double[]> data, List<int> clusters)
    {
        bool converged = true;

        for (int i = 0; i < k; i++)
        {
            double[] centroid = new double[data[0].Length];
            int count = 0;

            for (int j = 0; j < data.Count; j++)
            {
                if (clusters[j] == i)
                {
                    for (int d = 0; d < data[0].Length; d++)
                    {
                        centroid[d] += data[j][d];
                    }
                    count++;
                }
            }

            if (count > 0)
            {
                for (int d = 0; d < data[0].Length; d++)
                {
                    centroid[d] /= count;
                }

                if (!ArraysEqual(centroid, centroids[i]))
                {
                    centroids[i] = centroid;
                    converged = false;
                }
            }
        }

        return converged;
    }

    private double CalculateDistance(double[] pointA, double[] pointB)
    {
        double sum = 0.0;

        for (int i = 0; i < pointA.Length; i++)
        {
            double diff = pointA[i] - pointB[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }

    private bool ArraysEqual(double[] arrayA, double[] arrayB)
    {
        if (arrayA.Length != arrayB.Length)
        {
            return false;
        }

        for (int i = 0; i < arrayA.Length; i++)
        {
            if (arrayA[i] != arrayB[i])
            {
                return false;
            }
        }

        return true;
    }
}