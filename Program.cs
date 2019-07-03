using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                //Configure the Local JsReportServer as utility
                //This would save templates in the running application directory (bin/debug)
                var rs = new LocalReporting()
                    .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
                    .KillRunningJsReportProcesses()
                    .UseBinary(JsReportBinary.GetBinary())
                    .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                    .AsUtility()
                    .Create();


                //Render the invoice template as PDF, by default is renders as portrait.
                //If you want to render as landscape, set landscape=true in config.json file of invoice template
                //Here we are using saved templates in local server.
                Console.WriteLine("Rendering localy stored template jsreport/data/templates/Invoice into invoice.pdf");
                var invoiceReport = rs.RenderByNameAsync("Invoice", InvoiceData).Result;
                invoiceReport.Content.CopyTo(File.OpenWrite("invoice.pdf"));


                //Render the sample template as PDF, by default is renders as portrait.
                //If you want to render as landscape, set landscape=true in below template object.
                //Here we are managing template from our code
                Console.WriteLine("Rendering custom report fully described through the request object into customReport.pdf");
                var customReport = rs.RenderAsync(CustomRenderRequest).Result;
                customReport.Content.CopyTo(File.OpenWrite("customReport.pdf"));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        private static RenderRequest CustomRenderRequest = new RenderRequest()
        {
            Template = new Template()
            {
                Content = "HI First PDF from {{message}}",
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf,
                Chrome = new Chrome
                {
                    Landscape= false //If you want to render as landscape, set it to true.

                }

            },
            Data = new
            {
                message = "jsreport Examplr"
            }
           
        };

        static object InvoiceData = new
        {
            number = "100005",
            seller = new
            {
                name = "ABC Inc.",
                road = "Airoli Navi Mumba",
                country = "TX 12345"
            },
            buyer = new
            {
                name = "Test Corp.",
                road = "Airoli Navi Mumba",
                country = "INDIA"
            },
            items = new[]
            {
                new { name = "Report design", price = 300 }
            }
        };
    }
}
