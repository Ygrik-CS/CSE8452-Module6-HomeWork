using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SingletonTask
{
    class ConfigurationManager
    {
        private static ConfigurationManager _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, string> _settings = new Dictionary<string, string>();
        private string _path = "config.txt";

        private ConfigurationManager()
        {
        }

        public static ConfigurationManager GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new ConfigurationManager();
            }

            return _instance;
        }

        public void LoadSettings()
        {
            if (!File.Exists(_path)) return;

            var lines = File.ReadAllLines(_path);
            foreach (var line in lines)
            {
                var p = line.Split('=');
                if (p.Length == 2)
                {
                    //p[0], p[1]
                    _settings[p[0]] = p[1];
                }
            }
        }

        public void SaveSettings()
        {
            var list = new List<string>();
            foreach (var item in _settings)
            {
                list.Add($"{item.Key}={item.Value}");
            }

            File.WriteAllLines(_path, list);
        }

        public void LoadFromDatabase(string connStr)
        {
            throw new NotImplementedException();
        }

        public string GetSetting(string key)
        {
            if (!_settings.ContainsKey(key))
                throw new Exception("key not found");

            return _settings[key];
        }

        public void SetSetting(string key, string val)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("bad key");

            _settings[key] = val;
        }
    }

    class Program
    {
        static void Main1()
        {
            Thread t1 = new Thread(Test);
            Thread t2 = new Thread(Test);
            Thread t3 = new Thread(Test);

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            var cfg = ConfigurationManager.GetInstance();
            cfg.SetSetting("Theme", "Blue");
            cfg.SetSetting("ShowNotifications", "true");
            cfg.SetSetting("Language", "Ru"); // и другие
            cfg.SaveSettings();

            try
            {
                Console.WriteLine("theme: " + cfg.GetSetting("Theme"));
                Console.WriteLine("Notifications: " + cfg.GetSetting("ShowNotifications"));
                Console.WriteLine("Wrong Key: " + cfg.GetSetting("wrong_key"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("err: " + ex.Message);
            }

            Console.ReadLine();
        }

        static void Test()
        {
            var inst = ConfigurationManager.GetInstance();
            Console.WriteLine($"t:{Thread.CurrentThread.ManagedThreadId} hash:{inst.GetHashCode()}");
        }
    }
}


namespace BuilderTask
{
    class Report
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public string Footer { get; set; }


        public string Style { get; set; }


        public void EditContent(string newContent)
        {
            Content = newContent;
        }
    }


    interface IReportBuilder
    {
        void SetHeader(string header);
        void SetContent(string content);
        void SetFooter(string footer);
        void SetStyle(string style);
        Report GetReport();
    }


    class TextReportBuilder : IReportBuilder
    {
        private Report _report = new Report();

        public void SetHeader(string header)
        {
            _report.Header = header;
        }

        public void SetContent(string content)
        {
            _report.Content = content;
        }

        public void SetFooter(string footer)
        {
            _report.Footer = footer;
        }

        public void SetStyle(string style)
        {
            _report.Style = style;
        }

        public Report GetReport()
        {
            return _report;
        }
    }


    class HtmlReportBuilder : IReportBuilder
    {
        private Report _report = new Report();

        public void SetHeader(string header)
        {
            _report.Header = "<h1>" + header + "</h1>";
        }

        public void SetContent(string content)
        {
            _report.Content = "<p>" + content + "</p>";
        }

        public void SetFooter(string footer)
        {
            _report.Footer = "<footer>" + footer + "</footer>";
        }

        public void SetStyle(string style)
        {
            _report.Style = style;
        }

        public Report GetReport()
        {
            return _report;
        }
    }


    class XmlReportBuilder : IReportBuilder
    {
        private Report _report = new Report();

        public void SetHeader(string header)
        {
            _report.Header = "<header>" + header + "</header>";
        }

        public void SetContent(string content)
        {
            _report.Content = "<content>" + content + "</content>";
        }

        public void SetFooter(string footer)
        {
            _report.Footer = "<footer>" + footer + "</footer>";
        }

        public void SetStyle(string style)
        {
            _report.Style = style;
        }

        public Report GetReport()
        {
            return _report;
        }
    }


    class ReportDirector
    {
        public void ConstructReport(IReportBuilder builder)
        {
            builder.SetHeader("Заголовок");
            builder.SetContent("Текст отчета");
            builder.SetFooter("Подвал");
            builder.SetStyle("Базовый стиль");
        }
    }


    class Program
    {
        static void Main2()
        {
            ReportDirector director = new ReportDirector();

            IReportBuilder textBuilder = new TextReportBuilder();
            director.ConstructReport(textBuilder);
            Report rep1 = textBuilder.GetReport();

            Console.WriteLine(rep1.Header);
            Console.WriteLine(rep1.Content);
            Console.WriteLine(rep1.Footer);
            Console.WriteLine();

            IReportBuilder htmlBuilder = new HtmlReportBuilder();
            director.ConstructReport(htmlBuilder);
            Report rep2 = htmlBuilder.GetReport();

            Console.WriteLine(rep2.Header);
            Console.WriteLine(rep2.Content);
            Console.WriteLine(rep2.Footer);
            Console.WriteLine();

            IReportBuilder xmlBuilder = new XmlReportBuilder();
            director.ConstructReport(xmlBuilder);
            Report rep3 = xmlBuilder.GetReport();

            rep3.EditContent("<content>Новый изменение текст</content>");
            Console.WriteLine(rep3.Content);
        }
    }
}


namespace PrototypeTask
{
    class Product : ICloneable
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    class Discount : ICloneable
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    class Order : ICloneable
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public double Delivery { get; set; }
        public List<Discount> Discounts { get; set; } = new List<Discount>();
        public string PayMethod { get; set; }

        public object Clone()
        {
            Order clone = new Order();
            clone.Delivery = this.Delivery;
            clone.PayMethod = this.PayMethod;

            foreach (var p in this.Products)
            {
                clone.Products.Add((Product)p.Clone());
            }

            foreach (var d in this.Discounts)
            {
                clone.Discounts.Add((Discount)d.Clone());
            }

            return clone;
        }
    }

    class Program
    {
        static void Main()
        {
            Order ord1 = new Order();
            ord1.PayMethod = "Method 1";
            ord1.Delivery = 100;

            Product p1 = new Product { Name = "Prod 1", Price = 10, Amount = 1 };
            ord1.Products.Add(p1);

            Discount d1 = new Discount { Name = "Disc 1", Value = 5 };
            ord1.Discounts.Add(d1);

            Order ord2 = (Order)ord1.Clone();

            ord2.PayMethod = "Method 2";
            ord2.Products[0].Name = "Prod 2";


            Console.WriteLine(ord1.PayMethod + " | " + ord1.Products[0].Name);
            Console.WriteLine(ord2.PayMethod + " | " + ord2.Products[0].Name);
        }
    }
}