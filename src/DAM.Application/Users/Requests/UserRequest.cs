using DAM.Application.Users.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Users.Requests
{
    public class UserRequest : IRequest<HandlerResult<IEnumerable<UserDto>>>
    {
        public UserRequest()
        {
        }

        public class GetUsersRequest : IRequest<HandlerResult<IEnumerable<UserDto>>>
        {
            public string UserId { get; private set; }

            public GetUsersRequest(string userId)
            {
                UserId = userId;
            }
        }

        public class GetApproversRequest : IRequest<HandlerResult<IEnumerable<UserDto>>>
        {
            public GetApproversRequest()
            {
            }
        }
        
        public class GetUserFolderRequest : IRequest<HandlerResult<IEnumerable<UserFolderDto>>>
        {
            public string UserId { get; private set; }

            public GetUserFolderRequest(string userId)
            {
                UserId = userId;
            }
        }

        public class AddUserRequest : IRequest<HandlerResult<UserDto>>
        {
            public UserDto UserDto { get; private set; }

            public AddUserRequest(UserDto user)
            {
                UserDto = user;
            }
        }

        public class ShareFoldertoUserRequest : IRequest<HandlerResult<UserDto>>
        {
            public UserDto UserDto { get; private set; }

            public ShareFoldertoUserRequest(UserDto requestData)
            {
                UserDto = requestData;
            }
        }

        public class AddUserImageRequest : IRequest<HandlerResult<UserDto>>
        {
            public UserDto UserDto { get; private set; }

            public AddUserImageRequest(UserDto user)
            {
                UserDto = user;
            }
        }
        
        public class InviteNewUserRequest : IRequest<HandlerResult<InviteNewUserDto>>
        {
            public InviteNewUserDto InviteNewUserDto { get; private set; }
            public string UserId { get; set; }

            public InviteNewUserRequest(InviteNewUserDto inviteNewUserDto, string userId)
            {
                InviteNewUserDto = inviteNewUserDto;
                UserId = userId;
            }
        }

        public class GetUserOOORequest : IRequest<HandlerResult<IEnumerable<UserOOODto>>>
        {
            public string UserId { get; private set; }

            public GetUserOOORequest(string userId)
            {
                UserId = userId;
            }
        }
        public class AddUserOOORequest : IRequest<HandlerResult<UserOOODto>>
        {
            public UserOOODto UserOOO { get; private set; }

            public AddUserOOORequest(UserOOODto ooo)
            {
                UserOOO = ooo;
            }
        }

        public class EditUserOOORequest : IRequest<HandlerResult<UserOOODto>>
        {
            public UserOOODto UserOOO { get; private set; }

            public EditUserOOORequest(UserOOODto ooo)
            {
                UserOOO = ooo;
            }
        }
    }

}
