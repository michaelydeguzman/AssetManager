using DAM.Application.Projects.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Requests
{
    public class SaveProjectCommentRequest : IRequest<HandlerResult<ProjectCommentDto>>
    {
        public string UserId { get; set; }
        public ProjectCommentDto ProjectComment { get; set; }
        public SaveProjectCommentRequest(ProjectCommentDto projectComment, string userId)
        {
            ProjectComment = projectComment;
            UserId = userId;
        }
    }
}
