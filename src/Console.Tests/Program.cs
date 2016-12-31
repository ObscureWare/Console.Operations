﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2016 Sebastian Gruchacz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
// <summary>
//   Just some DEMO stuff. Used for visual testing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ConsoleTests
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Schema;

    using Obscureware.Console.Operations;
    using Obscureware.Console.Operations.Styles;
    using Obscureware.Console.Operations.Tables;

    using ObscureWare.Console;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            ConsoleController controller = new ConsoleController();
            
            //helper.ReplaceConsoleColor(ConsoleColor.DarkCyan, Color.Salmon);
            controller.ReplaceConsoleColors(
                new Tuple<ConsoleColor, Color>(ConsoleColor.DarkCyan, Color.Chocolate),
                new Tuple<ConsoleColor, Color>(ConsoleColor.Blue, Color.DodgerBlue),
                new Tuple<ConsoleColor, Color>(ConsoleColor.Yellow, Color.Gold),
                new Tuple<ConsoleColor, Color>(ConsoleColor.DarkBlue, Color.MidnightBlue));

            IConsole console = new SystemConsole(controller, isFullScreen: false);
            ConsoleOperations ops = new ConsoleOperations(console);

            //PrintColorsMessages(console);
            //PrintAllNamedColors(controller, console);
            //PrintFrames(ops, console);
            PrintTables(console);

            console.ReadLine();
        }

        private static void PrintTables(IConsole console)
        {
            var tableFrameColor = new ConsoleFontColor(Color.Silver, Color.Black);
            var tableHeaderColor = new ConsoleFontColor(Color.White, Color.Black);
            var tableOddRowColor = new ConsoleFontColor(Color.DarkGoldenrod, Color.Black);
            var tableEvenRowColor = new ConsoleFontColor(Color.DimGray, Color.Black);

            TableStyle tableStyle = new TableStyle(
                tableFrameColor,
                tableHeaderColor,
                tableOddRowColor,
                tableEvenRowColor,
                @"|-||||-||-|--", // simple, ascii table
                ' ',
                TableOverflowContentBehavior.Ellipsis);

            var headers = new[] {"Row 1", "Longer row 2", "Third row"};
            var values = new[]
            {
                new[] {"1", "2", "3"},
                new[] {"10", "223423", "3"},
                new[] {"1", "2", "3"},
                new[] {"12332 ", "22332423", "3223434234"},
                new[] {"1df ds fsd fsfs fsdf s", "2234  4234 23", "3 23423423"},
            };

            // ops.WriteTabelaricData(5, 5, 50, headers, values, tableStyle);


            console.WriteLine(tableFrameColor, "Small tables");

            DataTable<string> dt = new DataTable<string>(
                new ColumnInfo("Column a", ColumnAlignment.Left),
                new ColumnInfo("Column B", ColumnAlignment.Left),
                new ColumnInfo("Column V1", ColumnAlignment.Right),
                new ColumnInfo("Column V2", ColumnAlignment.Right));

            for (int i = 0; i < 20; i++)
            {
                dt.AddRow(
                    i.ToString(),
                    new[]
                        {
                            TestTools.AlphaSentence.BuildRandomStringFrom(5, 10).Trim(), TestTools.AlphaSentence.BuildRandomStringFrom(4, 15).Trim(),
                            TestTools.GetRandomFloat(10000).ToString("N2", CultureInfo.CurrentCulture), TestTools.GetRandomFloat(30000).ToString("N2", CultureInfo.CurrentCulture)
                        });
            }

            SimpleTablePrinter simpleTablePrinter = new SimpleTablePrinter(console, new SimpleTableStyle(tableHeaderColor, tableEvenRowColor));
            simpleTablePrinter.PrintTable(dt);
            Console.WriteLine();

            FramedTablePrinter framedPrinter = new FramedTablePrinter(console, tableStyle);
            framedPrinter.PrintTable(dt);
            Console.WriteLine();

            SpeflowStyleTablePrinter specflowPrinter = new SpeflowStyleTablePrinter(console, tableStyle);
            specflowPrinter.PrintTable(dt);
            Console.WriteLine();

            Console.ReadLine();

            console.WriteLine(tableFrameColor, "Positioned tables");
            Console.WriteLine();

            // TODO: PrintTableAt(dt, x, y);

            Console.ReadLine();

            console.WriteLine(tableFrameColor, "Large tables");
            Console.WriteLine();

            dt = new DataTable<string>(
                new ColumnInfo("Column A1", ColumnAlignment.Left),
                new ColumnInfo("Column B", ColumnAlignment.Left),
                new ColumnInfo("Column C", ColumnAlignment.Left),
                new ColumnInfo("Column V1", ColumnAlignment.Right, minLength: 9),
                new ColumnInfo("Column V2", ColumnAlignment.Right, minLength: 9),
                new ColumnInfo("Column VXX", ColumnAlignment.Right, minLength: 12));

            for (int i = 0; i < 20; i++)
            {
                dt.AddRow(
                    i.ToString(),
                    new[]
                        {
                            TestTools.AlphaSentence.BuildRandomStringFrom(10, 15).Trim(),
                            TestTools.AlphaSentence.BuildRandomStringFrom(8, 40).Trim(),
                            TestTools.AlphaSentence.BuildRandomStringFrom(20, 50).Trim(),
                            TestTools.GetRandomFloat(10000).ToString("N2", CultureInfo.CurrentCulture),
                            TestTools.GetRandomFloat(50000).ToString("N2", CultureInfo.CurrentCulture),
                            TestTools.GetRandomFloat(3000000).ToString("N2", CultureInfo.CurrentCulture)
                        });
            }

            simpleTablePrinter.PrintTable(dt);
            Console.WriteLine();

            framedPrinter.PrintTable(dt);
            Console.WriteLine();

            specflowPrinter.PrintTable(dt);

            console.WriteLine(tableFrameColor, "");
            Console.ReadLine();
        }

        private static void PrintFrames(ConsoleOperations ops, IConsole console)
        {
            var text1Colors = new ConsoleFontColor(Color.Gold, Color.Black);
            var text2Colors = new ConsoleFontColor(Color.Brown, Color.Black);
            var text3Colors = new ConsoleFontColor(Color.Black, Color.Silver);
            var frame2Colors = new ConsoleFontColor(Color.Silver, Color.Black);
            var solidFrameTextColors = new ConsoleFontColor(Color.Red, Color.Yellow);
            var solidFrameColors = new ConsoleFontColor(Color.Yellow, Color.Black);
            FrameStyle block3Frame = new FrameStyle(frame2Colors, text3Colors, @"┌─┐││└─┘", '░');
            FrameStyle doubleFrame = new FrameStyle(frame2Colors, text3Colors, @"╔═╗║║╚═╝", '▒');
            FrameStyle solidFrame = new FrameStyle(solidFrameColors, solidFrameTextColors, @"▄▄▄██▀▀▀", '▓');

            ops.WriteTextBox(5, 5, 50, 7,
                @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor.",
                text1Colors);
            ops.WriteTextBox(25, 15, 80, 37,
                @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor. Pellentesque facilisis. Nulla imperdiet sit amet magna. Vestibulum dapibus, mauris nec malesuada fames ac turpis velit, rhoncus eu, luctus et interdum adipiscing wisi. Aliquam erat ac ipsum. Integer aliquam purus. Quisque lorem tortor fringilla sed, vestibulum id, eleifend justo vel bibendum sapien massa ac turpis faucibus orci luctus non, consectetuer lobortis quis, varius in, purus.",
                text2Colors);

            ops.WriteTextBox(new Rectangle(26, 26, 60, 20),
                @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis. Maecenas malesuada elit lectus felis, malesuada ultricies. Curabitur et ligula. Ut molestie a, ultricies porta urna. Vestibulum commodo volutpat a, convallis ac, laoreet enim. Phasellus fermentum in, dolor. Pellentesque facilisis. Nulla imperdiet sit amet magna. Vestibulum dapibus, mauris nec malesuada fames ac turpis velit, rhoncus eu, luctus et interdum adipiscing wisi. Aliquam erat ac ipsum. Integer aliquam purus. Quisque lorem tortor fringilla sed, vestibulum id, eleifend justo vel bibendum sapien massa ac turpis faucibus orci luctus non, consectetuer lobortis quis, varius in, purus.",
                block3Frame);
            ops.WriteTextBox(new Rectangle(8, 10, 30, 7),
                @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.",
                doubleFrame);
            ops.WriteTextBox(new Rectangle(10, 20, 25, 7),
                @"Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.",
                solidFrame);

            console.WriteText(0, 20, "", Color.Gray, Color.Black); // reset

            Console.ReadLine();
            console.Clear();
        }

        private static void PrintAllNamedColors(ConsoleController controller, IConsole console)
        {
            var props =
                typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(p => p.PropertyType == typeof(Color));
            foreach (var propertyInfo in props)
            {
                Color c = (Color) propertyInfo.GetValue(null);
                ConsoleColor cc = controller.CloseColorFinder.FindClosestColor(c);
                Console.ForegroundColor = cc;
                Console.WriteLine("{0,-25} {1,-18} #{2,-8:X}", propertyInfo.Name, Enum.GetName(typeof(ConsoleColor), cc),
                    c.ToArgb());
            }

            Console.ReadLine();
            console.Clear();
        }

        private static void PrintColorsMessages(IConsole console)
        {
            console.WriteText(0, 0, "test message", Color.Red, Color.Black);
            console.WriteText(0, 1, "test message 2", Color.Cyan, Color.YellowGreen);
            console.WriteText(0, 2, "test message 3d ds sfsdfsad ", Color.Orange, Color.Plum);
            console.WriteText(0, 3, "test messaf sdf s sfsdfsad ", Color.DarkOliveGreen, Color.Silver);
            console.WriteText(0, 4, "tsd fsfsd fds fsd f fa fas fad ", Color.AliceBlue, Color.PaleVioletRed);
            console.WriteText(0, 5, "tsd fsfsd fds fsd f fa fas fad ", Color.Blue, Color.CadetBlue);
            console.WriteText(0, 6, "tsd fsdfsdfsd fds fa fas fad ", Color.Maroon, Color.ForestGreen);

            // lol: http://stackoverflow.com/questions/3811973/why-is-darkgray-lighter-than-gray
            console.WriteText(0, 10, "test message", Color.Gray, Color.Black);
            console.WriteText(0, 11, "test message 2", Color.DarkGray, Color.Black);
            console.WriteText(0, 12, "test message 3d ds sfsdfsad ", Color.DimGray, Color.Black);
            console.WriteText(0, 20, "", Color.Gray, Color.Black); // reset

            Console.ReadLine();
            console.Clear();
        }
    }

    // TODO: move to separate library of testing tools... tomorrow ;-)
    internal static class TestTools
    {
        private static readonly Random rnd = new Random();

        public static float GetRandomFloat()
        {
            return (float)rnd.NextDouble(); // TODO: add float type (Real, NegativeReal, PositiveReal) and range selection
        }

        public static float GetRandomFloat(int multiplier)
        {
            return GetRandomFloat() * multiplier;
        }

        /// <summary>
        /// Builds string of required length concatenating random characters from given string.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuildRandomStringFrom(this string sourceString, uint length)
        {
            char[] array = new char[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = sourceString[rnd.Next(0, sourceString.Length)];
            }

            return new string(array);
        }

        public static string BuildRandomStringFrom(this string sourceString, int minLength, int maxLength)
        {
            int length = rnd.Next(minLength, maxLength + 1);

            char[] array = new char[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = sourceString[rnd.Next(0, sourceString.Length)];
            }

            return new string(array);
        }

        public static string Numeric => @"0123456789";

        public static string UpperAlpha => @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string LowerAlpha => @"abcdefghijklmnopqrstuvwxyz";

        public static string UpperAlphanumeric => UpperAlpha + Numeric;

        public static string LowerAlphanumeric => LowerAlpha + Numeric;

        public static string MixedAlphanumeric => UpperAlphanumeric + LowerAlphanumeric;

        public static string AlphanumericIdentifier => UpperAlphanumeric + LowerAlphanumeric + @"______"; // increased probability ;-)

        public static string AlphaSentence => LowerAlpha + @"      "; // increased probability ;-)
    }
}