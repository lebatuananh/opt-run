using System.Linq.Expressions;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.WebApp.UseCases.Users;

public struct MutateUser
{
    public record GetListUserQueries(int Skip, int Take, string? Query) : IQueries;

    public record GetCustomerQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<AppUser>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
            }

            public override Expression<Func<AppUser, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetCustomerQuery>
            {
                public GetValidator()
                {
                    RuleFor(v => v.Id)
                        .NotEmpty()
                        .WithMessage("Id is required.");
                }
            }
        }
    }
}