using DAM.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using SpireDoc = Spire.Doc;
using SpirePdf = Spire.Pdf;
using SpireXls = Spire.Xls;
using SpirePPT = Spire.Presentation;

using static DAM.Application.Services.Interfaces.IConversionService;
using System.Diagnostics;
using DAM.Domain.Entities;
using LazZiya.ImageResize;
using DAM.Application.Watermarks.Enums;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Collections.Generic;
using ImageMagick;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using DAM.Application.Services.Classes;
using System.Net.Http.Headers;

namespace DAM.Application.Services
{
    public class ConversionService : IConversionService
    {
        private ConversionServiceConfig SetConversionServiceConfig(IConfiguration configuration)
        {
            return new ConversionServiceConfig()
            {
                ThumbnailWidth = Convert.ToInt32(configuration["ThumbnailWidth"]),
                ThumbnailHeight = Convert.ToInt32(configuration["ThumbnailHeight"]),
                FilePath = configuration["FilePath"],
                FFMpeg = configuration["FFMpeg"],
                DocToPDFApiUrl = configuration["DocToPDFApiUrl"]
            };
        }

        private bool ThumbnailCallback()
        {
            return true;
        }

        public Stream GetImageThumbnail(IConfiguration configuration, Stream stream, string extensionName)
        {
            ConversionServiceConfig config = SetConversionServiceConfig(configuration);

            Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image image = Bitmap.FromStream(stream);
            //Image pThumbnail = image.GetThumbnailImage(config.ThumbnailWidth, config.ThumbnailHeight, callback, new IntPtr());

            Image target = new Bitmap(config.ThumbnailWidth, config.ThumbnailHeight, PixelFormat.Format24bppRgb);
            Stream sm = new MemoryStream();

            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)config.ThumbnailWidth / (float)image.Width);
            nPercentH = ((float)config.ThumbnailHeight / (float)image.Height);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((config.ThumbnailWidth - (image.Width * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((config.ThumbnailHeight - (image.Height * nPercent)) / 2);
            }

            int destWidth = (int)(image.Width * nPercent);
            int destHeight = (int)(image.Height * nPercent);

            using (Graphics graphics = Graphics.FromImage(target))
            {
                graphics.Clear(Color.LightGray); //.Contains("png") || extensionName.Contains("tiff")  || extensionName.Contains("tif") ? Color.LightGray : Color.Transparent);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image,
                                new Rectangle(destX, destY, destWidth, destHeight),
                                new Rectangle(0, 0, image.Width, image.Height),
                                GraphicsUnit.Pixel);

                if (extensionName.Contains("png") || extensionName.Contains("tiff") || extensionName.Contains("tif"))
                {
                    target.Save(sm, ImageFormat.Png);
                }
                else
                {
                    target.Save(sm, ImageFormat.Jpeg);
                }

            }

            return sm;
        }

        public Stream GetMagickImageThumbnail(IConfiguration configuration, Stream stream, string extensionName)
        {
            ConversionServiceConfig config = SetConversionServiceConfig(configuration);
            Stream sm = new MemoryStream();

            using (var images = new MagickImageCollection())
            {
                var settings = new MagickReadSettings()
                {
                    Density = new Density(300),
                    FrameIndex = 0,
                    FrameCount = 1,
                    Width = config.ThumbnailWidth,
                    Height = config.ThumbnailHeight
                };

                images.Read(stream, settings);

                using (var horizontal = images[0])
                {
                    horizontal.Write(sm, MagickFormat.Jpeg);
                }
            }

            return sm;
        }

        public Stream GetPDFThumbnail(IConfiguration configuration, Stream stream)
        {
            ConversionServiceConfig config = SetConversionServiceConfig(configuration);
            Stream sm = new MemoryStream();

            using (var images = new MagickImageCollection())
            {
                var settings = new MagickReadSettings()
                {
                    Density = new Density(300),
                    FrameIndex = 0,
                    FrameCount = 1,
                    Width = config.ThumbnailWidth,
                    Height = config.ThumbnailHeight
                };

                images.Read(stream, settings);

                using (var horizontal = images[0])
                {
                    horizontal.Write(sm, MagickFormat.Jpeg);
                }
            }

            return sm;
        }

        public Stream GetVideoThumbnail(IConfiguration configuration, Stream stream, string videoFile, string key)
        {
            ConversionServiceConfig config = SetConversionServiceConfig(configuration);

            if (!Directory.Exists(config.FilePath)) Directory.CreateDirectory(config.FilePath);
            var fullPath = string.Concat(config.FilePath, "/", videoFile);

            using (var video = File.Create(fullPath))
            {
                stream.CopyTo(video);
            }

            Stream sm = MakeThumbnail(config.FilePath, videoFile, key, config.FFMpeg, config.ThumbnailWidth);

            return sm;
        }

