using AutoMapper;
using Peace_BankingAPI.Application.DTOs.Account;
using Peace_BankingAPI.Application.DTOs.Transaction;
using Peace_BankingAPI.Domain.Entities;

namespace Peace_BankingAPI.WebAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.Currency,
                    opt => opt.MapFrom(src => src.Currency.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<Transaction, TransferResultDto>()
                .ForMember(d => d.Success, o => o.MapFrom(_ => true))
                .ForMember(d => d.TransactionId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FromAccountNumber, o => o.MapFrom(s => s.SourceAccount.AccountNumber))
                .ForMember(d => d.ToAccountNumber, o => o.MapFrom(s => s.DestinationAccount!.AccountNumber))
                .ForMember(d => d.FromAccountNewBalance, o => o.Ignore())   // set manually
                .ForMember(d => d.ToAccountNewBalance, o => o.Ignore())     // set manually
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Currency.ToString()))
                .ForMember(d => d.CompletedAt, o => o.MapFrom(s => s.CompletedAt ?? DateTime.UtcNow));
        }
    }
}
