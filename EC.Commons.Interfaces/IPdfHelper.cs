using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public interface IPdfHelper
    {
        /// <summary>
        /// take html as a string and return a string which is the contents of the pdf.
        /// </summary>
        /// <param name="html"></param>

        string RenderHtmlToPdf(string html);

        /// <summary>
        /// take HTML as a string and write the PDF file to the given (absolute) file path.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="filePath"></param>

        string RenderHtmlToPdfFile(string html, string filePath);
    }
}
