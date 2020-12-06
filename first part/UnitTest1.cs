using Microsoft.VisualStudio.TestTools.UnitTesting;
using ver1;
using System;
using System.IO;

namespace ver1UnitTests
{

    public class ConsoleRedirectionToStringWriter : IDisposable
    {
        private StringWriter stringWriter;
        private TextWriter originalOutput;

        public ConsoleRedirectionToStringWriter()
        {
            stringWriter = new StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(stringWriter);
        }

        public string GetOutput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }
    }


    [TestClass]
    public class UnitTestCopier
    {
        [TestMethod]
        public void Copier_GetState_StateOff()
        {
            var copier = new Copier();
            copier.PowerOff();

            Assert.AreEqual(IDevice.State.off, copier.GetState());
        }

        [TestMethod]
        public void Copier_GetState_StateOn()
        {
            var copier = new Copier();
            copier.PowerOn();

            Assert.AreEqual(IDevice.State.on, copier.GetState());
        }


        // weryfikacja, czy po wywołaniu metody `Print` i włączonej kopiarce w napisie pojawia się słowo `Print`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_Print_DeviceOn()
        {
            var copier = new Copier();
            copier.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1 = new PDFDocument("aaa.pdf");
                copier.Print(in doc1);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Print` i wyłączonej kopiarce w napisie NIE pojawia się słowo `Print`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_Print_DeviceOff()
        {
            var copier = new Copier();
            copier.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1 = new PDFDocument("aaa.pdf");
                copier.Print(in doc1);
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Scan` i wyłączonej kopiarce w napisie NIE pojawia się słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_Scan_DeviceOff()
        {
            var copier = new Copier();
            copier.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                copier.Scan(out doc1);
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Scan"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Scan` i wyłączonej kopiarce w napisie pojawia się słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_Scan_DeviceOn()
        {
            var copier = new Copier();
            copier.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                copier.Scan(out doc1);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy wywołanie metody `Scan` z parametrem określającym format dokumentu
        // zawiera odpowiednie rozszerzenie (`.jpg`, `.txt`, `.pdf`)
        [TestMethod]
        public void Copier_Scan_FormatTypeDocument()
        {
            var copier = new Copier();
            copier.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                copier.Scan(out doc1, formatType: IDocument.FormatType.JPG);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".jpg"));

                copier.Scan(out doc1, formatType: IDocument.FormatType.TXT);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".txt"));

                copier.Scan(out doc1, formatType: IDocument.FormatType.PDF);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".pdf"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }


        // weryfikacja, czy po wywołaniu metody `ScanAndPrint` i wyłączonej kopiarce w napisie pojawiają się słowa `Print`
        // oraz `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_ScanAndPrint_DeviceOn()
        {
            var copier = new Copier();
            copier.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                copier.ScanAndPrint();
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `ScanAndPrint` i wyłączonej kopiarce w napisie NIE pojawia się słowo `Print`
        // ani słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void Copier_ScanAndPrint_DeviceOff()
        {
            var copier = new Copier();
            copier.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                copier.ScanAndPrint();
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        [TestMethod]
        public void Copier_PrintCounter()
        {
            var copier = new Copier();
            copier.PowerOn();

            IDocument doc1 = new PDFDocument("aaa.pdf");
            copier.Print(in doc1);
            IDocument doc2 = new TextDocument("aaa.txt");
            copier.Print(in doc2);
            IDocument doc3 = new ImageDocument("aaa.jpg");
            copier.Print(in doc3);

            copier.PowerOff();
            copier.Print(in doc3);
            copier.Scan(out doc1);
            copier.PowerOn();

            copier.ScanAndPrint();
            copier.ScanAndPrint();

            // 5 wydruków, gdy urządzenie włączone
            Assert.AreEqual(5, copier.PrintCounter);
        }

       [TestMethod]
        public void Copier_ScanCounter()
        {
            var copier = new Copier();
            copier.PowerOn();

            IDocument doc1;
            copier.Scan(out doc1);
            IDocument doc2;
            copier.Scan(out doc2);

            IDocument doc3 = new ImageDocument("aaa.jpg");
            copier.Print(in doc3);

            copier.PowerOff();
            copier.Print(in doc3);
            copier.Scan(out doc1);
            copier.PowerOn();

            copier.ScanAndPrint();
            copier.ScanAndPrint();

            // 4 skany, gdy urządzenie włączone
            Assert.AreEqual(4, copier.ScanCounter);
        }

        [TestMethod]
        public void Copier_PowerOnCounter()
        {
            var copier = new Copier();
            copier.PowerOn();
            copier.PowerOn();
            copier.PowerOn();

            IDocument doc1;
            copier.Scan(out doc1);
            IDocument doc2;
            copier.Scan(out doc2);

            copier.PowerOff();
            copier.PowerOff();
            copier.PowerOff();
            copier.PowerOn();

            IDocument doc3 = new ImageDocument("aaa.jpg");
            copier.Print(in doc3);

            copier.PowerOff();
            copier.Print(in doc3);
            copier.Scan(out doc1);
            copier.PowerOn();

            copier.ScanAndPrint();
            copier.ScanAndPrint();

            // 3 włączenia
            Assert.AreEqual(3, copier.Counter);
        }

    }

    [TestClass]
    public class UnitTestMultifunctionalDevice
    {
        [TestMethod]
        public void MultifunctionalDevice_GetState_StateOff()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOff();

            Assert.AreEqual(IDevice.State.off, multiDev.GetState());
        }

        [TestMethod]
        public void MultifunctionalDevice_GetState_StateOn()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            Assert.AreEqual(IDevice.State.on, multiDev.GetState());
        }


        // weryfikacja, czy po wywołaniu metody `Print` i włączonym multiurządzeniu w napisie pojawia się słowo `Print`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_Print_DeviceOn()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1 = new PDFDocument("aaa.pdf");
                multiDev.Print(in doc1);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Print` i wyłączonym multiurządzeniu w napisie NIE pojawia się słowo `Print`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_Print_DeviceOff()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1 = new PDFDocument("aaa.pdf");
                multiDev.Print(in doc1);
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Scan` i wyłączonym multiurządzeniu w napisie NIE pojawia się słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_Scan_DeviceOff()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                multiDev.Scan(out doc1);
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Scan"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `Scan` i wyłączonym multiurządzeniu w napisie pojawia się słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_Scan_DeviceOn()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                multiDev.Scan(out doc1);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy wywołanie metody `Scan` z parametrem określającym format dokumentu
        // zawiera odpowiednie rozszerzenie (`.jpg`, `.txt`, `.pdf`)
        [TestMethod]
        public void MultifunctionalDevice_Scan_FormatTypeDocument()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                IDocument doc1;
                multiDev.Scan(out doc1, formatType: IDocument.FormatType.JPG);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".jpg"));

                multiDev.Scan(out doc1, formatType: IDocument.FormatType.TXT);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".txt"));

                multiDev.Scan(out doc1, formatType: IDocument.FormatType.PDF);
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains(".pdf"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }


        // weryfikacja, czy po wywołaniu metody `ScanAndPrint` i wyłączonym multiurządzeniu w napisie pojawiają się słowa `Print`
        // oraz `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_ScanAndPrint_DeviceOn()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                multiDev.ScanAndPrint();
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsTrue(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        // weryfikacja, czy po wywołaniu metody `ScanAndPrint` i wyłączonym multiurządzeniu w napisie NIE pojawia się słowo `Print`
        // ani słowo `Scan`
        // wymagane przekierowanie konsoli do strumienia StringWriter
        [TestMethod]
        public void MultifunctionalDevice_ScanAndPrint_DeviceOff()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOff();

            var currentConsoleOut = Console.Out;
            currentConsoleOut.Flush();
            using (var consoleOutput = new ConsoleRedirectionToStringWriter())
            {
                multiDev.ScanAndPrint();
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Scan"));
                Assert.IsFalse(consoleOutput.GetOutput().Contains("Print"));
            }
            Assert.AreEqual(currentConsoleOut, Console.Out);
        }

        [TestMethod]
        public void MultifunctionalDevice_PrintCounter()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            IDocument doc1 = new PDFDocument("aaa.pdf");
            multiDev.Print(in doc1);
            IDocument doc2 = new TextDocument("aaa.txt");
            multiDev.Print(in doc2);
            IDocument doc3 = new ImageDocument("aaa.jpg");
            multiDev.Print(in doc3);

            multiDev.PowerOff();
            multiDev.Print(in doc3);
            multiDev.Scan(out doc1);
            multiDev.PowerOn();

            multiDev.ScanAndPrint();
            multiDev.ScanAndPrint();

            // 5 wydruków, gdy urządzenie włączone
            Assert.AreEqual(5, multiDev.PrintCounter);
        }

        [TestMethod]
        public void MultifunctionalDevice_ScanCounter()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            IDocument doc1;
            multiDev.Scan(out doc1);
            IDocument doc2;
            multiDev.Scan(out doc2);

            IDocument doc3 = new ImageDocument("aaa.jpg");
            multiDev.Print(in doc3);

            multiDev.PowerOff();
            multiDev.Print(in doc3);
            multiDev.Scan(out doc1);
            multiDev.PowerOn();

            multiDev.ScanAndPrint();
            multiDev.ScanAndPrint();

            // 4 skany, gdy urządzenie włączone
            Assert.AreEqual(4, multiDev.ScanCounter);
        }

        [TestMethod]
        public void MultifunctionalDevice_PowerOnCounter()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();
            multiDev.PowerOn();
            multiDev.PowerOn();

            IDocument doc1;
            multiDev.Scan(out doc1);
            IDocument doc2;
            multiDev.Scan(out doc2);

            multiDev.PowerOff();
            multiDev.PowerOff();
            multiDev.PowerOff();
            multiDev.PowerOn();

            IDocument doc3 = new ImageDocument("aaa.jpg");
            multiDev.Print(in doc3);

            multiDev.PowerOff();
            multiDev.Print(in doc3);
            multiDev.Scan(out doc1);
            multiDev.PowerOn();

            multiDev.ScanAndPrint();
            multiDev.ScanAndPrint();

            // 3 włączenia
            Assert.AreEqual(3, multiDev.Counter);
        }

        [TestMethod]
        public void MultifunctionalDevice_SendCounter()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();

            IDocument doc1;
            multiDev.Scan(out doc1);
            IDocument doc2;
            multiDev.Scan(out doc2);

            multiDev.Send(doc1, 666818777);

            IDocument doc3 = new ImageDocument("aaa.jpg");
            multiDev.Print(in doc3);

            multiDev.PowerOff();
            multiDev.Print(in doc3);
            multiDev.Send(doc1, 666817777);
            multiDev.Scan(out doc1);
            multiDev.PowerOn();

            multiDev.ScanAndPrint();
            multiDev.ScanAndPrint();
            multiDev.Send(doc2, 666817777);
            multiDev.Send(doc3, 666888777);

            // 3 wysłane, gdy urządzenie włączone
            Assert.AreEqual(3, multiDev.SendCounter);
        }

        [TestMethod]
        public void MultifunctionalDevice_RecieveCounter()
        {
            var multiDev = new MultifunctionalDevice();
            multiDev.PowerOn();
            IDocument doc0;
            int number;
            multiDev.Recieve(out doc0, out number);
            IDocument doc1;
            multiDev.Scan(out doc1);
            IDocument doc2;
            multiDev.Scan(out doc2);

            multiDev.Send(doc1, 666818777);

            IDocument doc3 = new ImageDocument("aaa.jpg");
            multiDev.Print(in doc3);

            multiDev.PowerOff();

            multiDev.Recieve(out doc3, out number);
            multiDev.Print(in doc3);
            multiDev.Send(doc1, 666817777);
            multiDev.Scan(out doc1);
            multiDev.PowerOn();

            multiDev.ScanAndPrint();
            multiDev.ScanAndPrint();
            multiDev.Send(doc2, 666817777);
            multiDev.Send(doc3, 666888777);
            multiDev.Recieve(out doc2, out number);

            // 3 wysłane, gdy urządzenie włączone
            Assert.AreEqual(2, multiDev.RecieveCounter);
        }

    }
}
