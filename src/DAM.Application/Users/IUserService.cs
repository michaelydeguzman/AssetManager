

using DAM.Application.Users.Constants;
using DAM.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Application.Templates
{
    public interface IUserService
    {
        #region User CRUD

        Task<UserDto> CreateUser(string firstName, string surname, string email, string jobTitle, string country, string region, string city, string state, string streetAddress, string postalCode, string province, string legalAgeGroup, string isUserNew, string password, UserStatus status);


        #endregion



    }
}
