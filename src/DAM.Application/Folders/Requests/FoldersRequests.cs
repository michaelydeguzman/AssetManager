using DAM.Application.Folders.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAM.Application.Folders.Requests
{
    public class FoldersRequest : IRequest<HandlerResult<IEnumerable<FolderDto>>>
    {
        public FoldersRequest()
        {
        }

        public class GetFolderRequest : IRequest<HandlerResult<IEnumerable<FolderDto>>>
        {
            public int FolderId { get; private set; }

            public GetFolderRequest(int folderId)
            {
                FolderId = folderId;
            }
        }

        public class AddFolderRequest : IRequest<HandlerResult<FolderDto>>
        {

            public string UserId { get; private set; }
            public FolderDto FolderDto { get; private set; }

            public AddFolderRequest(FolderDto folder, string userId)
            {
                FolderDto = folder;
                UserId = userId;
            }
        }

        public class UpdateFolderRequest : IRequest<HandlerResult<FolderDto>>
        {
            public string UserId { get; private set; }
            public FolderDto FolderDto { get; private set; }

            public UpdateFolderRequest(FolderDto folder, string userId)
            {
                FolderDto = folder;
                UserId = userId;
            }
        }

        public class MoveFolderRequest : IRequest<HandlerResult<MoveFolderDto>>
        {
            public string UserId { get; private set; }
            public MoveFolderDto MoveFolderDto { get; private set; }

            public MoveFolderRequest(MoveFolderDto moveFolder, string userId)
            {
                MoveFolderDto = moveFolder;
                UserId = userId;
            }
        }

        public class DeleteFolderRequest : IRequest<HandlerResult<DeleteFoldersDto>>
        {
            public string UserId { get; private set; }
            public DeleteFoldersDto DeleteFolders { get; private set; }

            public DeleteFolderRequest(DeleteFoldersDto folders, string userId)
            {
                DeleteFolders = folders;
                UserId = userId;
            }
        }

        public class GetFoldersAllAssetsRequest : IRequest<HandlerResult<Dictionary<string, Stream>>>
        {
            public string FolderId { get; private set; }
            public string UserId { get; private set; }
            public bool ShowWatermark { get; private set; }

            public GetFoldersAllAssetsRequest(string folderId, string userId, bool showWatermark)
            {
                FolderId = folderId;
                UserId = userId;
                ShowWatermark = showWatermark;
            }
        }

        public class BulkMoveFolderRequest : IRequest<HandlerResult<List<MoveFolderDto>>>
        {
            public string UserId { get; private set; }
            public List<MoveFolderDto> Folders { get; private set; }

            public BulkMoveFolderRequest(List<MoveFolderDto> folders, string userId)
            {
                Folders = folders;
                UserId = userId;
            }
        }

        public class OrderFoldersRequest : IRequest<HandlerResult<List<MoveFolderDto>>>
        {
            public string UserId { get; private set; }
            public List<MoveFolderDto> Folders { get; private set; }

            public OrderFoldersRequest(List<MoveFolderDto> folders, string userId)
            {
                Folders = folders;
                UserId = userId;
            }
        }

        public class CopyFolderRequest : IRequest<HandlerResult<CopyFolderDto>>
        {
            public string UserId { get; private set; }
            public CopyFolderDto CopyFolderDto { get; private set; }

            public CopyFolderRequest(CopyFolderDto copyFolder, string userId)
            {
                CopyFolderDto = copyFolder;
                UserId = userId;
            }
        }
    }
}
