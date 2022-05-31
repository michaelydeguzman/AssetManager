using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class DeleteProjectCommentRequest : IRequest<HandlerResult<ProjectCommentDto>>
    {
        public string UserId { get; set; }
        public DeleteProjectCommentDto ProjectComment { get; set; }
        public DeleteProjectCommentRequest(DeleteProjectCommentDto projectComment, string userId)
        {
            ProjectComment = projectComment;
            UserId = userId;
        }
    }
}
