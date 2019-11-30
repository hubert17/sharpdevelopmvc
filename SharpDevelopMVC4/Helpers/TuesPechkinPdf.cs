
using System.Drawing.Printing;
using TuesPechkin;

public static class TuesPechkinPdf
{
	private static readonly object _PadLock = new object();
	private static IConverter _converter = null;
	public static IConverter convertidor {
	    get {
	        if (_converter == null) {
	            lock (_PadLock) {
	                if (_converter == null) {
	                    _converter = new ThreadSafeConverter(new RemotingToolset<PdfToolset>(new Win32EmbeddedDeployment(new TempFolderDeployment())));
	                }
	            }
	        }
	        return _converter;
	    }
	}
	
	public static byte[] ToPdf(string htmlText)
	{
		// https://github.com/tuespetre/TuesPechkin/blob/develop/TuesPechkin/Document/GlobalSettings.cs
		var document = new HtmlToPdfDocument
		{
		    GlobalSettings =
		    {		 
				DPI = 300,	    	
		        ProduceOutline = true,
		        DocumentTitle = "Pretty Websites",
		        PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
		        Margins =
		        {
		            All = 1.375,
		            Unit = Unit.Centimeters
				}	        
			},
		    Objects = {
		        new ObjectSettings 
		        { 
		        	HtmlText = htmlText,
                    WebSettings = new WebSettings()
                    {
                        EnableJavascript = false,
                        PrintMediaType =  true,
                        EnableIntelligentShrinking = true
                    }
		        },
				//new ObjectSettings { PageUrl = pageUrl }
		    }
		};
		
		byte[] result = convertidor.Convert(document);
		//System.IO.File.WriteAllBytes(@"C:\Users\DRIVE_D\My Documents\_GIT_VCS\AspDotNetGabs\testpdf.pdf", result);
					
		return result;
	}
}
