// <copyright file="PrinterUtils.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils
{
    using System.Text;

    /// <summary>
    /// Printer utilities
    /// </summary>
    internal class PrinterUtils
    {
        /// <summary>
        /// Maximum number of chars per line
        /// </summary>
        private int chars = 32;

        /// <summary>
        /// String builder
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// New line separator
        /// </summary>
        private char lineSeparator = '\n';

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterUtils" /> class.
        /// </summary>
        public PrinterUtils()
        {
            this.builder = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterUtils" /> class.
        /// </summary>
        /// <param name="charsPerLine">Maximum number of chars per line</param>
        /// <param name="newLineSeparator">New line separator</param>
        public PrinterUtils(int charsPerLine, char newLineSeparator = '\n') : this()
        {
            this.chars = charsPerLine;
            this.lineSeparator = newLineSeparator;
        }

        /// <summary>
        /// Gets the constructed ticket message
        /// </summary>
        /// <returns>A <c>string</c> contained the message</returns>
        public string GetTicket()
        {
            string ret = string.Empty;

            if (this.builder != null && this.builder.Length > 0)
            {
                ret = this.builder.ToString();
                this.builder.Clear();
            }

            return ret;
        }

        /// <summary>
        /// Initilize a new ticket
        /// </summary>
        public void StartNewTicket()
        {
            if (this.builder != null)
            {
                this.builder.Clear();
            }
            else
            {
                this.builder = new StringBuilder();
            }
        }

        /// <summary>
        /// Adds a new empty line
        /// </summary>
        public void AddLineEmpty()
        {
            if (this.builder == null)
            {
                return;
            }

            this.builder.Append(this.lineSeparator);
        }

        /// <summary>
        /// Adds a new line, centered in the paper
        /// </summary>
        /// <param name="text">Line text</param>
        public void AddLineCentered(string text)
        {
            if (this.builder == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Length > this.chars)
            {
                int current = 0;
                for (int i = text.Length; i > this.chars; i -= this.chars)
                {
                    this.builder.Append(text.Substring(current, this.chars) + this.lineSeparator);
                    current += this.chars;
                }

                string pad = string.Empty;
                int center = (this.chars - text.Substring(current, text.Length - current).Length) / 2;

                for (int j = 0; j < center; j++)
                {
                    pad += " ";
                }

                this.builder.Append(pad + text.Substring(current, text.Length - current) + this.lineSeparator);
            }
            else
            {
                string pad = string.Empty;
                int center = (this.chars - text.Length) / 2;

                for (int j = 0; j < center; j++)
                {
                    pad += " ";
                }

                this.builder.Append(pad + text + this.lineSeparator);
            }
        }

        /// <summary>
        /// Adds a new line, left in the paper
        /// </summary>
        /// <param name="text">Line text</param>
        public void AddLine(string text)
        {
            this.AddLineLeft(text);
        }

        /// <summary>
        /// Adds a new line, left in the paper
        /// </summary>
        /// <param name="text">Line text</param>
        public void AddLineLeft(string text)
        {
            if (this.builder == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Length > this.chars)
            {
                int current = 0;
                for (int i = text.Length; i > this.chars; i -= this.chars)
                {
                    this.builder.Append(text.Substring(current, this.chars) + this.lineSeparator);
                    current += this.chars;
                }

                this.builder.Append(text.Substring(current, text.Length - current) + this.lineSeparator);
            }
            else
            {
                this.builder.Append(text + this.lineSeparator);
            }
        }

        /// <summary>
        /// Adds a new line, right in the paper
        /// </summary>
        /// <param name="text">Line text</param>
        public void AddLineRight(string text)
        {
            if (this.builder == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            // Valida si la longitud del tecto es mayor al maximo permitido
            if (text.Length > this.chars)
            {
                int current = 0;
                for (int i = text.Length; i > this.chars; i -= this.chars)
                {
                    this.builder.Append(text.Substring(current, this.chars) + this.lineSeparator);
                    current += this.chars;
                }

                string pad = string.Empty;

                for (int j = 0; j < (this.chars - text.Substring(current, text.Length - current).Length); j++)
                {
                    pad += " ";
                }

                this.builder.Append(pad + text.Substring(current, text.Length - current) + this.lineSeparator);
            }
            else
            {
                string pad = string.Empty;

                for (int j = 0; j < (this.chars - text.Length); j++)
                {
                    pad += " ";
                }

                this.builder.Append(pad + text + this.lineSeparator);
            }
        }

        /// <summary>
        /// Adds a new line, field to the left and value to the right of the paper
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="value">Field value</param>
        public void AddLineFieldValue(string field, string value)
        {
            string text = string.Empty;
            string pad = string.Empty;

            int padNumber = this.chars - (field.Length + value.Length);
            for (int i = 0; i < padNumber; i++)
            {
                pad += " ";
            }

            text += field + pad + value;

            this.builder.Append(text + this.lineSeparator);
        }
    }
}
