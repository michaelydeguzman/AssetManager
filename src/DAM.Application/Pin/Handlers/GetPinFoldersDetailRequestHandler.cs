// using Microsoft.Extensions.Configuration;
// using DAM.Application.Pin.Dtos;
// using DAM.Persistence;
// using System;
// using System.Collections.Generic;
// using System.Text;
// using static DAM.Application.Pin.Requests.PinFolderRequest;
// using System.Threading;
// using System.Linq;
// using AutoMapper;
// using DAM.Application.Folders.Dtos;
// using DAM.Domain.Entities;
// using DAM.Application.Folders.Enums;
// using DAM.Application.Extensions;

// namespace DAM.Application.Pin.Handlers
// {
//     public class GetPinFoldersDetailRequestHandler : HandlerBase<GetPinFoldersDetailRequest, HandlerResult<IEnumerable<FolderDto>>>
//     {
//         private readonly IDbContext _dbcontext;
//         private readonly IConfiguration _configuration;
//         private readonly IMapper _mapper;

//         public GetPinFoldersDetailRequestHandler(IDbContext dbcontext, IConfiguration configuration, IMapper mapper)
//         {
//             _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
//             _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//             _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//         }

//         public override HandlerResult<IEnumerable<FolderDto>> HandleRequest(GetPinFoldersDetailRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<FolderDto>> result)
//         {
//             var templist = _dbcontext.PinFolders.Where(a => a.UserId == request.UserId).ToList();
//             if (templist.Count > 0)
//             {
//                 var FoldersList = new List<int>();
//                 templist.ForEach(p =>
//                 {
//                     FoldersList.Add(p.FolderId);
//                 });

//                 if (FoldersList.Count > 0)
//                 {
//                     List<Folder> Folders = new List<Folder>();
//                     Folders = _dbcontext.Folders.Where(a => a.Status != Convert.ToInt32(FolderStatus.Archived) && a.Status != Convert.ToInt32(FolderStatus.Deleted) && FoldersList.Contains(a.Id)).ToList();
//                     result.Entity = _mapper.Map<IEnumerable<FolderDto>>(Folders);
//                     // Update Folder URLs
//                     foreach (var Folder in result.Entity)
//                     {
//                         #region Change Folder DTO with active Folder version
//                         FolderVersions version = new FolderVersions();
//                         List<FolderVersions> versions = new List<FolderVersions>();
//                         List<Tag> labels = new List<Tag>();
//                         versions = _dbcontext.FolderVersions.Where(a => a.FolderId == Folder.Id).ToList();
//                         Folder.FolderVersions = (List<FolderVersionsDto>)_mapper.Map<IEnumerable<FolderVersionsDto>>(versions);
//                         //get active version
//                         version = versions.Find((v) => v.ActiveVersion == 1);
//                         labels = _dbcontext.Tags.Where(a => a.FolderId == version.Id).ToList();
//                         Folder.Tags = (List<TagDto>)_mapper.Map<IEnumerable<TagDto>>(labels);
//                         //for the public share url, directly download
//                         Folder.DownloadUrl = version.DownloadUrl;
//                         Folder.Thumbnail = version.Thumbnail;
//                         //for the private share url, redirect to /asserts/FoldersId
//                         Folder.CopyUrl = _configuration["BaseUrl"] + "Folders/" + version.Id;
//                         Folder.CreatedByName = _dbcontext.AppUsers.First(u => u.Id == version.CreatedById).UserName;
//                         Folder.StatusName = ((FolderStatus)version.Status).GetDescription();
//                         //Enum.GetName(typeof(FolderStatus), Folder.Status);
//                         Folder.Size = Convert.ToInt32(version.Size);
//                         Folder.Extension = version.Extension;
//                         Folder.FileName = version.FileName;
//                         Folder.FileSizeText = version.FileSizeText;
//                         Folder.FileType = version.FileType;
//                         Folder.Key = version.Key;
//                         Folder.OriginalUrl = version.OriginalUrl;
//                         #endregion
//                     }
//                 }
//                 result.ResultType = ResultType.Success;
//             }
//             else
//             {
//                 result.ResultType = ResultType.NoData;
//             }
//             return result;
//         }
//     }
// }
