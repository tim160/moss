using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Common.Interfaces;
using Winnovative;
using System.IO;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(IPdfHelper))]

    public class PdfHelper : IPdfHelper
    {
        /// <summary>
        /// take html as a string and return a string which is the contents of the pdf.
        /// </summary>
        /// <param name="html"></param>

        public string RenderHtmlToPdf(string html)
        {
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
            htmlToPdfConverter.LicenseKey = "dPrp++ru++rs7eL76un16/vo6vXq6fXi4uLi++s=";
            htmlToPdfConverter.MediaType = "print";
            htmlToPdfConverter.PdfDocumentOptions.FitHeight = true;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            htmlToPdfConverter.HtmlViewerWidth = 700;
            htmlToPdfConverter.HtmlViewerHeight = 905;
            var pdfString = htmlToPdfConverter.ConvertHtml(html,"").ToString();

            return pdfString;
        }

        /// <summary>
        /// take HTML as a string and write the PDF file to the given (absolute) file path.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="filePath"></param>

        public string RenderHtmlToPdfFile(string html, string filePath)
        {
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
            htmlToPdfConverter.LicenseKey = "dPrp++ru++rs7eL76un16/vo6vXq6fXi4uLi++s=";
            htmlToPdfConverter.MediaType = "print";
            htmlToPdfConverter.PdfDocumentOptions.FitHeight = true;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            htmlToPdfConverter.HtmlViewerWidth = 700;
            htmlToPdfConverter.HtmlViewerHeight = 905;
            var pdfString = htmlToPdfConverter.ConvertHtml(html, "");

            FileStream fs = new FileStream(@filePath, FileMode.OpenOrCreate);
            fs.Write(pdfString, 0, pdfString.Length);
            fs.Close();

            return pdfString.ToString();
        }
    }
}
