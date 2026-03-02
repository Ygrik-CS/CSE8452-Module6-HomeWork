using System;

namespace  Strategy
{
    public interface IPaymentStrategy
    {
        void Pay(double val);
    }

    public class CardPayment : IPaymentStrategy
    {
        public void Pay(double val)
        {
            Console.WriteLine($"Оплата картой: {val}");
        }
    }

    public class PaypalTransfer : IPaymentStrategy
    {
        public void Pay(double val)
        {
            Console.WriteLine($"Перевод paypal: {val}");
        }
    }

    public class CryptoTransaction : IPaymentStrategy
    {
        public void Pay(double val)
        {
            Console.WriteLine($"Крипта: {val}");
        }
    }

    public class PaymentContext
    {
        private IPaymentStrategy _method;

        public void SetStrategy(IPaymentStrategy m)
        {
            _method = m;
        }

        public void ProcessPayment(double val)
        {
            if (_method != null)
            {
                _method.Pay(val);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new PaymentContext();
            Console.WriteLine("Введите сумму:");
            double s = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Способ оплаты (1 - карта, 2 - paypal, 3 - крипта):");
            string choice = Console.ReadLine();

            if (choice == "1") ctx.SetStrategy(new CardPayment());
            else if (choice == "2") ctx.SetStrategy(new PaypalTransfer());
            else if (choice == "3") ctx.SetStrategy(new CryptoTransaction());

            ctx.ProcessPayment(s);
        }
    }
}









namespace Observer
{
    public interface IObserver
    {
        void Update(string n, double v);
    }

    public interface ISubject
    {
        void Subscribe(IObserver o);
        void Unsubscribe(IObserver o);
        void Notify(string n, double v);
    }

    public class CurrencyExchange : ISubject
    {
        private List<IObserver> _list = new List<IObserver>();
        private Dictionary<string, double> _rates = new Dictionary<string, double>();

        public void Subscribe(IObserver o)
        {
            _list.Add(o);
        }

        public void Unsubscribe(IObserver o)
        {
            _list.Remove(o);
        }

        public void Notify(string n, double v)
        {
            foreach (var item in _list)
            {
                item.Update(n, v);
            }
        }

        public void SetRate(string n, double v)
        {
            _rates[n] = v;
            Notify(n, v);
        }
    }

    public class BankScreen : IObserver
    {
        public void Update(string n, double v)
        {
            Console.WriteLine($"Экран: {n} = {v}");
        }
    }

    public class MobileApp : IObserver
    {
        public void Update(string n, double v)
        {
            if (v < 100)
            {
                Console.WriteLine($"Приложение: {n} упал");
            }
        }
    }

    public class DbLog : IObserver
    {
        public void Update(string n, double v)
        {
            Console.WriteLine($"База: {n} -> {v}");
        }
    }

    class Program
    {
        static void Main1(string[] args)
        {
            var ex = new CurrencyExchange();
            var screen = new BankScreen();
            var app = new MobileApp();
            var log = new DbLog();

            ex.Subscribe(screen);
            ex.Subscribe(app);
            ex.Subscribe(log);

            ex.SetRate("USD", 95.6);
        
            ex.Unsubscribe(screen);
        
            ex.SetRate("USD", 92.1);
            ex.SetRate("EUR", 106.0);
        }
    }
}
