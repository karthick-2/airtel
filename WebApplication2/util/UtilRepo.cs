using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication2.util
{
    public interface IUtilRepo : IDisposable
    {
        /*void Errorlogtxt(string serrmsg, string stacktrace);*/
        Bitmap resizeimage(Stream streamimg, int resizewidth, int resizeheight);
        /* void SendEmailmessage(string toaddr, string ssubject, string emessage);*/
    }
}