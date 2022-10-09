using Cars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqXmlModule
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateXml("fuel.xml");
            CreateXml("fuel_with_namespace.xml", true);
            //QueryXml();
            QueryXmlWithNamespace();







            // Explicit Xml creation
            //foreach (var record in records)
            //{
            //    var car = new XElement("Car");
            //    var name = new XElement("Name", record.Name);
            //    var combined = new XElement("Combined", record.Combined);

            //    car.Add(name);
            //    car.Add(combined);
            //    cars.Add(car);
            //}
            //document.Add(cars);
            //document.Save("fuel.xml");
        }

        private static void QueryXmlWithNamespace()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel_with_namespace.xml");

            var xmlQueryWithNameSpace =
                from element in document.Element(ns + "Cars")?.Elements(ex + "Car")
                                                        ?? Enumerable.Empty<XElement>()
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in xmlQueryWithNameSpace)
            {
                Console.WriteLine(name);
            }
        }

        private static void QueryXml()
        {
            var document = XDocument.Load("fuel.xml");
            var xmlQuery =
                from element in document.Element("Cars").Elements("Car")
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;

            foreach (var name in xmlQuery)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXml(string fileName, bool useNamespaces = false)
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";

            var records = ProcessCars("fuel.csv");

            var document = new XDocument();
            var cars = new XElement(useNamespaces ? ns + "Cars" : "Cars",

                from record in records
                select new XElement(useNamespaces ? ex + "Car" : "" + "Car",
                                new XAttribute("Name", record.Name),
                                new XAttribute("Combined", record.Combined),
                                new XAttribute("Manufacturer", record.Manufacturer))
                );

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
            document.Add(cars);
            document.Save(fileName);
        }

        private static List<Car> ProcessCars(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                   File.ReadAllLines(path)
                       .Where(l => l.Length > 1)
                       .Select(l =>
                       {
                           var columns = l.Split(',');
                           return new Manufacturer
                           {
                               Name = columns[0],
                               Headquarters = columns[1],
                               Year = int.Parse(columns[2])
                           };
                       });
            return query.ToList();
        }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
