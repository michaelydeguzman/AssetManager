using DAM.Application.Styles.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.Styles.Requests
{
    public class StyleRequests : IRequest<HandlerResult<ThemeDto>>
    {
        

        public class EditStyleRequest : IRequest<HandlerResult<ThemeDto>>
        {
            public string UserId { get; set; }
            public ThemeDto Style { get; set; }

            public EditStyleRequest(ThemeDto style, string userId)
            {
                UserId = userId;
                Style = style; 
            }
        }

        public class AddStyleRequest : IRequest<HandlerResult<ThemeDto>>
        {
            public string UserId { get; set; }
            public ThemeDto Style { get; set; }

            public AddStyleRequest(ThemeDto style, string userId)
            {
                UserId = userId;
                Style = style;
            }
        }

        public class DeleteStyleRequest : IRequest<HandlerResult<List<ThemeDto>>>
        {
            public string UserId { get; set; }
            public List<ThemeDto> Styles { get; set; }

            public DeleteStyleRequest(List<ThemeDto> styles, string userId)
            {
                UserId = userId;
                Styles = styles;
            }
        }

        public class CopyStyleRequest : IRequest<HandlerResult<ThemeDto>>
        {
            public string UserId { get; set; }
            public ThemeDto Style { get; set; }

            public CopyStyleRequest(ThemeDto style, string userId)
            {
                UserId = userId;
                Style = style;
            }
        }
    }
}