        public Stream GetDocumentThumbnail(Stream stream)
        {
            Stream sm = new MemoryStream();

            SpireDoc.Document document = new SpireDoc.Document(stream, SpireDoc.FileFormat.Auto);
            System.Drawing.Image image = document.SaveToImages(0, SpireDoc.Documents.ImageType.Bitmap);
            image.Save(sm, ImageFormat.Jpeg);

            return sm;
        }

        public Stream GetPDFThumbnail(Stream stream)
        {
            Stream sm = new MemoryStream();
            SpirePdf.PdfDocument document = new SpirePdf.PdfDocument(stream, "a");
            Image image = document.SaveAsImage(0, SpirePdf.Graphics.PdfImageType.Bitmap);
            image.Save(sm, ImageFormat.Jpeg);

            return sm;
        }

        public Stream GetXLSThumbnail(Stream stream)
        {
            Stream sm = new MemoryStream();

            SpireXls.Workbook workbook = new SpireXls.Workbook();
            workbook.LoadFromStream(stream);
            SpireXls.Worksheet workSheet = workbook.Worksheets[0];
            Image image = workSheet.ToImage(1, 1, workSheet.LastRow, workSheet.LastColumn);
            image.Save(sm, ImageFormat.Jpeg);

            return sm;
        }

        public Stream GetPPTThumbnail(Stream stream)
        {
            Stream sm = new MemoryStream();

            SpirePPT.Presentation presentation = new SpirePPT.Presentation(stream, SpirePPT.FileFormat.Auto);

            Image image = presentation.Slides[0].SaveAsImage();
            image.Save(sm, ImageFormat.Jpeg);

            return sm;
        }

        public Stream GetPNGFromTIFF(Stream stream)
        {
            Stream sm = new MemoryStream();

            Image.FromStream(stream).Save(sm, ImageFormat.Png);

            return sm;
        }

        private Stream MakeThumbnail(string path, string videoFile, string key, string ffMpeg, int size)
        {
            var cmd = @"-i """ + videoFile + "\" -an -vframes 1 -vf scale=\"min(" + size.ToString() + "\\,iw):-1\" \"" + key + ".jpg\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffMpeg,
                Arguments = cmd,
                WorkingDirectory = Path.GetFullPath(path),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit(20000);
            }

            var videoFullPath = string.Concat(path, "/", videoFile);
            var thumbnailFullPath = string.Concat(path, "/", key, ".jpg");

            var ms = new MemoryStream();

            using (var fs = File.OpenRead(thumbnailFullPath))
            {
                fs.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                byte[] buf = new byte[ms.Length];
                ms.Read(buf, 0, buf.Length);
            }

            File.Delete(videoFullPath);
            File.Delete(thumbnailFullPath);

