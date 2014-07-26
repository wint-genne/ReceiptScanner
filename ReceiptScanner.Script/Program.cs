using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace ScriptProject
{
    public class Program
    {
        static void Main() {
            jQuery.OnDocumentReady(() =>
            {
                new KoMain();
            });
        }
    }
}