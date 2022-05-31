using DAM.Domain.Entities;
using DAM.Domain.Entities.Identity;
using MediatR;
using System.Collections.Generic;

namespace DAM.Application.PwoerBI.Requests
{
    public class PowerBIRequest
    {
        public PowerBIRequest(){}

        public class GetAssetTableRequest : IRequest<HandlerResult<IEnumerable<Asset>>> // each table request create a new one below, name is get + table name + table + request, and the IRequest<HandlerResult<IEnumerable<Asset>>> part change the Asset to the entity mathc to the table
        {
            public GetAssetTableRequest(){}
        }

        public class GetAssetVersionTableRequest : IRequest<HandlerResult<IEnumerable<AssetVersions>>> 
        {
            public GetAssetVersionTableRequest() { }
        }
        public class GetApprovalLevelApproversTableRequest : IRequest<HandlerResult<IEnumerable<ApprovalLevelApprover>>> 
        {
            public GetApprovalLevelApproversTableRequest() { }
        }
        public class GetApprovalLevelsTableRequest : IRequest<HandlerResult<IEnumerable<ApprovalLevel>>> 
        {
            public GetApprovalLevelsTableRequest() { }
        }
        public class GetAssetAccountsTableRequest : IRequest<HandlerResult<IEnumerable<AssetAccountMetaData>>> 
        {
            public GetAssetAccountsTableRequest() { }
        }
        public class GetAssetCountryRegionsTableRequest : IRequest<HandlerResult<IEnumerable<AssetCountryRegionMetaData>>> 
        {
            public GetAssetCountryRegionsTableRequest() { }
        }
        public class GetCartItemsTableRequest : IRequest<HandlerResult<IEnumerable<CartItem>>> 
        {
            public GetCartItemsTableRequest() { }
        }
        public class GetCompaniesTableRequest : IRequest<HandlerResult<IEnumerable<Company>>> 
        {
            public GetCompaniesTableRequest() { }
        }
        public class GetCountriesTableRequest : IRequest<HandlerResult<IEnumerable<Country>>> 
        {
            public GetCountriesTableRequest() { }
        }
        public class GetFolderCountryRegionsTableRequest : IRequest<HandlerResult<IEnumerable<FolderCountryRegionMetaData>>> 
        {
            public GetFolderCountryRegionsTableRequest() { }
        }
        public class GetFoldersTableRequest : IRequest<HandlerResult<IEnumerable<Folder>>> 
        {
            public GetFoldersTableRequest() { }
        }
        public class GetRegionsTableRequest : IRequest<HandlerResult<IEnumerable<Region>>> 
        {
            public GetRegionsTableRequest() { }
        }
        public class GetTagsTableRequest : IRequest<HandlerResult<IEnumerable<Tag>>> 
        {
            public GetTagsTableRequest() { }
        }
        public class GetUserFoldersTableRequest : IRequest<HandlerResult<IEnumerable<UserFolder>>> 
        {
            public GetUserFoldersTableRequest() { }
        }
        public class GetCartTableRequest : IRequest<HandlerResult<IEnumerable<Cart>>> 
        {
            public GetCartTableRequest() { }
        }
        public class GetAssetAuditTableRequest : IRequest<HandlerResult<IEnumerable<AssetAudit>>> 
        {
            public GetAssetAuditTableRequest() { }
        }
        public class GetAppUsersTableRequest : IRequest<HandlerResult<IEnumerable<ApplicationUser>>> 
        {
            public GetAppUsersTableRequest() { }
        }
    }
}
