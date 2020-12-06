using System;

namespace ver1
{

    class Program
    {
        static void Main()
        {
            var xeroxMachine = new Copier();
            xeroxMachine.PowerOn();
            IDocument doc1 = new PDFDocument("aaa.pdf");
            xeroxMachine.Print(in doc1);

            IDocument doc2;
            xeroxMachine.Scan(out doc2);

            xeroxMachine.ScanAndPrint();
            xeroxMachine.ScanAndPrint();

            System.Console.WriteLine("turn on: " + xeroxMachine.Counter);
            System.Console.WriteLine("prints: " + xeroxMachine.PrintCounter);
            System.Console.WriteLine("scans: " + xeroxMachine.ScanCounter);

/*            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();
            IDocument doc4 = new PDFDocument("aaa.pdf");
            multiDev.Scan(out doc4);
            IDocument doc5;
            int number;
            multiDev.Recieve(out doc5, out number);
            System.Console.WriteLine($"doc: {doc5.GetFileName()} and number: {number}");

            multiDev.Send(doc2, 666777888);*/
        }
    }
    public interface IDevice
    {
        enum State {on, off};

        void PowerOn(); // uruchamia urządzenie, zmienia stan na `on`
        void PowerOff(); // wyłącza urządzenie, zmienia stan na `off
        State GetState(); // zwraca aktualny stan urządzenia

        int Counter {get;}  // zwraca liczbę charakteryzującą eksploatację urządzenia,
                            // np. liczbę uruchomień, liczbę wydrukow, liczbę skanów, ...
    }

    public abstract class BaseDevice : IDevice
    {
        protected IDevice.State state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public void PowerOff()
        {
            state = IDevice.State.off;
            Console.WriteLine("... Device is off !");
        }

        public void PowerOn()
        {
            if (state.Equals(IDevice.State.off))
            {
                Counter++;
                state = IDevice.State.on;
                Console.WriteLine("Device is on ...");
            }
        }

        public int Counter { get; private set; } = 0;
    }

    public interface IPrinter : IDevice
    {
        /// <summary>
        /// Dokument jest drukowany, jeśli urządzenie włączone. W przeciwnym przypadku nic się nie wykonuje
        /// </summary>
        /// <param name="document">obiekt typu IDocument, różny od `null`</param>
        void Print(in IDocument document);
    }

    public class Printer : IPrinter
    {
        public int Counter { get; private set; } = 0;
        public int PrintCounter { get; private set; } = 0;
        private IDevice.State state = IDevice.State.off;
        

        public IDevice.State GetState() => state;

        public void PowerOff()
        {
            state = IDevice.State.off;
            Console.WriteLine("... Printer is off !");
        }

        public void PowerOn()
        {
            if (state.Equals(IDevice.State.off)) Counter++;
            state = IDevice.State.on;
            Console.WriteLine("Printer is on ...");
        }

        public void Print(in IDocument document)
        {
            if (state.Equals(IDevice.State.off))
            {
                return;
            }
            PrintCounter++;
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            var docName = document.GetFileName();
            Console.WriteLine(string.Format("{0} Print: {1}", date, docName));
        }
    }

    public interface IScanner : IDevice
    {
        // dokument jest skanowany, jeśli urządzenie włączone
        // w przeciwnym przypadku nic się dzieje
        void Scan(out IDocument document, IDocument.FormatType formatType);
    }

    public class Scanner : IScanner
    {
        public int Counter { get; private set; } = 0;
        public int ScanCounter { get; private set; } = 0;
        private IDevice.State state = IDevice.State.off;


        public IDevice.State GetState() => state;

        public void PowerOff()
        {
            state = IDevice.State.off;
            Console.WriteLine("... Scanner is off !");
        }

        public void PowerOn()
        {
            if (state.Equals(IDevice.State.off)) Counter++;
            state = IDevice.State.on;
            Console.WriteLine("Scanner is on ...");
        }

