using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication2.util
{
    
        public class UtilRepository : IUtilRepo, IDisposable
        {
        static string bugMail = ConfigurationSettings.AppSettings["BugMailTo"].ToString();
        string spath = ConfigurationSettings.AppSettings["error_log_path"].ToString();

        private string slogformat;
            private string serrortime;
            public Bitmap resizeimage(Stream streamimage, int resizewidth, int resizeheight)
            {
                System.Drawing.Image origin = System.Drawing.Image.FromStream(streamimage);
                int height = resizeheight, width = resizewidth;
                int originalW = origin.Width, originalH = origin.Height;
                float percentW, percentH, percent;
                percentW = (float)resizewidth / (float)originalW;
                percentH = (float)resizeheight / (float)originalH;
                if (percentH < percentW)
                {
                    percent = percentH;
                    width = (int)(originalW * percent);
                }
                else
                {
                    percent = percentW;
                    height = (int)(originalH * percent);
                }
                Bitmap thumbna = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(thumbna))
                {
                    g.Clear(Color.White);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(origin,
                        new Rectangle(0, 0, width, height),
                        new Rectangle(0, 0, originalW, originalH),
                        GraphicsUnit.Pixel);
                }
                return thumbna;


            }
            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        //_DB.Dispose();
                    }
                }
                this.disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

        }
    
}