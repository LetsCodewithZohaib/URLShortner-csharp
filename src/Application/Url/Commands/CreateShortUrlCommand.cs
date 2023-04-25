using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<string>
{
    public string Url { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
          .NotEmpty()
          .WithMessage("Url is required.");
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public CreateShortUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        //OrignalURL

        //put it in the DB

        //By using hash we can encode that

        //Return it back
        var URL = _context.Urls.Where(x => x.OriginalUrl == request.Url).FirstOrDefault();
        if (URL!=null)
        {
            var hashedst = _hashids.EncodeLong(URL.Id);
            return "https://localhost:7072/u/" + hashedst;           
        }

        var oriURL = request.Url;
        var domainURL = new UrlShortenerService.Domain.Entities.Url() { OriginalUrl = oriURL };
        _context.Urls.Add(domainURL);
        var id = await  _context.SaveChangesAsync(cancellationToken);
        var hashedstr= _hashids.EncodeLong(domainURL.Id);
        return "https://localhost:7072/u/"+ hashedstr;
    }
}
