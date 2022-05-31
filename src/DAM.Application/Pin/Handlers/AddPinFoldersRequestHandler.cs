using Microsoft.Extensions.Configuration;
using DAM.Application.Pin.Dtos;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using static DAM.Application.Pin.Requests.PinFolderRequest;
using System.Threading;
using System.Linq;
using AutoMapper;
using DAM.Domain.Entities;

namespace DAM.Application.Pin.Handlers
{
    public class AddPinFoldersRequestHandler : HandlerBase<AddPinFoldersRequest, HandlerResult<IEnumerable<PinFoldersDto>>>
    {
        private readonly IDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AddPinFoldersRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override HandlerResult<IEnumerable<PinFoldersDto>> HandleRequest(AddPinFoldersRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<PinFoldersDto>> result)
        {
            //do clean the pin Folder which is archive or delete           
            var oldPinList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).ToList();
            if( oldPinList.Count > 0)
            {
                var pinIds = new List<int>();
                oldPinList.ForEach(p =>
                {
                    pinIds.Add(p.FolderId);
                });
                var FolderList = _dbcontext.Folders.Where(a => pinIds.Contains(a.Id) && (a.Deleted == true)).ToList();
                if(FolderList.Count > 0)
                {
                    var removeList = new List<PinFolder>();
                    FolderList.ForEach(a =>
                    {
                        var pin = oldPinList.Find(p => p.FolderId == a.Id);
                        removeList.Add(pin);
                    });
                    _dbcontext.PinFolders.RemoveRange(removeList);
                    _dbcontext.SaveChanges();
                }
            }

            //add new pin
            if (request.PinObj.FolderId > 0)
            { 
                var prePinFolderList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).ToList();
                if(prePinFolderList.Count >= 12)
                {
                    var fullPinList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId);
                    result.Entity = _mapper.Map<IEnumerable<PinFoldersDto>>(fullPinList);
                    result.ResultType = ResultType.Fail;
                    return result;
                }

                if (!prePinFolderList.Exists(p => p.FolderId == request.PinObj.FolderId))
                {
                    _dbcontext.PinFolders.RemoveRange(prePinFolderList);
                    var newPinFolderList = new List<PinFolder>();
                    prePinFolderList.ForEach((p) =>
                    {
                        var pin = new PinFolder
                        {
                            UserId = p.UserId,
                            FolderId = p.FolderId,
                            OrderNumber = p.OrderNumber
                        };
                        newPinFolderList.Add(pin);
                    });
                    var newPin = new PinFolder
                    {
                        UserId = request.PinObj.UserId,
                        FolderId = request.PinObj.FolderId,
                        OrderNumber = prePinFolderList.Count + 1
                    };
                    newPinFolderList.Add(newPin);
                    _dbcontext.PinFolders.AddRange(newPinFolderList);
                    _dbcontext.SaveChanges();
                }
            }
            var latestPinFolderList = _dbcontext.PinFolders.Where(a => a.UserId == request.PinObj.UserId).OrderBy(a => a.OrderNumber);
            result.Entity = _mapper.Map<IEnumerable<PinFoldersDto>>(latestPinFolderList);
            result.ResultType = ResultType.Success;
            return result;
        }
    }
}