            return ms;
        }
        //TO DO convert specific file types to common one so that can be played on preview
        private Stream wmaToMP3(string path, string audioFile, string key, string ffMpeg, int size)
        {
            var cmd = @"-i " + audioFile + " -ab 64 -ar 22050" + key + ".mp3";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffMpeg,
                Arguments = cmd,
                WorkingDirectory = Path.GetFullPath(path),
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }

            var audioFullPath = string.Concat(path, "/", audioFile);

            var ms = new MemoryStream();

            using (var fs = File.OpenRead(audioFullPath))
            {
                fs.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                byte[] buf = new byte[ms.Length];
                ms.Read(buf, 0, buf.Length);
            }

            File.Delete(audioFullPath);

            return ms;
        }

        public Stream ApplyWatermark(Watermark watermark, Stream assetsm, Stream watermarksm)
        {
            var img = Image.FromStream(assetsm);
            var watermarkOutput = Image.FromStream(watermarksm);
            watermarkOutput.ScaleByWidth((int)(watermarkOutput.Width * (watermark.Size / 100)));

            TargetSpot targetSpot;
            switch (watermark.WatermarkPosition)
            {
                case (int)WatermarkPosition.TopLeft:
                    targetSpot = TargetSpot.TopLeft;
                    break;
                case (int)WatermarkPosition.TopRight:
                    targetSpot = TargetSpot.TopRight;
                    break;
                case (int)WatermarkPosition.BottomLeft:
                    targetSpot = TargetSpot.BottomLeft;
                    break;
                case (int)WatermarkPosition.BottomRight:
                    targetSpot = TargetSpot.BottomRight;
                    break;
                default:
                    targetSpot = TargetSpot.Center;
                    break;
            }
            var wmOptions = new ImageWatermarkOptions()
            {
                Location = targetSpot,
                Margin = 10,
                Opacity = (int)Math.Round(watermark.Opacity * 100)
            };
            img.AddImageWatermark(watermarkOutput, wmOptions);

            var ms = new MemoryStream();

            img.Save(ms, img.RawFormat);
            ms.Position = 0;
            return ms;
        }

        public Dictionary<string, Stream> ConvertPDFPagesToPNG(Stream input, string fileName,
            bool showWatermark = false, Stream watermarksm = null, Watermark watermark = null)
        {
            var listStream = new Dictionary<string, Stream>();

            PdfDocument doc = new PdfDocument();
            doc.LoadFromStream(input);

            for (int i = 0; i < doc.Pages.Count; i++)
            {
                var page = doc.Pages[i];
                var ms = new MemoryStream();
                Image emf = doc.SaveAsImage(i);

                emf.Save(ms, ImageFormat.Png);
                //emf.Save(@"c:\\temp\\" + Path.GetFileNameWithoutExtension(fileName) + $"_{i + 1}.png", ImageFormat.Png);
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);

                if (showWatermark)
                {
                    var afterwm = ApplyWatermark(watermark, ms, watermarksm);
                    afterwm.Seek(0, SeekOrigin.Begin);
                    listStream.Add(Path.GetFileNameWithoutExtension(fileName) + $"_{i + 1}.png", afterwm);
                }
                else
                {
                    listStream.Add(Path.GetFileNameWithoutExtension(fileName) + $"_{i + 1}.png", ms);
                }
            }

            return listStream;
        }

        public Dictionary<string, Stream> ConvertPDFPagesToJpeg(Stream input, string fileName,
            bool showWatermark = false, Stream watermarksm = null, Watermark watermark = null)
        {
            var listStream = new Dictionary<string, Stream>();

            PdfDocument doc = new PdfDocument();
            doc.LoadFromStream(input);

            for (int i = 0; i < doc.Pages.Count; i++)
            {
                var page = doc.Pages[i];
                var ms = new MemoryStream();
                Image emf = doc.SaveAsImage(i);

                emf.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);

                if (showWatermark)
                {
                    var afterwm = ApplyWatermark(watermark, ms, watermarksm);
                    afterwm.Seek(0, SeekOrigin.Begin);
                    listStream.Add(Path.GetFileNameWithoutExtension(fileName) + $"_{i + 1}.png", afterwm);
                }
                else
                {
                    listStream.Add(Path.GetFileNameWithoutExtension(fileName) + $"_{i + 1}.jpeg", ms);
                }
            }

            return listStream;
        }

        public Stream ConvertImageToPdf(Stream input, string extension)
        {
            var ms = new MemoryStream();
            Image img = null;

            if (extension == "webp")
            {
                var cs = ConvertImageToImage(input);
                cs.Seek(0, SeekOrigin.Begin);
                img = Image.FromStream(cs);
            }
            else
            {
                img = Image.FromStream(input);
            }

            PdfDocument doc = new PdfDocument();
            PdfUnitConvertor uinit = new PdfUnitConvertor();
            PdfImage image = PdfImage.FromImage(img);
            SizeF pageSize = uinit.ConvertFromPixels(img.Size, PdfGraphicsUnit.Point);
            PdfPageBase page = doc.Pages.Add(pageSize, new PdfMargins(0f));

            page.Canvas.DrawImage(image, new PointF(0, 0));

            doc.SaveToStream(ms, FileFormat.PDF);
            doc.Close();
            return ms;
        }

        private Stream ConvertImageToImage(Stream stream)
        {
            Stream sm = new MemoryStream();

            using (var images = new MagickImageCollection())
            {
                var settings = new MagickReadSettings()
                {
                    Density = new Density(300),
                    FrameIndex = 0,
                    FrameCount = 1
                };

                images.Read(stream, settings);

                using (var horizontal = images[0])
                {
                    horizontal.Write(sm, MagickFormat.Jpeg);
                }
            }
            return sm;
        }

        public HttpResponseMessage ConvertOfficeToPDF(IConfiguration configuration, Stream stream, string fileName, string extension)
        {
            ConversionServiceConfig config = SetConversionServiceConfig(configuration);

            try
            {
                var client = new HttpClient();
                var input = new SPAConvertInput()
                {
                    FileName = fileName,
                    FileExtension = extension,
                    AssetStream = ConvertToBase64(stream)
                };

                HttpResponseMessage response = client.PostAsync(config.DocToPDFApiUrl, new JsonContent(input)).GetAwaiter().GetResult();


                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }


      
        private string ConvertToBase64(Stream stream)
        {
            var bytes = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }
    }



}
