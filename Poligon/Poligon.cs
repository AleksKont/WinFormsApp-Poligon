﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

[assembly: InternalsVisibleTo("PoligonTests")]

namespace Poligon
{
    
    public class Poligon : IPoligon
    {
        private List<Point> _lst;
        private Stack<List<Point>> _stack;
        private List<(Point, Point, Point)> mesh;

        public List<(Point, Point, Point)> CreateMesh(IEnumerable<Point> points, Action<Point, Point, Point>? callback = null)
        {
            _lst = points.ToList();
            mesh = new List<(Point, Point, Point)>();
            _stack = new();
            Mesh(callback);
            return mesh;
        }

        private void Mesh(Action<Point, Point, Point>? callback = null)
        {
            bool isConvex = CheckConvex(_lst);
            while (_lst.Count > 2 || _stack.Count > 0)
            {
                if (_lst.Count == 2)
                {
                    _lst = _stack.Pop();
                    isConvex = CheckConvex(_lst);
                }

                if (isConvex) // в выпуклом разбиение делаем за О(n)
                {
                    var v = GetIndexOfLeftBottomPoint();
                    while (_lst.Count > 2)
                    {
                        var u = Next(v);
                        var w = Prev(v);

                        if (ExistVertexInside(v, u, w, out List<int> points))
                        {
                            var vi = GetIndexOfNearestPoint(_lst[u], _lst[w], points);
                            var (lst1, lst2) = Split(v, vi);
                            _stack.Push(lst2);
                            _lst = lst1;
                        }
                        else
                        {
                            mesh.Add((_lst[v], _lst[u], _lst[w]));
                            callback?.Invoke(_lst[v], _lst[u], _lst[w]);

                            // в выпуклом многоугольнике выбираем правую нижнюю только из двух смежных с предыдущей правой нижней
                            var nextV = GetIndexOfLeftBottomPoint(u, w);
                            _lst.RemoveAt(v);

                            // если правой нижней вершина стала следующая в списке, то уменьшаем ее индекс на 1 после удаления v
                            if (v < nextV)
                                nextV--;

                            v= nextV;
                        }
                    }
                }
                else // в невыпуклом разбиение делаем за О(n^2)
                {
                    var v = GetIndexOfLeftBottomPoint(); // можно сделать за О(n)
                    var u = Next(v);
                    var w = Prev(v);

                    if (ExistVertexInside(v, u, w, out List<int> points))
                    {
                        var vi = GetIndexOfNearestPoint(_lst[u], _lst[w], points);
                        var (lst1, lst2) = Split(v, vi);
                        _stack.Push(lst2);
                        _lst = lst1;
                    }
                    else
                    {
                        mesh.Add((_lst[v], _lst[u], _lst[w]));
                        callback?.Invoke(_lst[v], _lst[u], _lst[w]);
                        _lst.RemoveAt(v);
                        isConvex = CheckConvex(_lst);
                    }
                }
            }
        }

        private bool CheckConvex(List<Point> lst)
        {
            int sum = 0;
            var size = lst.Count;
            var p2 = lst[^2];
            var p1 = lst[^1];

            for (int i = 0; i < size; i++)
            {
                Point p0 = lst[i];
                int s = GetSign(p2, p1, p0);
                sum += s;
                p2 = p1;
                p1= p0;
            }

            return Math.Abs(sum) == size;
        }

        private int GetSign(Point p1, Point p2, Point p3)
        {
            var ab = new Point() { X = p2.X - p1.X, Y = p2.Y - p1.Y };
            var bc = new Point() { X = p3.X - p2.X, Y = p3.Y - p2.Y };

            return Math.Sign(ab.X * bc.Y - ab.Y * bc.X);
        }

        private int Next(int current) => (current + 1) % _lst.Count;

        private int Prev(int current) => (current + _lst.Count - 1) % _lst.Count;

        private (List<Point> lst1, List<Point> lst2) Split(int v1, int v2)
        {
            var w1 = Math.Min(v1, v2);
            var w2 = Math.Max(v1, v2);

            var lst1 = _lst.GetRange(w1, w2 - w1 + 1);
            var lst2 = _lst.GetRange(w2, _lst.Count - w2);
            lst2.AddRange(_lst.GetRange(0, w1 + 1));
            return (lst1, lst2);
        }

        private int GetIndexOfNearestPoint(Point point1, Point point2, List<int> points)
        {
            var abc = GetABC(point1, point2);
            var max0 = GethSquare(abc.A, abc.B, abc.C, _lst[points[0]]);
            var rsl = points[0];
            for (int i = 1;i < points.Count;i++)
            {
                var maxi = GethSquare(abc.A, abc.B, abc.C, _lst[0]);
                if (max0 < maxi)
                {
                    max0 = maxi;
                    rsl = i;
                }
            }
            return rsl;
        }

        private static (float A, float B, float C) GetABC(Point p1, Point p2)
        {
            var A = p1.Y - p2.Y;
            var B = p2.X - p1.X;
            var C = -p1.X * A - p1.Y * B;
            return (A, B, C);
        }

        //GethSquare работает быстрее
        private static float GethSquare(float A, float B, float C, Point p0)
        {
            var s = A * p0.X + B * p0.Y + C;
            return s * s / (A * A + B * B);
        }


        // O(n)
        private bool ExistVertexInside(int v, int u, int w, out List<int> points)
        {
            var rsl = false;
            points = new();
            for (int i=0; i < _lst.Count; i++)
            {
                if (i == v || i == u || i == w)
                    continue;

                var p = _lst[i];
                if (IsPointInPoligon(p, new[] {_lst[v], _lst[u], _lst[w]}))
                {
                    points.Add(i);
                    rsl = true;
                }
            }
            return rsl;
        }

        internal bool IsPointInPoligon(Point p, Point[] poligon)
        {
            int sum = 0;
            var size = poligon.Length;
            var p2 = poligon[^1];

            for (int i = 0; i < size; i++)
            {
                Point p1 = poligon[i];
                Point v1 = new Point() { X = p1.X - p.X, Y = p1.Y - p.Y };
                Point v2 = new Point() { X = p2.X - p1.X, Y = p2.Y - p1.Y };
                Point n2 = new Point() { X = v2.Y, Y = -v2.X }; // нормаль к v2 (стороне треугольника)

                // если точка внутри треугольника =>
                // скалярное произведение вектора от точки к вершине на нормаль к стороне одного знака для всех сторон
                var sign = Math.Sign(dot(v1, n2));
                sum += sign;

                p2 = p1;
            }

            return Math.Abs(sum) == 3;
        }

        private float dot(Point v1, Point v2) => v1.X * v2.X + v1.Y * v2.Y;


        private int GetIndexOfLeftBottomPoint()
        {
            // можно оптимизировать и сделать за O(n)
            return _lst.IndexOf(_lst.OrderBy(w => w.X).ThenBy(w => w.Y).First());
        }
        private int GetIndexOfLeftBottomPoint(int u, int w)
        {
            if (_lst[u].X < _lst[w].X)
                return u;
            if (_lst[u].X > _lst[w].X)
                return w;
            if (_lst[u].Y < _lst[w].Y)
                return u;
            if (_lst[u].Y > _lst[w].Y)
                return w;
            return u;
        }
    }
}