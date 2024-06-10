using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Service
{
    public class RenewalService : IRenewalService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public RenewalService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            
        }
        public async Task<RenewalResponseDto> CreateRenewal(RenewalRequestDto renewal)
        {
            var deal = await _repository.Deal.GetDeal(renewal.DealId, true) ?? throw new NotFoundException("No Deal for this renewal");
            var mapRenewal = _mapper.Map<RenewalsModel>(renewal, opts => opts.Items["deal"] = deal);
            _repository.Renewal.CreateRenewal(mapRenewal);
            deal.CommencementDate = deal.ExpiryDate;
            deal.ExpiryDate = deal.ExpiryDate.AddYears(mapRenewal.Term);
            deal.RenewalDate = deal.RenewalDate.AddYears(mapRenewal.Term);
            await _repository.SaveAsync();
            var renewalResponse = _mapper.Map<RenewalResponseDto>(mapRenewal);
            return renewalResponse;
        }

        public async Task DeleteRenewal(Guid id)
        {
            var renewal = await _repository.Renewal.GetRenewalByIdAsync(id, true);
            renewal.ToDeletedEntity();
            await _repository.SaveAsync();
        }

        public async Task<ICollection<RenewalResponseDto>> GetAllRenewalAsync() =>
         _mapper.Map<ICollection<RenewalResponseDto>>(await _repository.Renewal.GetRenewals(false));

        public async Task<RenewalResponseDto> GetRenewalAsync(Guid Id) =>
             _mapper.Map<RenewalResponseDto>(await _repository.Renewal.GetRenewalByIdAsync(Id, false) ?? throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, Constants.Renewal)));

        public async Task<(IEnumerable<RenewalResponseDto> renewals, MetaData metaData)> GetRenewalsPaginated(int page)
        {
            string pageSize = Environment.GetEnvironmentVariable("renewalsPageSize");
            int RenewalsPageSize;
            if (pageSize != null || pageSize == "")
            {
                RenewalsPageSize = 10;
            }
            else
            {
                try
                {

                    RenewalsPageSize = int.Parse(pageSize);
                }
                catch
                {
                    RenewalsPageSize = 10;
                }
            }
            var renewalsWithMetaData = await _repository.Renewal.GetPaginatedRenewal(false, page, RenewalsPageSize);
            var renewals = _mapper.Map<IEnumerable<RenewalResponseDto>>(renewalsWithMetaData);
            return (renewals, renewalsWithMetaData.MetaData);
        }

        public Task UpdateRenewal(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