        public void Scan(out IDocument document, IDocument.FormatType formatType)
        {
            ScanCounter++;
            switch (formatType)
            {
                case IDocument.FormatType.PDF:
                    document = new PDFDocument($"PDFScan{ScanCounter}.pdf");
                    break;
                case IDocument.FormatType.JPG:
                    document = new ImageDocument($"ImageScan{ScanCounter}.jpg");
                    break;
                default:
                    document = new TextDocument($"TextScan{ScanCounter}.txt");
                    break;
            }
            if (state.Equals(IDevice.State.off))
            {
                ScanCounter--;
                return;
            }
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            var docName = document.GetFileName();
            Console.WriteLine(string.Format("{0} Scan: {1}", date, docName));
        }
    }

    public interface IFax : IDevice
    {
        void Send(in IDocument document, in int number);
        void Recieve(out IDocument document, out int number);
    }

    public class Fax : IFax
    {
        private IDevice.State state = IDevice.State.off;
        public int SendCounter { get; private set; } = 0;

        public int RecieveCounter { get; private set; } = 0;

        public int Counter { get; private set; } = 0;

        public IDevice.State GetState() => state;

        public void PowerOff()
        {
            state = IDevice.State.off;
            Console.WriteLine("... Fax is off !");
        }

        public void PowerOn()
        {
            if (state.Equals(IDevice.State.off)) Counter++;
            state = IDevice.State.on;
            Console.WriteLine("Fax is on ...");
        }

        public void Recieve(out IDocument document, out int number)
        {
            Random rnd = new Random();
            number = rnd.Next(100000000, 999999999);
            RecieveCounter++;
            document = new ImageDocument($"Recieved{RecieveCounter}.png");
            if (state.Equals(IDevice.State.off))
            {
                RecieveCounter--;
                return;
            }
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            Console.WriteLine(string.Format("{0} Recieved: {1}, from number: {2}", date, document.GetFileName(), number));
        }

        public void Send(in IDocument document, in int number)
        {
            if (state.Equals(IDevice.State.off))
            {
                return;
            }
            SendCounter++;
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            var docName = document.GetFileName();
            Console.WriteLine(string.Format("{0} Sent: {1}, to number: {2}", date, docName, number));
        }
    }

    public class MultifunctionalDevice : Copier, IFax
    {
        public int SendCounter { get; private set; } = 0;
        public int RecieveCounter { get; private set; } = 0;

        public MultifunctionalDevice()
        {

        }

        public void Recieve(out IDocument document, out int number)
        {
            Random rnd = new Random();
            number = rnd.Next(100000000, 999999999);
            RecieveCounter++;
            document = new ImageDocument($"Recieved{RecieveCounter}.png");
            if (state.Equals(IDevice.State.off))
            {
                RecieveCounter--;
                return;
            }
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            Console.WriteLine(string.Format("{0} Recieved: {1}, from number: {2}", date, document.GetFileName(), number));
        }

        public void Send(in IDocument document, in int number)
        {
            if (state.Equals(IDevice.State.off))
            {
                return;
            }
            SendCounter++;
            var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            var docName = document.GetFileName();
            Console.WriteLine(string.Format("{0} Sent: {1}, to number: {2}", date, docName, number));
        }
    }

    public class Copier : BaseDevice
    {
        public int PrintCounter { get; private set; } = 0;
        public int ScanCounter { get; private set; } = 0;
        private IPrinter printer;
        private IScanner scanner;

        public Copier()
        {
            printer = new Printer();
            scanner = new Scanner();
        }

        public new void PowerOff()
        {
            printer.PowerOff();
            scanner.PowerOff();
            state = IDevice.State.off;
        }

        public void Print(in IDocument document)
        {
            if (state.Equals(IDevice.State.on))
            {
                printer.PowerOn();
                printer.Print(document);
                PrintCounter++;
            }
        }

        public void Scan(out IDocument document, IDocument.FormatType formatType)
        {
            document = new PDFDocument("");
            if (state.Equals(IDevice.State.on))
            {
                scanner.PowerOn();
                scanner.Scan(out document, formatType);
                ScanCounter++;
            }
        }

        public void Scan(out IDocument document)
        {
            var defaultDocFormat = IDocument.FormatType.JPG;
            Scan(out document, defaultDocFormat);
        }

        public void ScanAndPrint()
        {
            IDocument document;
            Scan(out document);
            Print(document);
        }
    }

}
