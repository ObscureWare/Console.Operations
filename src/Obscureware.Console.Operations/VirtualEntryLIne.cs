﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualEntryLine.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2017 Sebastian Gruchacz
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
//   Defines the VirtualEntryLine class that simulates command line behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ObscureWare.Console.Operations
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Drawing;

    using ObscureWare.Console;

    /// <summary>
    /// In order to use auto-completion I must simulate more or less all other expected behavior of console editing.
    /// </summary>
    /// <remarks>I'm gonna need this anyway for graphical console implementation in the future... So maybe better implement this correctly already...</remarks>
    public class VirtualEntryLine
    {
        private const int MAX_COMMAND_LENGTH = 1024; // more?! no problem, but what for?
        private readonly IConsole _console;
        private readonly ConsoleFontColor _cmdColor;
        private readonly List<string> _commandHistory = new List<string>();
        private int historyIndex = -1;
        private bool overWriting = false;

        public VirtualEntryLine(IConsole console, ConsoleFontColor cmdColor)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            this._console = console;
            this._cmdColor = cmdColor;
        }


        // TODO: this method is monstrosity, but it rather not be wise to cut it into pieces... It's slow enough...
        public string GetUserEntry(IAutoComplete acProvider)
        {
            if (acProvider == null)
            {
                throw new ArgumentNullException(nameof(acProvider));
            }

            var startPosition = this._console.GetCursorPosition();
            var consoleLineWidth = this._console.WindowWidth;
            var longestLineContentSoFar = 0;
            
            int currentCommandEndIndex = 0;
            char[] commandBuffer = new char[MAX_COMMAND_LENGTH];

            string[] autocompleteList = null;
            int autocompleteIndex = 0;

            while (true)
            {
                ConsoleKeyInfo key = this._console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    this._console.WriteLine();
                    string cmdContent = new string(commandBuffer, 0, currentCommandEndIndex);
                    if (!string.IsNullOrWhiteSpace(cmdContent))
                    {
                        // remember only new entries (on last position)
                        if (!this._commandHistory.Any() || !this._commandHistory.Last().Equals(cmdContent))
                        {
                            this._commandHistory.Add(cmdContent);
                            this.historyIndex = this._commandHistory.Count - 1;
                        }
                    }
                    return cmdContent;
                }
                
                if (key.Key == ConsoleKey.Tab)
                {
                    // TODO: hide/ show cursor her? probably not required

                    if (autocompleteList == null)
                    {
                        autocompleteList =
                            acProvider.MatchAutoComplete(new string(commandBuffer, 0, currentCommandEndIndex)).ToArray();
                        autocompleteIndex = -1;
                    }

                    if (autocompleteList.Any())
                    {
                        // reverse?
                        if ((key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift)
                        {
                            autocompleteIndex--;
                            if (autocompleteIndex < 0)
                            {
                                autocompleteIndex = autocompleteList.Length;
                            }
                        }
                        else
                        {
                            autocompleteIndex++;
                            if (autocompleteIndex >= autocompleteList.Length)
                            {
                                autocompleteIndex = 0;
                            }
                        }
                        
                        string acText = autocompleteList[autocompleteIndex];
                        this.ApplyText(acText, this._console, commandBuffer, startPosition, ref longestLineContentSoFar, ref currentCommandEndIndex);
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    try
                    {
                        this._console.HideCursor();

                        // TODO: multiline fix
                        var currentPosition = this._console.GetCursorPosition();
                        var deltaX = currentPosition.X - startPosition.X;
                        if (deltaX > 0)
                        {

                            this.RemoveCharsAt(commandBuffer, deltaX, 1);
                            this._console.SetCursorPosition(startPosition.X, startPosition.Y);
                            this._console.WriteText(this._cmdColor, new string(' ', currentCommandEndIndex + 1));

                            currentCommandEndIndex -= 1;

                            if (currentCommandEndIndex >= 0)
                            {
                                this._console.SetCursorPosition(startPosition.X, startPosition.Y);
                                this._console.WriteText(this._cmdColor,
                                    new string(commandBuffer, 0, currentCommandEndIndex));

                                this._console.SetCursorPosition(startPosition.X + deltaX - 1, startPosition.Y);
                            }
                        }
                    }
                    finally
                    {
                        this._console.ShowCursor();
                    }
                }
                else if (key.Key == ConsoleKey.Delete)
                {
                    // TODO: multiline fix
                    // TODO: implement DELETE key
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    // TODO: multiline fix
                    var currentPosition = this._console.GetCursorPosition();
                    var deltaX = currentPosition.X - startPosition.X;
                    if (deltaX > 0)
                    {

                        this._console.SetCursorPosition(currentPosition.X - 1, startPosition.Y);
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    // TODO: multiline fix
                    var currentPosition = this._console.GetCursorPosition();
                    var deltaX = currentPosition.X - startPosition.X;
                    if (deltaX < currentCommandEndIndex)
                    {
                        this._console.SetCursorPosition(currentPosition.X + 1, startPosition.Y);
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    // TODO: multiline fix
                    // TODO:  move cursor between lines
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    // TODO: multiline fix
                    // TODO:  move cursor between lines
                }
                else if (key.Key == ConsoleKey.Insert)
                {
                    // TODO if (shift) => paste

                    // switch insert mode
                    this.overWriting = !this.overWriting;
                }
                else if (key.Key == ConsoleKey.End)
                {
                    var endOfline = this.CalculateCursorPositionForLineIndex(startPosition, consoleLineWidth, currentCommandEndIndex);
                    this._console.SetCursorPosition(endOfline.X, endOfline.Y);
                }
                else if (key.Key == ConsoleKey.Home)
                {
                    this._console.SetCursorPosition(startPosition.X, startPosition.Y);
                }
                else if (key.Key == ConsoleKey.PageUp)
                {
                    if (this.historyIndex < this._commandHistory.Count && this.historyIndex >= 0)
                    {
                        string historyEntry = this._commandHistory[this.historyIndex--];
                        // looping?
                        //if (historyIndex < 0)
                        //{
                        //    historyIndex = this._commandHistory.Count - 1;
                        //}

                        this.ApplyText(historyEntry, this._console, commandBuffer, startPosition, ref longestLineContentSoFar, ref currentCommandEndIndex);
                    }
                }
                else if (key.Key == ConsoleKey.PageDown)
                {
                    if (this.historyIndex < this._commandHistory.Count - 1 && this.historyIndex >= -1)
                    {
                        string historyEntry = this._commandHistory[++this.historyIndex];
                        // looping?
                        //if (historyIndex >= this._commandHistory.Count)
                        //{
                        //    historyIndex = 0;
                        //}

                        this.ApplyText(historyEntry, this._console, commandBuffer, startPosition, ref longestLineContentSoFar, ref currentCommandEndIndex);
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    // do nothing
                    // TODO: or reset command-line?
                }
                else if (key.KeyChar != 0)
                {
                    var currentPosition = this._console.GetCursorPosition();
                    int lineIndex = this.CalculatePositionInLine(startPosition, currentPosition, consoleLineWidth);
                    
                    if ((key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control &&
                        key.Key == ConsoleKey.V)
                    {
                        // TODO: paste
                    }
                    else
                    {
                        if (currentCommandEndIndex < MAX_COMMAND_LENGTH)
                        {
                            if (lineIndex == currentCommandEndIndex)
                            {
                                // append at the end - in both INSERT modes the same
                                commandBuffer[currentCommandEndIndex] = key.KeyChar;
                                currentCommandEndIndex += 1;
                            }
                            else if (this.overWriting)
                            {
                                // overwrite in the middle
                                commandBuffer[lineIndex] = key.KeyChar;
                            }
                            else
                            {
                                // insert in the middle
                                this.InsertCharsAt(commandBuffer, lineIndex, currentCommandEndIndex, new []{ key.KeyChar });
                                currentCommandEndIndex++;
                            }

                            try
                            {
                                this._console.HideCursor();

                                this._console.SetCursorPosition(startPosition.X, startPosition.Y);
                                this._console.SetColors(this._cmdColor);
                                this._console.WriteText(new string(commandBuffer, 0, currentCommandEndIndex));
                                if (currentPosition.X < consoleLineWidth)
                                {
                                    this._console.SetCursorPosition(currentPosition.X + 1, currentPosition.Y);
                                }
                                else
                                {
                                    this._console.SetCursorPosition(0, currentPosition.Y + 1);
                                }
                                autocompleteList = null;
                            }
                            finally
                            {
                                this._console.ShowCursor();
                            }
                        }
                    }
                }
                else
                {
                    // delete, esc
                }
            }
        }

        /// <summary>
        /// Calculates real position of cursor in multi-line console line
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="currentPosition"></param>
        /// <param name="consoleLineWidth"></param>
        /// <returns></returns>
        internal int CalculatePositionInLine(Point startPosition, Point currentPosition, int consoleLineWidth)
        {
            return (currentPosition.Y - startPosition.Y) * consoleLineWidth + currentPosition.X - startPosition.X;
        }

        /// <summary>
        /// Calculates where shall put cursor for specific index of multi-lined console line
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="consoleLineWidth"></param>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        internal Point CalculateCursorPositionForLineIndex(Point startPosition, int consoleLineWidth, int targetIndex)
        {
            int fullLines = ((targetIndex + startPosition.X) / consoleLineWidth);
            return new Point(
                (targetIndex - (fullLines * consoleLineWidth) + startPosition.X),
                fullLines + startPosition.Y);
        }

        private void ApplyText(string text, IConsole console, char[] commandBuffer, Point startPosition, 
            ref int lineContentSoFar, ref int currentCommandEndIndex)
        {
            try
            {
                this._console.HideCursor();

                var textLength = text.Length;
                currentCommandEndIndex = textLength - 1;
                lineContentSoFar = Math.Max(lineContentSoFar, textLength);
                text.ToCharArray().CopyTo(commandBuffer, 0);
                console.SetCursorPosition(startPosition.X, startPosition.Y);
                console.WriteText(this._cmdColor, new string(' ', lineContentSoFar));
                console.SetCursorPosition(startPosition.X, startPosition.Y);
                console.WriteText(this._cmdColor, text);
            }
            finally
            {
                this._console.ShowCursor();
            }
        }

        private void RemoveCharsAt(char[] buffer, int from, int qty)
        {
            for (int i = from; i < from + qty && i < buffer.Length && i > 0; i++)
            {
                buffer[i - 1] = buffer[i];
            }
        }

        private void InsertCharsAt(char[] buffer, int from, int currentUsedMax, char[] newChars)
        {
            int newLen = newChars.Length;
            for (int i = currentUsedMax; i <= buffer.Length + newLen && i >= from; i--)
            {
                buffer[i + newLen] = buffer[i];
            }

            for (int i = 0; i < newLen && i < buffer.Length; i++)
            {
                buffer[from + i] = newChars[i];
            }
        }
    }
}
