using DAM.Application.DownloadPDF.dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.DownloadPDF.request
{
    public class DownloadPDFRequests : IRequest<HandlerResult<DownloadPDFDto>>
    {
        public DownloadPDFDto DownloadPDFDto { get; set; }
        
        public DownloadPDFRequests(DownloadPDFDto downloadPDFDto)
        {
            DownloadPDFDto = downloadPDFDto;
        }
    }
}
