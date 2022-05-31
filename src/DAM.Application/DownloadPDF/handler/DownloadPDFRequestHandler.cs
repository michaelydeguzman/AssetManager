using DAM.Application.DownloadPDF.dto;
using DAM.Application.DownloadPDF.request;
using Newtonsoft.Json;
//using Spire.Doc.Fields;
//using Spire.Pdf;
//using Spire.Pdf.Annotations;
//using Spire.Pdf.General.Find;
//using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SpirePDF = Spire.Pdf;

namespace DAM.Application.DownloadPDF.handler
{
    public class DownloadPDFRequestHandler : HandlerBase<DownloadPDFRequests, HandlerResult<DownloadPDFDto>>
    {
        public class Annotations
        {
            public string Annotation { get; set; }

            public string UUID { get; set; }
            public string Type { get; set; }
            public string Color { get; set; }
            public float[,] Lines { get; set; }
            public string Class { get; set; }
            public string Content { get; set; }
            public string Page { get; set; }
            public string AuthorName { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public string CreatedAt { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public Rectangle[] Rectangles { get; set; }
        }

        public class Rectangle
        {
            public float Width { get; set; }
            public float Height { get; set; }
            public string CreatedAt { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
        }

        public override HandlerResult<DownloadPDFDto> HandleRequest(DownloadPDFRequests request, CancellationToken cancellationToken, HandlerResult<DownloadPDFDto> result)
        {
            var dto = request.DownloadPDFDto;

            result.Entity = new DownloadPDFDto()
            {
                OutputStream = new MemoryStream(),
                FileName = dto.FileName,
                ContentType = "application/pdf"
            };

            SpirePDF.PdfDocument doc = new SpirePDF.PdfDocument(dto.AssetStream);


            var annotationJson = JsonConvert.DeserializeObject<List<Annotations>>(dto.Annotation);
            if (annotationJson != null)
            {
                foreach (var annotation in annotationJson)
                {
                    if (annotation.Type != "Comment" && annotation.Page != null)
                    {
                        SpirePDF.PdfPageBase pageBase = doc.Pages[Convert.ToInt32(annotation.Page) - 1];
                        RectangleF rect = new RectangleF(annotation.X, annotation.Y, annotation.Width, annotation.Height);
                        Color color = Color.Black;

                        switch (dto.Rotation)
                        {
                            case 0:
                                pageBase.Rotation = SpirePDF.PdfPageRotateAngle.RotateAngle0;
                                break;
                            case 90:
                                pageBase.Rotation = SpirePDF.PdfPageRotateAngle.RotateAngle90;
                                break;
                            case 180:
                                pageBase.Rotation = SpirePDF.PdfPageRotateAngle.RotateAngle180;
                                break;
                            case 270:
                                pageBase.Rotation = SpirePDF.PdfPageRotateAngle.RotateAngle270;
                                break;
                        }

                        if (annotation.Color != null)
                        {
                            color = ColorTranslator.FromHtml(annotation.Color.IndexOf('#') >= 0 ? annotation.Color : "#" + annotation.Color);
                        }
                        if (annotation.Rectangles != null && annotation.Rectangles.Length > 0)
                        {
                            rect = new RectangleF(annotation.Rectangles[0].X, annotation.Rectangles[0].Y, annotation.Rectangles[0].Width, annotation.Rectangles[0].Height);
                        }

                        switch (annotation.Type)
                        {
                            case "highlight":
                                var hlUniqueYs = annotation.Rectangles.GroupBy(rec => rec.Y).Select(group => group.First().Y).ToList();
                                var hlRects = new List<RectangleF>();
                                for(var i = 0; i< hlUniqueYs.Count; i++)
                                {
                                    var uniqueY = hlUniqueYs[i];
                                    float minX = annotation.Rectangles[0].X;
                                    float thisWidth = 0;
                                    float minHeight = annotation.Rectangles[0].Height;
                                    foreach (var annotRect in annotation.Rectangles)
                                    {
                                        if (annotRect.Y == uniqueY)
                                        {
                                            if (minX > annotRect.X)
                                            {
                                                minX = annotRect.X;
                                            }
                                            thisWidth += (annotRect.Width + 10);
                                            if (minHeight > annotRect.Height)
                                            {
                                                minHeight = annotRect.Height;
                                            }
                                        }
                                    }
                                    var soRect = new RectangleF(minX, uniqueY, thisWidth, minHeight);
                                    hlRects.Add(soRect);
                                    i++;
                                }
                                foreach (var eRect in hlRects)
                                {
                                    SpirePDF.Annotations.PdfTextMarkupAnnotation highlight = new SpirePDF.Annotations.PdfTextMarkupAnnotation(annotation.AuthorName, "", eRect);

                                    highlight.TextMarkupAnnotationType = SpirePDF.Annotations.PdfTextMarkupAnnotationType.Highlight;
                                    highlight.TextMarkupColor = color;
                                    highlight.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                    highlight.Text = annotation.Content != null ? annotation.Content : "";

                                    pageBase.AnnotationsWidget.Add(highlight);
                                }
                                break;

                            case "strikeout":
                                var soUniqueYs = annotation.Rectangles.GroupBy(rec => rec.Y).Select(group => group.First().Y).ToList();
                                var soRects = new List<RectangleF>();
                                foreach (var uniqueY in soUniqueYs)
                                {
                                    float minX = annotation.Rectangles[0].X;
                                    float thisWidth = 0;
                                    float minHeight = annotation.Rectangles[0].Height;
                                    foreach (var annotRect in annotation.Rectangles)
                                    {
                                        if(annotRect.Y == uniqueY)
                                        {
                                            if(minX > annotRect.X)
                                            {
                                                minX = annotRect.X;
                                            }
                                            thisWidth += (annotRect.Width+10);
                                            if (minHeight > annotRect.Height)
                                            {
                                                minHeight = annotRect.Height;
                                            }
                                        }
                                    }
                                    minHeight = minHeight / 2;
                                    var soRect = new RectangleF(minX, uniqueY, thisWidth, minHeight);
                                    soRects.Add(soRect);
                                }

                                foreach(var eRect in soRects)
                                {
                                    SpirePDF.Annotations.PdfTextMarkupAnnotation strikeout = new SpirePDF.Annotations.PdfTextMarkupAnnotation(annotation.AuthorName, "", eRect);

                                    strikeout.TextMarkupAnnotationType = SpirePDF.Annotations.PdfTextMarkupAnnotationType.StrikeOut;
                                    strikeout.TextMarkupColor = color;
                                    strikeout.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                    strikeout.Text = annotation.Content != null ? annotation.Content : "";

                                    pageBase.AnnotationsWidget.Add(strikeout);
                                }
                               
                                break;

                            case "drawing":
                                //int[] linePoints = new int[annotation.Lines.Length / 2];
                                //for (var i = 0; i < (annotation.Lines.Length / 2) - 1; i++)
                                //{
                                //    linePoints[i] = ((int)annotation.Lines[i, 0] << 16) | ((int)annotation.Lines[i, 1]);
                                //}

                                //SpirePDF.Annotations.PdfLineAnnotation line = new SpirePDF.Annotations.PdfLineAnnotation();
                                //line.BeginLineStyle = SpirePDF.Annotations.PdfLineEndingStyle.Circle;
                                //line.EndLineStyle = SpirePDF.Annotations.PdfLineEndingStyle.Circle;
                                //line.

                                PointF[] points = new PointF[annotation.Lines.Length / 2];

                                for (var i = 0; i < (annotation.Lines.Length / 2) - 1; i++)
                                {
                                    points[i] = new PointF(annotation.Lines[i, 0], annotation.Lines[i, 1]);
                                }

                                points[(annotation.Lines.Length / 2) - 1] = new PointF(annotation.Lines[0, 0], annotation.Lines[0, 1]);

                                SpirePDF.Annotations.PdfPolygonAnnotation line = new SpirePDF.Annotations.PdfPolygonAnnotation(pageBase, points);

                                line.Author = annotation.AuthorName;
                                line.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                line.Color = color;
                                line.Subject = "Drawing";
                                line.Border = new SpirePDF.Annotations.PdfAnnotationBorder(annotation.Width);

                                pageBase.AnnotationsWidget.Add(line);
                                break;
                            case "textbox":
                                SpirePDF.Annotations.PdfFreeTextAnnotation textAnnotation = new SpirePDF.Annotations.PdfFreeTextAnnotation(rect);

                                textAnnotation.Flags = SpirePDF.Annotations.PdfAnnotationFlags.Print;
                                textAnnotation.AnnotationIntent = SpirePDF.Annotations.PdfAnnotationIntent.FreeTextTypeWriter;
                                textAnnotation.TextMarkupColor = color;
                                textAnnotation.MarkupText = annotation.Content;
                                textAnnotation.Name = annotation.AuthorName;
                                textAnnotation.Border = new SpirePDF.Annotations.PdfAnnotationBorder(0f);
                                textAnnotation.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                textAnnotation.Text = annotation.Content != null ? annotation.Content : "";

                                pageBase.AnnotationsWidget.Add(textAnnotation);
                                break;
                            case "point":
                                SpirePDF.Annotations.PdfPopupAnnotation text = new SpirePDF.Annotations.PdfPopupAnnotation();

                                var comment = annotationJson.Find(a => a.Annotation == annotation.UUID);

                                text.Icon = SpirePDF.Annotations.PdfPopupIcon.Comment;
                                text.Location = new PointF(annotation.X, annotation.Y);
                                text.Open = false;
                                text.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                text.Color = Color.Yellow;
                                text.Text = comment != null ? comment.Content : "";

                                pageBase.AnnotationsWidget.Add(text);
                                break;
                            case "area":
                                SpirePDF.Annotations.PdfPolygonAnnotation square = new SpirePDF.Annotations.PdfPolygonAnnotation(pageBase,
                                    new PointF[] {
                                    new PointF( annotation.X, annotation.Y ),
                                    new PointF( annotation.X + annotation.Width, annotation.Y ),
                                    new PointF( annotation.X + annotation.Width, annotation.Y + annotation.Height ),
                                    new PointF( annotation.X, annotation.Y + annotation.Height),
                                    new PointF( annotation.X, annotation.Y )
                                    });

                                square.Author = annotation.AuthorName;
                                square.ModifiedDate = Convert.ToDateTime(annotation.CreatedAt);
                                square.Color = Color.Red;
                                square.Rectangle = new RectangleF(annotation.X, annotation.Y, annotation.Width, annotation.Height);
                                square.Subject = "Rectangle";

                                pageBase.AnnotationsWidget.Add(square);
                                break;
                            default:
                                break;
                        }
                    }

                }
            }

            var outputStream = doc.SaveToStream(SpirePDF.FileFormat.PDF)[0];
            outputStream.Seek(0, SeekOrigin.Begin);

            result.Entity = new DownloadPDFDto()
            {
                OutputStream = outputStream,
                FileName = dto.FileName,
                ContentType = "application/pdf"
            };
            return result;

            //PdfPageBase page = doc.Pages.Add();
            //PdfFont font = new PdfFont(PdfFontFamily.Helvetica, 13);
            //string text = "HelloWorld";
            //PointF point = new PointF(200, 100);
            //page.Canvas.DrawString(text, font, PdfBrushes.CadetBlue, point);
            //PdfTextMarkupAnnotation annotation1 = new PdfTextMarkupAnnotation("Administrator", "Demo about Annotation.", text,new RectangleF() , font);
            //annotation1.Border = new PdfAnnotationBorder(0.75f);
            //annotation1.TextMarkupColor = Color.Green;
            //annotation1.Location = new PointF(point.X + doc.PageSettings.Margins.Left, point.Y + doc.PageSettings.Margins.Left);
            //(page as PdfNewPage).Annotations.Add(annotation1);

            //(page as PdfNewPage).Annotations[0].Text = "Annotation Edited:(Demo about Annotation)";

        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }
    }
}
