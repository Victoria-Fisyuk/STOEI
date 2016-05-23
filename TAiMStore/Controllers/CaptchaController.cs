using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.Mvc;
using TAiMStore.HtmlHelpers;

namespace TAiMStore.Controllers
{
    public class CaptchaController : Controller
    {
        private const string FontFamily = "Arial";
        private static readonly Color Background = Color.FromArgb(216, 216, 216);

        /// <summary>
        /// Get captcha image
        /// </summary>
        /// <param name="challengeGuid">Challenge Guid</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Captcha image</returns>
        public void Render(string challengeGuid, int? width, int? height)
        {
            string key = Captcha.SessionKeyPrefix + challengeGuid;
            string solution = (string)HttpContext.Session[key];
            if (solution != null)
            {
                if (width.HasValue && height.HasValue)
                {
                    Random random = new Random();
                    Bitmap bitmap = new Bitmap(width.Value, height.Value, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Rectangle rect = new Rectangle(0, 0, width.Value, height.Value);
                    float bestFontSize;
                    using (var font = new Font(FontFamily, 1f))
                    {
                        SizeF testSize = g.MeasureString(solution, font);
                        bestFontSize = Math.Min(width.Value / testSize.Width,
                                                              height.Value / testSize.Height) * 1.5f;
                    }
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    var finalFont = new Font(FontFamily, bestFontSize, FontStyle.Bold);
                    GraphicsPath path = new GraphicsPath();
                    path.AddString(solution, finalFont.FontFamily, (int)finalFont.Style, finalFont.Size, rect, format);
                    float v = 4F;
                    PointF[] points =
			    {
				    new PointF(random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				    new PointF(rect.Width - random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				    new PointF(random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v),
				    new PointF(rect.Width - random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v)
			    };
                    Matrix matrix = new Matrix();
                    matrix.Translate(0F, 0F);
                    path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
                    HatchBrush hatchBrush = new HatchBrush(HatchStyle.Wave, Background, Background);
                    g.FillRectangle(hatchBrush, rect);
                    hatchBrush = new HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.Black, Color.Black);
                    g.FillPath(hatchBrush, path);
                    hatchBrush = new HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.Black, Color.Black);
                    g.FillPath(hatchBrush, path);
                    finalFont.Dispose();
                    hatchBrush.Dispose();
                    g.Dispose();

                    Response.ContentType = "image/jpeg";
                    bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        /// Обновление изображения captcha
        /// </summary>
        /// <returns>Новое изображение captcha</returns>
        public PartialViewResult RefreshCaptcha()
        {
            return PartialView("_Captcha");
        }
    }
}
