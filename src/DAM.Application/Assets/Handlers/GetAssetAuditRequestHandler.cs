using AutoMapper;
using Azure.Storage.Blobs;
using DAM.Application.Accounts.Dtos;
using DAM.Application.Assets.Dtos;
using DAM.Application.Assets.Requests;
using DAM.Application.Cache;
using DAM.Application.CountryRegions.Dtos;
using DAM.Domain.Entities;
using DAM.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DAM.Application.Assets.Enums;

namespace DAM.Application.Assets.Handlers
{
    public class GetAssetAuditRequestHandler : HandlerBase<GetAssetAuditRequest, HandlerResult<IEnumerable<AssetAuditDto>>>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetAssetAuditRequestHandler(ICacheProvider cacheProvider, IDbContext dbcontext, IMapper mapper, IConfiguration configuration)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override HandlerResult<IEnumerable<AssetAuditDto>> HandleRequest(GetAssetAuditRequest request, CancellationToken cancellationToken, HandlerResult<IEnumerable<AssetAuditDto>> result)
        {
            List<Asset> assets = new List<Asset>();
            List<AccountDto> accounts = new List<AccountDto>();
            List<RegionDto> regions = new List<RegionDto>();
            List<TagDto> tags = new List<TagDto>();

            string[] imageExt = new string[] { "png", "jpg", "tif", "tiff", "jpeg", "bmp", "eps" };

            var assetVersions = (from a in _dbcontext.Assets.ToList()
                                 join av in _dbcontext.AssetVersions.ToList()
                                 on a.Id equals av.AssetId
                                 where imageExt.Contains(av.Extension.ToLower()) && av.ActiveVersion == 1
                                 select av).ToList();

            var assetAuditResult = new List<AssetAuditDto>();

            foreach (var assetVersion in assetVersions)
            {
                //get related asset
                var asset = _dbcontext.Assets.Where(a => a.Id == assetVersion.AssetId).Include(asset => asset.AssetAccounts).ThenInclude(aa => aa.Account)
                .Include(asset => asset.AssetCountryRegions).ThenInclude(aa => aa.Region).ThenInclude(aa => aa.Country).FirstOrDefault();

                var assetAudit = _mapper.Map<AssetAuditDto>(assetVersion);
                assetAudit.CreatedById = assetVersion.CreatedById;
                assetAudit.CreatedBy = _dbcontext.AppUsers.First(u => u.Id == assetVersion.CreatedById).Email;
                assetAudit.ModifiedBy = assetVersion.ModifiedBy != null ? _dbcontext.AppUsers.First(u => u.Id == assetVersion.ModifiedById).Email : null;
                assetAudit.ModifiedById = assetVersion.ModifiedById;
                assetAudit.Status = Enum.GetName(typeof(AssetStatus), assetVersion.Status);
                assetAudit.Name = asset.Title;
                assetAudit.Countries = _mapper.Map<List<CountryDto>>(asset.AssetCountryRegions.Select(aa => aa.Country).Distinct().ToList());
                assetAudit.Accounts = _mapper.Map<List<AccountDto>>(asset.AssetAccounts.Select(aa => aa.Account).ToList());
                assetAudit.Regions = _mapper.Map<List<RegionDto>>(asset.AssetCountryRegions.Select(aa => aa.Region).ToList());

                assetAuditResult.Add(assetAudit);
            }

            result.Entity = assetAuditResult;
            result.ResultType = ResultType.Success;
            return result;
        }

        public override Exception OnException(Exception ex)
        {
            return new Exception(ex.Message, ex);
        }

    }
}