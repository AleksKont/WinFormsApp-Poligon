using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poligon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poligon.Tests
{
    [TestClass()]
    public class PoligonTests
    {
        [TestMethod()]
        public void IsPointInPoligonTest()
        {


            Poligon p = new Poligon();

            var tst1 = p.IsPointInPoligon(new Point() { X = 0, Y = 0 }, new[]
            {
                new Point { X = 1, Y = 0 },
                new Point { X = 0, Y = 1 },
                new Point { X = -1, Y = -1 },
            });

            Assert.AreEqual(tst1, true);

            var tst2 = p.IsPointInPoligon(new Point() { X = 1, Y = 1 }, new[]
            {
                new Point { X = 1, Y = 0 },
                new Point { X = 0, Y = 1 },
                new Point { X = -1, Y = -1 },
            });

            Assert.AreEqual(tst2, false);


            var tst3 = p.IsPointInPoligon(new Point() { X = -7, Y = 6 }, new[]
            {
                new Point { X = -6, Y = 4 },
                new Point { X = -8, Y = 6 },
                new Point { X = -7, Y = 9 },
            });

            Assert.AreEqual(tst3, true);
        }
    }
}