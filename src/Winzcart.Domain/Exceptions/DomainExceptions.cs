namespace Winzcart.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class InsufficientPointsException : DomainException
{
    public InsufficientPointsException(int required, int available)
        : base($"Insufficient points. Required: {required}, Available: {available}") { }
}

public class CouponExpiredException : DomainException
{
    public CouponExpiredException() : base("This coupon has expired.") { }
}

public class CouponFullyRedeemedExceptioin : DomainException
{
    public CouponFullyRedeemedExceptioin() : base("This coupon has reached its maximum redemption limit.") { }
}

public class DuplicateBillException : DomainException
{
    public DuplicateBillException() : base("A bill with the same merchant, amount, and date already exists.") { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.") { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message) { }
}
