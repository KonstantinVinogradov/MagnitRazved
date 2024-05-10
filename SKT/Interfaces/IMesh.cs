using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
    public interface IMesh
    {
        IEnumerable<Element> Elements { get; }
        IList<Vector3D> Points { get; }
        Vector3D GetElementCenter(Element elem);
        double GetElementMeasure(Element elem);

    }
    public class Element
    {
        public Element(int[] vernums, int materialNumber)
        {
            Vernums = vernums;
            MaterialNumber = materialNumber;
        }
        public int[] Vernums { get; }
        public int MaterialNumber { get; }
    }
    public class Material
    {
        public double I { get; set; }
        public Vector3D P { get; set; }
    }
    public class MeshBuilder
    {
        /// <summary>
        /// Тут точки должны быть в определенном порядке
        /// </summary>
        public IList<Vector3D> PointsBase { get; } = new List<Vector3D>();
        /// <summary>
        /// Количество узлов
        /// </summary>
        public int Xn { get; set; }
        public int Yn { get; set; }
        public int Zn { get; set; }

        public IList<Domain> Domains = new List<Domain>();
        public IMesh Build()
        {
            int[,,] domainInfo = new int[Xn - 1, Yn - 1, Zn - 1];
            foreach (var domain in Domains)
            {
                //for (int i = domain.FromX; i < domain.ToX; i++)
                //    for (int j = domain.FromY; j < domain.ToY; j++)
                //        for (int k = domain.FromZ; k < domain.ToZ; k++)
                //            //domainInfo[i, j, k] = domain.MaterialNum;
            }
            List<Element> elements = [];
            for (int i = 0; i < Xn - 1; i++)
            {
                for (int j = 0; j < Yn - 1; j++)
                {
                    for (int k = 0; k < Zn - 1; k++)
                    {

                    }
                }

            }
            throw new NotImplementedException();
        }
    }
    public class Domain
    {
        public int FromX { get; set; }
        public int ToX { get; set; }
        public int NX { get; set; }
        public int FromY { get; set; }
        public int ToY { get; set; }
        public int NY { get; set; }
        public int FromZ { get; set; }
        public int ToZ { get; set; }
        /// <summary>
        /// на сколько элементов разбить домен (1 ничего не изменит)
        /// </summary>
        public int NZ { get; set; }
    }
}
